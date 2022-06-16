using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPMVC1.Models.ViewModel
{
    public class FooterViewModel
    {
        public string CompanyName { get; set; }
        public string Year { get; set; }
    }

    public class FiveMainViewModel
    {
        public FooterViewModel Foot { get; set; }
        public string Text { get; set; }
    }
}