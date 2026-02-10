using PRG2_Assignment___S10272516___S10274395__Janessa_Tan_Jia_En___Than_Thar_Wint_Htal_;

//==========================================================
// Student Number : S10272516
// Student Name : Janessa Tan Jia En
// Partner Name : Than Thar Wint Htal
//==========================================================

// ====================================================
// FEATURE 1 : Load files (restaurants and food items)
// By: Janessa
// ====================================================
Dictionary<string, Restaurants> restaurantDict = new Dictionary<string, Restaurants>();
void InitializeRestaurants()
{
    using (StreamReader sr = new StreamReader("restaurants.csv"))
    {
        string? line = sr.ReadLine();
        while ((line = sr.ReadLine()) != null)
        {
            string[] data = line.Split(",");
            string rID = data[0];
            string rName = data[1];
            string rEmail = data[2];
            Restaurants r = new Restaurants(rID, rName, rEmail);
            restaurantDict.Add(rID, r);
        }
    }
}

Dictionary<string, FoodItem> foodItemDict = new Dictionary<string, FoodItem>();
void InitializeFoodItems()
{
    using (StreamReader sr = new StreamReader("fooditems.csv"))
    {
        string? line = sr.ReadLine();
        while ((line = sr.ReadLine()) != null)
        {
            string[] data = line.Split(",");
            string rID = data[0];
            string fiName = data[1];
            string fiDesc = data[2];
            string descOrPrice = "";
            for (int i = 3; i < (data.Length - 1); i++)
            {
                descOrPrice += data[i];
                if (i < data.Length - 1)
                {
                    descOrPrice += ", ";
                }
            }
            double fiPrice = Convert.ToDouble(data[data.Length - 1]);
            FoodItem fi = new FoodItem(fiName, fiDesc, fiPrice, null);
            foodItemDict.Add(fiName, fi);
        }
    }
}


// ====================================================
// FEATURE 2 : Load files (customers and orders) [loading of orders is work in progress]
// By: Janessa
// ====================================================
Dictionary<string, Customer> customerDict = new Dictionary<string, Customer>();
void InitializeCustomers()
{
    using (StreamReader sr = new StreamReader("customers.csv"))
    {
        string? line = sr.ReadLine();
        while ((line = sr.ReadLine()) != null)
        {
            string[] data = line.Split(",");
            string cName = data[0];
            string cEmail = data[1];
            Customer c = new Customer(cEmail, cName);
            customerDict.Add(cEmail, c);
        }
    }
}

Dictionary<int, Order> orderDict = new Dictionary<int, Order>();
void InitializeOrders()
{
    //using (StreamReader sr = new StreamReader("orders.csv"))
    //{
    //    string? line = sr.ReadLine();
    //    while ((line = sr.ReadLine()) != null)
    //    {
    //        string[] data = line.Split(",");
    //        int oID = Convert.ToInt32(data[0]);
    //        string cEmail = data[1];
    //        string rID = data[2];
    //        DateOnly oDeliveryDate = DateOnly.Parse(data[3]);
    //        TimeOnly oDeliveryTime = TimeOnly.Parse(data[4]);
    //        DateTime oDeliveryDateTime = new DateTime(oDeliveryDate, oDeliveryTime);
    //        string oDeliveryAddress = data[5];
    //        DateTime oDateTime = Convert.ToDateTime(data[6]);
    //        double oTotal = Convert.ToDouble(data[7]);
    //        string oStatus = data[8];
    //        string foodData = "";
    //        for (int i = 9; i < data.Length; i++)
    //        {
    //            foodData += data[i];
    //            if (i < data.Length - 1)
    //            {
    //                foodData += ",";
    //            }
    //        }

    //        string[] foodItems = foodData.Split('|');
    //        string fNames = "";
    //        for (int i = 0; i < foodItems.Count(); i++)
    //        {
    //            string[] foodAndQuantity = foodItems[i].Split(",");
    //            string fName = foodAndQuantity[0].TrimStart('\"');
    //            fNames += (fName + " ");
    //            int fQty = Convert.ToInt32(foodAndQuantity[1].TrimEnd('\"'));
    //            Console.WriteLine(fNames);
    //            Order o = new Order();
    //            FoodItem fi = foodItemDict[fName];
    //            OrderedFoodItem ofi = new OrderedFoodItem(fNames, fi.ItemDesc, fi.ItemPrice, null, fQty, oTotal);
    //            o.AddOrderedFoodItem(ofi);
    //            orderDict.Add(oID, o);
    //        }
    //    }
    //}
}


Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

InitializeRestaurants();
Console.WriteLine($"{restaurantDict.Count} restaurants loaded!");

InitializeFoodItems();
Console.WriteLine($"{foodItemDict.Count} food items loaded!");

InitializeCustomers();
Console.WriteLine($"{customerDict.Count} customers loaded!");

InitializeOrders();
Console.WriteLine($"{orderDict.Count} orders loaded! (WORK IN PROGRESS)");


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
                    ListRestaurantsAndMenu(restaurantDict);
                }
                else if (choice == "2")
                {
                    ListAllOrders(orderDict, customerDict, restaurantDict);
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
        void ListRestaurantsAndMenu(Dictionary<string,Restaurants>restaurantDict)
        {
            Console.WriteLine("All Restaurants and Menu Items");
            Console.WriteLine("===============================");

            for (int i = 0; i < restaurantDict.Count; i++)
            {
                Restaurants r = restaurantDict[i];

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
                        Dictionary<string, FoodItem> foodItemDict = m.GetAllItems();

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
            Dictionary<int, Order> orderDict,
            Dictionary<string, Customer> customerDict,
            Dictionary<string, Restaurants> restaurantDict
        {
            Console.WriteLine("All Orders");
            Console.WriteLine("===========");

            Console.WriteLine("OrderID   Customer   Restaurant   Delivery Time   Amount   Status");

            for (int i = 0; i < orderDict.Count; i++)
            {
                Order o = orderDict[i];
                Customer c = customerDict[i];
                Restaurants r = restaurantDict[i];
                string customerName = c.EmailAddr;
                string restaurantName = r.RestaurantId;

                // Find customer name
                for (int j = 0; j < customerDict.Count; j++)
                {
                    if (customerDict[j].EmailAddr == c.EmailAddr)
                    {
                        customerName = customerDict[j].CustomerName;
                        break;
                    }
                }

                // Find restaurant name
                for (int j = 0; j < restaurantDict.Count; j++)
                {
                    if (restaurantDict[j].RestaurantId == r.RestaurantId)
                    {
                        restaurantName = restaurantDict[j].RestaurantName;
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