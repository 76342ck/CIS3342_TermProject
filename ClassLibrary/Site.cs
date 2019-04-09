using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Site
    {
        public string SiteDesc { get; set; }
        public string APIKey { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }

        public Site()
        {

        }

        public Site(string desc, string key, string email, string addr, string phone)
        {
            this.SiteDesc = desc;
            this.APIKey = key;
            this.Email = email;
            this.Address = addr;
            this.PhoneNo = phone;
        }
    }
}
