using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using WebApplication2.Repository;

namespace WebApplication2.Business
{
    public class DocumentManager
    {
        public readonly DocumentRepository DocumentRepository = new DocumentRepository();
        public List<Dcument> GetAllDocument()
        {
            //return new EFResitory().GetAll();
            //return new ADOResitory().GetAll();
            return DocumentRepository.GetAll();
        }

        public Dcument GetDocumentByID(int id)
        {
            //return new EFResitory().GetByID(id);
            //return new ADOResitory().GetByID(id);
            return DocumentRepository.GetByID(id);
        }
    }

    public class ADOResitory
    {
        static string ConnectString = "Data Source=(LocalDB)\\MSSQLLocalDB;AttachDbFilename=C:\\WORK\\Code\\Company Project\\Hub\\VeryGoodNet\\C# ASP\\Magic\\Magic Test\\WebApplication2\\WebApplication2\\bin\\AADB\\AADB.mdf;Integrated Security = True;";
        public List<Dcument> GetAll()
        {
            string queryString = "select * FROM [dbo].[Dcument]";
            List<Dcument> results = new List<Dcument>();
            using (SqlConnection connection = new SqlConnection(connectionString: ConnectString))
            {
                using (SqlCommand command = new SqlCommand(cmdText: queryString, connection: connection))
                {
                    try
                    {
                        connection.Open();
                        using (SqlDataReader read = command.ExecuteReader())
                        {
                            while (read.Read())
                            {
                                results.Add(new Dcument()
                                {
                                    Id = int.Parse(read["Id"].ToString()),
                                    Title = read["Title"].ToString(),
                                    Content = read["Content"].ToString(),
                                    UserName = read["UserName"].ToString(),
                                });
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace.ToString() + ex.Message);
                    }
                }
            }

            return results;
        }

        public Dcument GetByID(int id)
        {
            return GetAll().FirstOrDefault(d => d.Id == id);
        }
    }

    // 当已经存在数据库模型的时候，通过数据库模型构建代码访问【用到EntityFramework】
    // Add -> New Item -> ADO.NET Entity Data Model
    public class EFResitory
    {
        public List<Dcument> GetAll()
        {
            using (Model1 context = new Model1())
            {
                return context.Dcuments.ToList();
            }
        }

        public Dcument GetByID(int id)
        {
            using (Model1 context = new Model1())
            {
                return context.Dcuments.FirstOrDefault(d => d.Id == id);
            }
        }
    }

    public class SerialPortManager
    {
        public readonly System.IO.Ports.SerialPort SP = new System.IO.Ports.SerialPort();
        static readonly object lck = new object();
        static SerialPortManager instance;

        private SerialPortManager()
        {
            SP.BaudRate = 115200;
            SP.PortName = "COM4";
        }

        public static SerialPortManager GetInstance()
        {
            if (instance == null)
            {
                lock (lck)
                {
                    instance = instance ?? new SerialPortManager();
                }
            }

            return instance;
        }

        public string Send(string text)
        {
            lock (SP)
            {
                if (!SP.IsOpen)
                    SP.Open();
                SP.Write(text);
                Thread.Sleep(3000);
                return "接收数据" + SP.ReadExisting();
            }
        }
    }
}