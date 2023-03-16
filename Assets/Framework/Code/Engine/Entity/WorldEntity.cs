using System;
using System.Linq;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.U2D;

namespace Jape
{
    public abstract class WorldEntity : Entity
    {
        protected override Texture2D Icon => GetIcon("IconWorldEntity");

        [SerializeField] 
        [Eject] 
        protected Filter filter;

        protected override Filter Filter => filter;

        public override Enum BaseOutputs() { return BaseOutputsFlags.None | 
                                                    BaseOutputsFlags.OnTrigger | 
                                                    BaseOutputsFlags.OnTouch |
                                                    BaseOutputsFlags.OnStay |
                                                    BaseOutputsFlags.OnLeave |
                                                    BaseOutputsFlags.OnDestroy; }

        protected override Type[] Components()
        {
            return new[]
            {
                typeof(World),
                typeof(ProBuilderMesh),
                Packages.ProBuilder.ColliderBehaviour(),
                Packages.ProBuilder.TriggerBehaviour(),
                typeof(SpriteShapeController),
                typeof(MeshFilter),
                typeof(Renderer),
                typeof(Collider),
                typeof(Collider2D),
                typeof(Rigidbody),
                typeof(Rigidbody2D)
            }.Concat(base.Components()).ToArray();
        }

        protected virtual void TouchAction(GameObject gameObject) {}
        protected virtual void StayAction(GameObject gameObject) {}
        protected virtual void LeaveAction(GameObject gameObject) {} 
        
        protected sealed override void Touch(GameObject gameObject)
        {
            Launch(Jape.BaseOutputs.OnTouch, gameObject); 
            TouchAction(gameObject);
        }

        protected sealed override void Stay(GameObject gameObject)
        {
            Launch(Jape.BaseOutputs.OnStay, gameObject); 
            StayAction(gameObject);
        }

        protected sealed override void Leave(GameObject gameObject)
        {
            Launch(Jape.BaseOutputs.OnLeave, gameObject); 
            LeaveAction(gameObject);
        }
    }
}