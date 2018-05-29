using Rabbit.Go.Core.Utilities;
using System.Collections.Generic;
using System.Reflection;

namespace Rabbit.Go.Core.Reflective
{
    public interface IMethodDescriptorTable
    {
        MethodDescriptor Get(MethodInfo methodInfo);

        void Set(MethodInfo methodInfo);
    }

    public class MethodDescriptorTable: IMethodDescriptorTable
    {
        private readonly Dictionary<int, MethodDescriptor> _table = new Dictionary<int, MethodDescriptor>();

        public MethodDescriptor Get(MethodInfo method)
        {
            return _table.TryGetValue(method.GetHashCode(), out var descriptor) ? descriptor : null;
        }

        public void Set(MethodInfo methodInfo)
        {
            if (_table.ContainsKey(methodInfo.GetHashCode()))
                return;
            _table[methodInfo.GetHashCode()] = MethodDescriptorUtilities.CreateMethodDescriptor(methodInfo);
        }
    }
}