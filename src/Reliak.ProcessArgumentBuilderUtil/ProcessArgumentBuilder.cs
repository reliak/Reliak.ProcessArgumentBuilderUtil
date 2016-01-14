using Reliak.ProcessArgumentBuilderUtil.ArgumentEscapeHandlers;
using Reliak.ProcessArgumentBuilderUtil.Arguments;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reliak.ProcessArgumentBuilderUtil
{
    /// <summary>
    /// Default implementation for <see cref="IProcessArgumentBuilder"/>
    /// </summary>
    public class ProcessArgumentBuilder : IProcessArgumentBuilder
    {
        private readonly List<IProcessArgumentBuilder> _arguments = new List<IProcessArgumentBuilder>();
        private readonly IArgumentEscapeHandler _argumentEscapeHandler;
        private readonly string _defaultArgumentKeyValueSeparator;

        /// <summary>
        /// Creates a new instance of <see cref="ProcessArgumentBuilder"/>
        /// </summary>
        /// <param name="escapeHandler">The handler used to escape arguments</param>
        /// <param name="defaultArgumentKeyValueSeparator">Specifies the default separator, when specifying a named argument.
        /// The default is " ", Example: "-f somefile.txt"
        /// </param>
        public ProcessArgumentBuilder(IArgumentEscapeHandler escapeHandler, string defaultArgumentKeyValueSeparator = " ")
        {
            _argumentEscapeHandler = escapeHandler;
            _defaultArgumentKeyValueSeparator = defaultArgumentKeyValueSeparator;
        }

        /// <summary>
        /// Adds an option (flag) to the arguments. It is assumed that no escape is required.
        /// </summary>
        /// <param name="option">An option (flag) with no argument value, e.g. "-v" or "--verbose"</param>
        public ProcessArgumentBuilder AddOption(string option)
        {
            if (!string.IsNullOrWhiteSpace(option))
            {
                _arguments.Add(BuildArgumentKey(option));
            }

            return this;
        }

        /// <summary>
        /// Appends an argument to the existing arguments.
        /// </summary>
        /// <param name="argumentValue">The value of the argument. This value will be escaped when required.</param>
        /// <param name="isSensitiveArgument">Specifies whether this argument contains sensitive information (like a password etc.)</param>
        public ProcessArgumentBuilder AddArgument(string argumentValue, bool isSensitiveArgument = false)
        {
            if (!string.IsNullOrWhiteSpace(argumentValue))
            {
                _arguments.Add(BuildArgumentValue(argumentValue, isSensitiveArgument, true));
            }

            return this;
        }

        /// <summary>
        /// Appends an argument to the existing arguments.
        /// </summary>
        /// <param name="argumentValue">The value of the argument. This value will NOT be escaped and added as is.</param>
        /// <param name="isSensitiveArgument">Specifies whether this argument contains sensitive information (like a password etc.)</param>
        public ProcessArgumentBuilder AddArgumentUnescaped(string argumentValue, bool isSensitiveArgument = false)
        {
            if (!string.IsNullOrWhiteSpace(argumentValue))
            {
                _arguments.Add(BuildArgumentValue(argumentValue, isSensitiveArgument, false));
            }

            return this;
        }

        /// <summary>
        /// Adds multiple (non-sensitive) arguments at once.
        /// </summary>
        /// <param name="argumentValues">The values of the arguments. These values will be escaped when required.</param>
        /// <returns></returns>
        public ProcessArgumentBuilder AddArguments(params string[] argumentValues)
        {
            foreach (var argumentValue in argumentValues)
            {
                AddArgument(argumentValue, false);
            }

            return this;
        }

        /// <summary>
        /// Adds multiple sensitive arguments at once.
        /// </summary>
        /// <param name="argumentValues">The values of the arguments. These values will be escaped when required.</param>
        /// <returns></returns>
        public ProcessArgumentBuilder AddArgumentsSensitive(params string[] argumentValues)
        {
            foreach (var argumentValue in argumentValues)
            {
                AddArgument(argumentValue, true);
            }

            return this;
        }

        /// <summary>
        /// Adds an argument key and value to the existing arguments.
        /// </summary>
        /// <param name="argumentKey">The key (option) for the argument value, e.g. "-f"</param>
        /// <param name="argumentValue">The value of the argument. This value will be escaped when required.</param>
        /// <param name="keyValueSeparator">
        /// The separator between the key and value of the argument. When <c>null</c>, the default separator (specified in the constructor) will be used.
        /// </param>
        /// <param name="isSensitiveArgument">Specifies whether the value of the argument contains sensitive information</param>
        public ProcessArgumentBuilder AddNamedArgument(string argumentKey, string argumentValue, string keyValueSeparator = null, bool isSensitiveArgument = false)
        {
            return AddNamedArgumentInternal(argumentKey, argumentValue, true, keyValueSeparator, isSensitiveArgument);
        }

        /// <summary>
        /// Adds an argument key and value to the existing arguments.
        /// </summary>
        /// <param name="argumentKey">The key (option) for the argument value, e.g. "-f"</param>
        /// <param name="argumentValue">The value of the argument. This value will NOT be escaped and added as is.</param>
        /// <param name="keyValueSeparator">
        /// The separator between the key and value of the argument. When <c>null</c>, the default separator (specified in the constructor) will be used.
        /// </param>
        /// <param name="isSensitiveArgument">Specifies whether the value of the argument contains sensitive information</param>
        public ProcessArgumentBuilder AddNamedArgumentUnescaped(string argumentKey, string argumentValue, string keyValueSeparator = null, bool isSensitiveArgument = false)
        {
            return AddNamedArgumentInternal(argumentKey, argumentValue, false, keyValueSeparator, isSensitiveArgument);
        }

        private ProcessArgumentBuilder AddNamedArgumentInternal(string argumentKey, string argumentValue, bool escapeArgumentValue, string keyValueSeparator = null, bool isSensitiveArgument = false)
        {
            if (!string.IsNullOrWhiteSpace(argumentKey))
            {
                if (string.IsNullOrWhiteSpace(argumentValue))
                {
                    throw new ArgumentException($"Argument {argumentValue} must not be null or empty");
                }

                keyValueSeparator = keyValueSeparator ?? _defaultArgumentKeyValueSeparator;

                _arguments.Add(new CompositeProcessArgument(
                    BuildArgumentKey(argumentKey + keyValueSeparator),      // key
                    BuildArgumentValue(argumentValue, isSensitiveArgument, escapeArgumentValue)  // value
                ));
            }

            return this;
        }

        private ProcessValueArgument BuildArgumentValue(string argumentValue, bool isSensitiveArgument, bool escapeArgument)
        {
            if (escapeArgument)
            {
                argumentValue = _argumentEscapeHandler.Escape(argumentValue);
            }

            return new ProcessValueArgument(argumentValue, isSensitiveArgument);
        }

        private ProcessKeyArgument BuildArgumentKey(string argumentKey)
        {
            return new ProcessKeyArgument(argumentKey);
        }

        public string Build()
        {
            return BuildInternal(f => f.Build());
        }

        public string BuildSafe(string sensitiveArgumentPlaceholder = "**sensitive data**")
        {
            return BuildInternal(f => f.BuildSafe(sensitiveArgumentPlaceholder));
        }

        private string BuildInternal(Func<IProcessArgumentBuilder,string> selector)
        {
            return string.Join(" ", _arguments.Select(f => selector(f))).Trim();
        }
    }
}