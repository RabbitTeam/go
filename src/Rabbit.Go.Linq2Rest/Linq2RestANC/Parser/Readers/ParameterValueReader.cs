// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueReader.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ParameterValueReader type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser.Readers
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;

	internal class ParameterValueReader
	{
		private readonly IList<IValueExpressionFactory> _expressionFactories;

		public ParameterValueReader(IEnumerable<IValueExpressionFactory> expressionFactories)
		{
			CustomContract.Requires(expressionFactories != null);

			_expressionFactories = expressionFactories.Concat(
				new IValueExpressionFactory[]
				{
					new EnumExpressionFactory(),
					new BooleanExpressionFactory(),
					new ByteExpressionFactory(),
					new GuidExpressionFactory(),
					new DateTimeExpressionFactory(),
					new TimeSpanExpressionFactory(),
					new DateTimeOffsetExpressionFactory(),
					new DecimalExpressionFactory(),
					new DoubleExpressionFactory(),
					new SingleExpressionFactory(),
					new ByteArrayExpressionFactory(),
					new StreamExpressionFactory(),
					new LongExpressionFactory(),
					new IntExpressionFactory(),
					new ShortExpressionFactory(),
					new UnsignedIntExpressionFactory(),
					new UnsignedLongExpressionFactory(),
					new UnsignedShortExpressionFactory()
				})
				.ToList();
		}

		public Expression Read(Type type, string token, IFormatProvider formatProvider)
		{
			CustomContract.Requires(token != null);
			CustomContract.Requires(type != null);

			var factory = _expressionFactories.FirstOrDefault(x => x.Handles(type));

			return factory == null
				? GetKnownConstant(type, token, formatProvider)
				: factory.Convert(token,type);
		}

		private static Expression GetParseExpression(string filter, IFormatProvider formatProvider, Type type)
		{
			CustomContract.Requires(type != null);

			var parseMethods = type.GetMethods(BindingFlags.Static | BindingFlags.Public).Where(x => x.Name == "Parse").ToArray();
			if (parseMethods.Length > 0)
			{
				var withFormatProvider =
					parseMethods.FirstOrDefault(
						x =>
						{
							var parameters = x.GetParameters();
							return parameters.Length == 2
								&& typeof(string).IsAssignableFrom(parameters[0].ParameterType)
								&& typeof(IFormatProvider).IsAssignableFrom(parameters[1].ParameterType);
						});
				if (withFormatProvider != null)
				{
					return Expression.Call(withFormatProvider, Expression.Constant(filter), Expression.Constant(formatProvider));
				}

				var withoutFormatProvider = parseMethods.FirstOrDefault(
						x =>
						{
							var parameters = x.GetParameters();
							return parameters.Length == 1
								&& typeof(string).IsAssignableFrom(parameters[0].ParameterType);
						});

				if (withoutFormatProvider != null)
				{
					return Expression.Call(withoutFormatProvider, Expression.Constant(filter));
				}
			}

			return null;
		}

		private Expression GetKnownConstant(Type type, string token, IFormatProvider formatProvider)
		{
			CustomContract.Requires(token != null);
			CustomContract.Requires(type != null);

			if (type.IsEnum())
			{
				var enumValue = Enum.Parse(type, token.Replace("'", string.Empty), true);
				return Expression.Constant(enumValue);
			}

			if (typeof(IConvertible).IsAssignableFrom(type))
			{
				return Expression.Constant(Convert.ChangeType(token, type, formatProvider), type);
			}

			if (type.IsGenericType() && typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
			{
				if (string.Equals("null", token, StringComparison.CurrentCultureIgnoreCase))
				{
					return Expression.Constant(null);
				}

				var genericTypeArgument = type.GetGenericArguments()[0];
				var value = Read(genericTypeArgument, token, formatProvider);
				if (value != null)
				{
					return Expression.Convert(value, type);
				}
			}

			return GetParseExpression(token, formatProvider, type);
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_expressionFactories != null);
		}
	}
}