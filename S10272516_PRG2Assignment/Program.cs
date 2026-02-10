using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.AccessControl;

namespace S10272516_S10274395_PRG2Assignment;
class Program
{
    // Beside the Features //
    private static readonly Dictionary<string, Restaurants> restaurantDict = new Dictionary<string, Restaurants>();
    private static readonly Dictionary<string, Customer> customerDict = new Dictionary<string, Customer>();
    private static readonly Dictionary<int, Order> orderDict = new Dictionary<int, Order>();

    // Food items are unique by name (as per spec). Keep a global lookup.
    private static readonly Dictionary<string, FoodItem> foodItemByName = new Dictionary<string, FoodItem>(StringComparer.OrdinalIgnoreCase);

    // Order mapping (because Order class has no direct customer/restaurant properties)
    private static readonly Dictionary<int, string> orderToCustomerEmail = new Dictionary<int, string>();
    private static readonly Dictionary<int, string> orderToRestaurantId = new Dictionary<int, string>();

    // Refund stack (rejected/cancelled)
    private static readonly Stack<Order> refundStack = new Stack<Order>();

    // Constants
    private const double DeliveryFee = 5.00;
    private const double PlatformFeeRate = 0.30; // 30%

    // ====== Program Entry ======
    static void Main(string[] args)
    {
        Program p = new Program();
        p.Run();
    }

    void Run()
    {
        Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

        InitializeRestaurants();
        Console.WriteLine($"{restaurantDict.Count} restaurants loaded!");

        InitializeFoodItems();
        Console.WriteLine($"{foodItemByName.Count} food items loaded!");

        InitializeCustomers();
        Console.WriteLine($"{customerDict.Count} customers loaded!");

        InitializeOrders();
        Console.WriteLine($"{orderDict.Count} orders loaded!");

        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("====== Gruberoo Food Delivery System ======");
            Console.WriteLine("1. List all restaurants and menu items");
            Console.WriteLine("2. List all orders");
            Console.WriteLine("3. Create a new order");
            Console.WriteLine("4. Process an order");
            Console.WriteLine("5. Modify an existing order");
            Console.WriteLine("6. Delete an existing order");
            Console.WriteLine("7. Advanced: Bulk process today's pending orders");
            Console.WriteLine("8. Advanced: Display totals and Gruberoo earnings");
            Console.WriteLine("0. Exit");
            Console.Write("Enter your choice: ");

            string choice = Console.ReadLine()?.Trim() ?? string.Empty;
            Console.WriteLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        ListRestaurantsAndMenu();
                        break;
                    case "2":
                        ListAllOrders();
                        break;
                    case "3":
                        CreateNewOrder();
                        break;
                    case "4":
                        ProcessOrder();
                        break;
                    case "5":
                        ModifyOrder();
                        break;
                    case "6":
                        DeleteOrder();
                        break;
                    case "7":
                        Advanced_BulkProcessTodayPending();
                        break;
                    case "8":
                        Advanced_DisplayTotalsAndEarnings();
                        break;
                    case "0":
                        SaveQueueAndStackOnExit();
                        Console.WriteLine("Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    // ====================================================
    // FEATURE 1 : Load files (restaurants and food items)
    // ====================================================


    // ====================================================
    // FEATURE 2 : Load files (customers and orders)
    // ====================================================
    void InitializeCustomers()
    {
        // customers.csv -> Name, Email
        if (!File.Exists("customers.csv"))
            throw new FileNotFoundException("Missing customers.csv");

        using (var sr = new StreamReader("customers.csv"))
        {
            string? line = sr.ReadLine();
            while ((line = sr.ReadLine()) != null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                string[] data = SplitCsv(line);
                if (data.Length < 2) continue;

                string cName = data[0].Trim();
                string cEmail = data[1].Trim();

                if (!customerDict.ContainsKey(cEmail))
                    customerDict[cEmail] = new Customer(cEmail, cName);
            }
        }
    }

    void InitializeOrders()
    {
        // orders.csv expected columns (aligned with your WIP and the handout):
        // 0. OrderID
        // 1. CustomerEmail
        // 2. RestaurantID
        // 3. DeliveryDate (dd/MM/yyyy)
        // 4. DeliveryTime (HH:mm)
        // 5. DeliveryAddress
        // 6. OrderDateTime (dd/MM/yyyy HH:mm)
        // 7. Total
        // 8. Status 
        // 9. Items  ("ItemName1:Qty1;ItemName2:Qty2;...")

        if (!File.Exists("orders.csv")) return;

        using (var sr = new StreamReader("orders.csv"))
        {
            string? first = sr.ReadLine(); // header
            string? line;
            while ((line = sr.ReadLine() )!= null)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] raw = SplitCsv(line);

                if (raw.Length < 10)
                    raw = MergeTailFields(raw, 9);

                if (raw.Length < 10) continue;

                if (!int.TryParse(raw[0], out int oID)) continue;
                string cEmail = raw[1].Trim();
                string rID = raw[2].Trim();

                DateTime deliveryDate = ParseDate(raw[3]);
                DateTime deliveryDateTime = ParseDateTime($"{raw[3]} {raw[4]}");
                string address = raw[5].Trim();

                DateTime orderDateTime = ParseDateTime(raw[6]);
                double total = ParseDouble(raw[7]);
                string status = raw[8].Trim();

                string itemsBlob = raw[9].Trim().Trim('"');
                var items = ParseItemsBlob(itemsBlob); // List<(name,qty)>

                //Build Order
                var o = new Order
                {
                    OrderID = oID,
                    OrderDateTime = orderDateTime,
                    DeliveryDateTime = deliveryDateTime,
                    DeliveryAddress = address,
                    OrderStatus = status,
                    OrderPaymentMethod ="", // unknown from file
                    OrderPaid = false

                };

                foreach (var (name, qty) in items)
                {
                    if (!foodItemByName.TryGetValue(name, out FoodItem baseItem)) continue;
                    // SubTotal in OrderedFoodItem represents unit price (naming quirk).
                    var ofi = new OrderedFoodItem(baseItem.ItemName, baseItem.ItemDesc, baseItem.ItemPrice, baseItem.Customize, qty, baseItem.ItemPrice);
                    o.AddOrderedFoodItem(ofi);
                }

                // Compute total if missing; otherwise trust file
                double computed = o.CalculateOrderTotal() + DeliveryFee;
                o.OrderTotal = total > 0 ? total : computed;

                // Save in dictionaries
                orderDict[oID] = o;
                orderToCustomerEmail[oID] = cEmail;
                orderToRestaurantId[oID] = rID;

                // Link to restaurant queue and customer list
                if (restaurantDict.TryGetValue(rID, out var res))
                    res.AddOrder(o);

                if (customerDict.TryGetValue(cEmail, out var cust))
                    cust.AddOrder(o);
            }
        }
    }

    // ====================================================
    // FEATURE 3 : List Restaurants and Menu
    // ====================================================
    void ListRestaurantsAndMenu()
    {
        Console.WriteLine("All Restaurants and Menu Items");
        Console.WriteLine("===============================");

        foreach (var r in restaurantDict.Values.OrderBy(x => x.RestaurantId))
        {
            Console.WriteLine($"Restaurant: {r.RestaurantName} ({r.RestaurantId})");

            var menus = r.GetAllMenus();
            if (menus.Count == 0)
            {
                Console.WriteLine("No menu avaliable.");
                continue;
            }

            foreach (var m in menus)
            {
                var items = m.GetAllItems();
                if (items.Count == 0)
                {
                    Console.WriteLine(" (no items)");
                }

                foreach (var item in items)
                {
                    Console.WriteLine($" - {item.ItemName}: {item.ItemDesc} - ${item.ItemPrice:0.00}");
                }
            }
        }
    }

    // ====================================================
    // FEATURE 4 : List All Orders
    // ====================================================
}

