using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TestExpressionStep4.Test
{
    public class CnblogsQueryProvider : QueryProvider
    {
        public override string GetQueryText(Expression expression)
        {
            // 翻译查询条件
            SearchDto dto = new PostExpressionVisitor().ProcessExpression(expression);
            // 生成URL
            string url = PostHelper.BuildUrl(dto);
            return url;
        }

        public override object Execute(Expression expression)
        {
            string url = GetQueryText(expression);
            IEnumerable<Post> results = PostHelper.FindWebEntity(url);
            return results;
        }
    }
}
