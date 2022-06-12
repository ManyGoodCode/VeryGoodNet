using ASPMVC1.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ASPMVC1.DataAccessLayer
{
    public class PersonDal : DbContext
    {
        // 此处 "AADB" 为配置文件里的的名称
        public PersonDal() : base(nameOrConnectionString:"AADB")
        { 
        
        }

        // 数据库里面的表数据
        public DbSet<Person> Employees { get; set; }

        // TblEmployee”是表名称，是运行时自动生成的。
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Person>().ToTable(tableName: "TblPerson");
            base.OnModelCreating(modelBuilder);
        }
    }
}