using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPMVC1.Models
{
    public class School
    {
        public string Name { get; set; }

        public Address Address { get; set; }
    }

    public class Address
    {
        public string Province { get; set; }
        public string City { get; set; }
    }
}