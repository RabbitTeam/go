// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValueExpressionFactoryBase.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ValueExpressionFactoryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal abstract class ValueExpressionFactoryBase<T> : IValueExpressionFactory
	{
		public bool Handles(Type type)
		{
#if !NETFX_CORE
			return typeof(T) == type;
#else
			return typeof(T).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
#endif
		}

	    /// <inheritdoc />
	    /// <summary>
	    /// Converts the passed OData style value to an <see cref="T:System.Linq.Expressions.Expression" />.
	    /// </summary>
	    /// <param name="token">The value token to convert.</param>
	    /// <param name="type">The value type.</param>
	    /// <returns>The value as a <see cref="T:System.Linq.Expressions.ConstantExpression" />.</returns>
	    public virtual ConstantExpression Convert(string token, Type type)
	    {
	        return Convert(token);
	    }

	    public abstract ConstantExpression Convert(string token);
	}
}