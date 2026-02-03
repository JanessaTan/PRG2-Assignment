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
    class Customer
    {
        private string emailAddr;
        private string customerName;

        public string EmailAddr { get; set; }
        public string CustomerName { get; set; }

        public List<Order> Orders = new List<Order>();

        public Customer() { }
        public Customer(string emailAddr, string customer)
        {
            EmailAddr = emailAddr;
            CustomerName = customer;
        }

        // +AddOrder(Order)
        public void AddOrder(Order o)
        {
            if (o != null)
                Orders.Add(o);
        }

        // +DisplayAllOrders() 
        public void DisplayAllOrders()
        {
            if (Orders.Count == 0)
            {
                Console.WriteLine("No orders.");
                return;
            }
            foreach (Order o in Orders)
                Console.WriteLine(o.ToString());
        }

        // +RemoveOrder(Order) : bool
        public bool RemoveOrder(Order o)
        {
            return Orders.Remove(o);
        }

        // Modify /Delete features 
        public List<Order> GetOrdersByStatus(string status)
        {
            List<Order> list = new List<Order>();
            foreach (Order o in Orders)

            {
                if (o.OrderStatus = status)
                    list.Add(o);
            }
            return list;
        }

        public List<Order> GetAllOrders()
        {
            return Orders;
        }

        // +ToString():string
        public override string ToString()
        {
            return CustomerName +" (" + EmailAddr + ")";
        }
    }
}