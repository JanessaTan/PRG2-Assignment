using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//==========================================================
// Student Number : S10272516
// Student Name : Janessa Tan Jia En
// Partner Name : Than Thar Wint Htal
//========================================================== 

namespace S10272516_S10274395_PRG2Assignment
{
    class FoodItem
    {
        private string itemName;
        private string itemDesc;
        private double itemPrice;
        private string? customize;

        public string ItemName { get; set; }
        public string ItemDesc { get; set; }
        public double ItemPrice { get; set; }
        public string? Customize { get; set; }

        public FoodItem() { }
        public FoodItem(string n, string d, double p, string? c)
        {
            ItemName = n;
            ItemDesc = d;
            ItemPrice = p;
            Customize = c;
        }

        public override string ToString()
        {
            return $"  - {ItemName}: {ItemDesc} - ${ItemPrice}";
        }
    }
}