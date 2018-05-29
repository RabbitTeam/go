// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParameterValueWriter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ParameterValueWriter type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider.Writers
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.Contracts;
	using System.Globalization;
	using System.Linq;

#if NETFX_CORE
	using System.Reflection;
#endif

	internal class ParameterValueWriter
	{
		private readonly IList<IValueWriter> _valueWriters;

		public ParameterValueWriter(IEnumerable<IValueWriter> valueWriters)
		{
			CustomContract.Requires(valueWriters != null);

			_valueWriters = valueWriters.Concat(
				new IValueWriter[]
				{
					new EnumValueWriter(),
					new StringValueWriter(),
					new BooleanValueWriter(),
					new IntValueWriter(),
					new LongValueWriter(),
					new ShortValueWriter(),
					new UnsignedIntValueWriter(),
					new UnsignedLongValueWriter(),
					new UnsignedShortValueWriter(),
					new ByteArrayValueWriter(),
					new StreamValueWriter(),
					new DecimalValueWriter(),
					new DoubleValueWriter(),
					new SingleValueWriter(),
					new ByteValueWriter(),
					new GuidValueWriter(),
					new DateTimeValueWriter(),
					new TimeSpanValueWriter(),
					new DateTimeOffsetValueWriter()
				})
				.ToList();
		}

		public string Write(object value)
		{
			if (value == null)
			{
				return "null";
			}

			var type = value.GetType();

			var writer = _valueWriters.FirstOrDefault(x => x.Handles(type));

			if (writer != null)
			{
				return writer.Write(value);
			}

#if !NETFX_CORE
			if (typeof(Nullable<>).IsAssignableFrom(type))
			{
				var genericParameter = type.GetGenericArguments()[0];

				return Write(Convert.ChangeType(value, genericParameter, CultureInfo.CurrentCulture));
			}

#else
			var typeInfo = type.GetTypeInfo();
			if (typeof(Nullable<>).GetTypeInfo().IsAssignableFrom(typeInfo))
			{
				var genericParameter = typeInfo.GenericTypeArguments[0];

				return Write(Convert.ChangeType(value, genericParameter, CultureInfo.CurrentCulture));
			}
#endif

			return value.ToString();
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_valueWriters != null);
		}
	}
}
