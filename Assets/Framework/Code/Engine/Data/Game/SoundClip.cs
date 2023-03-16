using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Jape
{
    public class SoundClip : GameData
    {
        public enum Mode { Linear, Simultaneous };

        [Space(8)]

        public Mode mode;

        [Space(4)]

        public bool loop;

        [Space(8)]

        public Sound[] sounds;

        [Serializable]
        public class Sound
        {
            [Space(8)]

            public Clip[] clips;

            [Space(4)]

            [PropertyRange(0, 100)] 
            public float volume = 100;

            [PropertySpace(8, 8)]

            public float delay;

            public Clip GetClip() { return clips.ElementAtOrDefault(Random.Int(0, clips.Length - 1)); }

            public float GetVolume(Clip clip) { return volume * clip.volume / 10000; }
            public float GetSpeed(Clip clip) { return clip.speed; }
        }

        [Serializable]
        public class Clip
        {
            [Space(8)]

            public AudioClip audio;

            [Space(8)]

            [PropertyRange(0, 100)] 
            public float volume = 100;

            [PropertyRange(-3, 3)] 
            public float speed = 1;
        }
    }
}