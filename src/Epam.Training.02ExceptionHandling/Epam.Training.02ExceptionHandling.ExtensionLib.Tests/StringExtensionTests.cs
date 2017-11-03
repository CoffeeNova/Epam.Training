using System;
using NUnit.Framework;

namespace Epam.Training._02ExceptionHandling.ExtensionLib.Tests
{
    [TestFixture]
    class StringExtensionTests
    {
        private string _testString = "asdad56cxvhe24e@";
        
        [Test]
        [Combinatorial]
        public void ParseInt_should_return_integers([Values("4", "13", "566", "-777", "55555")] string value)
        {
            //Arrange
            var expected = int.Parse(value);

            //Act
            var actual = value.ParseInt();

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Should_throw_ArgumentNullException()
        {
            //Arrange
            string value = null;

            //Act / Assert
            // ReSharper disable once ExpressionIsAlwaysNull
             Assert.Throws<ArgumentNullException>(() => value.ParseInt());
        }

        [TestCase("7b", 1)]
        [TestCase("1b3", 1)]
        [TestCase("-7!", 2)]
        [TestCase("-72<asd7", 3)]
        public void Should_throw_NumberException(string value, int position)
        {
            //Act / Assert
            // ReSharper disable once ExpressionIsAlwaysNull
            var ex = Assert.Throws<NumberException>(() => value.ParseInt());
            Assert.AreEqual(position, ex.CharPosition);
        }

        [Test]
        public void Should_return_only_first_number()
        {
            //Arrange
            var expected = 56;

            //Act
            var actual = _testString.ParseFirstNumber();

            //Assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Should_return_all_numbers_in_string()
        {
            //Arrange
            var expected = new [] {56, 24};

            //Act
            var actual = _testString.ParseNumbers();

            //Assert
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}
