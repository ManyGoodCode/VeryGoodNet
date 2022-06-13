using ASPMVC1.DataAccessLayer;
using ASPMVC1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPMVC1.BusinessLayer
{
    public class PersonService
    {
        public List<Person> GetPersons()
        {
            PersonDal personDal = new PersonDal();
            return personDal.Employees.ToList();
        }

        public void SavePerson(Person p)
        {
            PersonDal personDal = new PersonDal();
            personDal.Employees.Add(p);
            personDal.SaveChanges();
        }
    }
}