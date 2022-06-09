using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestExpressionStep4.Test
{
    public class DoWork
    {
        static void Dork()
        {
            CnblogsQueryProvider provider = new CnblogsQueryProvider();
            Query<Post> queryable = new Query<Post>(provider);
            IQueryable<Post> query =
                from p in queryable
                where p.Diggs >= 10 &&
                p.Comments > 10 &&
                p.Views > 10 &&
                p.Comments < 20
                select p ;

            List<Post> list = query.ToList();
            Console.ReadLine();
        }
    }
}
