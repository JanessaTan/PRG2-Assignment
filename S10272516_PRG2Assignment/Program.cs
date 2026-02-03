using S10272516_PRG2Assignment;

//==========================================================
// Student Number : S10272516
// Student Name : Janessa Tan Jia En
// Partner Name : Than Thar Wint Htal
//==========================================================

List<Restaurants> rCollection = new List<Restaurants>();

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
            rCollection.Add(r);
        }
    }
}

Dictionary<string, FoodItem> rID_fi_dict = new Dictionary<string, FoodItem>();

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
            double fiPrice = Convert.ToDouble(data[3]);
            FoodItem fi = new FoodItem(fiName, fiDesc, fiPrice, null);
            rID_fi_dict.Add(rID, fi);
        }
    }
}

List<Customer> cCollection = new List<Customer>();

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
            rCollection.Add(c);
        }
    }
}

List<Order> oCollection = new List<Order>();

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
            DateOnly oDeliveryDate = Convert.ToDateTime(data[3]);
            TimeOnly oDeliveryTime = Convert.ToDateTime(data[4]);
            DateTime oDeliveryDateTime = new DateTime(oDeliveryDate, oDeliveryTime);
            string oDeliveryAddress = data[5];
            DateTime oDateTime = Convert.ToDateTime(data[6]);
            double oTotal = Convert.ToDouble(data[7]);
            string oStatus = data[8];
            string foodItems = data[9].Split("|");
            string foodItemInfo = foodItems.Split(",");
            foodItemInfo.Trim();
            string fName = foodItemInfo[0];
            int fQty = foodItemInfo[1];
            OrderedFoodItem ofi = new OrderedFoodItem(fName, null, null, null, fQty, oTotal);
            Order o = new Order(oID, oDateTime, oTotal, oStatus, oDeliveryDateTime, oDeliveryAddress, null, null);
            o.AddOrderedFoodItem(ofi);
            oCollection.Add(o);
        }
    }
}


Console.WriteLine("Welcome to the Gruberoo Food Delivery System");

InitializeRestaurants();
Console.WriteLine($"{rCollection.Count} restaurants loaded!");

InitializeFoodItems();
Console.WriteLine($"{rID_fi_dict.Count} food items loaded!");

InitializeCustomers();
Console.WriteLine($"{cCollection.Count} customers loaded!");

InitializeOrders();
Console.WriteLine($"{oCollection.Count} orders loaded!");