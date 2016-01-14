using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reliak.ProcessArgumentBuilderUtil.Arguments
{
    internal class ProcessKeyArgument : IProcessArgumentBuilder
    {
        private readonly string _key;

        public ProcessKeyArgument(string key)
        {
            _key = key;
        }

        public string BuildSafe(string sensitiveArgumentPlaceholder)
        {
            return Build();
        }

        public string Build()
        {
            return _key;
        }
    }
}