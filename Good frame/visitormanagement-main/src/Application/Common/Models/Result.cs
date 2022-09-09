using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CleanArchitecture.Blazor.Application.Common.Interfaces;

namespace CleanArchitecture.Blazor.Application.Common.Models
{
    /// <summary>
    /// ��װ���״̬�ʹ�����Ϣ��
    /// A. �Ƿ��ͽ��
    /// 1. ��̬Success ͬ��/�첽����; ����״̬True, ������Ϣ Array.Empty
    /// 2. ��̬Failure ͬ��/�첽����; ����״̬False,������Ϣ errors
    /// 
    /// 
    /// B. ���ͽ�� �̳� �Ƿ��ͽ��
    /// 1. new ��̬Success ͬ��/�첽����; ����״̬True, ������Ϣ Array.Empty
    /// 2. new ��̬Failure ͬ��/�첽����; ����״̬False,������Ϣ errors
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

