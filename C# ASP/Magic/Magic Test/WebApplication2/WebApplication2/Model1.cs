using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace WebApplication2
{
    // ���Ѿ��������ݿ�ģ�͵�ʱ��ͨ�����ݿ�ģ�͹���������ʡ��õ�EntityFramework��
    // Add -> New Item -> ADO.NET Entity Data Model
    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=Model1")
        {
        }

        public virtual DbSet<Dcument> Dcuments { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
