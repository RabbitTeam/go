// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HttpWebRequestAdapter.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Takes a System.Net.HttpWebRequest and wraps it in an IHttpRequest Implementation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Linq2Rest.Implementations
{
    using System;
    using System.Diagnostics.Contracts;
    using System.IO;
    using System.Net;
    using Linq2Rest.Provider;
    using System.Net.Http;

    /// <summary>
    /// Takes a System.Net.Http.HttpClient and wraps it in an IHttpRequest Implementation.
    /// </summary>
    internal class HttpClientAdapter : IHttpRequest
	{
		/// <summary>
		/// The HttpWebRequest we are adapting to IHttpRequest.
		/// </summary>
		public HttpClient HttpClient { get; private set; }

        public HttpRequestMessage HttpMessage { get; private set; }

        private string requestMimeType { get; set; }
        private string responseMimeType { get; set; }
        private MemoryStream requestStream { get; set; }

		public HttpClientAdapter(HttpClient httpClient, HttpRequestMessage httpMessage)
		{
			HttpClient = httpClient;
            HttpMessage = httpMessage ?? new HttpRequestMessage();
		}

        //private HttpClientAdapter()
        //{ }


        public static System.Net.Http.HttpMethod AsNetMethod(Provider.HttpMethod method)
        {
            switch (method)
            {
                case Provider.HttpMethod.Delete:
                    return System.Net.Http.HttpMethod.Delete;
                case Provider.HttpMethod.Get:
                    return System.Net.Http.HttpMethod.Get;
                case Provider.HttpMethod.Post:
                    return System.Net.Http.HttpMethod.Post;
                case Provider.HttpMethod.Put:
                    return System.Net.Http.HttpMethod.Put;
                default:
                    throw new ArgumentException("Cannot convert Provider.HttpMethod " + method.ToString() + " to System.Net.Http.HttpMethod");
            }
        }

		/// <summary>
		/// Creates a basic HttpWebRequest that can then be built off of depending on what other functionality is needed.
		/// </summary>
		/// <param name="uri">The uri to send the request to.</param>
		/// <param name="method">The Http Request Method.</param>
		/// <param name="requestMimeType">The MIME type of the data we are sending.</param>
		/// <param name="responseMimeType">The MIME we accept in response.</param>
		/// <returns>Returns an HttpWebRequest initialized with the given parameters.</returns>
		public static HttpClientAdapter CreateHttpClientAdapter(Uri uri, Provider.HttpMethod method, string responseMimeType, string requestMimeType)
		{
			CustomContract.Requires(uri != null);
			CustomContract.Requires(responseMimeType != null);
			CustomContract.Requires(method != Provider.HttpMethod.None);

			requestMimeType = requestMimeType ?? responseMimeType;

            var httpMessage = new HttpRequestMessage(AsNetMethod(method), uri);

            var httpClientAdapter = new HttpClientAdapter(new HttpClient(), httpMessage);

			if (method == Provider.HttpMethod.Post || method == Provider.HttpMethod.Put)
			{
                httpClientAdapter.requestMimeType = requestMimeType;
            }

			httpClientAdapter.HttpClient.DefaultRequestHeaders.Add("Accept", responseMimeType);

			return httpClientAdapter;
		}

		public Stream GetRequestStream()
		{
            requestStream = new MemoryStream();
            return requestStream;
		}

		public Stream GetResponseStream()
		{
            if (requestStream == null)
                requestStream = new MemoryStream();
            var content = new StreamContent(requestStream);
            HttpMessage.Content = content;
            var resp = HttpClient.SendAsync(HttpMessage).Result;
            return resp.Content.ReadAsStreamAsync().Result;
		}
	}
}
