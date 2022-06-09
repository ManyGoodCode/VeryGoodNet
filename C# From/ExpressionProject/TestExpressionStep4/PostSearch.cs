using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TestExpressionStep4
{
    public class PostSearch : IEnumerable<Post>
    {
        private SearchDto dto;
        public PostSearch Where(Expression<Func<Post, Boolean>> predicate)
        {
            dto = new PostExpressionVisitor().ProcessExpression(predicate);
            return this;
        }

        public PostSearch Select<TResult>(Expression<Func<Post, TResult>> selector)
        {
            return this;
        }

        IEnumerator<Post> IEnumerable<Post>.GetEnumerator()
        {
            return (IEnumerator<Post>)((IEnumerable)this).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            string url = PostHelper.BuildUrl(dto);
            IEnumerable<Post> posts = PostHelper.FindWebEntity(url);
            return posts.GetEnumerator();
        }
    }
}
