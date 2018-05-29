// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IHttpRequestFactory.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the public interface for a Lin2Rest.Provider.IHttpRequest object factory.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Provider
{
	using System;
	using System.Diagnostics.Contracts;

	/// <summary>
	/// Defines the public interface for a Lin2Rest.Provider.IHttpRequest object factory.
	/// </summary>
	[ContractClass(typeof(HttpRequestFactoryContracts))]
	public interface IHttpRequestFactory
	{
		/// <summary>
		/// Creates an IHttpRequest that can be used to send an http request.
		/// </summary>
		/// <param name="uri">The location the request is to be sent to.</param>
		/// <param name="method">The method to use to send the request.</param>
		/// <param name="responseMimeType">The Mime type we accept in response.</param>
		/// <param name="requestMimeType">The Mime type we are sending in request.</param>
		/// <returns>The HttpRequest we are creating.</returns>
		IHttpRequest Create(Uri uri, HttpMethod method, string responseMimeType, string requestMimeType);
	}

	[ContractClassFor(typeof(IHttpRequestFactory))]
	internal abstract class HttpRequestFactoryContracts : IHttpRequestFactory
	{
		public IHttpRequest Create(Uri uri, HttpMethod method, string responseMimeType, string requestMimeType)
		{
			CustomContract.Requires<ArgumentNullException>(uri != null);
			CustomContract.Requires<ArgumentNullException>(responseMimeType != null);
			CustomContract.Requires<ArgumentException>(method != HttpMethod.None);

			throw new NotImplementedException();
		}
	}
}
