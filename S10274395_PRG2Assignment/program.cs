using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number : S10274395
// Student Name : Than Thar Wint Htal
// Partner Name : Janessa Tan Jia En
//========================================================== 

namespace S10272516_PRG2Assignment
{
    class program
    {
        static Dictionary<string, Restaurant> restaurants = new Dictionary<string, Restaurant>();
        static Dictionary<string, Customer> customers = new Dictionary<string, Customer>();
        static List<Order> allOrders = new List<Order> <allOrders>();
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome to the Gruberoo Food Delivery System");
            Console.WriteLine($""{restaurants.Count}restaurants loaded1");
            Console.WriteLine($"{restaurants.Values.Sum(restaurants => restaurants.GetAllMenus()
            .Sum(m => m.GetAllItems().Count))} food items loaded!");
            Console.WriteLine($"{customers.Count}customers loaded!");
            Console.WriteLine($"{allOrders.Count} orders loaded!");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("======Gruberoo Food Delivery System =====");
                Console.WriteLine("1. List all restaurants and menu items");
                Console.WriteLine("2. List all Orders");
                Console.WriteLine("3. Create a new order");
                Console.WriteLine("4. Process an order");
                Console.WriteLine("5. Modify an existind Order");
                Console.WriteLine("6. Delete an existing order");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                if (choice == "1")
                    ListRestaurantsAndMenu();
                else if (choice == "2")
                    ListAllOrders();
                else if (choice == "0")
                {
                    SaveQueueAndStack(); return;
                }

                else
                {
                    Console.WriteLine("Feature not implemented yet. Please choose 1 or 2 for now.");
                }

                Console.WriteLine();

              
            }
                
        }

        //===============================
        //FEATURE 3 (exact format)
        //===============================
        static void ListRestaurantsAndMenu()
        {
            Console.WriteLine("All Restaurants and Menu Items");
            Console.WriteLine("===============================");

            foreach (var r in restaurants.Values.OrderBy(x => x.restaurantName))
            {
                Console.WriteLine($"Restaurant: {r.restaurantName}({r.restaurantId})");

                var menus = r.GetAllMenus();
                if (menus.Count ==0)
                {
                    Console.WriteLine("(no menu)");
                    continue;
                }
                foreach (var m in menus)
                {
                    var items = m.GetAllItems();
                    foreach (var fi in items)
                    {
                        Console.WriteLine($" -{fi.ItemName} : {fi.ItemDesc} -${fi.ItemPrice:0.00}");
                        

                    }
                }
            }

        }

        //===============================
        //FEATURE 4(exact format)
        //===============================
        static voide ListAllOrders()
        {
            Console.WriteLine("All Orders");
            Console.WriteLine("============");
            Console.WriteLine(
            "{0,-8}{1,-15}{2,-16}{3,-19}{4,7}{5,-10}",
            "Order ID", "Customer", "Restaurant", "Delivery Date/Time", "Amount", "Status");
            Console.WriteLine(
            "{0,-8}{1,-15}{2,-16}{3,-19}{4,7}{5,-10}",
            "--------", "---------------", "----------------", "-------------------", "-------", "----------");

            foreach (var o in allOrders.OrderBy(x=> x.orderId))
            {
                string custName = o.orderPaymentMethod;
                if (customers.ContainsKey(o.CustomerEmail))
                    custName = customers[o.CustomerEmail].customerNmae;
                else if (!string.IsNullOrWhiteSpace(o.CustomerEmail))
                    custName =o.CustomerEmail;
                string restName = o.RestaurantID;

                if (restaurants.ContainsKey(o.RestaurantId))
                    restName = restaurants[o.RestaurantId].restaurantName;

                doubel amoutn = o.orderTotal;

                Console.WriteLine(

                    $"{o.orderId,8}" +
                    $"{Trunc(custName, 15),-15} " +
                    $"{Trunc(restName, 16),-16} " +
                    $"{o.deliveryDateTime:dd/MM/yyyy HH:mm} " +
                    $"${amount,6:0.00} " +
                    $"{o.orderStatus}"
                );
            }

            static string Trunc(string s , int max)
            {
                if (string.IsNullOrEmpty(s))
                    return "";
                return s.Length <= max ? s:s.Substring(0, max);
            }

            static void SaveQueueAndStack()
            {
                Console.WriteLine("Goodbye!");
            }
        }
    }
}