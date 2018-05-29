// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FunctionExtensions.cs" company="Reimers.dk">
//   Copyright ?Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the FunctionExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Parser
{
	using System;
	using System.Collections.Generic;

	internal static class FunctionExtensions
	{
		private static readonly Dictionary<string, Type> KnownFunctions = new Dictionary<string, Type>
																			  {
																				  { "length", typeof(int) },
																				  { "substring", typeof(string) },
																				  { "substringof", typeof(bool) },
																				  { "endswith", typeof(bool) },
																				  { "startswith", typeof(bool) },
																				  { "indexof", typeof(int) },
																				  { "tolower", typeof(string) },
																				  { "toupper", typeof(string) },
																				  { "trim", typeof(string) },
																				  { "year", typeof(int) },
																				  { "month", typeof(int) },
																				  { "day", typeof(int) },
																				  { "hour", typeof(int) },
																				  { "minute", typeof(int) },
																				  { "second", typeof(int) },
																				  { "floor", typeof(int) },
																				  { "ceiling", typeof(int) },
																			      { "round", typeof(double) },
																			      { "in", typeof(bool) }
                                                                              };

		public static Type GetFunctionType(this string functionName)
		{
			return KnownFunctions[functionName];
		}
	}
}