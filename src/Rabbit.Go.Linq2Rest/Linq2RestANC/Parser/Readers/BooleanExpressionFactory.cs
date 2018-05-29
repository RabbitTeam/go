// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BooleanExpressionFactory.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the BooleanExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System.Linq.Expressions;
	using System.Text.RegularExpressions;

	internal class BooleanExpressionFactory : ValueExpressionFactoryBase<bool>
	{
		private static readonly Regex TrueRegex = new Regex("1|true", RegexOptions.IgnoreCase | RegexOptions.Compiled);
		private static readonly Regex FalseRegex = new Regex("0|false", RegexOptions.IgnoreCase | RegexOptions.Compiled);

		public override ConstantExpression Convert(string token)
		{
			if (TrueRegex.IsMatch(token))
			{
				return Expression.Constant(true);
			}

			if (FalseRegex.IsMatch(token))
			{
				return Expression.Constant(false);
			}

			return Expression.Constant(null);
		}
	}
}