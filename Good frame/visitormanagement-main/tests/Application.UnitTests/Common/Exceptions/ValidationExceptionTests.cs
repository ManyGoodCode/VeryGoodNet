using CleanArchitecture.Blazor.Application.Common.Exceptions;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace CleanArchitecture.Blazor.Application.UnitTests.Common.Exceptions
{
    /// <summary>
    /// 失败验证器的单元测试
    /// </summary>
    public class ValidationExceptionTests
    {
        [Test]
        public void SingleValidationFailureCreatesASingleElementErrorDictionary()
        {
            List<ValidationFailure> failures = new List<ValidationFailure>
            {
                new ValidationFailure("Age", "'Age' must be over 18"),
            };

            List<string> actual = new ValidationException(failures: failures).ErrorMessages;
            actual.Should().BeEquivalentTo(new List<string>() { "'Age' must be over 18" });
        }

        /// <summary>
        /// ValidationException 内部已经实现了对集合属性的 GroupBy / Select
        /// </summary>
        [Test]
        public void MulitpleValidationFailureForMultiplePropertiesCreatesAMultipleElementErrorDictionaryEachWithMultipleValues()
        {
            List<ValidationFailure> failures = new List<ValidationFailure>
            {
                new ValidationFailure("Age", "'Age' must be 18 or older"),
                new ValidationFailure("Age", "'Age' must be 25 or younger"),
                new ValidationFailure("Password", "'Password' must contain at least 8 characters"),
                new ValidationFailure("Password", "'Password' must contain a digit"),
                new ValidationFailure("Password", "'Password' must contain upper case letter"),
                new ValidationFailure("Password", "'Password' must contain lower case letter"),
            };

            List<string> actual = new ValidationException(failures: failures).ErrorMessages;
            actual.Count.Should().Be(2);
            actual.Should().Contain(r => r.Equals("'Age' must be 18 or older, 'Age' must be 25 or younger"));
        }
    }
}

