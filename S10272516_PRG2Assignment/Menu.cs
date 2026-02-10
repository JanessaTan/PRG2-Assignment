using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10272516_S10274395_PRG2Assignment
{
    class Menu
    {
        public string MenuId { get; set; }
        public string MenuName { get; set; }
        public string RestaurantId { get; set; }
        public List<FoodItem> items = new List<FoodItem>();

        public Menu() { }

        public Menu(string id, string name, string restaurantId)
        {
            MenuId = id;
            MenuName = name;
            RestaurantId = restaurantId;
        }

        public void AddFoodItem(FoodItem item)
        {
            if (item != null)
                items.Add(item);
        }

        public bool RemoveFoodItem(FoodItem item)  // FIXED: typo was RemoveFoodItme
        {
            return items.Remove(item);
        }

        public void DisplayFoodItems()
        {
            if (items.Count == 0)
            {
                Console.WriteLine("(no items)");
                return;
            }

            foreach (var fi in items)
                Console.WriteLine(" - " + fi.ToString());
        }

        public List<FoodItem> GetAllItems()
        {
            return new List<FoodItem>(items);
        }

        public override string ToString()
        {
            return $"Menu: {MenuName} ({MenuId})";
        }
    }
}
