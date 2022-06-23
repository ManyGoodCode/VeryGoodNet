using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApplication2.Models;
using WebApplication2.Repository;

namespace WebApplication2.Business
{
    public class DocumentManager
    {
        public readonly  DocumentRepository DocumentRepository = new DocumentRepository();
        public List<Document> GetAllDocument()
        {
            return DocumentRepository.GetAll();
        }

        public Document GetDocumentByID(int id)
        {
            return DocumentRepository.GetByID(id);
        }
    }
}