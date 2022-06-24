using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.ViewModel
{
    public class DocumentListView
    {
        public int Count { get; set; }
        public int PageCount { get; set; }
        public int CurrentPage { get; set; }
        public List<Dcument> Docs { get; set; }
    }
}