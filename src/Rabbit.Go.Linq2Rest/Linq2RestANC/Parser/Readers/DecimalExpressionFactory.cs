// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DecimalExpressionFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DecimalExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Globalization;
	using System.Linq.Expressions;

	internal class DecimalExpressionFactory : ValueExpressionFactoryBase<decimal>
	{
		public override ConstantExpression Convert(string token)
		{
			decimal number;
			if (decimal.TryParse(token.Trim('M', 'm'), NumberStyles.Any, CultureInfo.InvariantCulture, out number))
			{
				return Expression.Constant(number);
			}

			throw new FormatException("Could not read " + token + " as decimal.");
		}
	}
}