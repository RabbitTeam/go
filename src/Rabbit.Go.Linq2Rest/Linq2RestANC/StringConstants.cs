// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringConstants.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the StringConstants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest
{
	internal static class StringConstants
	{
		internal const string OrderByParameter = "$orderby";
		internal const string SelectParameter = "$select";
		internal const string FilterParameter = "$filter";
		internal const string SkipParameter = "$skip";
		internal const string TopParameter = "$top";
        internal const string ExpandParameter = "$expand";
		internal const string JsonMimeType = "application/json";
		internal const string XmlMimeType = "application/xml";
	}
}