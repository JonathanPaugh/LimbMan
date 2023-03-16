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
                if (Mode.IsServer)
                {
                    if (JapeNet.Server.Server.GetClient(client).suspended) { Log.Write("Client already suspended"); return; }
                    JapeNet.Server.Server.GetClient(client).suspended = true;
                    JapeNet.Server.Server.Send.Suspend(client);
                }
                else if (Mode.IsClient)
                {
                    ClientAccessError();
                } 
                else
                {
                    OfflineAccessError();
                }
            }

            public static void SuspendAll()
            {
                if (Mode.IsServer)
                {
                    foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetRemoteClients())
                    {
                        SuspendClient(client.id);
                    }
                }
                else if (Mode.IsClient)
                {
                    ClientAccessError();
                } 
                else
                {
                    OfflineAccessError();
                }
            }

            public static void RestoreClient(int client)
            {
                if (Mode.IsServer)
                {
                    if (!JapeNet.Server.Server.GetClient(client).suspended) { Log.Write("Client not suspended"); return; }
                    JapeNet.Server.Server.GetClient(client).suspended = false;
                    JapeNet.Server.Server.Send.Restore(client);
                }
                else if (Mode.IsClient)
                {
                    ClientAccessError();
                } 
                else
                {
                    OfflineAccessError();
                }
            }

            public static void RestoreAll()
            {
                if (Mode.IsServer)
                {
                    foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetRemoteClients())
                    {
                        RestoreClient(client.id);
                    }
                }
                else if (Mode.IsClient)
                {
                    ClientAccessError();
                } 
                else
                {
                    OfflineAccessError();
                }
            }

            public static void SpawnLocal(int client, string id, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, int player = 0, bool active = true)
            {
                if (Mode.IsServer)
                {
                    JapeNet.Server.Server.Send.Spawn(client, id, prefab, position, rotation, NetManager.GetParentId(parent), player, active, false);
                }
                else if (Mode.IsClient)
                {
                    ClientAccessError();
                } 
                else
                {
                    OfflineAccessError();
                }
            }

            public static void SpawnTemporaryLocal(int client, string id, GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, int player = 0, bool active = true)
            {
                if (Mode.IsServer)
                {
                    JapeNet.Server.Server.Send.Spawn(client, id, prefab, position, rotation, NetManager.GetParentId(parent), player, active, true);
                }
                else if (Mode.IsClient)
                {
                    ClientAccessError();
                } 
                else
                {
                    OfflineAccessError();
                }
            }

            public static GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, int player = 0, bool active = true)
            {
                if (Mode.IsServer)
                {
                    GameObject clone = Clone();
                    foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetRemoteClients())
                    {
                        SpawnLocal(client.id, clone.Identifier(), prefab, position, rotation, parent, player);
                    }
                    return clone;
                }

                if (!Mode.IsOnline)
                {
                    return Clone();
                } 

                ClientAccessError();
                return null;

                GameObject Clone()
                {
                    GameObject clone = NetManager.Spawn(Ids.Generate(), prefab.name, position, rotation, NetManager.GetParentId(parent), player, active, false);
                    return clone;
                }
            }

            public static GameObject SpawnTemporary(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null, int player = 0, bool active = true)
            {
                if (Mode.IsServer)
                {
                    GameObject clone = Clone();
                    foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetRemoteClients())
                    {
                        SpawnTemporaryLocal(client.id, clone.Identifier(), prefab, position, rotation, parent, player);
                    }
                    return clone;
                }

                if (!Mode.IsOnline)
                {
                    return Clone();
                } 

                ClientAccessError();
                return null;

                GameObject Clone()
                {
                    GameObject clone = NetManager.Spawn(Ids.Generate(), prefab.name, position, rotation, NetManager.GetParentId(parent), player, active, true);
                    return clone;
                }
            }

            public static void DespawnLocal(int client, GameObject instance) { DespawnLocal(client, instance.Identifier()); }
            public static void DespawnLocal(int client, string instanceId)
            {
                if (Mode.IsServer)
                {
                    JapeNet.Server.Server.Send.Despawn(client, instanceId);
                }
                else if (Mode.IsClient)
                {
                    ClientAccessError();
                } 
                else
                {
                    OfflineAccessError();
                }
            }

            public static void Despawn(GameObject instance) { Despawn(instance.Identifier()); }
            public static void Despawn(string instanceId)
            {
                if (Mode.IsServer)
                {
                    NetManager.Despawn(instanceId);

                    foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetRemoteClients())
                    {
                        DespawnLocal(client.id, instanceId);
                    }
                }
                else if (!Mode.IsOnline)
                {
                    NetManager.Despawn(instanceId);
                } 
                else
                {
                    ClientAccessError();
                }
            }

            public static void Parent(GameObject gameObject, Transform parent)
            {
                if (Mode.IsServer)
                {
                    NetManager.Parent(gameObject.Identifier(), parent.gameObject.Identifier());
                    JapeNet.Server.Server.Send.Parent(gameObject.Identifier(), parent.gameObject.Identifier());
                }
                else if (!Mode.IsOnline)
                {
                    NetManager.Parent(gameObject.Identifier(), parent.gameObject.Identifier());
                } 
                else
                {
                    ClientAccessError();
                }
            }

            public static void SetActive(GameObject gameObject, bool value)
            {
                if (Mode.IsServer)
                {
                    NetManager.SetActive(gameObject.Identifier(), value);
                    JapeNet.Server.Server.Send.SetActive(gameObject.Identifier(), value);
                }
                else if (!Mode.IsOnline)
                {
                    NetManager.SetActive(gameObject.Identifier(), value);
                } 
                else
                {
                    ClientAccessError();
                }
            }

            public static void SceneChange(string scenePath)
            {
                if (Mode.IsServer)
                {
                    if (Game.ActiveScene().path == scenePath) { Log.Write("Scene already active"); return; }
                    SuspendAll();
                    NetManager.SceneChange(scenePath, () =>
                    {
                        foreach (JapeNet.Server.Server.Connection client in JapeNet.Server.Server.GetRemoteClients())
                        {
                            SceneChangeLocal(client.id, scenePath);
                        }
                    });
                }
                else if (!Mode.IsOnline)
                {
                    NetManager.SceneChange(scenePath, null);
                } 
                else
                {
                    ClientAccessError();
                }
            }

            public static void SceneChangeLocal(int client, string scenePath)
            {
                if (Mode.IsServer)
                {
                    if (!JapeNet.Server.Server.GetClient(client).suspended) { SuspendClient(client); }
                    JapeNet.Server.Server.Send.SceneChange(client, scenePath);
                }
                else if (Mode.IsClient)
                {
                    ClientAccessError();
                } 
                else
                {
                    OfflineAccessError();
                }
            }

            private static void ClientAccessError()
            {
                Log.Write("Client cannot call server commands");
            }

            private static void DedicatedAccessError()
            {
                Log.Write("Must be a dedicated server to call this command");
            }
        }
    }
}