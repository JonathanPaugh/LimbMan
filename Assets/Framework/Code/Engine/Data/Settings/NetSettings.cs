using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Jape
{
    public class NetSettings : SettingsData
    {
        public string MasterServerKey()
        {
            #if UNITY_EDITOR || UNITY_STANDALONE_LINUX
            return "8733A5417F3398458B22632997B3D";
            #endif
            #pragma warning disable 162
            return null;
            #pragma warning restore 162
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

        [PropertyOrder(2)]
        public int serverWebPort;

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

        [Space(32)]

        [PropertyOrder(6)]
        public int clientStreamRate = 1;

        [PropertyOrder(6)]
        public int serverStreamRate = 1;

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