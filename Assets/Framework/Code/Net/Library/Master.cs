using System;
using Jape;
using UnityEngine;

namespace JapeNet
{
	internal static class Master
    {
        private static NetSettings Settings => NetManager.Settings;

        private static string Ip => Settings.masterServerIp;
        private static int Port => Settings.masterServerPort;

        internal static Response ServerCreate()
        {
            Request request = Request.Create($"http://{Ip}:{Port}");
            return request.GetResponse();
        }

        internal static Response ServerInfo()
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                JsonUtility.ToJson(new Request.ClientBody("Info"))
            );
            return request.GetResponse();
        }

        internal static Response ServerActivate()
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}", 
                JsonUtility.ToJson(new Request.ServerBody("Activate"))
            );
            return request.GetResponse();
        }

        internal static Response ServerDestroy()
        {
            Request request = Request.Delete
            (
                $"http://{Ip}:{Port}",
                Settings.MasterServerKey()
            );
            return request.GetResponse();
        }

        internal static Response PlayerConnect()
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}",
                JsonUtility.ToJson(new Request.ServerBody("Connect"))
            );
            return request.GetResponse();
        }

        internal static Response PlayerDisconnect()
        {
            Request request = Request.Post
            (
                $"http://{Ip}:{Port}",
                JsonUtility.ToJson(new Request.ServerBody("Disconnect"))
            );
            return request.GetResponse();
        }
    }
}