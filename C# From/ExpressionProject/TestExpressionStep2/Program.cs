using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace TestExpressionStep2
{
    class Program
    {
        static void Main(string[] args)
        {
            // 进程1；直接返回常量值
            Console.WriteLine("----------------------------进程1----------------------------");
            ConstantExpression constant1 = Expression.Constant(value: 10);
            Expression<Func<int>> e1 = Expression.Lambda<Func<int>>(body: constant1);
            Console.WriteLine(e1.Compile().Invoke());


            // 进程2；在方法体里面创建变量，操作之后再进行返回
            Console.WriteLine("----------------------------进程2----------------------------");
            ParameterExpression para1 = Expression.Parameter(type: typeof(int));
            BlockExpression block1 = Expression.Block(new[] { para1 },
                Expression.AddAssign(left: para1, right: Expression.Constant(20)),
                para1);
            Expression<Func<int>> e2 = Expression.Lambda<Func<int>>(block1);
            Console.WriteLine(e2.Compile().Invoke());


            // 进程3；为参数+10后返回
            Console.WriteLine("----------------------------进程3----------------------------");
            LabelTarget returnTarget = Expression.Label(type: typeof(Int32));
            LabelExpression returnLabel = Expression.Label(target: returnTarget, defaultValue: Expression.Constant(10, typeof(Int32)));
            ParameterExpression para2 = Expression.Parameter(type: typeof(int));
            BlockExpression block2 = Expression.Block(
                arg0: Expression.AddAssign(para2, Expression.Constant(10)),
                arg1: Expression.Return(returnTarget, para2),
                arg2: returnLabel);

            Expression<Func<int, int>> e3 = Expression.Lambda<Func<int, int>>(block2, para2);
            Console.WriteLine(e3.Compile().Invoke(20));


            // 进程4；Switch Case
            Console.WriteLine("----------------------------进程4----------------------------");
            ParameterExpression genPara = Expression.Parameter(type: typeof(int));
            SwitchExpression swExpression = Expression.Switch(
                genPara,
                Expression.Constant("不详"),                                                               // 默认值
                Expression.SwitchCase(Expression.Constant("B"), 
                Expression.Constant(1)),
                Expression.SwitchCase(Expression.Constant("G"),
                Expression.Constant(0))
                );
            Expression<Func<int, string>> e4 = Expression.Lambda<Func<int, string>>(swExpression, genPara);
            Console.WriteLine(e4.Compile().Invoke(1));
            Console.WriteLine(e4.Compile().Invoke(0));
            Console.WriteLine(e4.Compile().Invoke(11));
            while (true) ;
        }
    }
}
