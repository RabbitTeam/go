// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateTimeOffsetExpressionFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the DateTimeOffsetExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Linq.Expressions;
	using System.Text.RegularExpressions;
	using System.Xml;

	internal class DateTimeOffsetExpressionFactory : ValueExpressionFactoryBase<DateTimeOffset>
	{
		private static readonly Regex DateTimeOffsetRegex = new Regex(@"datetimeoffset['\""](\d{4}\-\d{2}\-\d{2}(T\d{2}\:\d{2}\:\d{2}(\.\d+)?)?([\-\+]\d{2}:\d{2}|Z)?)['\""]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public override ConstantExpression Convert(string token)
		{
			var match = DateTimeOffsetRegex.Match(token);
			if (match.Success)
			{
				var dateTimeOffset = XmlConvert.ToDateTimeOffset(match.Groups[1].Value);
				return Expression.Constant(dateTimeOffset);
			}

			throw new FormatException("Could not read " + token + " as DateTimeOffset.");
		}
	}
}