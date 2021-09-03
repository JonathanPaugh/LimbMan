using System;
using System.Collections.Generic;
using System.Linq;
using Jape;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JapeNet
{
    public class Sync : NetElement, IEntityComponent
    {
        public enum SyncMode { None, Local, Global }
        public enum SyncScaleMode { None, Local }

        protected override int ServerStreamRate => rate != 0 ? rate : NetManager.Settings.serverStreamRate;

        public override string Key => pooled ? $"{PairType.FullName}_Pool_{pool}" : base.PairKey;
        public override string PairKey => Key;

        private static Dictionary<int, List<Sync>> pools = new Dictionary<int, List<Sync>>();

        [SerializeField] private SyncMode position = SyncMode.Local;
        [SerializeField] private SyncMode rotation = SyncMode.Local;
        [SerializeField] private SyncScaleMode scale = SyncScaleMode.Local;

        [Space(8)]
        [PropertySpace(SpaceAfter = 8)]

        [SerializeField] 
        private int rate = 0;

        [SerializeField] 
        private bool always = true;

        [HorizontalGroup]
        [SerializeField] 
        [LabelText("Pool")]
        private bool pooled = false;

        [HorizontalGroup]
        [ShowIf(nameof(pooled))]
        [HideLabel]
        [SerializeField]
        private int pool = 0;

        private bool poolMaster;

        private Vector3 positionPrevious = Vector3.zero;
        private Quaternion rotationPrevious = Quaternion.identity;
        private Vector3 scalePrevious = Vector3.zero;

        protected override void Enabled() { AddPool(); }
        protected override void Disabled() { RemovePool(); }

        internal override void PushStreamData(object[] data)
        {
            if (!pooled) { base.PushStreamData(data); return; }

            foreach (Sync sync in pools[pool])
            {
                sync.stream.PushData(data);
            }
        }

        protected override void SendStream(NetStream.ServerWriter stream)
        {
            if (pooled && !poolMaster) { return; }

            if (position != SyncMode.None)
            {
                Vector3 temp = new Vector3();

                switch (position)
                {
                    case SyncMode.Local:
                        temp = transform.localPosition;
                        break;

                    case SyncMode.Global:
                        temp = transform.position;
                        break;
                }

                if (always)
                {
                    stream.Stream(temp);
                }
                else
                {
                    bool send = positionPrevious != temp;
                    stream.Stream(send);

                    if (send)
                    {
                        stream.Stream(temp);
                        positionPrevious = temp;
                    }
                }
            }

            if (rotation != SyncMode.None)
            {
                Quaternion temp = new Quaternion();

                switch (rotation)
                {
                    case SyncMode.Local:
                        temp = transform.localRotation;
                        break;

                    case SyncMode.Global: 
                        temp = transform.rotation;
                        break;
                }

                if (always)
                {
                    stream.Stream(temp);
                }
                else
                {
                    bool send = rotationPrevious != temp;
                    stream.Stream(send);

                    if (send)
                    {
                        stream.Stream(temp);
                        rotationPrevious = temp;
                    }
                }
            }

            if (scale != SyncScaleMode.None)
            {
                Vector3 temp = new Vector3();

                switch (scale)
                {
                    case SyncScaleMode.Local: 
                        temp = transform.localScale;
                        break;
                }

                if (always)
                {
                    stream.Stream(temp);
                }
                else
                {
                    bool send = scalePrevious != temp;
                    stream.Stream(send);

                    if (send)
                    {
                        stream.Stream(temp);
                        scalePrevious = temp;
                    }
                }
            }
        }

        protected override void ReceiveStream(NetStream.ClientReader stream)
        {
            if (position != SyncMode.None)
            {
                if (always)
                {
                    StreamPosition();
                }
                else
                {
                    if (stream.StreamFirst<bool>())
                    {
                        StreamPosition();
                    }
                }
            }

            if (rotation != SyncMode.None)
            {
                if (always)
                {
                    StreamRotation();
                }
                else
                {
                    if (stream.StreamFirst<bool>())
                    {
                        StreamRotation();
                    }
                }
                
            }

            if (scale != SyncScaleMode.None)
            {
                if (always)
                {
                    StreamScale();
                }
                else
                {
                    if (stream.StreamFirst<bool>())
                    {
                        StreamScale();
                    }
                }
            }

            void StreamPosition()
            {
                switch (position)
                {
                    case SyncMode.Local: 
                        transform.localPosition = stream.StreamFirst<Vector3>(); 
                        break;

                    case SyncMode.Global: 
                        transform.position = stream.StreamFirst<Vector3>(); 
                        break;
                }
            }

            void StreamRotation()
            {
                switch (rotation)
                {
                    case SyncMode.Local: 
                        transform.localRotation = stream.StreamFirst<Quaternion>(); 
                        break;

                    case SyncMode.Global: 
                        transform.rotation = stream.StreamFirst<Quaternion>(); 
                        break;
                }
            }

            void StreamScale()
            {
                switch (scale)
                {
                    case SyncScaleMode.Local: 
                        transform.localScale = stream.StreamFirst<Vector3>(); 
                        break;
                }
            }
        }

        private void AddPool()
        {
            if (!pooled) { return; }
            if (!pools.TryGetValue(pool, out List<Sync> syncs))
            {
                poolMaster = true;
                syncs = new List<Sync>();
                pools.Add(pool, syncs);
            }

            if (syncs.Contains(this))
            {
                this.Log().Warning("Already in pool");
                return;
            }

            syncs.Add(this);
        }

        private void RemovePool()
        {
            if (!pooled) { return; }
            if (!pools.TryGetValue(pool, out List<Sync> syncs))
            {
                this.Log().Warning($"Pool {pool} does not exist");
                return;
            }

            if (!syncs.Contains(this))
            {
                this.Log().Warning($"Unable to find in pool {pool}");
            }

            syncs.Remove(this);

            if (syncs.Count > 0)
            {
                if (poolMaster)
                {
                    poolMaster = false;
                    syncs[0].poolMaster = true;
                }
            }
            else
            {
                pools.Remove(pool);
            }
        }
    }
}