using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Epam.Training._02ExceptionHandling.ExtensionLib
{
    public static class StringExtension
    {
        /// <summary>
        /// Parses string to the int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">Trows when <paramref name="value"/> is null or empty.</exception>
        /// <exception cref="NumberException">"Throws when <paramref name="value"/> contains not only integers and the negation sign."</exception>
        public static int ParseInt(this string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(value);

            return TryParseToInt(value);
        }


        /// <summary>
        /// Parses the first number.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        /// <exception cref="System.ArgumentException">Throws when <paramref name="value"/> contains not only integers and the negation sign.</exception>
        public static int ParseFirstNumber(this string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(value);

            var match = Regex.Match(value, DigitPattern);
            if (!match.Success)
                throw new ArgumentException("The argument does not contain any positive or negative integers.", nameof(value));
            return match.Value.ParseInt();
        }

        /// <summary>
        /// Parses all numbers.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public static int[] ParseNumbers(this string value)
        {
            if (string.IsNullOrEmpty(value))
                throw new ArgumentNullException(value);

            return Regex.Matches(value, @"-?\d+").OfType<Match‌>().Select(m => ParseInt(m.Value)).ToArray();
        }

        private static int TryParseToInt(string value)
        {
            var result = 0;
            var useNegation = value[0].Equals('-');
            var factoring = new Func<int, int>(cp => // cp - current char position
            {
                if (!char.IsDigit(value[cp]))
                    throw new NumberException("The argument must contain only integers and the negation sign", cp);

                var digitPower = value.Length - (cp + 1);
                var factor = IntPow(10, digitPower); //equals to Math.Pow(10, digitPower) but faster

                return CharToInt(value[cp]) * factor;
            });

            for (var i = useNegation ? 1 : 0; i < value.Length; i++)
            {
                result = useNegation ? result - factoring(i) : result + factoring(i);
            }

            return result;
        }

        private static int IntPow(int num, int exp)
        {
            var result = 1;
            while (exp > 0)
            {
                if (exp % 2 == 1)
                    result *= num;
                exp >>= 1;
                num *= num;
            }

            return result;
        }

        private static int CharToInt(char input)
        {
            if (input < 48 && input > 57)
                throw new ArgumentOutOfRangeException(nameof(input), "Digital char must be from 48 to 57");

            var result = 0;
            if (input >= 48 && input <= 57)
                result = input - '0';

            return result;
        }

        private const string DigitPattern = @"-?\d+";
    }
}
