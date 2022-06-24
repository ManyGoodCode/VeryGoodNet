using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication2.Repository
{
    public class DocumentRepository
    {
        private static List<Dcument> Documents = new List<Dcument>();
        private static readonly int Count = 20;

        static DocumentRepository()
        {
            for (int i = 0; i < Count; i++)
            {
                Documents.Add(new Dcument()
                {
                    Id = i,
                     UserName = "用户" + i,
                     Title = "标题" + i,
                     Content = "内容" + i,
                });
            }
        }

        public List<Dcument> GetAll()
        {
            return Documents;
        }

        public Dcument GetByID(int id)
        {
            return Documents.FirstOrDefault(d => d.Id == id);
        }
    }

    public class DocumentDataBaseRepository
    {

    }
}