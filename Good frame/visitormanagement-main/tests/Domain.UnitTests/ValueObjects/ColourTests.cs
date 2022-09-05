using CleanArchitecture.Blazor.Domain.Exceptions;
using CleanArchitecture.Blazor.Domain.ValueObjects;
using FluentAssertions;
using NUnit.Framework;

namespace CleanArchitecture.Blazor.Domain.UnitTests.ValueObjects
{
    /// <summary>
    /// 单元测试 - 颜色和字符串之间的转换
    /// 
    /// 字符串和颜色之间的转换；已经执行Func委托抛出异常的测试
    /// 
    /// </summary>
    public class ColourTests
    {
        [Test]
        public void ShouldReturnCorrectColourCode()
        {
            string code = "#FFFFFF";
            Colour colour = Colour.From(code);
            colour.Code.Should().Be(expected: code);
        }

        [Test]
        public void ToStringReturnsCode()
        {
            Colour colour = Colour.White;
            colour.ToString().Should().Be(expected: colour.Code);
        }

        [Test]
        public void ShouldPerformImplicitConversionToColourCodeString()
        {
            string code = Colour.White;
            code.Should().Be(expected: "#FFFFFF");
        }

        /// <summary>
        /// 显式转换
        /// </summary>
        [Test]
        public void ShouldPerformExplicitConversionGivenSupportedColourCode()
        {
            Colour colour = (Colour)"#FFFFFF";
            colour.Should().Be(expected: Colour.White);
        }

        /// <summary>
        /// 方法执行抛出异常的捕获
        /// </summary>
        [Test]
        public void ShouldThrowUnsupportedColourExceptionGivenNotSupportedColourCode()
        {
            FluentActions.Invoking(func: () => Colour.From("##FF33CC"))
                .Should().Throw<UnsupportedColourException>();
        }
    }
}

