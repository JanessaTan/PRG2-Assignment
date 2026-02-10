using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S10272516_PRG2Assignment
{
    class SpecialOffer
    {
        public string OfferCode { get; set; }
        public string OfferDesc { get; set; }
        public double Discount { get; set; }

        public SpecialOffer() { }

        public SpecialOffer(string code, string desc, double discount)
        {
            OfferCode = code;
            OfferDesc = desc;
            Discount = discount;
        }

        public override string ToString()
        {
            return OfferCode + " - " + OfferDesc + " (" + (Discount * 100) + "% off)";
        }
    }
}
