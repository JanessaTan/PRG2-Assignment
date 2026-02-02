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

        private List<Menu> menus = new List<Menu>();
        public Queu<Order> OrderQueue { get; } = new Queue<Order>();

        // implements (SpecialOffer)
        private List<SpecialOffer> offers = new List<SpecialOffer>();

        public Restaurants() { }
        public Restaurants(string id, string n, string email)
        {
            restaurantId = id;
            restaurantName = n;
            retaurantEmail = email;

        }

        public void AddMenu (Menu m)
        {
            if (m != null) menus.Add(m);
        }

        public bool RemoveMenu (Menu m)
        {
            return menus.Remove(m);
        }

        public List<Menu> GetAllMenus()
        {
            return new List<Menu>(GetAllMenus);
        }

        public void AddSpecialOffer (SpecialOffer offer)
        {
            if (offer != null) offers.Add(offer);
        }

        public List <SpecialOffer> GetAllOffers()
        {
            return new List<SpecialOffer>(offers);

        }

        public void EnqueueOrder(Order o)
        {
            if (o != null) OrderQueue.Enqueue(o);
        }

        // +DisplayMenu() : void
        public void DisplayMenu()
        {
            if (menus.Count == 0)
            {
                Console.WriteLine("(no menus)");
                return;
            }

            foreach (var m in menus)
            {
                Console.WriteLine(m.ToString());
                m.DisplayFoodItems();
            }
        }

        // +DisplayOrders() : void
        public void DisplayOrders()
        {
            if (OrderQueue.Count == 0) { Console.WriteLine("(no orders)"); return; }
            foreach (var o in OrderQueue)
                Console.WriteLine(o.ToString());
        }

        // +DisplaySpecialOffers() :void 
        public void DisplaySpecialOffers()
        {
            if (offers.Count == 0)
            {
                Console.WriteLine("(no special offers)");
                return;
            }

            foreach (var s in offers) Console.WriteLine(" - " +
                s.ToString());
        }

        public override string ToString()
        {
            return $"Restaurant : {restaurantName} (restaurantId}) - {restaurantEmail}";

        }




            
    }
}

