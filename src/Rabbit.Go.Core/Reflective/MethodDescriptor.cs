using System;
using System.Collections.Generic;

namespace Rabbit.Go.Core
{
    public class MethodDescriptor
    {
        public string RequestMethod { get; set; }
        public TemplateString RequestLine { get; set; }
        public ParameterDescriptor[] Parameters { get; set; }
        public IDictionary<string, TemplateString> Headers { get; set; } = new Dictionary<string, TemplateString>(StringComparer.OrdinalIgnoreCase);
    }

    public class ParameterDescriptor
    {
        public Type ParameterType { get; set; }
        public string ParameterName { get; set; }
        public object[] Attributes { get; set; }
        public string Name { get; set; }
    }
}