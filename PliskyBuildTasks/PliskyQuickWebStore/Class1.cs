using Newtonsoft.Json;
using Plisky.Build;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace PliskyDirtyWebStorage {
    public class PDirtyStorage : VersionStorage {
        
        public PDirtyStorage(string initialisationValue) : base(initialisationValue) {
        }

        public string HTTPCombined(string url, string method, string body) {

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = method.ToUpper(); //"POST";

            if (!string.IsNullOrEmpty(body)) {
                UTF8Encoding encoding = new UTF8Encoding();
                Byte[] byteArray = encoding.GetBytes(body);

                request.ContentLength = byteArray.Length;
                request.ContentType = @"application/json";

                using (Stream dataStream = request.GetRequestStream()) {
                    dataStream.Write(byteArray, 0, byteArray.Length);
                }
            } else {
                request.ContentLength = 0;
            }

            WebResponse response = request.GetResponse();
            if (response.ContentLength == 0) { return null; }

            Stream responseStream = response.GetResponseStream();
            try {
                using (StreamReader reader = new StreamReader(responseStream, Encoding.UTF8)) {
                    responseStream = null;
                    return reader.ReadToEnd();
                }

            } catch (WebException ex) {
                /* WebResponse errorResponse = ex.Response;
                 using (Stream ers = errorResponse.GetResponseStream()) {
                     StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                     String errorText = reader.ReadToEnd();
                     // log errorText
                 }*/
                //Bilge.Dump(ex, "Web call error for :", url);
                throw;
            } finally {
                if (responseStream != null) {
                    responseStream.Dispose();
                }
            }



        }
        protected override CompleteVersion ActualLoad() {
            string result = HTTPCombined(base.InitValue, "GET", "");
            return JsonConvert.DeserializeObject<CompleteVersion>(result);

        }

        protected override void ActualPersist(CompleteVersion cv) {
            
        }
    }
}
