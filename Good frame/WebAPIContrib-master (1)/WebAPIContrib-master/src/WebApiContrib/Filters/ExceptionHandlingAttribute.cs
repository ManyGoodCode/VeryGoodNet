using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using WebApiContrib.Messages;

namespace WebApiContrib.Filters
{
    public class ExceptionHandlingAttribute : System.Web.Http.Filters.ExceptionFilterAttribute
    {
        public IDictionary<Type, HttpStatusCode> Mappings { get; private set; }

        public ExceptionHandlingAttribute()
        {
            Mappings = new Dictionary<Type, HttpStatusCode>
            {
                {typeof (ArgumentNullException), HttpStatusCode.BadRequest},
                {typeof (ArgumentException), HttpStatusCode.BadRequest}
            };
        }


        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception != null)
            {
                HttpRequestMessage request = actionExecutedContext.Request;
                Exception exception = actionExecutedContext.Exception;

                if (actionExecutedContext.Exception is HttpException)
                {
                    HttpException httpException = (HttpException)exception;
                    actionExecutedContext.Response =
                        request.CreateResponse(
                            statusCode: (HttpStatusCode)httpException.GetHttpCode(),
                             value: new Error
                             {
                                 Message = exception.Message
                             });
                }
                else if (Mappings.ContainsKey(exception.GetType()))
                {
                    HttpStatusCode httpStatusCode = Mappings[exception.GetType()];
                    actionExecutedContext.Response =
                        request.CreateResponse(
                            statusCode: httpStatusCode,
                            value: new Error
                            {
                                 Message = exception.Message
                            });
                }
                else
                {
                    actionExecutedContext.Response =
                        actionExecutedContext.Request.CreateResponse(
                            statusCode:HttpStatusCode.InternalServerError,
                            value: new Error 
                            { 
                                Message = exception.Message 
                            });
                }
            }
        }
    }
}
