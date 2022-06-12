using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ASPMVC1.Models.ViewModel
{
    public class StudentListViewModel
    {
        public List<StudentViewModel> Students { get; set; }
        public string LoginName { get; set; }
    }

    public class StudentViewModel
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public string ScoreColor { get; set; }
    }
}