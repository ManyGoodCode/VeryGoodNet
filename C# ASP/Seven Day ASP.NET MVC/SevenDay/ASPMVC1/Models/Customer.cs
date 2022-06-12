using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPMVC1.Models
{
    public class Customer
    {
        public string Name { get; set; }
        public string Address { get; set; }

        public override string ToString()
        {
            return string.Format("Name:{0} Address:{1}", Name, Address);
        }
    }
}