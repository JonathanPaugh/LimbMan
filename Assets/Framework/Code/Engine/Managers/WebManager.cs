using System;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using UnityEngine;

namespace Jape
{
    public sealed class WebManager : Manager<WebManager> 
    {
        private new static bool InitOnLoad => false;

        private bool saving;
        private bool loading;
        private bool deleting;
        
        private Action onSaveResponse;
        private Action<byte[]> onLoadResponse;
        private Action onDeleteResponse;

        private byte[] saveData;
        private byte[] socketData;

        [UsedImplicitly]
        private void WebSaveReceiveOpen(int length)
        {
            saveData = new byte[length];
        }

        [UsedImplicitly]
        private void WebSaveReceiveData(int pointer)
        {
            Marshal.Copy((IntPtr)pointer, saveData, 0, saveData.Length);
            WebFree(pointer);
        }

        [UsedImplicitly]
        private void WebSaveReceiveClose()
        {
            if (saving)
            {
                saving = false;
                onSaveResponse?.Invoke();
                
            }

            if (loading)
            {
                loading = false;
                onLoadResponse?.Invoke(saveData);
            }

            if (deleting)
            {
                deleting = false;
                onDeleteResponse?.Invoke();
            }
        }

        [UsedImplicitly]
        private void WebSocketReceiveOpen(int length)
        {
            socketData = new byte[length];
        }

        [UsedImplicitly]
        private void WebSocketReceiveClose(int pointer)
        {
            Marshal.Copy((IntPtr)pointer, socketData, 0, socketData.Length);
            GameObject.Find("NetManager").SendMessage("WebSocketReceive", socketData, SendMessageOptions.RequireReceiver);
            WebFree(pointer);
        }

        [DllImport("__Internal")]
        private static extern void WebFree(int pointer);

        [DllImport("__Internal")]
        private static extern bool WebMobile();

        [DllImport("__Internal")]
        private static extern void WebSaveRequestSave(int profile, byte[] data, int length);

        [DllImport("__Internal")]
        private static extern void WebSaveRequestLoad(int profile);

        [DllImport("__Internal")]
        private static extern void WebSaveRequestDelete(int profile);

        [DllImport("__Internal")]
        private static extern void WebSocketConnect(string domain);

        [DllImport("__Internal")]
        private static extern void WebSocketDisconnect();

        [DllImport("__Internal")]
        private static extern void WebSocketSend(byte[] data, int length);

        public static bool IsMobile()
        {
            return WebMobile();
        } 

        public static class Save
        {
            public static void RequestSave(int profile, byte[] data, Action onResponse)
            {
                if (Instance.saving)
                {
                    Log.Warning("Cannot save, already saving");
                    return;
                }

                if (Instance.loading)
                {
                    Log.Warning("Cannot save, already loading");
                    return;
                }

                if (Instance.deleting)
                {
                    Log.Warning("Cannot save, already deleting");
                    return;
                }

                Instance.saving = true;
                Instance.onSaveResponse = onResponse;
                WebSaveRequestSave(profile, data, data.Length);
            }

            public static void RequestLoad(int profile, Action<byte[]> onResponse)
            {
                if (Instance.saving)
                {
                    Log.Warning("Cannot load, already saving");
                    return;
                }

                if (Instance.loading)
                {
                    Log.Warning("Cannot load, already loading");
                    return;
                }

                if (Instance.deleting)
                {
                    Log.Warning("Cannot load, already deleting");
                    return;
                }

                Instance.loading = true;
                Instance.onLoadResponse = onResponse;
                WebSaveRequestLoad(profile);
            }

            public static void RequestDelete(int profile, Action onResponse)
            {
                if (Instance.saving)
                {
                    Log.Warning("Cannot delete, already saving");
                    return;
                }

                if (Instance.loading)
                {
                    Log.Warning("Cannot delete, already loading");
                    return;
                }

                if (Instance.deleting)
                {
                    Log.Warning("Cannot delete, already deleting");
                    return;
                }

                Instance.deleting = true;
                Instance.onDeleteResponse = onResponse;
                WebSaveRequestDelete(profile);
            }
        }

        public static class Socket
        {
            public static void Connect(string domain)
            {
                WebSocketConnect(domain);
            }

            public static void Disconnect()
            {
                WebSocketDisconnect();
            }

            public static void Send(byte[] data)
            {
                WebSocketSend(data, data.Length);
            }
        }
    }
}