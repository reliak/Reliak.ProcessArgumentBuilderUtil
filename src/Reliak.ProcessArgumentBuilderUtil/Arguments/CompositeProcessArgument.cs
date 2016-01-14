using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reliak.ProcessArgumentBuilderUtil.Arguments
{
    internal class CompositeProcessArgument : IProcessArgumentBuilder
    {
        private readonly IProcessArgumentBuilder[] _argumentBuilders;

        public CompositeProcessArgument(params IProcessArgumentBuilder[] argumentBuilders)
        {
            _argumentBuilders = argumentBuilders;
        }

        public string BuildSafe(string sensitiveArgumentPlaceholder)
        {
            return string.Join("", _argumentBuilders.Select(f => f.BuildSafe(sensitiveArgumentPlaceholder)).ToArray());
        }

        public string Build()
        {
            return string.Join("", _argumentBuilders.Select(f => f.Build()).ToArray());
        }
    }
}