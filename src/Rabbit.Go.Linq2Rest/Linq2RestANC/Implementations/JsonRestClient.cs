// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonRestClient.cs" company="Reimers.dk">
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
	using Linq2Rest.Provider;

	/// <summary>
	/// Defines a REST client implementation for JSON requests.
	/// </summary>
	public class JsonRestClient : RestClientBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonRestClient"/> class.
		/// </summary>
		/// <param name="uri">The base <see cref="Uri"/> for the REST service.</param>
		public JsonRestClient(Uri uri)
			: this(uri, new HttpRequestFactory())
		{
			CustomContract.Requires<ArgumentNullException>(uri != null);
			CustomContract.Requires<ArgumentException>(uri.Scheme == HttpUtility.UriSchemeHttp || uri.Scheme == HttpUtility.UriSchemeHttps);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonRestClient"/> class.
		/// </summary>
		/// <param name="uri">The base <see cref="Uri"/> for the REST service.</param>
		/// <param name="httpRequestFactory">The factory to use to create our HTTP Requests.</param>
		public JsonRestClient(Uri uri, IHttpRequestFactory httpRequestFactory)
			: base(uri, StringConstants.JsonMimeType, httpRequestFactory)
		{
			CustomContract.Requires<ArgumentNullException>(uri != null);
			CustomContract.Requires<ArgumentException>(uri.Scheme == HttpUtility.UriSchemeHttp || uri.Scheme == HttpUtility.UriSchemeHttps);
			CustomContract.Requires<ArgumentException>(httpRequestFactory != null);
		}
	}
}