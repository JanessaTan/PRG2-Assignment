using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10272516_S10274395_PRG2Assignment
{
    class Customer
    {
        public string EmailAddr { get; set; }
        public string CustomerName { get; set; }
        public List<Order> Orders = new List<Order>();

        public Customer() { }

        public Customer(string emailAddr, string customer)
        {
            EmailAddr = emailAddr;
            CustomerName = customer;
        }

        public void AddOrder(Order o)
        {
            if (o != null)
                Orders.Add(o);
        }

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

        public bool RemoveOrder(Order o)
        {
            return Orders.Remove(o);
        }

        public List<Order> GetOrdersByStatus(string status)
        {
            List<Order> list = new List<Order>();
            foreach (Order o in Orders)
            {
                if (o.OrderStatus == status)
                    list.Add(o);
            }
            return list;
        }

        public List<Order> GetAllOrders()
        {
            return Orders;
        }

        public override string ToString()
        {
            return CustomerName + " (" + EmailAddr + ")";
        }
    }
}
