using System.Collections.Generic;
using System.Linq;
using CleanArchitecture.Blazor.Domain.Common;
using CleanArchitecture.Blazor.Domain.Exceptions;

namespace CleanArchitecture.Blazor.Domain.ValueObjects
{
    public class Colour : ValueObject
    {
        static Colour() { }
        private Colour() { }

        private Colour(string code)
        {
            Code = code;
        }

        public static Colour From(string code)
        {
            Colour colour = new Colour { Code = code };
            if (!SupportedColours.Contains(colour))
            {
                throw new UnsupportedColourException(code);
            }

            return colour;
        }

        public static Colour White => new Colour("#FFFFFF");
        public static Colour Red => new Colour("#FF5733");
        public static Colour Orange => new Colour("#FFC300");
        public static Colour Yellow => new Colour("#FFFF66");
        public static Colour Green => new Colour("#CCFF99 ");
        public static Colour Blue => new Colour("#6666FF");
        public static Colour Purple => new Colour("#9966CC");
        public static Colour Grey => new Colour("#999999");
        public string Code { get; private set; }

        public static implicit operator string(Colour colour)
        {
            return colour.ToString();
        }

        /// <summary>
        /// 显示转换
        /// </summary>
        public static explicit operator Colour(string code)
        {
            return From(code);
        }

        public override string ToString()
        {
            return Code;
        }

        protected static IEnumerable<Colour> SupportedColours
        {
            get
            {
                yield return White;
                yield return Red;
                yield return Orange;
                yield return Yellow;
                yield return Green;
                yield return Blue;
                yield return Purple;
                yield return Grey;
            }
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Code;
        }
    }
}
