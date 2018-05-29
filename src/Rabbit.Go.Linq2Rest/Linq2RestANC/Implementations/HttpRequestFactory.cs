// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpRequestFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Creates a basic IHttpRequest.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Implementations
{
	using System;
	using Linq2Rest.Provider;

	/// <summary>
	/// Creates a basic IHttpRequest.
	/// </summary>
	public class HttpRequestFactory : IHttpRequestFactory
	{
		/// <summary>
		/// Creates an IHttpRequest that can be used to send an http request.
		/// </summary>
		/// <param name="uri">The location the request is to be sent to.</param>
		/// <param name="method">The method to use to send the request.</param>
		/// <param name="responseMimeType">The Mime type we accept in response.</param>
		/// <param name="requestMimeType">The Mime type we are sending in request.</param>
		/// <returns>The HttpRequest we are creating.</returns>
		public IHttpRequest Create(Uri uri, HttpMethod method, string responseMimeType, string requestMimeType)
		{
			var httpWebRequestAdapter = HttpClientAdapter.CreateHttpClientAdapter(uri, method, responseMimeType, requestMimeType);

            return httpWebRequestAdapter;

        }
	}
}
