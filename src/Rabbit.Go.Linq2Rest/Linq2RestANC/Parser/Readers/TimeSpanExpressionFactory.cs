// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TimeSpanExpressionFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the TimeSpanExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Linq.Expressions;
	using System.Text.RegularExpressions;
	using System.Xml;

	internal class TimeSpanExpressionFactory : ValueExpressionFactoryBase<TimeSpan>
	{
		private static readonly Regex TimeSpanRegex = new Regex(@"^time['\""](P.+)['\""]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public override ConstantExpression Convert(string token)
		{
			var match = TimeSpanRegex.Match(token);
			if (match.Success)
			{
				try
				{
					var timespan = XmlConvert.ToTimeSpan(match.Groups[1].Value);
					return Expression.Constant(timespan);
				}
				catch
				{
					throw new FormatException("Could not read " + token + " as TimeSpan.");
				}
			}

			throw new FormatException("Could not read " + token + " as TimeSpan.");
		}
	}
}
