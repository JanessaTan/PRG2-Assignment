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

    public class Restaurants
    {
        private string restaurantId;
        private string restaurantName;
        private string restaurantEmail;


        public string RestaurantId { get; set; }
        public string RestaurantName { get; set; }
        public string RestaurantEmail { get; set; }

        public List<Menu> Menus = new List<Menu>();
        public Queue<Order> Orders = new List<Order>();
        public List<SpecialOffer> Offers = new List<SpecialOffer>();

        public Restaurants() { }
        public Restaurants(string id, string n, string email)
        {
            restaurantId = id;
            restaurantName = n;
            restaurantEmail = email;

        }

        public void AddMenu(Menu m)
        {
            if (m != null) Menus.Add(m);
        }

        public bool RemoveMenu(Menu m)
        {
            return Menus.Remove(m);
        }

        public List<Menu> GetAllMenus()
        {
            return Menus;
        }

        public void AddSpecialOffer(SpecialOffer offer)
        {
            if (offer != null)
                Offers.Add(offer);
        }

        public List<SpecialOffer> GetAllOffers()
        {
            return Offers;

        }

        public void AddOrder(Order o)
        {
            if (o != null)
                Orders.Add(o);
        }

        // +DisplayMenu() : void
        public void DisplayMenu()
        {
            if (Menus.Count == 0)
            {
                Console.WriteLine("(no menus)");
                return;
            }

            foreach (var m in Menus)
            {
                Console.WriteLine("Menu: " + m.MenuName);
                m.DisplayFoodItems();
            }
        }

        // +DisplayOrders() : void
        public void DisplayOrders()
        {
            if (Orders.Count == 0)
            {
                Console.WriteLine("(no orders)");
                return;
            }
            foreach (var o in Orders)
                Console.WriteLine(o.ToString());
        }

        // +DisplaySpecialOffers() :void 
        public void DisplaySpecialOffers()
        {
            if (Offers.Count == 0)
            {
                Console.WriteLine("(no special offers)");
                return;
            }

            foreach (var s in Offers)
                Console.WriteLine(" - " +s.ToString());
        }

        public override string ToString()
        {
            return "Restaurant: " + RestaurantName + " (" + RestaurantId + ") - " + RestaurantEmail;

        }
    }
}

