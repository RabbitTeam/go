// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ByteExpressionFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ByteExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Globalization;
	using System.Linq.Expressions;

	internal class ByteExpressionFactory : ValueExpressionFactoryBase<byte>
	{
		public override ConstantExpression Convert(string token)
		{
			byte number;
			if (byte.TryParse(token, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out number))
			{
				return Expression.Constant(number);
			}

			throw new FormatException("Could not read " + token + " as byte.");
		}
	}
}