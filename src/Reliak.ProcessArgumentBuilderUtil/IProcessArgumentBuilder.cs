using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reliak.ProcessArgumentBuilderUtil
{
    /// <summary>
    /// Common interface for ProcessArgumentBuilders
    /// </summary>
    public interface IProcessArgumentBuilder
    {
        /// <summary>
        /// Builds the arguments, which then can be used to invoke a process
        /// </summary>
        string Build();

        /// <summary>
        /// Builds the arguments in a safe way, i.e. sensitive arguments will
        /// be replaced with <paramref name="sensitiveArgumentPlaceholder"/>
        /// </summary>
        /// <param name="sensitiveArgumentPlaceholder">The string that should be used instead of the sensitive argument value</param>
        string BuildSafe(string sensitiveArgumentPlaceholder);
    }
}