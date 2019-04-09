using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary
{
    public class Department
    {
        public string DepartmentID { get; set; }
        public string DepartmentName { get; set; }

        public Department()
        {

        }

        public Department(string id, string name)
        {
            this.DepartmentID = id;
            this.DepartmentName = name;
        }
    }
}
