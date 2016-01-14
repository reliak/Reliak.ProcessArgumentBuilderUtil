using Reliak.ProcessArgumentBuilderUtil.ArgumentEscapeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Reliak.ProcessArgumentBuilderUtil.Test.ArgumentEscapeHandlers
{
    public class PosixShellArgumentEscapeHandlerTest
    {
        [Theory]
        [InlineData("-", "sometext", "sometext")]
        [InlineData("-", "some text", "\"some text\"")]
        [InlineData("-", "some-text", "some-text")]

        // arguments starting with sigil
        [InlineData("-", "-some-text", "\"-some-text\"")]

        // argument containing quotes
        [InlineData("-", "this\"is\"cool", "\"this\\\"is\\\"cool\"")]

        // argument ending with backslash
        [InlineData("-", "test\\", "\"test\\\\\"")]
        [InlineData("-", "test file\\", "\"test file\\\\\"")]
        public void Test_Quoting(string sigil, string input, string expected)
        {
            // Arrange
            var handler = new PosixShellArgumentEscapeHandler(sigil);

            // Act
            var result = handler.Escape(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test_Null()
        {
            // Arrange
            var handler = new PosixShellArgumentEscapeHandler("-");

            // Act
            var result = handler.Escape(null);

            // Assert
            Assert.Equal("\"\"", result);
        }
    }
}