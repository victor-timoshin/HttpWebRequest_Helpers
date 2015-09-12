using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HttpWebRequestHelpers
{
	public class JsonRequestHelper
	{
		public static T GetObjectRest<T>(string server, string relativePath, IDictionary<string, string> headers = null)
		{
			Uri serverUri = new Uri(server);
			Uri relativeUri = new Uri(relativePath, UriKind.Relative);

			HttpWebRequest request = WebRequest.Create(new Uri(serverUri, relativeUri)) as HttpWebRequest;
			request.ContentType = "application/json; charset=utf-8";
			request.Method = "GET";

			if (headers != null)
			{
				foreach (KeyValuePair<string, string> item in headers)
					request.Headers.Add(item.Key, Convert.ToString(item.Value));
			}

			try
			{
				HttpWebResponse response = request.GetResponse() as HttpWebResponse;
				if (response.StatusCode != HttpStatusCode.OK)
				{
				}

				using (var responseStreamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
				{
					using (var responseJsonReader = new JsonTextReader(responseStreamReader))
					{
						var serializer = new JsonSerializer();
						return serializer.Deserialize<T>(responseJsonReader);
					}
				}
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
					using (var responseStreamReader = new StreamReader(errorResponse.GetResponseStream()))
					{
						using (var responseJsonReader = new JsonTextReader(responseStreamReader))
						{
							var serializer = new JsonSerializer();
							return serializer.Deserialize<T>(responseJsonReader);
						}
					}
				}

				return default(T);
			}
			finally
			{
				request.Abort();
			}
		}

		public static async Task<T> GetObjectRestAsync<T>(string server, string relativePath)
		{
			Uri serverUri = new Uri(server);
			Uri relativeUri = new Uri(relativePath, UriKind.Relative);

			HttpWebRequest request = WebRequest.Create(new Uri(serverUri, relativeUri)) as HttpWebRequest;
			request.ContentType = "application/json; charset=utf-8";
			request.Method = "GET";

			try
			{
				HttpWebResponse response = await Task<WebResponse>.Factory.FromAsync(request.BeginGetResponse(null, null), request.EndGetResponse) as HttpWebResponse;
				if (response.StatusCode != HttpStatusCode.OK)
				{
				}

				using (var responseStreamReader = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("utf-8")))
				{
					using (var responseJsonReader = new JsonTextReader(responseStreamReader))
					{
						var serializer = new JsonSerializer();
						return serializer.Deserialize<T>(responseJsonReader);
					}
				}
			}
			catch (WebException ex)
			{
				if (ex.Response != null)
				{
					HttpWebResponse errorResponse = ex.Response as HttpWebResponse;
					using (var responseStreamReader = new StreamReader(errorResponse.GetResponseStream()))
					{
						using (var responseJsonReader = new JsonTextReader(responseStreamReader))
						{
							var serializer = new JsonSerializer();
							return serializer.Deserialize<T>(responseJsonReader);
						}
					}

				}

				return default(T);
			}
			finally
			{
				request.Abort();
			}
		}

		public static string PostObjectRest<T>(string server, string relativePath, T data, IDictionary<string, string> headers = null)
		{
			Uri serverUri = new Uri(server);
			Uri relativeUri = new Uri(relativePath, UriKind.Relative);

			var request = WebRequest.Create(new Uri(serverUri, relativeUri)) as HttpWebRequest;
			request.ContentType = "application/json";
			request.Method = "POST";

			if (headers != null)
			{
				foreach (KeyValuePair<string, string> item in headers)
					request.Headers.Add(item.Key, Convert.ToString(item.Value));
			}

			string postData = JsonConvert.SerializeObject(data).ToString();
			byte[] bytes = new UTF8Encoding().GetBytes(postData);

			request.ContentLength = bytes.Length;

			string message = string.Empty;
			using (var streamWriter = request.GetRequestStream())
			{
				try
				{
					streamWriter.Write(bytes, 0, bytes.Length);
					streamWriter.Flush();
					streamWriter.Close();

					var response = request.GetResponse() as HttpWebResponse;
					using (StreamReader streamReader = new StreamReader(response.GetResponseStream()))
					{
						message = streamReader.ReadToEnd();
						streamReader.Close();
					}
				}
				catch (Exception ex)
				{
					message = "Exception thrown when contacting service: " + ex.ToString();
				}

				return message;
			}
		}
	}
}