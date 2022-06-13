using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ASPMVC1.Models
{
    public class Person
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Salary { get; set; }
    }

    public class PersonModel
    {
        public string Name { get; set; }

        public int Salary { get; set; }
        public string Color { get; set; }
    }

    public class PersonLstView
    {
        public List<PersonModel> PersonModels { get; set; }

        public string LoginUser { get; set; }
    }
}