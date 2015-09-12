using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace HttpWebRequestHelpers
{
	public class XmlRequestHelper
	{
		public static XDocument GetObjectXML(string server, string relativePath, string token)
		{
			Uri serverUri = new Uri(server);
			Uri relativeUri = new Uri(relativePath, UriKind.Relative);

			HttpWebRequest request = WebRequest.Create(new Uri(serverUri, relativeUri)) as HttpWebRequest;
			request.Headers.Add("X-Access-Token", token);
			request.ContentType = "application/xml; charset=utf-8";
			request.Method = "GET";
			request.Proxy = null;

			try
			{
				HttpWebResponse response = request.GetResponse() as HttpWebResponse;
				if (response.StatusCode != HttpStatusCode.OK)
				{
				}

				using (Stream responseStream = response.GetResponseStream())
				{
					return XDocument.Parse(new StreamReader(responseStream).ReadToEnd());
				}
			}
			catch (Exception ex)
			{
				var exception = ex.ToString();
				return default(XDocument);
			}
			finally
			{
				request.Abort();
			}
		}
	}
}