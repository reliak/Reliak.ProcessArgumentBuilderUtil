using Reliak.ProcessArgumentBuilderUtil.ArgumentEscapeHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Reliak.ProcessArgumentBuilderUtil.Test
{
    public class ProcessArgumentBuilderTest
    {
        [Theory]
        [InlineData(typeof(WindowsArgumentEscapeHandler), "/")]
        [InlineData(typeof(PosixShellArgumentEscapeHandler), "-")]
        public void Test_With_No_Arguments(Type escapeHandlerType, string sigil)
        {
            // Arrange
            var escapeHandler = Activator.CreateInstance(escapeHandlerType, new object[] { sigil }) as IArgumentEscapeHandler;
            var builder = new ProcessArgumentBuilder(escapeHandler);

            // Act
            var result = builder.Build();

            // Assert
            Assert.Equal("", result);
        }

        [Theory]
        [InlineData(typeof(WindowsArgumentEscapeHandler), "-")]
        [InlineData(typeof(PosixShellArgumentEscapeHandler), "-")]
        public void Test_With_Single_Option(Type escapeHandlerType, string sigil)
        {
            // Arrange
            var escapeHandler = Activator.CreateInstance(escapeHandlerType, new object[] { sigil }) as IArgumentEscapeHandler;
            var builder = new ProcessArgumentBuilder(escapeHandler);
            builder.AddOption("-v");

            // Act
            var result = builder.Build();

            // Assert
            Assert.Equal("-v", result);
        }

        [Theory]
        [InlineData(typeof(WindowsArgumentEscapeHandler), "/")]
        [InlineData(typeof(PosixShellArgumentEscapeHandler), "-")]
        public void Test_With_Single_Argument(Type escapeHandlerType, string sigil)
        {
            // Arrange
            var escapeHandler = Activator.CreateInstance(escapeHandlerType, new object[] { sigil }) as IArgumentEscapeHandler;
            var builder = new ProcessArgumentBuilder(escapeHandler);
            builder.AddArgument("somefile.txt");

            // Act
            var result = builder.Build();

            // Assert
            Assert.Equal("somefile.txt", result);
        }

        [Theory]
        // Windows
        [InlineData(":", "/", "test.txt", "output", "test.txt", "output", typeof(WindowsArgumentEscapeHandler))]
        [InlineData(":", "/", "test.txt", "output folder", "test.txt", "\"output folder\"", typeof(WindowsArgumentEscapeHandler))]

        // Posix
        [InlineData(" ", "-", "test.txt", "output", "test.txt", "output", typeof(PosixShellArgumentEscapeHandler))]
        [InlineData(" ", "-", "test.txt", "output folder", "test.txt", "\"output folder\"", typeof(PosixShellArgumentEscapeHandler))]
        public void Test_Multiple_Arguments(string argumentValueSeparator, string sigil, string firstArgument, string secondArgument,
            string firstArgumentExpectedOutput, string secondArgumentExpectedOutput, Type escapeHandlerType)
        {
            // Arrange
            var s = sigil;
            var escapeHandler = Activator.CreateInstance(escapeHandlerType, new object[] { sigil }) as IArgumentEscapeHandler;
            var builder = new ProcessArgumentBuilder(escapeHandler, argumentValueSeparator);
            builder.AddOption($"{s}help");
            builder.AddNamedArgument($"{s}f", firstArgument);
            builder.AddNamedArgument($"{s}o", secondArgument, "=");

            // Act
            var result = builder.Build();

            // Assert
            Assert.Equal($"{s}help {s}f{argumentValueSeparator}{firstArgumentExpectedOutput} {s}o={secondArgumentExpectedOutput}", result);
        }

        [Theory]
        [InlineData(typeof(WindowsArgumentEscapeHandler), "/")]
        [InlineData(typeof(PosixShellArgumentEscapeHandler), "-")]
        public void Test_That_Argument_Starting_With_Sigil_Is_Quoted(Type escapeHandlerType, string sigil)
        {
            // Arrange
            var escapeHandler = Activator.CreateInstance(escapeHandlerType, new object[] { sigil }) as IArgumentEscapeHandler;
            var builder = new ProcessArgumentBuilder(escapeHandler);
            builder.AddArgument($"{sigil}test");

            // Act
            var result = builder.Build();

            // Assert
            Assert.Equal($"\"{sigil}test\"", result);
        }

        private const string SAFE_PLACEHOLDER = "***";

        [Theory]
        // Windows
        [InlineData(":", "mysecret", true, "-p:" + SAFE_PLACEHOLDER, typeof(WindowsArgumentEscapeHandler), "-")]
        [InlineData(":", "notsecret", false, "-p:notsecret", typeof(WindowsArgumentEscapeHandler), "-")]

        // Posix
        [InlineData(" ", "mysecret", true, "-p " + SAFE_PLACEHOLDER, typeof(PosixShellArgumentEscapeHandler), "-")]
        [InlineData(" ", "notsecret", false, "-p notsecret", typeof(PosixShellArgumentEscapeHandler), "-")]
        public void Test_ToStringSafe(string argumentValueSeparator, string argumentValue, bool isSensitive, string expected, Type escapeHandlerType, string sigil)
        {
            // Arrange
            var escapeHandler = Activator.CreateInstance(escapeHandlerType, new object[] { sigil }) as IArgumentEscapeHandler;
            var builder = new ProcessArgumentBuilder(escapeHandler, argumentValueSeparator);
            builder.AddNamedArgument("-p", argumentValue, isSensitiveArgument: isSensitive);

            // Act
            var result = builder.BuildSafe(SAFE_PLACEHOLDER);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
