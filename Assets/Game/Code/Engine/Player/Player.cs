using System;
using Jape;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = Jape.Random;
using Time = Jape.Time;

namespace Game
{
    public class Player : Element
    {
        public Rig rig;

        [HideInInspector]
        public Movement movement;

        [HideInInspector]
        public new Rigidbody2D rigidbody;

        [HideInInspector]
        public new BoxCollider2D collider;

        [HideInInspector]
        public new EntFuncAudio audio;

        [HideInInspector]
        public Animator animator;

        [HideInInspector] 
        public AnimatorOverrideController animatorOverrides;

        [HideInInspector]
        public GameAnimation.AnimationClipOverrides animatorClips;

        protected Receiver<Damage> damageReceiver = Receive(delegate(Element element, Damage damage)
        {
            ((Player)element).Die();
        });

        protected override void Init()
        {
            movement = GetComponent<Movement>();
            rigidbody = GetComponent<Rigidbody2D>();
            collider = GetComponent<BoxCollider2D>();
            audio = GetComponentInChildren<EntFuncAudio>();
            animator = GetComponent<Animator>();
            
            if (animator != null)
            {
                animatorOverrides = new AnimatorOverrideController(animator.runtimeAnimatorController);
                animatorClips = new GameAnimation.AnimationClipOverrides(animatorOverrides.overridesCount);
            }

            Jape.Game.DefaultCamera.GetComponent<Camera>().SetTarget(this);
        }

        public void Die()
        {
            EntFuncAudio.Create(Database.GetAsset<SoundClip>("Explosion", true).Load<SoundClip>(), Vector3.zero, GameManager.Instance.transform);

            Jape.Game.CloneGameObject(Database.GetAsset<GameObject>("ParticleDeath", true).Load<GameObject>(), transform.position); 

            DetachLimb("RenderLeftLeg");
            DetachLimb("RenderRightLeg");
            DetachLimb("RenderLeftArm");
            DetachLimb("RenderRightArm");

            Destroy(gameObject);

            GameManager.Instance.Timer.Stop();

            Timer.Delay(0.5f, Time.Counter.Seconds, GameManager.Spawn);

            void DetachLimb(string name)
            {
                GameObject gameObject = transform.FindChildDeep(name).gameObject;
                gameObject.transform.SetParent(null);
                Destroy(gameObject.GetComponent("SpriteSkin"));
                Rigidbody2D rigidbody = gameObject.AddComponent<Rigidbody2D>();
                SceneManager.MoveGameObjectToScene(gameObject, Jape.Game.ActiveScene());

                rigidbody.AddForce(Random.Vector(new Vector2(-10, -10), new Vector2(10, 10)), ForceMode2D.Impulse);
                rigidbody.AddTorque(Random.Float(-10, 10), ForceMode2D.Impulse);
            }
        }

        private void OnParticleCollision(GameObject gameObject)
        {
            Die();
        }

        [Serializable]
        public class Rig
        {
            public GameObject Root;
            public GameObject Head;
            public GameObject RightHand;
            public GameObject LeftHand;
            public GameObject RightFoot;
            public GameObject LeftFoot;
        }
    }
}