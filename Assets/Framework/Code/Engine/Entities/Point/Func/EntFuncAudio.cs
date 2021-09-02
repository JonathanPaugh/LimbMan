using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

namespace Jape
{
    [AddComponentMenu("")]
    public class EntFuncAudio : EntFunc
    {
        protected override Texture Icon => GetIcon("IconAudio");

        [Space(8)]

        public SoundClip soundClip;

        public AudioMixerGroup mixer;

        [Space(8)]

        [Range(0, 100)] 
        public float volume = 100;

        [Space(8)]

        public bool spatial;
        [ShowIf(nameof(spatial))]
        public AudioRolloffMode rolloff;
        [ShowIf(nameof(spatial))]
        public float range = 500;
        public float spread = 0;
        public float doppler = 0;

        [Space(8)]

        public bool autoStart = false;
        public bool destroyOnComplete = true;

        private List<AudioSource> audioSources = new List<AudioSource>();

        private Job job;

        protected override void Activated() { job = CreateJob().Set(PlayRoutine()); }
        protected override void Init() { if (autoStart) { AudioPlay(); }}

        [Route]
        public void AudioPlay()
        {
            if (job.IsProcessing()) { this.Log().Response("Cannot start when audio is playing"); return; }
            job.Start();
        }

        private IEnumerable PlayRoutine()
        {
            do 
            {
                foreach (SoundClip.Sound sound in soundClip.sounds)
                {
                    if (soundClip.mode == SoundClip.Mode.Simultaneous) { RunJob(PlaySound(sound)); } 
                    else { yield return RunJob(PlaySound(sound)).WaitIdle(); }
                }
            } while (soundClip.loop);

            yield return new WaitUntil(() => audioSources.Count == 0);

            if (destroyOnComplete) { Destroy(gameObject); }

            yield return null;
        }

        private IEnumerable PlaySound(SoundClip.Sound sound)
        {
            AudioSource source = CreateAudioSource();

            SoundClip.Clip clip = sound.GetClip();

            source.clip = clip.audio;
            source.volume = sound.GetVolume(clip) * (volume / 100);
            source.pitch = sound.GetSpeed(clip);

            source.time = default;
            if (source.pitch < 0) { source.time = source.clip.length - 0.01f; }

            yield return Wait.Seconds(sound.delay);

            source.Play();

            yield return Wait.Until(() => source == null || !source.isPlaying);

            DestroyAudioSource(source);
        }
        
        private AudioSource CreateAudioSource()
        {
            GameObject gameObject = Game.CreateGameObject("AudioSource", typeof(AudioSource));
            gameObject.transform.SetParent(transform, false);
            AudioSource source = gameObject.GetComponent<AudioSource>();
            source.loop = false;
            source.playOnAwake = false;
            source.outputAudioMixerGroup = mixer;
            if (spatial)
            {
                source.spatialBlend = 1;
                source.maxDistance = range;
                source.rolloffMode = rolloff;
                source.spread = spread;
                source.dopplerLevel = doppler;
            }
            audioSources.Add(source);
            return source;
        }

        private void DestroyAudioSource(AudioSource audioSource)
        {
            audioSources.Remove(audioSource);
            Destroy(audioSource.gameObject);
        }

        public static EntFuncAudio Create(SoundClip soundClip, Vector3 position, Transform parent = default, bool play = true)
        {
            EntFuncAudio entity = Game.CreateEntity<EntFuncAudio>();
            entity.transform.SetParent(parent);
            entity.Position = position;

            entity.soundClip = soundClip;

            if (play) { entity.AudioPlay(); }

            return entity;
        }
    }
}