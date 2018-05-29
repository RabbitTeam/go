// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MethodProvider.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the MethodProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Linq;
	using System.Reflection;

    internal static class MethodProvider
	{
		private static readonly MethodInfo InnerContainsMethod;
		private static readonly MethodInfo InnerIndexOfMethod;
		private static readonly MethodInfo EndsWithMethod1;
		private static readonly MethodInfo InnerStartsWithMethod;
		private static readonly PropertyInfo InnerLengthProperty;
		private static readonly MethodInfo InnerSubstringMethod;
		private static readonly MethodInfo InnerToLowerMethod;
		private static readonly MethodInfo InnerToUpperMethod;
		private static readonly MethodInfo InnerTrimMethod;
		private static readonly PropertyInfo InnerDayProperty;
		private static readonly PropertyInfo InnerHourProperty;
		private static readonly PropertyInfo InnerMinuteProperty;
		private static readonly PropertyInfo InnerSecondProperty;
		private static readonly PropertyInfo InnerMonthProperty;
		private static readonly PropertyInfo InnerYearProperty;
		private static readonly MethodInfo InnerDoubleRoundMethod;
		private static readonly MethodInfo InnerDecimalRoundMethod;
		private static readonly MethodInfo InnerDoubleFloorMethod;
		private static readonly MethodInfo InnerDecimalFloorMethod;
		private static readonly MethodInfo InnerDoubleCeilingMethod;
		private static readonly MethodInfo InnerDecimalCeilingMethod;

		static MethodProvider()
		{
			var stringType = typeof(string);
			var datetimeType = typeof(DateTime);
			var mathType = typeof(Math);
			
			InnerContainsMethod = stringType.GetMethod("Contains", new[] { stringType });
			InnerIndexOfMethod = stringType.GetMethod("IndexOf", new[] { stringType });
			EndsWithMethod1 = stringType.GetMethod("EndsWith", new[] { stringType });
			InnerStartsWithMethod = stringType.GetMethod("StartsWith", new[] { stringType });
			InnerLengthProperty = stringType.GetProperty("Length", typeof(int), Type.EmptyTypes);
			InnerSubstringMethod = stringType.GetMethod("Substring", new[] { typeof(int) });
			InnerToLowerMethod = stringType.GetMethod("ToLowerInvariant", Type.EmptyTypes);
			InnerToUpperMethod = stringType.GetMethod("ToUpperInvariant", Type.EmptyTypes);
			InnerTrimMethod = stringType.GetMethod("Trim", Type.EmptyTypes);

			InnerDayProperty = datetimeType.GetProperty("Day", typeof(int), Type.EmptyTypes);
			InnerHourProperty = datetimeType.GetProperty("Hour", typeof(int), Type.EmptyTypes);
			InnerMinuteProperty = datetimeType.GetProperty("Minute", typeof(int), Type.EmptyTypes);
			InnerSecondProperty = datetimeType.GetProperty("Second", typeof(int), Type.EmptyTypes);
			InnerMonthProperty = datetimeType.GetProperty("Month", typeof(int), Type.EmptyTypes);
			InnerYearProperty = datetimeType.GetProperty("Year", typeof(int), Type.EmptyTypes);

			InnerDoubleRoundMethod = mathType.GetMethod("Round", new[] { typeof(double) });
			InnerDecimalRoundMethod = mathType.GetMethod("Round", new[] { typeof(decimal) });
			InnerDoubleFloorMethod = mathType.GetMethod("Floor", new[] { typeof(double) });
			InnerDecimalFloorMethod = mathType.GetMethod("Floor", new[] { typeof(decimal) });
			InnerDoubleCeilingMethod = mathType.GetMethod("Ceiling", new[] { typeof(double) });
			InnerDecimalCeilingMethod = mathType.GetMethod("Ceiling", new[] { typeof(decimal) });
		}

		public static MethodInfo IndexOfMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerIndexOfMethod;
			}
		}

		public static MethodInfo ContainsMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerContainsMethod;
			}
		}

		public static MethodInfo EndsWithMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return EndsWithMethod1;
			}
		}

		public static MethodInfo StartsWithMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerStartsWithMethod;
			}
		}

		public static PropertyInfo LengthProperty
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<PropertyInfo>() != null);
				return InnerLengthProperty;
			}
		}

		public static MethodInfo SubstringMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerSubstringMethod;
			}
		}

		public static MethodInfo ToLowerMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerToLowerMethod;
			}
		}

		public static MethodInfo ToUpperMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerToUpperMethod;
			}
		}

		public static MethodInfo TrimMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerTrimMethod;
			}
		}

		public static PropertyInfo DayProperty
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<PropertyInfo>() != null);
				return InnerDayProperty;
			}
		}

		public static PropertyInfo HourProperty
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<PropertyInfo>() != null);
				return InnerHourProperty;
			}
		}

		public static PropertyInfo MinuteProperty
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<PropertyInfo>() != null);
				return InnerMinuteProperty;
			}
		}

		public static PropertyInfo SecondProperty
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<PropertyInfo>() != null);
				return InnerSecondProperty;
			}
		}

		public static PropertyInfo MonthProperty
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<PropertyInfo>() != null);
				return InnerMonthProperty;
			}
		}

		public static PropertyInfo YearProperty
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<PropertyInfo>() != null);
				return InnerYearProperty;
			}
		}

		public static MethodInfo DoubleRoundMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerDoubleRoundMethod;
			}
		}

		public static MethodInfo DecimalRoundMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerDecimalRoundMethod;
			}
		}

		public static MethodInfo DoubleFloorMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerDoubleFloorMethod;
			}
		}

		public static MethodInfo DecimalFloorMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerDecimalFloorMethod;
			}
		}

		public static MethodInfo DoubleCeilingMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerDoubleCeilingMethod;
			}
		}

		public static MethodInfo DecimalCeilingMethod
		{
			get
			{
				CustomContract.Ensures(CustomContract.Result<MethodInfo>() != null);
				return InnerDecimalCeilingMethod;
			}
		}

		public static MethodInfo GetAnyAllMethod(string name, Type collectionType)
		{
			CustomContract.Requires(collectionType != null);

			var implementationType = GetIEnumerableImpl(collectionType);

			var elemType = implementationType.GetGenericArguments()[0];
			var predType = typeof(Func<,>).MakeGenericType(elemType, typeof(bool));

			var allMethod = (MethodInfo)GetGenericMethod(
														 typeof(Enumerable),
														 name, 
														 new[] { elemType }, 
														 new[] { implementationType, predType }, 
														 BindingFlags.Static);

			return allMethod;
		}

		public static Type GetIEnumerableImpl(Type type)
		{
			CustomContract.Requires(type != null);

			// Get IEnumerable implementation. Either type is IEnumerable<T> for some T, 
			// or it implements IEnumerable<T> for some T. We need to find the interface.
			if (IsIEnumerable(type))
			{
				return type;
			}

			var interfaces = type.FindInterfaces((m, o) => IsIEnumerable(m), null);

			CustomContract.Assume(interfaces.Count() > 0);

			var t = interfaces.First();

			return t;
		}

		private static MethodBase GetGenericMethod(Type type, string name, Type[] typeArgs, Type[] argTypes, BindingFlags flags)
		{
			CustomContract.Requires(typeArgs != null);
			CustomContract.Requires(type != null);
			//CustomContract.Assume(Type.DefaultBinder != null);

			var typeArity = typeArgs.Length;
			var methods = type.GetMethods()
				.Where(m => m.Name == name)
				.Where(m => m.GetGenericArguments().Length == typeArity)
                .Where(m => m.GetParameters().Length == argTypes.Length
                            && m.GetParameters().All(p => p.GetCustomAttribute<ParamArrayAttribute>() == null
                                                          && !(p.IsIn || p.IsOut || p.IsOptional)))
				.Select(m => m.MakeGenericMethod(typeArgs));

            //return Type.DefaultBinder.SelectMethod(flags, methods.ToArray(), argTypes, null);
            return methods.First();
		}

		private static bool IsIEnumerable(Type type)
		{
			CustomContract.Requires(type != null);

			return type.IsGenericType()
				&& type.GetGenericTypeDefinition() == typeof(IEnumerable<>);
		}
	}
}