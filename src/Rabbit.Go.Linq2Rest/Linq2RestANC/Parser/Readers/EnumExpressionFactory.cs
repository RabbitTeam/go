// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EnumExpressionFactory.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EnumExpressionFactory type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
    using System;
    using System.Collections.Concurrent;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text.RegularExpressions;

    internal class EnumExpressionFactory : IValueExpressionFactory
	{
		private static readonly Regex EnumRegex = new Regex("^(.+)'(.+)'$", RegexOptions.Compiled);
		private static readonly ConcurrentDictionary<string, Type> KnownTypes = new ConcurrentDictionary<string, Type>();
		
		public bool Handles(Type type)
		{
			return type.IsEnum();
		}

		public ConstantExpression Convert(string token,Type type)
		{
		    return Expression.Constant(Enum.Parse(type, token));
/*			var match = EnumRegex.Match(token);
			if (match.Success)
			{
				var type = KnownTypes.GetOrAdd(match.Groups[1].Value, LoadType);
				var value = match.Groups[2].Value;

				CustomContract.Assume(type != null);

				return Expression.Constant(Enum.Parse(type, value));
			}

			throw new FormatException("Could not read " + token + " as Enum.");*/
		}

		private Type LoadType(string arg)
		{
			return AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(a => a.GetTypes())
				.FirstOrDefault(t => t.FullName == arg);
		}
	}
}