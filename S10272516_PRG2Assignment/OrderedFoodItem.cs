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
    class OrderedFoodItem : FoodItem
    {
        private int qtyOrdered;
        private double subTotal;

        public int QtyOrdered { get; set; }
        public double SubTotal { get; set; }

        public OrderedFoodItem() : base() { }
        public OrderedFoodItem(string n, string d, double p, string? c, int q, double st) : base(n, d, p, c)
        {
            QtyOrdered = q;
            SubTotal = st;
        }

        public double CalculateSubtotal()
        {
            return QtyOrdered * itemPrice;
        }
    }
}
