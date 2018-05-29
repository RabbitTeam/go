// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlRestClient.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines a REST client implementation for JSON requests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Implementations
{
	using System;
	using System.Diagnostics.Contracts;

	/// <summary>
	/// Defines a REST client implementation for JSON requests.
	/// </summary>
	public class XmlRestClient : RestClientBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="XmlRestClient"/> class.
		/// </summary>
		/// <param name="uri">The base <see cref="Uri"/> for the REST service.</param>
		public XmlRestClient(Uri uri)
			: base(uri, StringConstants.XmlMimeType)
		{
			CustomContract.Requires<ArgumentNullException>(uri != null);
			CustomContract.Requires<ArgumentException>(uri.Scheme == HttpUtility.UriSchemeHttp || uri.Scheme == HttpUtility.UriSchemeHttps);
		}
	}
}