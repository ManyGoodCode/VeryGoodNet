using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;

namespace WebApplication2.Repository
{
    public class DocumentRepository
    {
        private static List<Document> Documents = new List<Document>()
        {
            new Document(){ ID = 1, UserName = "用户1", Title = "标题1", Content ="内容1"},
            new Document(){ ID = 2,  UserName =  "用户2", Title = "标题2", Content ="内容2"},
            new Document(){ ID = 3,UserName = "用户3", Title = "标题3", Content ="内容3"},
            new Document(){ID = 4, UserName = "用户4", Title = "标题4", Content ="内容4"},
        };

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