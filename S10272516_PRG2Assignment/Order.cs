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

namespace S10272516_PRG2Assignment
{
    class Order
    {
        private int orderID;
        private DateTime orderDateTime;
        private double orderTotal;
        private string orderStatus;
        private DateTime deliveryDateTime;
        private string deliveryAddress;
        private string orderPaymentMethod;
        private bool orderPaid;

        public int OrderID { get; set; }
        public DateTime OrderDateTime { get; set; }
        public double OrderTotal { get; set; }
        public string OrderStatus { get; set; }
        public DateTime DeliveryDateTime { get; set; }
        public string DeliveryAddress { get; set; }
        public string OrderPaymentMethod { get; set; }
        public bool OrderPaid { get; set; }
        public List<OrderedFoodItem> OrderList { get; set; } = new List<OrderedFoodItem>();
        public SpecialOffers Offer { get; set; }

        public Order() { }

        public double CalculateOrderTotal()
        {
            double total = 0;
            foreach (OrderedFoodItem item in OrderList)
            {
                total += item.CalculateSubtotal();
            }

            if (Offer != null)
            {
                total -= total * Offer.discount;
            }

            OrderTotal = total;
            return total;
        }

        public void AddOrderedFoodItem(OrderedFoodItem item)
        {
            OrderList.Add(item);
        }

        public bool RemoveOrderedFoodItem(OrderedFoodItem item)
        {
            return OrderList.Remove(item);
        }

        public void DisplayOrderedFoodItems()
        {
            for (int i = 0; i < OrderList.Count; i++)
            {
                Console.WriteLine($"{i+1}. {OrderList[i].ItemName} - {OrderList[i].QtyOrdered}");
            }
        }

        public override string ToString()
        {
            return $"{OrderID}, Total: ${OrderTotal}, Status: {OrderStatus}";
        }
    }
}
