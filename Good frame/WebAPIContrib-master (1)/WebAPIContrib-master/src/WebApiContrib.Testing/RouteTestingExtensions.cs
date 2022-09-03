using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Rhino.Mocks;
using WebApiContrib.Testing.Internal.Extensions;

namespace WebApiContrib.Testing
{
    public static class RouteTestingExtensions
    {
        private static readonly Dictionary<string, Type> httpMethodLookup = new Dictionary<string, Type>
            {
                {"GET", typeof(System.Web.Http.HttpGetAttribute)},
                {"POST", typeof(System.Web.Http.HttpPostAttribute)},
                {"DELETE", typeof(System.Web.Http.HttpDeleteAttribute)},
                {"PUT", typeof(System.Web.Http.HttpPutAttribute)},
                {"HEAD", typeof(System.Web.Http.HttpHeadAttribute)},
                {"PATCH", typeof(System.Web.Http.HttpPatchAttribute)},
                {"OPTIONS", typeof(System.Web.Http.HttpOptionsAttribute)},
            };

        public static System.Web.Routing.RouteData ShouldMapTo<TController>(
            this string relativeUrl,
            Expression<Action<TController>> action,
            string httpMethod = "GET") where TController : System.Web.Http.ApiController
        {
            return relativeUrl.Route(httpMethod).ShouldMapTo(action, httpMethod);
        }

        private static System.Web.Routing.RouteData ShouldMapTo<TController>(
            this RouteData routeData) where TController : System.Web.Http.ApiController
        {
            string expectedController = typeof(TController).Name;
            if (expectedController.EndsWith("Controller"))
            {
                expectedController = expectedController.Substring(0, expectedController.LastIndexOf("Controller", StringComparison.Ordinal));
            }

            string actualController = routeData.Values.GetValue("controller").ToString();
            Assert.AreSameString(actualController, expectedController);
            return routeData;
        }

        private static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Action<TController>> action, string httpMethod) where TController : ApiController
        {
            Assert.IsNotNull(routeData, "The URL did not match any route");
            routeData.ShouldMapTo<TController>();

            MethodCallExpression methodCall = (MethodCallExpression)action.Body;
            string actualAction = (routeData.Values.GetValue("action") ?? httpMethod).ToString();
            string expectedAction = methodCall.Method.ActionName();
            Assert.AreSameString(expectedAction, actualAction);

            if (string.Compare(httpMethod, actualAction, StringComparison.OrdinalIgnoreCase) != 0)
            {
                bool hasHttpMethodAttribute = methodCall.Method.HasAttribute(httpMethodLookup[httpMethod]);
                Assert.IsTrue(hasHttpMethodAttribute);
            }

            for (int i = 0; i < methodCall.Arguments.Count; i++)
            {
                ParameterInfo param = methodCall.Method.GetParameters()[i];
                bool isReferenceType = !param.ParameterType.IsValueType;
                bool isNullable = isReferenceType ||
                                  (param.ParameterType.UnderlyingSystemType.IsGenericType && param.ParameterType.UnderlyingSystemType.GetGenericTypeDefinition() == typeof(Nullable<>));

                string controllerParameterName = param.Name;
                bool routeDataContainsValueForParameterName = routeData.Values.ContainsKey(controllerParameterName);
                object actualValue = routeData.Values.GetValue(controllerParameterName);
                object expectedValue = null;
                Expression expressionToEvaluate = methodCall.Arguments[i];

                if (expressionToEvaluate.NodeType == ExpressionType.Convert
                    && expressionToEvaluate is UnaryExpression)
                {
                    expressionToEvaluate = ((UnaryExpression)expressionToEvaluate).Operand;
                }

                switch (expressionToEvaluate.NodeType)
                {
                    case ExpressionType.Constant:
                        expectedValue = ((ConstantExpression)expressionToEvaluate).Value;
                        break;

                    case ExpressionType.New:
                    case ExpressionType.MemberAccess:
                        expectedValue = Expression.Lambda(expressionToEvaluate).Compile().DynamicInvoke();
                        break;
                }

                if (isNullable && (string)actualValue == String.Empty && expectedValue == null)
                {
                    continue;
                }

                if (!isNullable && !routeDataContainsValueForParameterName)
                {
                    object defaultValue = param.ParameterType.GetDefault();
                    actualValue = defaultValue != null ? defaultValue.ToString() : string.Empty;
                }

                if (actualValue == RouteParameter.Optional ||
                    (actualValue != null && actualValue.ToString().Equals(typeof(RouteParameter).FullName)))
                {
                    actualValue = null;
                }

                if (expectedValue is DateTime)
                {
                    actualValue = Convert.ToDateTime(actualValue);
                }
                else
                {
                    expectedValue = (expectedValue == null ? expectedValue : expectedValue.ToString());
                }

                string errorMsgFmt = "Value for parameter '{0}' did not match: expected '{1}' but was '{2}'";
                if (routeDataContainsValueForParameterName)
                {
                    errorMsgFmt += ".";
                }
                else
                {
                    errorMsgFmt += "; no value found in the route context action parameter named '{0}' - does your matching route contain a token called '{0}'?";
                }

                Assert.AreEqual(expectedValue, actualValue, String.Format(errorMsgFmt, controllerParameterName, expectedValue, actualValue));
            }

            return routeData;
        }

        public static object GetValue(this System.Web.Routing.RouteValueDictionary routeValues, string key)
        {
            foreach(string routeValueKey in routeValues.Keys)
            {
                if(!string.Equals(routeValueKey, key, StringComparison.OrdinalIgnoreCase))
                    continue;
                
                if(routeValues[routeValueKey] == null)
                    return null;

                return routeValues[routeValueKey].ToString();
            }

            return null;
        }

        public static RouteData Route(this string url, string httpMethod = "GET")
        {
            System.Web.HttpRequestBase mockRequest = Rhino.Mocks.MockRepository.GeneratePartialMock<HttpRequestBase>();
            mockRequest
                .Expect(x => x.AppRelativeCurrentExecutionFilePath)
                .Return(url)
                .Repeat.Any();
            mockRequest
                .Expect(x => x.PathInfo)
                .Return(string.Empty)
                .Repeat.Any();
            mockRequest
                .Expect(x => x.HttpMethod)
                .Return(httpMethod)
                .Repeat.Any();

            System.Web.HttpContextBase mockContext = MockRepository.GeneratePartialMock<HttpContextBase>();
            mockContext
                .Expect(x => x.Request)
                .Return(mockRequest)
                .Repeat.Any();

            return System.Web.Routing.RouteTable.Routes.GetRouteData(mockContext);
        }
    }
}