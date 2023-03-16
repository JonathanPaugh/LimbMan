using System;
using UnityEngine;
using UnityEngine.Networking;

namespace JapeNet
{
    public partial class Response
    {
        private enum Status { Incomplete, Success, Error }

        private readonly UnityWebRequestAsyncOperation request;

        private Status status;
        private bool disposed;

        private string value;

        private Action onSuccess = delegate {};
        private Action onError = delegate {};

        public Response(UnityWebRequestAsyncOperation request)
        {
            this.request = request;
            request.completed += Completed;
            Application.quitting += Abort;
        }

        public Response Then(Action response)
        {
            if (status == Status.Success) { Respond(); }
            else { onSuccess += Respond; }
            return this;

            void Respond()
            {
                onSuccess -= Respond;
                response?.Invoke();
            }
        }

        public Response Read(Action<string> response)
        {
            if (status == Status.Success) { Respond(); }
            else { onSuccess += Respond; }
            return this;

            void Respond()
            {
                onSuccess -= Respond;
                response?.Invoke(value);
            }
        }

        public Response ReadJson<T>(Action<T> response) => ReadJson(response, null);
        public Response ReadJson<T>(Action<T> response, Func<string, string> modifyJson)
        {
            if (status == Status.Success) { Respond(); }
            else { onSuccess += Respond; }
            return this;

            void Respond()
            {
                onSuccess -= Respond;
                string temp = modifyJson == null ? value : modifyJson(value);
                response?.Invoke(JsonUtility.FromJson<T>(temp));
            }
        }

        public Response Error(Action response)
        {
            if (status == Status.Error) { Respond(); }
            else { onError += Respond; }
            return this;

            void Respond()
            {
                onError -= Respond;
                response?.Invoke();
            }
        }

        public void Abort()
        {
            if (status != Status.Incomplete) { return; }

            request.completed -= Completed;
            request.webRequest?.Abort();

            Dispose();   
        }

        private void Dispose()
        {
            if (disposed) { return; }
            disposed = true;
            request.webRequest?.Dispose();
        }

        private void Completed(AsyncOperation _)
        {
            if (status != Status.Incomplete) { return; }

            request.completed -= Completed;

            switch (request.webRequest.result)
            {
                case UnityWebRequest.Result.Success:
                    status = Status.Success;
                    value = request.webRequest.downloadHandler.text;
                    onSuccess.Invoke();
                    break;

                default:
                    status = Status.Error;
                    onError.Invoke();
                    break;
            }
            
            Dispose();
        }
    }
}
