using MyPRG2Final;
using System;
using System.Collections.Generic;

//==========================================================
// Student Number : S10274395
// Student Name   : Than Thar Wint Htal
// Partner Name   : Janessa Tan Jia En
//==========================================================

namespace S10272516_PRG2Assignment
{
    class Program
    {
        void Run()
        {

            List<Restaurants> restaurantList = new List<Restaurants>();
            List<Customer> customerList = new List<Customer>();
            List<Order> orderList = new List<Order>();

            Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

            Console.WriteLine(restaurantList.Count + " restaurants loaded!");
            Console.WriteLine(customerList.Count + " customers loaded!");
            Console.WriteLine(orderList.Count + " orders loaded!");
            Console.WriteLine();

            while (true)
            {
                Console.WriteLine("====== Gruberoo Food Delivery System ======");
                Console.WriteLine("1. List all restaurants and menu items");
                Console.WriteLine("2. List all orders");
                Console.WriteLine("0. Exit");
                Console.Write("Enter your choice: ");

                string choice = Console.ReadLine();
                Console.WriteLine();

                if (choice == "1")
                {
                    ListRestaurantsAndMenu(restaurantList);
                }
                else if (choice == "2")
                {
                    ListAllOrders(orderList, customerList, restaurantList);
                }
                else if (choice == "0")
                {
                    Console.WriteLine("Goodbye!");
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid choice.");
                }

                Console.WriteLine();
            }
        }

        // ====================================================
        // FEATURE 3 : List Restaurants and Menu
        // ====================================================
        void ListRestaurantsAndMenu(List<Restaurants> restaurantList)
        {
            Console.WriteLine("All Restaurants and Menu Items");
            Console.WriteLine("===============================");

            for (int i = 0; i < restaurantList.Count; i++)
            {
                Restaurants r = restaurantList[i];

                Console.WriteLine("Restaurant: " + r.RestaurantName +
                                  " (" + r.RestaurantId + ")");

                List<Menu> menuList = r.GetAllMenus();

                if (menuList.Count == 0)
                {
                    Console.WriteLine("No menu available.");
                }
                else
                {
                    for (int j = 0; j < menuList.Count; j++)
                    {
                        Menu m = menuList[j];
                        List<FoodItem> itemList = m.GetAllItems();

                        for (int k = 0; k < itemList.Count; k++)
                        {
                            FoodItem item = itemList[k];

                            Console.WriteLine(" - " + item.ItemName +
                                              " : " + item.ItemDesc +
                                              " - $" + item.ItemPrice.ToString("0.00"));
                        }
                    }
                }
            }
        }

        // ====================================================
        // FEATURE  : List All Orders
        // ====================================================
        void ListAllOrders(
            List<Order> orderList,
            List<Customer> customerList,
            List<Restaurants> restaurantList)
        {
            Console.WriteLine("All Orders");
            Console.WriteLine("===========");

            Console.WriteLine("OrderID   Customer   Restaurant   Delivery Time   Amount   Status");

            for (int i = 0; i < orderList.Count; i++)
            {
                Order o = orderList[i];

                string customerName = o.CustomerEmail;
                string restaurantName = o.RestaurantId;

                // Find customer name
                for (int j = 0; j < customerList.Count; j++)
                {
                    if (customerList[j].EmailAddr == o.CustomerEmail)
                    {
                        customerName = customerList[j].CustomerName;
                        break;
                    }
                }

                // Find restaurant name
                for (int j = 0; j < restaurantList.Count; j++)
                {
                    if (restaurantList[j].RestaurantId == o.RestaurantId)
                    {
                        restaurantName = restaurantList[j].RestaurantName;
                    }
                }

                Console.WriteLine(
                    o.OrderID + "   " +
                    customerName + "   " +
                    restaurantName + "   " +
                    o.DeliveryDateTime.ToString("dd/MM/yyyy HH:mm") + "   $" +
                    o.OrderTotal.ToString("0.00") + "   " +
                    o.OrderStatus
                );
            }
        }

        // ===== Program Starting Point =====
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Run();
        }
    }
}
