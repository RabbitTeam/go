// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnsignedIntExpressionFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the UnsignedIntExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Linq.Expressions;

	internal class UnsignedIntExpressionFactory : ValueExpressionFactoryBase<uint>
	{
		public override ConstantExpression Convert(string token)
		{
			uint number;
			if (uint.TryParse(token, out number))
			{
				return Expression.Constant(number);
			}

			throw new FormatException("Could not read " + token + " as Unsigned Integer.");
		}
	}
}