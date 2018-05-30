using Rabbit.Go.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rabbit.Go.Core.Reflective
{
    public interface IMethodDescriptorTable
    {
        MethodDescriptor Get(Type type, MethodInfo methodInfo);

        void Set(Type type, MethodInfo methodInfo);
    }

    public class MethodDescriptorTable : IMethodDescriptorTable
    {
        private readonly Dictionary<(Type, MethodInfo), MethodDescriptor> _table = new Dictionary<(Type, MethodInfo), MethodDescriptor>();

        public MethodDescriptor Get(Type type, MethodInfo methodInfo)
        {
            return _table.TryGetValue((type, methodInfo), out var descriptor) ? descriptor : null;
        }

        public void Set(Type type, MethodInfo methodInfo)
        {
            var key = (type, methodInfo);
            if (_table.ContainsKey(key))
                return;
            _table[key] = MethodDescriptorUtilities.CreateMethodDescriptor(type, methodInfo);
        }
    }
}