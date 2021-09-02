using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using UnityEngine;

namespace JapeNet
{
	public static partial class Net
    {
        public static partial class Server
        {
            public static void SuspendClient(int client)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        OfflineAccessError();
                        return;
                    }

                    case NetManager.Mode.Server:
                    {
                        if (JapeNet.Server.Server.Clients[client].suspended) { Log.Write("Client already suspended"); return; }
                        JapeNet.Server.Server.Clients[client].suspended = true;
                        JapeNet.Server.Server.Send.Suspend(client);
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static void SuspendAll()
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        OfflineAccessError();
                        return;
                    }

                    case NetManager.Mode.Server:
                    {
                        foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetConnectedClients())
                        {
                            SuspendClient(client.id);
                        }
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static void RestoreClient(int client)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        OfflineAccessError();
                        return;
                    }

                    case NetManager.Mode.Server:
                    {
                        if (!JapeNet.Server.Server.Clients[client].suspended) { Log.Write("Client not suspended"); return; }
                        JapeNet.Server.Server.Clients[client].suspended = false;
                        JapeNet.Server.Server.Send.Restore(client);
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static void RestoreAll()
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        OfflineAccessError();
                        return;
                    }

                    case NetManager.Mode.Server:
                    {
                        foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetConnectedClients())
                        {
                            RestoreClient(client.id);
                        }
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static void SpawnLocal(int client, string id, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, int player = 0, bool active = true)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        OfflineAccessError();
                        return;
                    }

                    case NetManager.Mode.Server:
                    {

                        JapeNet.Server.Server.Send.Spawn(client, id, prefab, position, rotation, NetManager.GetParentId(parent), player, active, false);
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static void SpawnTemporaryLocal(int client, string id, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, int player = 0, bool active = true)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        OfflineAccessError();
                        return;
                    }

                    case NetManager.Mode.Server:
                    {

                        JapeNet.Server.Server.Send.Spawn(client, id, prefab, position, rotation, NetManager.GetParentId(parent), player, active, true);
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, int player = 0, bool active = true)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        return Clone();
                    }

                    case NetManager.Mode.Server:
                    {
                        GameObject clone = Clone();
                        foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetConnectedClients())
                        {
                            SpawnLocal(client.id, clone.Id(), prefab, position, rotation, parent, player);
                        }
                        return clone;
                    }

                    default:
                    {
                        ClientAccessError();
                        return null;
                    }
                }

                GameObject Clone()
                {
                    GameObject clone = NetManager.Spawn(Ids.Generate(), prefab.name, position, rotation, NetManager.GetParentId(parent), player, active, false);
                    return clone;
                }
            }

            public static GameObject SpawnTemporary(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, int player = 0, bool active = true)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        return Clone();
                    }

                    case NetManager.Mode.Server:
                    {
                        GameObject clone = Clone();
                        foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetConnectedClients())
                        {
                            SpawnTemporaryLocal(client.id, clone.Id(), prefab, position, rotation, parent, player);
                        }
                        return clone;
                    }

                    default:
                    {
                        ClientAccessError();
                        return null;
                    }
                }

                GameObject Clone()
                {
                    GameObject clone = NetManager.Spawn(Ids.Generate(), prefab.name, position, rotation, NetManager.GetParentId(parent), player, active, true);
                    return clone;
                }
            }

            public static void DespawnLocal(int client, GameObject instance) { DespawnLocal(client, instance.Id()); }
            public static void DespawnLocal(int client, string instanceId)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        OfflineAccessError();
                        return;
                    }

                    case NetManager.Mode.Server:
                    {
                        JapeNet.Server.Server.Send.Despawn(client, instanceId);
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static void Despawn(GameObject instance) { Despawn(instance.Id()); }
            public static void Despawn(string instanceId)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        NetManager.Despawn(instanceId);
                        return;
                    }

                    case NetManager.Mode.Server:
                    {
                        NetManager.Despawn(instanceId);

                        foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetConnectedClients())
                        {
                            DespawnLocal(client.id, instanceId);
                        }

                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static void Parent(GameObject gameObject, Transform parent)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        NetManager.Parent(gameObject.Id(), parent.gameObject.Id());
                        return;
                    }

                    case NetManager.Mode.Server:
                    {
                        NetManager.Parent(gameObject.Id(), parent.gameObject.Id());
                        JapeNet.Server.Server.Send.Parent(gameObject.Id(), parent.gameObject.Id());
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static void SetActive(GameObject gameObject, bool value)
            {
                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        NetManager.SetActive(gameObject.Id(), value);
                        return;
                    }

                    case NetManager.Mode.Server:
                    {
                        NetManager.SetActive(gameObject.Id(), value);
                        JapeNet.Server.Server.Send.SetActive(gameObject.Id(), value);
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static void SceneChange(string scenePath)
            {

                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        NetManager.SceneChange(scenePath, null);
                        return;
                    }

                    case NetManager.Mode.Server:
                    {
                        if (Game.ActiveScene().path == scenePath) { Log.Write("Scene already active"); return; }
                        SuspendAll();
                        NetManager.SceneChange(scenePath, () =>
                        {
                            foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetConnectedClients())
                            {
                                SceneChangeLocal(client.id, scenePath);
                            }
                        });
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            public static void SceneChangeLocal(int client, string scenePath)
            {

                switch (NetManager.GetMode())
                {
                    case NetManager.Mode.Offline:
                    {
                        OfflineAccessError();
                        return;
                    }

                    case NetManager.Mode.Server:
                    {
                        if (!JapeNet.Server.Server.Clients[client].suspended) { SuspendClient(client); }
                        JapeNet.Server.Server.Send.SceneChange(client, scenePath);
                        return;
                    }

                    default:
                    {
                        ClientAccessError();
                        return;
                    }
                }
            }

            private static void ClientAccessError()
            {
                Log.Write("Client cannot call server commands");
            }
        }
    }
}