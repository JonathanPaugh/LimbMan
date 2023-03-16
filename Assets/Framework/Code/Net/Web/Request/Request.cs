using System.Text;
using UnityEngine.Networking;

namespace JapeNet
{
    public partial class Request
    {
        private readonly UnityWebRequest request;

        private Request(UnityWebRequest request)
        {
            this.request = request;
            request.certificateHandler = new Certificate();
        }

        private static Request BodyRequest(string url, string method, string data)
        {
            UnityWebRequest request = new(url, method)
            {
                uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(data)),
                downloadHandler = new DownloadHandlerBuffer()
            };
            return new Request(request);
        }

        public static Request Get(string url) { return new Request(UnityWebRequest.Get(url)); }

        public static Request Post(string url, string data)
        {
            return BodyRequest(url, UnityWebRequest.kHttpVerbPOST, data);
        }

        public static Request Create(string url)
        {
            UnityWebRequest request = new(url, UnityWebRequest.kHttpVerbCREATE)
            {
                downloadHandler = new DownloadHandlerBuffer()
            };
            return new Request(request);
        }

        public static Request Delete(string url, string data)
        {
            return BodyRequest(url, UnityWebRequest.kHttpVerbDELETE, data);
        }

        public Request AddHeader(string key, string value)
        {
            request.SetRequestHeader(key, value);
            return this;
        }

        public Response GetResponse()
        {
            return new Response(request.SendWebRequest());
        }

        public void Dispose()
        {
            request.Dispose();
        }

        private class Certificate : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }
    }
}
