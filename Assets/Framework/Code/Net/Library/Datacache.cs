using Jape;
using Sirenix.Utilities;
using UnityEngine;

namespace JapeNet
{
	internal static class Datacache
    {
        private static NetSettings Settings => NetManager.Settings;

        private static string Ip => Settings.databaseIp;
        private static int Port => Settings.databasePort;

        internal static Response Get(string key)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datacache.GetBody(key).ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Set(string key, string value)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datacache.SetBody(key, value).ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Remove(string key)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datacache.RemoveBody(key).ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Subscribe(string channel, Request.Datacache.SubscribeBody.Mode mode)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datacache.SubscribeBody(channel, mode).ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Unsubscribe(string subscription)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datacache.UnsubscribeBody(subscription).ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Publish(string channel, string value)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datacache.PublishBody(channel, value).ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Receive(string subscription)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datacache.ReceiveBody(subscription).ToJson()
            );
            return request.GetResponse();
        }
    }
}