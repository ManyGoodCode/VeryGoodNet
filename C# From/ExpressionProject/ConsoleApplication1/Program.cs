using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace TestExpressionStep1
{
    class Program
    {
        static void Main(string[] args)
        {
            // 进程1；可以将Lambda表达式通过Lamda表达式类进行包装
            Console.WriteLine("----------------------------进程1----------------------------");
            Expression<Func<int, int>> e1 = x => x + 1;
            //Expression<Func<int, int, int>> e2 = (x, y) => { return x + y; }                       // 编译不通过
            Expression<Func<int, int, int>> e3 = (x, y) => x + y;
            //Expression<Action<int>> e4 = x => { };                                                 // 编译不通过
            Console.WriteLine(e1);
            Console.WriteLine(e3);

            // 进程2；解析Lambda表达式
            Console.WriteLine("----------------------------进程2----------------------------");
            LambdaExpression lamda1 = e1 as LambdaExpression;
            Console.WriteLine(string.Format("Body:{0} Return:{1}", lamda1.Body, lamda1.ReturnType.ToString()));
            foreach (ParameterExpression parameter in lamda1.Parameters)
            {
                Console.WriteLine(string.Format("Name:{0} Type:{1}", parameter.Name, parameter.Type.ToString()));
            }

            // 进程3；创建并执行Action表达式
            Console.WriteLine("----------------------------进程3----------------------------");
            LoopExpression loop = Expression.Loop(body:
                Expression.Call(
                instance: null,
                method: typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }),
                arguments: Expression.Constant("Hello")
                ));
            // 创建一个代码块表达式包含我们上面的代码
            BlockExpression block = Expression.Block(loop);
            Expression<Action> lamda2 = Expression.Lambda<Action>(block);
            //lamda2.Compile().Invoke();                                                              // 编译后执行

            // 进程4；创建并执行数组遍历表达式
            Console.WriteLine("----------------------------进程4----------------------------");
            ParameterExpression arrayExpression = Expression.Parameter(type: typeof(int[]), name: "Array");
            ParameterExpression indexExpression = Expression.Parameter(type: typeof(int), name: "Index");
            ParameterExpression valueExpression = Expression.Parameter(type: typeof(int), name: "Value");
            Expression arrayAccess = Expression.ArrayAccess(arrayExpression, indexExpression);                                                                                         // 函数为 Lambda<TDelegate>(Expression body, params ParameterExpression[] parameters);

            Expression<Func<int[], int, int, int>> lamda3 = Expression.Lambda<Func<int[], int, int, int>>(
                Expression.Assign(left: arrayAccess, right: Expression.Add(left: arrayAccess, right: valueExpression)),
                arrayExpression,
                indexExpression,
                valueExpression);
            Console.WriteLine("ArrayAccess:{0}  Lamda3:{1}", arrayAccess.ToString(), lamda3.ToString());
            Console.WriteLine(lamda3.Compile().Invoke(arg1: new int[] { 10, 20, 30 }, arg2: 0, arg3: 5));

            // 进程5；创建对象表达式
            Console.WriteLine("----------------------------进程5----------------------------");
            NewExpression createExpression = Expression.New(type:typeof(Dictionary<int,string>));
            Console.WriteLine(createExpression.ToString());

            // 进程6；比较大小
            Console.WriteLine("----------------------------进程6----------------------------");
            Expression<Func<int, int, bool>> largeSum = (num1, num2) => (num1 + num2) > 1000;
            InvocationExpression invokeExpression = Expression.Invoke(largeSum, Expression.Constant(539), Expression.Constant(281));
            Console.WriteLine(invokeExpression.ToString());
            while (true) ;
        }
    }
}
