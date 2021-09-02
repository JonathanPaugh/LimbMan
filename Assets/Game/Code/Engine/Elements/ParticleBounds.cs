using UnityEngine;
using Jape;
using Sirenix.OdinInspector;

namespace Game
{
    public class ParticleBounds : Element
    {
        [SerializeField]
        [ReadOnly]
        private new Collider2D collider;

        [SerializeField]
        [ReadOnly]
        private new ParticleSystem particleSystem;

        protected override void Frame()
        {
            Set();

            if (collider == null || particleSystem == null) { return; }

            ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
            int count = particleSystem.GetParticles(particles);

            if (count <= 0) { return; }

            for (int i = 0; i < count; i++)
            {
                if (collider.OverlapPoint(particles[i].position)) { continue; }
                particles[i].remainingLifetime = -1;
            }

            particleSystem.SetParticles(particles, count);
        }

        protected override void FrameEditor() { Set(); }

        private void Set()
        {
            if (collider == null) { collider = GetComponent<Collider2D>(); }
            if (particleSystem == null)
            {
                particleSystem = GetComponentInChildren<ParticleSystem>();
                
            }
        }
    }
}