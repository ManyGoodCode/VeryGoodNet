using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Repository
{
    public class DocumentRepository
    {
        private static List<Document> Documents = new List<Document>();
        private static readonly int Count = 20;

        static DocumentRepository()
        {
            for (int i = 0; i < Count; i++)
            {
                Documents.Add(new Document()
                {
                     ID = i,
                     UserName = "用户" + i,
                     Title = "标题" + i,
                     Content = "内容" + i,
                });
            }
        }

        public List<Document> GetAll()
        {
            return Documents;
        }

        public Document GetByID(int id)
        {
            return Documents.FirstOrDefault(d => d.ID == id);
        }
    }
}