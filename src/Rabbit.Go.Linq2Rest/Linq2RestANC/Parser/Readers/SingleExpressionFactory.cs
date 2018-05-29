// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SingleExpressionFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SingleExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Globalization;
	using System.Linq.Expressions;

	internal class SingleExpressionFactory : ValueExpressionFactoryBase<float>
	{
		public override ConstantExpression Convert(string token)
		{
			float number;
			if (float.TryParse(token.Trim('F', 'f'), NumberStyles.Any, CultureInfo.InvariantCulture, out number))
			{
				return Expression.Constant(number);
			}

			throw new FormatException("Could not read " + token + " as short.");
		}
	}
}