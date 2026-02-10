using S10272516_PRG2Assignment;

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
            Console.WriteLine(r.RestaurantId, r.RestaurantName, r.RestaurantEmail);
            restaurantDict.Add(rID, r);
        }
    }
}

List<FoodItem> foodItemList = new List<FoodItem>();
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
            foodItemList.Add(fi);
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
    using (StreamReader sr = new StreamReader("orders.csv"))
    {

        string? line = sr.ReadLine();
        while ((line = sr.ReadLine()) != null)
        {
            string[] data = line.Split(",");
            int oID = Convert.ToInt32(data[0]);
            string cEmail = data[1];
            string rID = data[2];
            DateOnly oDeliveryDate = DateOnly.Parse(data[3]);
            TimeOnly oDeliveryTime = TimeOnly.Parse(data[4]);
            DateTime oDeliveryDateTime = new DateTime(oDeliveryDate, oDeliveryTime);
            string oDeliveryAddress = data[5];
            DateTime oDateTime = Convert.ToDateTime(data[6]);
            double oTotal = Convert.ToDouble(data[7]);
            string oStatus = data[8];
            string foodDatas = "";
            int foodQty = 0;
            string[] foodNameQty;
            int firstQuote = line.IndexOf('"');
            string[] foodData;

            if (firstQuote != -1)
            {
                foodDatas = line.Substring(firstQuote + 1, line.Length - firstQuote - 2);
                foodData = foodDatas.Split("|");

                foreach (var itm in foodData)
                {
                    foodNameQty = itm.Split(",");
                    string foodName = foodNameQty[0].Trim();
                    if (foodNameQty.Count() > 1)
                    {
                        foodQty = Convert.ToInt32(foodNameQty[1].Trim());
                    }
                    else
                    {
                        foodQty = 1;
                    }
                    Order o = new Order();
                    OrderedFoodItem ofi = new OrderedFoodItem(foodName, null, 0, null, foodQty, o.CalculateOrderTotal()); // CHANGE!!!
                    o.AddOrderedFoodItem(ofi);
                    orderDict.Add(oID, o);
                }
            }

            //    Order o = new Order();
            //    FoodItem fi = foodItemDict[fName];
            //    OrderedFoodItem ofi = new OrderedFoodItem(fNames, fi.ItemDesc, fi.ItemPrice, null, fQty, oTotal);
            //    o.AddOrderedFoodItem(ofi);
            //    orderDict.Add(oID, o);
            //}
        }
    }
}


Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

InitializeRestaurants();
Console.WriteLine($"{restaurantDict.Count} restaurants loaded!");

InitializeFoodItems();
Console.WriteLine($"{foodItemList.Count} food items loaded!");

InitializeCustomers();
Console.WriteLine($"{customerDict.Count} customers loaded!");

InitializeOrders();
Console.WriteLine($"{orderDict.Count} orders loaded! (WORK IN PROGRESS)");
