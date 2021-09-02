using System;
using Jape;
using UnityEngine;
using UnityEngine.Networking;

namespace JapeNet
{
    public partial class Response
    {
        private UnityWebRequestAsyncOperation request;

        private string value;
        private bool completed;
        private bool disposed;

        public Response(UnityWebRequestAsyncOperation request)
        {
            this.request = request;
            request.completed += Completed;
            Application.quitting += Abort;
        }

        private void Completed(AsyncOperation _)
        {
            request.completed -= Completed;
            completed = true;
            value = request.webRequest.downloadHandler.text;
            Dispose();
        }

        public void Read(Action<string> response)
        {
            if (request.isDone)
            {
                response.Invoke(value);
            }
            else
            {
                request.completed += delegate
                {
                    response.Invoke(disposed ? value : request.webRequest.downloadHandler.text);
                };
            }
        }

        public void ReadJson<T>(Action<T> response)
        {
            if (request.isDone)
            {
                response.Invoke(JsonUtility.FromJson<T>(value));
            }
            else
            {
                request.completed += delegate
                {
                    response.Invoke(disposed ? JsonUtility.FromJson<T>(value) : JsonUtility.FromJson<T>(request.webRequest.downloadHandler.text));
                };
            }
        }

        private void Abort()
        {
            if (completed) { return; }
            request.completed -= Completed;
            request.webRequest?.Abort();
            Dispose();   
        }

        private void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            request.webRequest.Dispose();
        }
    }
}
