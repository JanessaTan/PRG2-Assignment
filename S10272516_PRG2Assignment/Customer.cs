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
    public class Customer
    {
        private string emailAddr;
        private string customerName;

        public string EmailAddr { get; set; }
        public string CustomerName { get; set; }

        public Customer() { }
        public Customer(string emailAddr, string customer)
        {
            EmailAddr = emailAddr;
            CustomerName = customer;
        }

        // +AddOrder(Order)
        public void AddOrder(Order o)
        {
            if (o != null) orders.Add(o);
        }

        // +DisplayAllOrders() 
        public voide DisplayAllOrders()
        {
            if (orders.Count == 0)
            {
                Console.WriteLine("No orders.");
                return;
            }
            for each(Order o in orders) Console.WriteLine(o.ToString());
        }

        // +RemoveOrder(Order) : bool
        public bool RemoveOrder(Order o)
        {
            return DisplayAllOrders().Remove(o);
        }

        // Modify /Delete features 
        public List<Order> GetOrdersByStatus(string status)
        {
            List<Order> list = new List<Order>();
            foreach (Order o in orders)

            {
                if (o.OrderStatus = status) list.Add(o);
            }
            return list;
        }

        public List<Order> GetAllOrders()
        {
            return new List<Order>(orders);
        }

        // +ToString():string
        public override string ToString()
        {
            return $"{CustomerName} ({EmailAddress})";
        }


        
    }

}