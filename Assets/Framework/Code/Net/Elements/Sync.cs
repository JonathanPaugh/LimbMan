using System.Collections;
using System.Collections.Generic;
using Jape;
using Sirenix.OdinInspector;
using UnityEngine;

namespace JapeNet
{
    public class Sync : NetElement, IEntityComponent
    {
        public enum SyncMode { None, Local, Global }
        public enum SyncScaleMode { None, Local }

        protected override int ServerStreamRate => rate != 0 ? rate : DefaultStreamRate;

        public override Key PairKey => Key;
        protected override Key GenerateKey() => pooled ? new Key(PairType, poolId.ToString(), Key.IdentifierEncoding.Ascii) : base.GenerateKey();

        private PoolCoordinator SyncPoolCoordinator => NetManager.Instance.syncPoolCoordinator;

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
        private int poolId = 0;

        [ShowInInspector, ShowIf(nameof(ShowPoolMaster)), ReadOnly]
        private bool poolMaster;
        private bool ShowPoolMaster => Game.IsRunning && Mode.IsServer && pooled;

        private Vector3 positionPrevious = Vector3.zero;
        private Quaternion rotationPrevious = Quaternion.identity;
        private Vector3 scalePrevious = Vector3.zero;

        protected override void Enabled() { AddPool(); }
        protected override void Disabled() { RemovePool(); }

        internal override void ReceiveStreamData(NetSide side, object[] data)
        {
            if (!pooled)
            {
                ReceiveStreamDataLow(side, data);
                return;
            }

            if (!SyncPoolCoordinator.TryGetPool(poolId, out PoolCoordinator.Pool pool))
            {
                Log.Write($"Unable to find pool {poolId}");
            }

            foreach (Sync sync in pool)
            {
                sync.ReceiveStreamDataLow(side, data);
            }
        }

        internal void ReceiveStreamDataLow(NetSide side, object[] data)
        {
            base.ReceiveStreamData(side, data);
        }

        protected override void SendStream(NetStream.IServerWriter stream)
        {
            if (pooled && !poolMaster) { return; }

            if (position != SyncMode.None)
            {
                Vector3 temp = new();

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
                Quaternion temp = new();

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
                Vector3 temp = new();

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

        protected override void ReceiveStream(NetStream.IClientReader stream)
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
            if (NetManager.Instance == null) { return; }
            if (!SyncPoolCoordinator.TryGetPool(poolId, out PoolCoordinator.Pool pool))
            {
                poolMaster = true;
                pool = SyncPoolCoordinator.CreatePool(poolId);
            }

            if (pool.Contains(this))
            {
                this.Log().Warning("Already in pool");
                return;
            }

            pool.Add(this);
        }

        private void RemovePool()
        {
            if (!pooled) { return; }
            if (NetManager.Instance == null) { return; }
            if (!SyncPoolCoordinator.TryGetPool(poolId, out PoolCoordinator.Pool pool))
            {
                this.Log().Warning($"Pool {poolId} does not exist");
                return;
            }

            if (!pool.Contains(this))
            {
                this.Log().Warning($"Unable to find in pool {poolId}");
            }

            pool.Remove(this);

            if (!pool.IsEmpty)
            {
                if (poolMaster)
                {
                    poolMaster = false;
                    pool.GetMaster().poolMaster = true;
                }
            }
            else
            {
                SyncPoolCoordinator.RemovePool(poolId);
            }
        }

        internal class PoolCoordinator
        {
            private Dictionary<int, Pool> pools = new();

            public bool TryGetPool(int id, out Pool pool)
            {
                return pools.TryGetValue(id, out pool);
            }

            public Pool CreatePool(int id)
            {
                Pool pool = new();
                pools.Add(id, pool);
                return pool;
            }

            public void RemovePool(int id)
            {
                pools.Remove(id);
            }

            internal class Pool : IEnumerable<Sync>
            {
                private List<Sync> syncs = new();

                public bool IsEmpty => syncs.Count <= 0;

                public void Add(Sync sync)
                {
                    syncs.Add(sync);
                }

                public void Remove(Sync sync)
                {
                    syncs.Remove(sync);
                }

                public bool Contains(Sync sync)
                {
                    return syncs.Contains(sync);
                }

                public Sync GetMaster()
                {
                    return syncs?[0];
                }

                public IEnumerator<Sync> GetEnumerator()
                {
                    return syncs.GetEnumerator();
                }

                IEnumerator IEnumerable.GetEnumerator()
                {
                    return GetEnumerator();
                }
            }
        }
    }
}