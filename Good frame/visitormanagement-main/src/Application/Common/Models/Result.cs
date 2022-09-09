using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;

namespace CleanArchitecture.Blazor.Application.Common.Models
{
    /// <summary>
    /// 封装结果状态和错误信息。
    /// A. 非泛型结果
    /// 1. 静态Success 同步/异步方法; 设置状态True, 错误信息 Array.Empty
    /// 2. 静态Failure 同步/异步方法; 设置状态False,错误信息 errors
    /// 
    /// 
    /// B. 泛型结果 继承 非泛型结果
    /// 1. new 静态Success 同步/异步方法; 设置状态True, 错误信息 Array.Empty
    /// 2. new 静态Failure 同步/异步方法; 设置状态False,错误信息 errors
    /// </summary>
    public class Result : IResult
    {
        internal Result()
        {

        }

        internal Result(bool succeeded, IEnumerable<string> errors)
        {
            Succeeded = succeeded;
            Errors = errors.ToArray();
        }

        public bool Succeeded { get; set; }

        public string[] Errors { get; set; }

        public static Result Success()
        {
            return new Result(true, Array.Empty<string>());
        }

        public static Task<Result> SuccessAsync()
        {
            return Task.FromResult(new Result(true, Array.Empty<string>()));
        }

        public static Result Failure(IEnumerable<string> errors)
        {
            return new Result(false, errors);
        }

        public static Task<Result> FailureAsync(IEnumerable<string> errors)
        {
            return Task.FromResult(new Result(false, errors));
        }
    }

    public class Result<T> : Result, IResult<T>
    {
        public T Data { get; set; }

        public static new Result<T> Failure(IEnumerable<string> errors)
        {
            return new Result<T> { Succeeded = false, Errors = errors.ToArray() };
        }

        public static new async Task<Result<T>> FailureAsync(IEnumerable<string> errors)
        {
            return await Task.FromResult(Failure(errors));
        }

        public static Result<T> Success(T data)
        {
            return new Result<T> { Succeeded = true, Data = data };
        }

        public static async Task<Result<T>> SuccessAsync(T data)
        {
            return await Task.FromResult(Success(data));
        }
    }
}

