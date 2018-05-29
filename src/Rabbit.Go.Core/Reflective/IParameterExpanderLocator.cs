using System;

namespace Rabbit.Go.Core.Reflective
{
    public interface IParameterExpanderLocator
    {
        IParameterExpander Get(Type type);
    }
}