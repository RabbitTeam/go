// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RestClientBase.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the base REST client implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Implementations
{
	using System;
	using System.Diagnostics.Contracts;
	using System.IO;
	using Linq2Rest.Provider;

	/// <summary>
	/// Defines the base REST client implementation.
	/// </summary>
	public abstract class RestClientBase : IRestClient
	{
		private readonly string _acceptHeader;
		private readonly IHttpRequestFactory _httpRequestFactory;

		/// <summary>
		/// Initializes a new instance of the <see cref="RestClientBase"/> class.
		/// </summary>
		/// <param name="uri">The base <see cref="Uri"/> for the REST service.</param>
		/// <param name="acceptHeader">The accept header to use in web requests.</param>
		protected RestClientBase(Uri uri, string acceptHeader)
			: this(uri, acceptHeader, new HttpRequestFactory())
		{
			CustomContract.Requires<ArgumentNullException>(uri != null);
			CustomContract.Requires<ArgumentException>(uri.Scheme == HttpUtility.UriSchemeHttp || uri.Scheme == HttpUtility.UriSchemeHttps);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RestClientBase"/> class.
		/// </summary>
		/// <param name="uri">The base <see cref="Uri"/> for the REST service.</param>
		/// <param name="acceptHeader">The accept header to use in web requests.</param>
		/// <param name="httpRequestFactory">The factory used to create Linq2Rest.Provider.IHttpRequest implementations.</param>
		protected RestClientBase(Uri uri, string acceptHeader, IHttpRequestFactory httpRequestFactory)
		{
			CustomContract.Requires<ArgumentNullException>(uri != null);
			CustomContract.Requires<ArgumentException>(uri.Scheme == HttpUtility.UriSchemeHttp || uri.Scheme == HttpUtility.UriSchemeHttps);
			CustomContract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(acceptHeader));
			CustomContract.Requires<ArgumentException>(httpRequestFactory != null);

			_acceptHeader = acceptHeader;
			_httpRequestFactory = httpRequestFactory;
			ServiceBase = uri;
		}

		/// <summary>
		/// Gets the base <see cref="Uri"/> for the REST service.
		/// </summary>
		public Uri ServiceBase { get; private set; }

		/// <summary>
		/// Gets a service response.
		/// </summary>
		/// <param name="uri">The <see cref="Uri"/> to load the resource from.</param>
		/// <returns>A string representation of the resource.</returns>
		public Stream Get(Uri uri)
		{
			var stream = GetResponseStream(uri, HttpMethod.Get);

			CustomContract.Assert(stream != null);

			return stream;
		}

		/// <summary>
		/// Posts the passed data to the service.
		/// </summary>
		/// <param name="uri">The <see cref="Uri"/> to load the resource from.</param>
		/// <param name="input">The <see cref="Stream"/> representation to post.</param>
		/// <returns>The service response as a <see cref="Stream"/>.</returns>
		public Stream Post(Uri uri, Stream input)
		{
			var stream = GetResponseStream(uri, HttpMethod.Post, input);

			CustomContract.Assert(stream != null);

			return stream;
		}

		/// <summary>
		/// Puts the passed data to the service.
		/// </summary>
		/// <param name="input">The <see cref="Stream"/> representation to put.</param>
		/// <param name="uri">The <see cref="Uri"/> to load the resource from.</param>
		/// <returns>The service response as a <see cref="Stream"/>.</returns>
		public Stream Put(Uri uri, Stream input)
		{
			var stream = GetResponseStream(uri, HttpMethod.Put, input);

			CustomContract.Assert(stream != null);

			return stream;
		}

		/// <summary>
		/// Deletes the resource at the service.
		/// </summary>
		/// <param name="uri">The <see cref="Uri"/> to load the resource from.</param>
		/// <returns>The service response as a <see cref="Stream"/>.</returns>
		public Stream Delete(Uri uri)
		{
			var stream = GetResponseStream(uri, HttpMethod.Delete, null);

			CustomContract.Assert(stream != null);

			return stream;
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <filterpriority>2</filterpriority>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		/// <param name="disposing">True if disposing managed types.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		private Stream GetResponseStream(Uri uri, HttpMethod method, Stream requestStream = null)
		{
			CustomContract.Requires(uri != null);

			var request = _httpRequestFactory.Create(uri, method, _acceptHeader, _acceptHeader);

			if (requestStream != null)
			{
				request.WriteRequestStream(requestStream);
			}

			return request.GetResponseStream();
		}

		[ContractInvariantMethod]
		private void Invariants()
		{
			CustomContract.Invariant(_acceptHeader != null);
			CustomContract.Invariant(_httpRequestFactory != null);
		}
	}
}