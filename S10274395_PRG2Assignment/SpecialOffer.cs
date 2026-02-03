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
    class SpecialOffer
    {
        private string offerCode;
        private string offerDesc;
        private double discount;

        public string OfferCode  { get; set; }
        public string OfferDesc {  get; set; }
        public double Discount { get; set; }

        public SpecialOffer() { }
        public SpecialOffer(string code, string desc, double discount)
        {
            OfferCode = code;
            OfferDesc = desc;
            Discount = dicount;
        }

        public override string ToString()
        {
            return $"{offerCode} - {offerDesc} ({discount * 100:0.#} % off)"
;
        }

        //
        class SpecialOffers : SpecialOffer
        {
            public SpecialOffers() : base() { }
            public SpecialOffers(string code, string desc, double discountPercent)
                : base(code, desc, dicount) { }
        }
    }
}
