using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reliak.ProcessArgumentBuilderUtil.Arguments
{
    internal class ProcessValueArgument : IProcessArgumentBuilder
    {
        private readonly string _value;
        private readonly bool _isSensitive;

        public ProcessValueArgument(string value, bool isSensitive = false)
        {
            _value = value;
            _isSensitive = isSensitive;
        }

        public string BuildSafe(string sensitiveArgumentPlaceholder)
        {
            return _isSensitive ? sensitiveArgumentPlaceholder : Build();
        }

        public string Build()
        {
            return _value;
        }
    }
}