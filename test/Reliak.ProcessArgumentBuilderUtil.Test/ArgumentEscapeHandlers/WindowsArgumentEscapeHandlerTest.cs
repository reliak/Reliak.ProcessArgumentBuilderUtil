using Reliak.ProcessArgumentBuilderUtil.ArgumentEscapeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Reliak.ProcessArgumentBuilderUtil.Test.ArgumentEscapeHandlers
{
    public class WindowsArgumentEscapeHandlerTest
    {
        [Theory]
        [InlineData("-", "sometext", "sometext")]
        [InlineData("-", "some text", "\"some text\"")]
        [InlineData("-", "some-text", "some-text")]

        // arguments starting with sigil
        [InlineData("-", "-some-text", "\"-some-text\"")]
        [InlineData("/", "/some-text", "\"/some-text\"")]

        // argument containing quotes
        [InlineData("-", "this\"is\"cool", "^\"this\\^\"is\\^\"cool^\"")]

        // argument containing & meta character
        [InlineData("-", "this\"&whoami\"", "^\"this\\^\"^&whoami\\^\"^\"")]

        // argument ending with backslash
        [InlineData("-", "test\\", "test\\")]
        [InlineData("-", "test file\\", "\"test file\\\\\"")] // we expect (\\") not (\"), because latter would mean we want to escape the quote
        public void Test_Quoting(string sigil, string input, string expected)
        {
            // Arrange
            var handler = new WindowsArgumentEscapeHandler(sigil);

            // Act
            var result = handler.Escape(input);

            // Assert
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Test_Null()
        {
            // Arrange
            var handler = new WindowsArgumentEscapeHandler("/");

            // Act
            var result = handler.Escape(null);

            // Assert
            Assert.Equal("\"\"", result);
        }
    }
}