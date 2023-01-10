using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class NetSettings : SettingsData
    {
        public string MasterKey
        {
            get
            {
                #if UNITY_EDITOR || UNITY_STANDALONE_LINUX
                return Environment.GetEnvironmentVariable("API_JAPE_MASTER");
                #endif
                #pragma warning disable 162
                return null;
                #pragma warning restore 162
            }
        }

        public string DatabaseKey
        {
            get
            {
                #if UNITY_EDITOR || UNITY_STANDALONE_LINUX
                return Environment.GetEnvironmentVariable("API_JAPE_DATABASE");
                #endif
                #pragma warning disable 162
                return null;
                #pragma warning restore 162
            }
        }

        [PropertyOrder(1)]
        public string serverName;

        [PropertyOrder(1)]
        [SerializeField]
        [HideIf(Game.GameIsRunning)]
        private string serverIp;

        [PropertyOrder(1)]
        [NonSerialized] 
        private string serverIpRuntime;

        [PropertyOrder(1)]
        [ShowIf(Game.GameIsRunning)]
        [ShowInInspector]
        public string ServerIp
        {
            get => string.IsNullOrEmpty(serverIpRuntime) ? serverIp : serverIpRuntime;
            set
            {
                if (Game.IsRunning) { serverIpRuntime = value; }
                else { serverIp = value; }
            }
        }

        [PropertyOrder(2)]
        public int serverPort;

        [Space(8)]

        [PropertyOrder(3)]
        public Protocol masterServerProtocol;

        [PropertyOrder(3)]
        public string masterServerIp;

        [PropertyOrder(3)]
        public int masterServerPort;

        [Space(8)]

        [PropertyOrder(4)]
        public Protocol databaseProtocol;

        [PropertyOrder(4)]
        public string databaseIp;

        [PropertyOrder(4)]
        public int databasePort;

        [Space(8)]

        [PropertyOrder(5)]
        public int maxPlayers;

        [Space(8)]

        [PropertyOrder(5)]
        public int bufferSize;

        [Space(8)]

        [PropertyOrder(5)]
        public bool tcpBatching;

        public bool IsClient()
        {
            return (!Application.dataPath.Contains(serverName) && SystemInfo.graphicsDeviceID != 0) || Game.IsWeb;
        }

        public bool IsClientBuild()
        {
            return IsClient() && Game.IsBuild;
        }

        public bool IsServer()
        {
            return (Application.dataPath.Contains(serverName) || SystemInfo.graphicsDeviceID == 0) && !Game.IsWeb;
        }

        public bool IsServerBuild()
        {
            return IsServer() && Game.IsBuild;
        }
    }
}