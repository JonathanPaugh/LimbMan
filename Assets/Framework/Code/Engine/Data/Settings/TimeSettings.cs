using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class TimeSettings : SettingsData
    {
        private bool VSyncActive() { return vSync != 0; }

        public void Apply()
        {
            QualitySettings.vSyncCount = vSync;
            Application.targetFrameRate = Game.IsWeb ? webRate : frameRate;
            UnityEngine.Time.fixedDeltaTime = Time.ConvertRate(tickRate);
            UnityEngine.Time.maximumParticleDeltaTime = particleThreshold;
            UnityEngine.Time.maximumDeltaTime = Game.IsWeb ? webThreshold : tickThreshold;
        }

        [PropertySpace(4)]

        [ShowInInspector]
        [DelayedProperty]
        public int VSync
        { 
            get
            {
                if (QualitySettings.vSyncCount != vSync) { QualitySettings.vSyncCount = vSync; }
                return vSync;
            }
            set
            {
                vSync = value;
                QualitySettings.vSyncCount = vSync;

                if (VSyncActive()) { FrameRate = -1; }

                Save();
            }
        }

        [SerializeField, HideInInspector]
        private int vSync;

        [PropertySpace(4)]

        [ShowInInspector]
        [DelayedProperty]
        public int FrameRate
        { 
            get
            {
                if (Application.targetFrameRate != frameRate) { Application.targetFrameRate = frameRate; }
                return frameRate;
            }
            set
            {
                frameRate = value;
                Application.targetFrameRate = frameRate;
                Save();
            }
        }

        [SerializeField, HideInInspector]
        private int frameRate = -1;

        [ShowInInspector]
        [DelayedProperty]
        public int TickRate
        {
            get
            {
                if (Time.ConvertInterval(UnityEngine.Time.fixedDeltaTime) != tickRate) { UnityEngine.Time.fixedDeltaTime = Time.ConvertRate(tickRate); }
                return tickRate;
            }
            set 
            {
                tickRate = value;
                UnityEngine.Time.fixedDeltaTime = Time.ConvertRate(tickRate);
                Save();
            }
        }

        [SerializeField, HideInInspector]
        private int tickRate = 60;

        [PropertyOrder(1)]
        public int webRate = 60;

        [PropertySpace(4)]

        [ShowInInspector]
        [DelayedProperty]
        [PropertyOrder(1)]
        public float TickThreshold
        {
            get
            {
                UnityEngine.Time.maximumDeltaTime = tickThreshold;
                return tickThreshold;
            }
            set 
            {
                tickThreshold = value;
                UnityEngine.Time.maximumDeltaTime = tickThreshold;
                Save();
            }
        }

        [SerializeField, HideInInspector]
        private float tickThreshold = 0.3f;

        [SerializeField]
        [PropertyOrder(2)]
        private float webThreshold = 1.0f;

        [ShowInInspector]
        [DelayedProperty]
        [PropertyOrder(2)]
        public float ParticleThreshold
        {
            get
            {
                UnityEngine.Time.maximumParticleDeltaTime = particleThreshold;
                return particleThreshold;
            }
            set
            {
                particleThreshold = value;
                UnityEngine.Time.maximumParticleDeltaTime = particleThreshold;
                Save();
            }
        }

        [SerializeField, HideInInspector]
        private float particleThreshold = 0.1f;
    }
}