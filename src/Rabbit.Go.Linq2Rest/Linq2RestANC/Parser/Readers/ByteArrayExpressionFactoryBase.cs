// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteArrayExpressionFactoryBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ByteArrayExpressionFactoryBase type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Linq.Expressions;
	using System.Text.RegularExpressions;

	internal class ByteArrayExpressionFactoryBase<T> : ValueExpressionFactoryBase<T>
	{
		private static readonly Regex ByteArrayRegex = new Regex(@"(X|binary)['\""]([A-Za-z0-9=\+\/]+)['\""]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

		public override ConstantExpression Convert(string token)
		{
			var match = ByteArrayRegex.Match(token);
			if (match.Success)
			{
				try
				{
					var buffer = System.Convert.FromBase64String(match.Groups[2].Value);
					return Expression.Constant(buffer);
				}
				catch
				{
					return Expression.Constant(null);
				}
			}

			throw new FormatException("Could not read " + token + " as byte array.");
		}
	}
}