using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reliak.ProcessArgumentBuilderUtil.ArgumentEscapeHandlers
{
    /// <summary>
    /// Common interface for classes that handle the escape of arguments
    /// </summary>
    public interface IArgumentEscapeHandler
    {
        /// <summary>
        /// Returns the escaped value of <paramref name="argument"/>
        /// </summary>
        /// <param name="argument">The value of the argument</param>
        string Escape(string argument);
    }
}