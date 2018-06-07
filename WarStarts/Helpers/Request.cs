using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace WarStarts.Helpers
{
    public static class RequestManager
    {
        public static HttpStatusCode CallTibiaSite(string url, out string responseString)
        {
            responseString = null;

            HttpStatusCode httpStatusCode;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    httpStatusCode = response.StatusCode;

                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        responseString = reader.ReadToEnd();

                    }
                }
            }

            catch (WebException e)
            {
                using (HttpWebResponse response = (HttpWebResponse)e.Response)
                {
                    httpStatusCode = response.StatusCode;
                }
            }

            return httpStatusCode;
        }
    }
}