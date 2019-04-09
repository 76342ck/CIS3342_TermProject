using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Product
    {
        public string ProductID { get; set; }
        public string ProductDesc { get; set; }
        public float Price { get; set; }
        public string ImgURL { get; set; }

        public Product()
        {

        }

        public Product(string id, string desc, float price, string imgURL)
        {
            this.ProductID = id;
            this.ProductDesc = desc;
            this.Price = price;
            this.ImgURL = imgURL;
        }
    }
}
