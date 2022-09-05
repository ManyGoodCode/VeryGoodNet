using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Exceptions;
using CleanArchitecture.Blazor.Application.Common.Interfaces;
using CleanArchitecture.Blazor.Application.Common.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Serilog.Context;

namespace CleanArchitecture.Blazor.Infrastructure.Middlewares
{
    internal class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ICurrentUserService currentUserService;
        private readonly ILogger<ExceptionHandlingMiddleware> logger;
        private readonly IStringLocalizer<ExceptionHandlingMiddleware> localizer;

        public ExceptionHandlingMiddleware(
            ICurrentUserService currentUserService,
            ILogger<ExceptionHandlingMiddleware> logger,
            IStringLocalizer<ExceptionHandlingMiddleware> localizer)
        {
            this.currentUserService = currentUserService;
            this.logger = logger;
            this.localizer = localizer;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                string userId = await currentUserService.UserId();
                if (!string.IsNullOrEmpty(userId))
                    LogContext.PushProperty("UserId", userId);
                string errorId = Guid.NewGuid().ToString();
                LogContext.PushProperty("ErrorId", errorId);
                LogContext.PushProperty("StackTrace", exception.StackTrace);
                Result? responseModel = await Result.FailureAsync(new string[]
                {
                    exception.Message
                });

                HttpResponse response = context.Response;
                response.ContentType = "application/json";
                if (exception is not CustomException && exception.InnerException != null)
                {
                    while (exception.InnerException != null)
                    {
                        exception = exception.InnerException;
                    }
                }

                if (!string.IsNullOrEmpty(exception.Message))
                {
                    responseModel.Errors = new string[] { exception.Message };
                }

                switch (exception)
                {
                    case CustomException e:
                        response.StatusCode = (int)e.StatusCode;
                        if (e.ErrorMessages is not null)
                        {
                            responseModel.Errors = e.ErrorMessages.ToArray();
                        }
                        break;

                    case KeyNotFoundException:
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;

                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                await response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(responseModel));
            }
        }
    }
}
