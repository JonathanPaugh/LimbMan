using System;
using Jape;
using Sirenix.Utilities;
using UnityEngine;

namespace JapeNet
{
	public static partial class Net
    {
        public static partial class Server
        {
            public static class Datacache
            {
                public static void Test()
                {
                    Set("Test", "Test", _ =>
                    {
                        Get("Test", data =>
                        {
                            Remove("Test", remove =>
                            {
                                Log.Write(data);
                                Log.Write(remove);
                            });
                        });
                    });

                    int i = 0;
                    Timer.CreateGlobal().ChangeMode(Timer.Mode.Loop).Set(0.25f).IterationAction(() =>
                    {
                        Publish("Test", i++.ToString());
                    }).Start();

                    Subscribe("Test", Request.Datacache.SubscribeBody.Mode.Concurrent, subscription =>
                    {
                        Log.Write($"Subscribed: {subscription}");

                        Timer.CreateGlobal().ChangeMode(Timer.Mode.Loop).Set(1).IterationAction(() =>
                        {
                            Receive(subscription, data =>
                            {
                                if (data == null) { return; }
                                data.ForEach(d => Log.Write(d));
                            });
                        }).Start();

                        Timer.DelayGlobal(8, Jape.Time.Counter.Seconds, () =>
                        {
                            Unsubscribe(subscription, data =>
                            {
                                if (data == null) { return; }
                                data.ForEach(d => Log.Write(d));
                                Log.Write("Unsubscribed");
                            });
                        });
                    });
                }

                public static void Get(string key, Action<string> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datacache.Get(key)
                                     .Read(response)
                                     .Error(error);
                }

                public static void Set(string key, string value, Action<bool> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datacache.Set(key, value)
                                     .Read(data => response?.Invoke(bool.Parse(data)))
                                     .Error(error);
                }

                public static void Remove(string key, Action<bool> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datacache.Remove(key)
                                     .Read(data => response?.Invoke(bool.Parse(data)))
                                     .Error(error);
                }

                public static void Subscribe(string channel, Request.Datacache.SubscribeBody.Mode mode, Action<string> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datacache.Subscribe(channel, mode)
                                     .Read(response)
                                     .Error(error);
                }

                public static void Unsubscribe(string subscription, Action<string[]> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datacache.Unsubscribe(subscription)
                                     .Read(data => response?.Invoke(JsonUtility.FromJson<Response.SubscriptionBody>(data)?.values))
                                     .Error(error);
                }

                public static void Publish(string channel, string value, Action<long> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datacache.Publish(channel, value)
                                     .Read(data => response?.Invoke(long.Parse(data)))
                                     .Error(error);
                }

                public static void Receive(string subscription, Action<string[]> response = null, Action error = null)
                {
                    if (!Mode.IsServerOnly) { DedicatedAccessError(); return; }

                    JapeNet.Datacache.Receive(subscription)
                                     .Read(data => response?.Invoke(JsonUtility.FromJson<Response.SubscriptionBody>(data)?.values))
                                     .Error(error);
                }
            }
        }
    }
}