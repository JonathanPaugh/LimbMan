using System.Collections.Generic;
using Jape;
using UnityEngine;

namespace JapeNet
{
	internal static class Datastore
    {
        private static NetSettings Settings => NetManager.Settings;

        private static string Ip => Settings.databaseIp;
        private static int Port => Settings.databasePort;

        internal static Response Get(string store, string collection, string key)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datastore.GetBody(store, collection, key).ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Insert(string store, string collection, string data)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datastore.InsertBody(store, collection, data).ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Update(string store, string collection, string key, string data)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datastore.UpdateBody(store, collection, key, data).ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Remove(string store, string collection, string key, string[] data)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datastore.RemoveBody(store, collection, key, data).ToJson()
            );
            return request.GetResponse();
        }

        internal static Response Delete(string store, string collection, string key)
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                new Request.Datastore.DeleteBody(store, collection, key).ToJson()
            );
            return request.GetResponse();
        }
    }
}