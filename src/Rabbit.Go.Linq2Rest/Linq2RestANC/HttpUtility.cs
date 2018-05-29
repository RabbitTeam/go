/*
 * HttpUtility.cs
 *
 * This code is derived from System.Net.HttpUtility.cs of Mono
 * (http://www.mono-project.com).
 *
 * The MIT License
 *
 * Copyright (c) 2005-2009 Novell, Inc. (http://www.novell.com)
 * Copyright (c) 2012-2014 sta.blockhead
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

/*
 * Authors:
 * - Patrik Torstensson <Patrik.Torstensson@labs2.com>
 * - Wictor Wilén (decode/encode functions) <wictor@ibizkit.se>
 * - Tim Coleman <tim@timcoleman.com>
 * - Gonzalo Paniagua Javier <gonzalo@ximian.com>
 */

namespace Linq2Rest
{
	using System.IO;
	using System.Linq;
	using System.Text;

	internal sealed class HttpUtility
	{
		private static readonly char[] HexChars = "0123456789abcdef".ToCharArray();

        public const string UriSchemeHttp = "http";
        public const string UriSchemeHttps = "https";

        public static string UrlEncode(string s)
		{
			return UrlEncode(s, Encoding.UTF8);
		}

		public static string UrlEncode(string s, Encoding encoding)
		{
			int len;
			if (s == null || (len = s.Length) == 0)
			{
				return s;
			}

			var needEncode = s.Where(c => (c < '0') || (c < 'A' && c > '9') || (c > 'Z' && c < 'a') || (c > 'z')).Any(c => !NotEncoded(c));

			if (!needEncode)
			{
				return s;
			}

			if (encoding == null)
			{
				encoding = Encoding.UTF8;
			}

			// Avoided GetByteCount call.
			var bytes = new byte[encoding.GetMaxByteCount(len)];
			var realLen = encoding.GetBytes(s, 0, len, bytes, 0);

			return Encoding.ASCII.GetString(UrlEncodeToBytesInternally(bytes, 0, realLen));
		}

		private static byte[] UrlEncodeToBytesInternally(byte[] bytes, int offset, int count)
		{
			using (var res = new MemoryStream())
			{
				var end = offset + count;
				for (int i = offset; i < end; i++)
				{
					UrlEncodeChar((char)bytes[i], res, false);
				}

				return res.ToArray();
			}
		}

		private static bool NotEncoded(char c)
		{
			return c == '!' ||
				   c == '\'' ||
				   c == '(' ||
				   c == ')' ||
				   c == '*' ||
				   c == '-' ||
				   c == '.' ||
				   c == '_';
		}

		private static void UrlEncodeChar(char c, Stream result, bool isUnicode)
		{
			if (c > 255)
			{
				// FIXME: What happens when there is an internal error?
				//if (!isUnicode)
				//  throw new ArgumentOutOfRangeException ("c", c, "c must be less than 256.");

				result.WriteByte((byte)'%');
				result.WriteByte((byte)'u');

				var i = (int)c;
				var idx = i >> 12;
				result.WriteByte((byte)HexChars[idx]);

				idx = (i >> 8) & 0x0F;
				result.WriteByte((byte)HexChars[idx]);

				idx = (i >> 4) & 0x0F;
				result.WriteByte((byte)HexChars[idx]);

				idx = i & 0x0F;
				result.WriteByte((byte)HexChars[idx]);

				return;
			}

			if (c > ' ' && NotEncoded(c))
			{
				result.WriteByte((byte)c);
				return;
			}

			if (c == ' ')
			{
				result.WriteByte((byte)'+');
				return;
			}

			if ((c < '0') ||
				(c < 'A' && c > '9') ||
				(c > 'Z' && c < 'a') ||
				(c > 'z'))
			{
				if (isUnicode && c > 127)
				{
					result.WriteByte((byte)'%');
					result.WriteByte((byte)'u');
					result.WriteByte((byte)'0');
					result.WriteByte((byte)'0');
				}
				else
				{
					result.WriteByte((byte)'%');
				}

				var idx = ((int)c) >> 4;
				result.WriteByte((byte)HexChars[idx]);

				idx = ((int)c) & 0x0F;
				result.WriteByte((byte)HexChars[idx]);
			}
			else
			{
				result.WriteByte((byte)c);
			}
		}
	}
}
