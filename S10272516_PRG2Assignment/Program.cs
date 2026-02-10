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
            while ((line = sr.ReadLine())!= null)
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


    // ====================================================
    // FEATURE 5 : Create a new order
    // ====================================================
    void CreateNewOrder()
    {
        Console.WriteLine("Create New Order");
        Console.WriteLine("================");

        // 1) Customer
        string cEmail = PromptNonEmpty("Enter Customer Email: ");
        if (!customerDict.ContainsKey(cEmail))
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        // 2) Restaurant
        string rID = PromptNonEmpty("Enter Restaurant ID: ");
        if (!restaurantDict.TryGetValue(rID, out var restaurant))
        {
            Console.WriteLine("Restaurant not found.");
            return;
        }

        // 3) Delivery Date & Time
        DateTime deliveryDate = PromptDate("Enter Delivery Date (dd/MM/yyyy): ");
        TimeSpan deliveryTime = PromptTime("Enter Delivery Time (hh:mm): ");
        DateTime deliveryDateTime = deliveryDate.Date + deliveryTime;

        if (deliveryDateTime <= DateTime.Now)
        {
            Console.WriteLine("Delivery date/time must be in the future.");
            return;
        }

        // 4) Address
        string address = PromptNonEmpty("Enter Delivery Address: ");

        // 5) Items selection
        var flatItems = FlattenRestaurantItems(restaurant);
        if (flatItems.Count == 0)
        {
            Console.WriteLine("Restaurant has no items to order.");
            return;
        }

        Console.WriteLine("Available Food Items:");
        for (int i = 0; i < flatItems.Count; i++)
        {
            var fi = flatItems[i];
            Console.WriteLine($"{i + 1}. {fi.ItemName} - ${fi.ItemPrice:0.00}");
        }

        var chosen = new List<(FoodItem item, int qty)>();
        while (true)
        {
            int index = PromptInt("Enter item number (0 to finish): ", min: 0, max: flatItems.Count);
            if (index == 0) break;
            var item = flatItems[index - 1];
            int qty = PromptInt("Enter quantity: ", min: 1, max: 100);
            chosen.Add((item, qty));
        }

        if (chosen.Count == 0)
        {
            Console.WriteLine("No items selected. Order not created.");
            return;
        }

        // 6) Special request (single request for the whole order)
        string? specialRequest = null;
        if (PromptYesNo("Add special request? [Y/N]: "))
        {
            specialRequest = PromptNonEmpty("Enter special request: ");
        }

        // Build order
        int newId = GetNextOrderId();
        var o = new Order
        {
            OrderID = newId,
            OrderDateTime = DateTime.Now,
            DeliveryDateTime = deliveryDateTime,
            DeliveryAddress = address,
            OrderStatus = "Pending",
            OrderPaymentMethod = "",
            OrderPaid = false
        };

        foreach (var (fi, qty) in chosen)
        {
            var ofi = new OrderedFoodItem(fi.ItemName, fi.ItemDesc, fi.ItemPrice, specialRequest, qty, fi.ItemPrice);
            o.AddOrderedFoodItem(ofi);
        }

        double itemsTotal = o.CalculateOrderTotal();
        double grand = itemsTotal + DeliveryFee;

        Console.WriteLine($"Order Total: ${itemsTotal:0.00} + ${DeliveryFee:0.00} (delivery) = ${grand:0.00}");
        if (!PromptYesNo("Proceed to payment? [Y/N]: "))
        {
            Console.WriteLine("Order not created.");
            return;
        }

        string pm = PromptPaymentMethod();
        o.OrderPaymentMethod = pm;
        o.OrderPaid = true;
        o.OrderTotal = grand;

        // Save to structures
        orderDict[o.OrderID] = o;
        orderToCustomerEmail[o.OrderID] = cEmail;
        orderToRestaurantId[o.OrderID] = rID;

        customerDict[cEmail].AddOrder(o);
        restaurant.AddOrder(o);

        // Append to orders.csv
        AppendOrderToCsv(o, cEmail, rID);

        Console.WriteLine($"Order {o.OrderID} created successfully! Status: {o.OrderStatus}");
    }

    // ====================================================
    // FEATURE 6 : Process an order (per restaurant queue)
    // ====================================================


    // ====================================================
    // FEATURE 7 : Modify an existing order (Pending only)
    // ====================================================
    void ModifyOrder()
    {
        Console.WriteLine("Modify Order");
        Console.WriteLine("============");

        string cEmail = PromptNonEmpty("Enter Customer Email: ");
        if (!customerDict.TryGetValue(cEmail, out var customer))
        {
            Console.WriteLine("Customer not found.");
            return;
        }

        var pendingOrders = customer.GetOrdersByStatus("Pending");
        if (pendingOrders.Count == 0)
        {
            Console.WriteLine("No Pending orders found for this customer.");
            return;
        }

        Console.Write("Pending Orders: ");
        Console.WriteLine(string.Join(" ", pendingOrders.Select(x => x.OrderID)));

        int oId = PromptInt("Enter Order ID: ");
        var o = pendingOrders.FirstOrDefault(x => x.OrderID == oId);
        if (o == null)
        {
            Console.WriteLine("Invalid Order ID or order is not Pending.");
            return;
        }

        // Show details
        Console.WriteLine("Order Items:");
        o.DisplayOrderedFoodItems();
        Console.WriteLine($"Address: {o.DeliveryAddress}");
        Console.WriteLine($"Delivery Date/Time: {o.DeliveryDateTime:dd/M/yyyy, HH:mm}");

        Console.Write("Modify: [1] Items  [2] Address  [3] Delivery Time: ");
        string opt = (Console.ReadLine() ?? "").Trim();

        double before = o.CalculateOrderTotal() + DeliveryFee;

        if (opt == "1")
        {
            // Modify items from the restaurant that owns this order
            if (!orderToRestaurantId.TryGetValue(oId, out var rID) || !restaurantDict.TryGetValue(rID, out var restaurant))
            {
                Console.WriteLine("Unable to determine restaurant for this order.");
                return;
            }

            var flatItems = FlattenRestaurantItems(restaurant);
            if (flatItems.Count == 0)
            {
                Console.WriteLine("Restaurant has no items.");
                return;
            }

            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("[A]dd  [U]pdate Qty  [R]emove  [F]inish");
                string action = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

                if (action == "F") break;

                if (action == "A")
                {
                    for (int i = 0; i < flatItems.Count; i++)
                        Console.WriteLine($"{i + 1}. {flatItems[i].ItemName} - ${flatItems[i].ItemPrice:0.00}");

                    int idx = PromptInt("Enter item number to add: ", min: 1, max: flatItems.Count) - 1;
                    int qty = PromptInt("Enter quantity: ", min: 1, max: 100);
                    var fi = flatItems[idx];

                    var ofi = new OrderedFoodItem(fi.ItemName, fi.ItemDesc, fi.ItemPrice, fi.Customize, qty, fi.ItemPrice);
                    o.AddOrderedFoodItem(ofi);
                    Console.WriteLine($"{fi.ItemName} x{qty} added.");
                }
                else if (action == "U")
                {
                    if (o.OrderList.Count == 0)
                    {
                        Console.WriteLine("No items to update.");
                        continue;
                    }
                    for (int i = 0; i < o.OrderList.Count; i++)
                        Console.WriteLine($"{i + 1}. {o.OrderList[i].ItemName} - {o.OrderList[i].QtyOrdered}");

                    int idx = PromptInt("Select item to update: ", min: 1, max: o.OrderList.Count) - 1;
                    int qty = PromptInt("Enter new quantity: ", min: 1, max: 100);
                    o.OrderList[idx].QtyOrdered = qty;
                    Console.WriteLine("Quantity updated.");
                }
                else if (action == "R")
                {
                    if (o.OrderList.Count == 0)
                    {
                        Console.WriteLine("No items to remove.");
                        continue;
                    }
                    for (int i = 0; i < o.OrderList.Count; i++)
                        Console.WriteLine($"{i + 1}. {o.OrderList[i].ItemName} - {o.OrderList[i].QtyOrdered}");

                    int idx = PromptInt("Select item to remove: ", min: 1, max: o.OrderList.Count) - 1;
                    var removed = o.OrderList[idx];
                    o.RemoveOrderedFoodItem(removed);
                    Console.WriteLine($"{removed.ItemName} removed.");
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }
            }
        }
        else if (opt == "2")
        {
            string newAddress = PromptNonEmpty("Enter new Address: ");
            o.DeliveryAddress = newAddress;
            Console.WriteLine("Address updated.");
        }
        else if (opt == "3")
        {
            TimeSpan newTime = PromptTime("Enter new Delivery Time (hh:mm): ");
            o.DeliveryDateTime = o.DeliveryDateTime.Date + newTime;
            if (o.DeliveryDateTime <= DateTime.Now)
            {
                Console.WriteLine("Delivery time must be in the future. Reverting change.");
            }
            else
            {
                Console.WriteLine($"Order {o.OrderID} updated. New Delivery Time: {o.DeliveryDateTime:HH:mm}");
            }
        }
        else
        {
            Console.WriteLine("Invalid choice.");
            return;
        }

        double after = o.CalculateOrderTotal() + DeliveryFee;
        o.OrderTotal = after;

        if (after > before)
        {
            Console.WriteLine($"Order total increased from ${before:0.00} to ${after:0.00}.");
            if (PromptYesNo("Proceed to payment for the difference? [Y/N]: "))
            {
                if (string.IsNullOrEmpty(o.OrderPaymentMethod))
                    o.OrderPaymentMethod = PromptPaymentMethod();
                o.OrderPaid = true;
            }
            else
            {
                Console.WriteLine("Payment declined. (Order remains Pending with updated total.)");
            }
        }

        Console.WriteLine("Updated Order Details:");
        Console.WriteLine($"Order {o.OrderID}  Total: ${o.OrderTotal:0.00}  Status: {o.OrderStatus}");
    }

    // ====================================================
    // FEATURE 8 : Delete an existing order (Pending → Cancelled + refund)
    // ====================================================

    // ====================================================
    // ADVANCED (a) : Bulk processing of unprocessed orders for current day
    // ====================================================


    // ====================================================
    // ADVANCED (b) : Display total order amount & Gruberoo earnings
    // ====================================================

    void Advanced_DisplayTotalsAndEarnings()
    {
        Console.WriteLine("Advanced - Totals and Gruberoo Earnings");
        Console.WriteLine("=======================================");

        double overallDeliveredLessDelivery = 0.0;
        double overallRefunds = 0.0;

        // Build a snapshot of refunds per restaurant
        var refundByRestaurant = new Dictionary<string, double>();
        foreach (var o in refundStack)
        {
            if (orderToRestaurantId.TryGetValue(o.OrderID, out var rID))
            {
                if (!refundByRestaurant.ContainsKey(rID)) refundByRestaurant[rID] = 0.0;
                refundByRestaurant[rID] += o.OrderTotal;
            }
        }

        foreach (var r in restaurantDict.Values.OrderBy(x => x.RestaurantId))
        {
            // Delivered orders mapped to this restaurant
            var delivered = orderDict.Values.Where(o =>
                o.OrderStatus == "Delivered" &&
                orderToRestaurantId.TryGetValue(o.OrderID, out var rid) &&
                rid == r.RestaurantId).ToList();

            double deliveredLessDelivery = delivered.Sum(o => Math.Max(0, o.OrderTotal - DeliveryFee));
            double refunds = refundByRestaurant.TryGetValue(r.RestaurantId, out var amt) ? amt : 0.0;

            overallDeliveredLessDelivery += deliveredLessDelivery;
            overallRefunds += refunds;

            Console.WriteLine($"{r.RestaurantName} ({r.RestaurantId})");
            Console.WriteLine($" - Total order amount (Delivered, less delivery): ${deliveredLessDelivery:0.00}");
            Console.WriteLine($" - Total refunds: ${refunds:0.00}");
        }

        Console.WriteLine("---------------------------------------");
        Console.WriteLine($"Total order amount (all restaurants, less delivery): ${overallDeliveredLessDelivery:0.00}");
        Console.WriteLine($"Total refunds: ${overallRefunds:0.00}");

        // Gruberoo earns 30% of successful order amounts (less delivery fee per order).
        double gruberooEarns = overallDeliveredLessDelivery * PlatformFeeRate;
        Console.WriteLine($"Final amount Gruberoo earns (30%): ${gruberooEarns:0.00}");
    }

    // ====================================================
    // Exit: Save Queue & Stack to NEW files
    // ====================================================
    void SaveQueueAndStackOnExit()
    {
        // queue.csv columns: RestaurantId,OrderID,Status,DeliveryDateTime,CustomerEmail,Amount
        using (var sw = new StreamWriter("queue.csv", false))
        {
            sw.WriteLine("RestaurantID,OrderID,Status,DeliveryDateTime,CustomerEmail,Amount");
            foreach (var r in restaurantDict.Values)
            {
                foreach (var o in r.Orders)
                {
                    string cEmail = orderToCustomerEmail.TryGetValue(o.OrderID, out var ce) ? ce : "";
                    sw.WriteLine($"{r.RestaurantId},{o.OrderID},{o.OrderStatus},{o.DeliveryDateTime:dd/MM/yyyy HH:mm},{cEmail},{o.OrderTotal:0.00}");
                }
            }
        }

        // stack.csv columns: OrderID,Status,ProcessedAt,CustomerEmail,RestaurantID,Amount
        using (var sw = new StreamWriter("stack.csv", false))
        {
            sw.WriteLine("OrderID,Status,ProcessedAt,CustomerEmail,RestaurantID,Amount");
            foreach (var o in refundStack)
            {
                string cEmail = orderToCustomerEmail.TryGetValue(o.OrderID, out var ce) ? ce : "";
                string rId = orderToRestaurantId.TryGetValue(o.OrderID, out var rid) ? rid : "";
                sw.WriteLine($"{o.OrderID},{o.OrderStatus},{DateTime.Now:dd/MM/yyyy HH:mm},{cEmail},{rId},{o.OrderTotal:0.00}");
            }
        }
    }

    // ====================================================
    // Helpers
    // ====================================================

    static string[] SplitCsv(string Line)
    {
        // This function splits a csv line into columns.
        var list = new List<string>();
        bool inQuotes = false;

        var cur = new System.Text.StringBuilder();
        foreach (char ch in line)
        {
            if (ch == '"')
            {
                inQuotes = 1 inQuotes;
                continue;
            }

            // If comma appears outside quotes new column
            if (ch == ',' && !inQuotes)
            {
                list.Add(cur.ToString());
                cur.Clear();
            }
            else
            {
                // Otherwise keep building current column text
                cur.Append(ch);
            }
        }

        //Add last column
        list.Add(cur.ToString());

        return list.ToArray();
    }
    // ========================================
    // Merge Extra CSV Fields Into One Field
    // ========================================
    static string[] MergeTailFields(string[] raw, int startIndex)
    {
        // Used when item list accidentally splits across multiple columns.

        if (raw.Length <= startIndex)
            return raw;

        var head = raw.Take(startIndex).ToList();
        var tail = string.Join(",", raw.Skip(startIndex));
        head.Add(tail);

        return head.ToArray();

    }

    // =======================================
    // Parse Date (dd/MM/yyyy)
    // =======================================
    static DateTime ParseDate(string s)
    {
        DateTime.TryParseExact(
            s.Trim(),
            "dd/MM/yyyy",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime d);

        return d;
    }

    // =======================================
    // Parse DateTime (Supports Multiple Formats)
    // =======================================
    static DateTime ParseDateTime(string s)
    {
        string[] formats =
        {
            "dd/MM/yyyy HH:mm",
            "d/M/yyyy HH:mm",
            "dd/MM/yyyy H:mm",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss",
            "M/d/yyyy H:mm",
            "M/d/yyyy HH:mm"
        };

        DateTime.TryParseExact(
            s.Trim(),
            formats,
            CultureInfo.InvariantCulture,
            DateTimeStyles.AllowWhiteSpaces,
            out DateTime dt);

        return dt;
    }

    // =====================================
    // Parse Double Value 
    // =====================================

    static double ParseDouble(string s)
    {
        // Using invariant culture first 
        if (double.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out double v))
            return v;

        // Trying Current system culture
        if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out v))
            return v;

        // Parsing fails  to return 0
        return 0.0;
    }

    // ==============================================
    // Convert Items Text Blob Into List
    // ==============================================
    static List<(string name, int qty)> ParseItemsBlob(string itemsBlob)
    {
        var list = new List<(string, int)>();

        if (string.IsNullOrWhiteSpace(itemsBlob))
            return list;

        var parts = itemsBlob.Split('|');

        foreach (var p in parts)
        {
            var kv = p.Split(',');

            if (kv.Length < 2)
                continue;

            string name = kv[0].Trim().Trim('"');

            // Default quantity
            if (!int.TryParse(kv[1].Trim().Trim('"'), out int qty))
                qty = 1;

            list.Add((name, qty));

        }

        return list;
    }

    // ==========================================
    // Generate Next Order ID
    // ==========================================
    static int GetNextOrderId()
    {
        return (orderDict.Count == 0)
            ? 1001
            : orderDict.Keys.Max() + 1;

    }

    // ==============================================
    // Get All Unique Food Items From Restaurant
    // ==============================================
    static List<FoodItem> FlattenRestaurantItems(Restaurants r)
    {
        var result = new List<FoodItem>();

        foreach (var m in r.GetAllMenus())
        {
            var items = m.GetAllItems();

            if (items != null && items.Count > 0)
                result.AddRange(items);
        }

        // Remove duplicates based on item name
        return result
            .GroupBy(x => x.ItemName, StringComparer.OrdinalIgnoreCase)
            .Select(g => g.First())
            .ToList();
    }

    // =======================================
    // Remove Order From Restaurant Queue
    // =======================================
    static void RemoveOrderFromQueue(Restaurants r, int orderId)
    {
        int n = r.Orders.Count;

        var tmp = new Queue<Order>();

        for (int i = 0; i < n; i++)
        {
            var o = r.Orders.Dequeue();

            if (o.OrderID != orderId)
                tmp.Enqueue(o);
        }

        // Put remaining orders back into queue
        foreach (var o in tmp)
            r.Orders.Enqueue(o);
    }

    // =======================================
    // Append Order Into orders.csv
    // =======================================
    void AppendOrderToCsv(Order o, string cEmail, string rID)
    {
        // Convert ordered items into text format
        string itemsBlob = string.Join("|",
            o.OrderList.Select(ofi => $"{ofi.ItemName},{ofi.QtyOrdered}")
        );

        bool fileExists = File.Exists("orders.csv");

        using (var sw = new StreamWriter("orders.csv", append: true))
        {
            // If file does not exist, write header first
            if (!fileExists)
            {
                sw.WriteLine("OrderID,CustomerEmail,RestaurantID,DeliveryDate,DeliveryTime,DeliveryAddress,OrderDateTime,Total,Status,Items");
            }

            sw.WriteLine(
                $"{o.OrderID},{cEmail},{rID}," +
                $"{o.DeliveryDateTime:dd/MM/yyyy}," +
                $"{o.DeliveryDateTime:HH:mm}," +
                $"\"{o.DeliveryAddress}\"," +
                $"{o.OrderDateTime:dd/MM/yyyy HH:mm}," +
                $"{o.OrderTotal:0.00}," +
                $"{o.OrderStatus}," +
                $"\"{itemsBlob}\""
            );
        }
    }

    // =======================================
    // Prompt User Input (Non-Empty String)
    // =======================================
    static string PromptNonEmpty(string label)
    {
        while (true)
        {
            Console.Write(label);

            string? s = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(s))
                return s.Trim();

            Console.WriteLine("Value cannot be empty.");
        }
    }

    // =======================================
    // Prompt Date Input
    // =======================================
    static DateTime PromptDate(string label)
    {
        while (true)
        {
            Console.Write(label);

            string? s = Console.ReadLine();

            if (DateTime.TryParseExact(
                s?.Trim(),
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime dt))
                return dt.Date;

            Console.WriteLine("Invalid date. Use dd/MM/yyyy.");
        }
    }

    // =======================================
    // Prompt Time Input
    // =======================================
    static TimeSpan PromptTime(string label)
    {
        while (true)
        {
            Console.Write(label);

            string? s = Console.ReadLine();

            if (TimeSpan.TryParseExact(
                s?.Trim(),
                "hh\\:mm",
                CultureInfo.InvariantCulture,
                out TimeSpan t))
                return t;

            Console.WriteLine("Invalid time. Use hh:mm (24-hour).");
        }
    }

    // =======================================
    // Prompt Integer Input (With Range)
    // =======================================
    static int PromptInt(string label, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(label);

            string? s = Console.ReadLine();

            if (int.TryParse(s, out int v) && v >= min && v <= max)
                return v;

            Console.WriteLine($"Enter a valid integer between {min} and {max}.");
        }
    }

    // =======================================
    // Prompt Yes / No Input
    // =======================================
    static bool PromptYesNo(string label)
    {
        while (true)
        {
            Console.Write(label);

            string? s = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(s))
                continue;

            s = s.Trim().ToUpperInvariant();

            if (s == "Y") return true;
            if (s == "N") return false;
        }
    }

    // =======================================
    // Prompt Payment Method
    // =======================================
    static string PromptPaymentMethod()
    {
        while (true)
        {
            Console.Write("Payment method: [CC] Credit Card / [PP] PayPal / [CD] Cash on Delivery: ");

            string s = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();

            if (s == "CC" || s == "PP" || s == "CD")
                return s;

            Console.WriteLine("Invalid method. Enter CC, PP or CD.");
        }
    }

    // =======================================
    // Truncate Long String With Ellipsis
    // =======================================
    static string Trunc(string s, int len)
    {
        if (string.IsNullOrEmpty(s))
            return s;

        return s.Length <= len
            ? s
            : (s.Substring(0, len - 1) + "…");
    }

}




