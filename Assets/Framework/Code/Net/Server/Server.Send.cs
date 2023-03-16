using System.Collections.Generic;
using Jape;
using UnityEngine;

namespace JapeNet.Server
{
    public partial class Server
    {
        public enum Packets
        {
            _CONNECT_PACKETS_ = 1,
            Register,
            VerifyTcp,
            VerifyUdp,
            Connect,
            _SYSTEM_PACKETS_,
            Pong,
            Suspend,
            Restore,
            PlayerConnect,
            PlayerDisconnect,
            SceneChange,
            _DATA_PACKETS_,
            Spawn,
            Despawn,
            Parent,
            SetActive,
            Field,
            Call,
            Stream,
            Sync,
            Response,
            ResponseClose,
        }

        public static class Send
        {
            internal static void Register(int client)
            {
                using (Packet packet = new((int)Packets.Register))
                {
                    packet.Write(client);
                    SendTcp(client, packet);
                }
            }

            internal static void VerifyTcp(int client)
            {
                using (Packet packet = new((int)Packets.VerifyTcp))
                {
                    SendTcp(client, packet);
                }
            }

            internal static void VerifyUdp(int client)
            {
                using (Packet packet = new((int)Packets.VerifyUdp))
                {
                    SendUdp(client, packet);
                }
            }

            internal static void Connect(int client)
            {
                using (Packet packet = new((int)Packets.Connect))
                {
                    SendTcp(client, packet);
                }
            }

            internal static void Pong(int client, string time)
            {
                using (Packet packet = new((int)Packets.Pong))
                {
                    packet.Write(time);

                    SendTcp(client, packet);
                }
            }

            internal static void Suspend(int client)
            {
                using (Packet packet = new((int)Packets.Suspend))
                {
                    SendTcp(client, packet);
                }
            }

            internal static void Restore(int client)
            {
                using (Packet packet = new((int)Packets.Restore))
                {
                    SendTcp(client, packet);
                }
            }

            internal static void Field(Element.Key elementKey, string name, object value, int exclude)
            {
                using (Packet packet = new((int)Packets.Field))
                {
                    packet.Write(elementKey);
                    packet.Write(name);
                    packet.Write(value);

                    SendTcpExcluding(exclude, packet);
                }
            }

            internal static void Call(Element.Key elementKey, string name, object[] args, int exclude)
            {
                using (Packet packet = new((int)Packets.Call))
                {
                    packet.Write(elementKey);
                    packet.Write(name);
                    packet.Write(args);

                    SendTcpExcluding(exclude, packet);
                }
            }

            internal static void Stream(Element.Key elementKey, object[] streamData, int exclude)
            {
                using (Packet packet = new((int)Packets.Stream))
                {
                    packet.Write(elementKey);
                    packet.Write(streamData);

                    SendUdpExcluding(exclude, packet);
                }
            }

            internal static void Sync(Element.Key elementKey, Dictionary<string, object> syncData, int exclude)
            {
                using (Packet packet = new((int)Packets.Sync))
                {
                    packet.Write(elementKey);
                    packet.Write(syncData.Count);
                    foreach (KeyValuePair<string, object> data in syncData)
                    {
                        packet.Write(data.Key);
                        packet.Write(data.Value);
                    }

                    SendUdpExcluding(exclude, packet);
                }
            }

            internal static void Response(int client, int index, object value, bool dispose)
            {
                using (Packet packet = new((int)Packets.Response))
                {
                    packet.Write(index);
                    packet.Write(value);
                    packet.Write(dispose);

                    SendTcp(client, packet);
                }
            }

            internal static void ResponseClose(int client, int index)
            {
                using (Packet packet = new((int)Packets.ResponseClose))
                {
                    packet.Write(index);
                    SendTcp(client, packet);
                }
            }

            internal static void PlayerConnect(int client)
            {
                using (Packet packet = new((int)Packets.PlayerConnect))
                {
                    packet.Write(client);
                    SendTcpExcluding(client, packet);
                }
            }

            internal static void PlayerDisconnect(int client)
            {
                using (Packet packet = new((int)Packets.PlayerDisconnect))
                {
                    packet.Write(client);
                    SendTcpExcluding(client, packet);
                }
            }

            internal static void Spawn(int client, string id, GameObject prefab, Vector3 position, Quaternion rotation, string parentId, int player, bool active, bool temporary)
            {
                using (Packet packet = new((int)Packets.Spawn))
                {
                    packet.Write(id);
                    packet.Write(prefab.name);
                    packet.Write(position);
                    packet.Write(rotation);
                    packet.Write(parentId);
                    packet.Write(player);
                    packet.Write(active);
                    packet.Write(temporary);

                    SendTcp(client, packet);
                }
            }

            internal static void Despawn(int client, string id)
            {
                using (Packet packet = new((int)Packets.Despawn))
                {
                    packet.Write(id);

                    SendTcp(client, packet);
                }
            }

            internal static void Parent(string id, string parentId)
            {
                using (Packet packet = new((int)Packets.Parent))
                {
                    packet.Write(id);
                    packet.Write(parentId);

                    SendTcpAll(packet);
                }
            }

            internal static void SetActive(string id, bool value)
            {
                using (Packet packet = new((int)Packets.SetActive))
                {
                    packet.Write(id);
                    packet.Write(value);

                    SendTcpAll(packet);
                }
            }

            internal static void SceneChange(int client, string scenePath)
            {
                using (Packet packet = new((int)Packets.SceneChange))
                {
                    packet.Write(scenePath);

                    SendTcp(client, packet);
                }
            }
        }
    }
}