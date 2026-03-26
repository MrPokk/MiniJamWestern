using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

namespace InGame.Script.Component_Sound
{
    public class SoundController : MonoBehaviour
    {
        [SerializeField] private SoundsSO _setting;

        private static SoundController s_instance;
        public static SoundController Instance
        {
            get
            {
                if (s_instance == null)
                {
                    var instanceFind = FindFirstObjectByType<SoundController>();
                    if (instanceFind == null)
                    {
                        s_instance = new GameObject("[SoundController]").AddComponent<SoundController>();
                    }
                    else
                    {
                        s_instance = instanceFind;
                    }
                }
                return s_instance;
            }
        }

        private readonly List<AudioSource> _sfxPool = new();

        private readonly Dictionary<SoundType, AudioSource> _activeMusicSources = new();
        private HashSet<SoundType> _loopingMusicTypes = new();

        private void Update()
        {
            foreach (var musicType in _loopingMusicTypes)
            {
                if (_activeMusicSources.TryGetValue(musicType, out var source))
                {
                    if (!source.isPlaying && source.clip != null && source.time == 0)
                    {
                        source.Play();
                    }
                }
            }
        }

        private AudioSource GetFreeSource()
        {
            foreach (var s in _sfxPool)
            {
                if (!s.isPlaying) return s;
            }

            var newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            _sfxPool.Add(newSource);
            return newSource;
        }

        private AudioSource GetMusicSource(SoundType type)
        {
            if (_activeMusicSources.TryGetValue(type, out var existingSource))
            {
                return existingSource;
            }

            var newSource = gameObject.AddComponent<AudioSource>();
            newSource.playOnAwake = false;
            _activeMusicSources.Add(type, newSource);
            return newSource;
        }
        public static void PlaySound(SoundType sound, AudioSource source = null, float volume = 1f)
        {
            PlaySoundInternal(sound, source, volume, 1.0f);
        }

        public static void PlaySoundRandomVolume(SoundType sound, float minVolume = 0.85f, float maxVolume = 1.15f, AudioSource source = null, float pitch = 1f)
        {
            PlaySoundInternal(sound, source, Random.Range(minVolume, maxVolume), pitch);
        }

        public static void PlaySoundRandomPitch(SoundType sound, float minPitch = 0.85f, float maxPitch = 1.15f, AudioSource source = null, float volume = 1f)
        {
            PlaySoundInternal(sound, source, volume, Random.Range(minPitch, maxPitch));
        }

        public static void PlaySoundRandomVolumeAndPitch(SoundType sound, float minVolume, float maxVolume, float minPitch, float maxPitch, AudioSource source = null)
        {
            PlaySoundInternal(sound, source, Random.Range(minVolume, maxVolume), Random.Range(minPitch, maxPitch));
        }

        private static void PlaySoundInternal(SoundType sound, AudioSource source, float volume, float pitch)
        {
            if (s_instance == null || s_instance._setting == null || s_instance._setting.sounds == null) return;

            var index = (int)sound;
            if (index < 0 || index >= s_instance._setting.sounds.Length) return;

            var list = s_instance._setting.sounds[index];
            if (list.sounds == null || list.sounds.Length == 0) return;

            var clip = list.sounds[Random.Range(0, list.sounds.Length)];

            AudioSource target = source;
            if (target == null)
            {
                target = s_instance.GetFreeSource();
                target.spatialBlend = 0f;
            }

            target.outputAudioMixerGroup = list.mixer;
            target.clip = clip;
            target.volume = volume * list.volume;
            target.pitch = pitch;
            target.Play();
        }

        public static void PlayMusic(SoundType musicType, bool loop = true, float volume = 1f, float pitch = 1f)
        {
            if (s_instance == null) return;

            int index = (int)musicType;
            if (index < 0 || index >= s_instance._setting.sounds.Length) return;

            var list = s_instance._setting.sounds[index];
            if (list.sounds == null || list.sounds.Length == 0) return;

            var clip = list.sounds[Random.Range(0, list.sounds.Length)];
            var source = s_instance.GetMusicSource(musicType);

            source.outputAudioMixerGroup = list.mixer;
            source.clip = clip;
            source.volume = volume * list.volume;
            source.pitch = pitch;
            source.loop = loop;

            if (loop) s_instance._loopingMusicTypes.Add(musicType);
            else s_instance._loopingMusicTypes.Remove(musicType);

            source.Play();
        }

        public static void PlayMusicRandomPitch(SoundType musicType, float minPitch = 0.8f, float maxPitch = 1.5f, bool loop = true, float volume = 1f)
        {
            PlayMusic(musicType, loop, volume, Random.Range(minPitch, maxPitch));
        }

        public static void StopMusic(SoundType musicType)
        {
            if (s_instance != null && s_instance._activeMusicSources.TryGetValue(musicType, out var source))
            {
                source.Stop();
                s_instance._loopingMusicTypes.Remove(musicType);
            }
        }

        public static void StopAllMusic()
        {
            if (s_instance == null) return;
            foreach (var kvp in s_instance._activeMusicSources)
            {
                kvp.Value.Stop();
            }
            s_instance._loopingMusicTypes.Clear();
        }

        public static void PauseMusic(SoundType musicType)
        {
            if (s_instance != null && s_instance._activeMusicSources.TryGetValue(musicType, out var source))
            {
                if (source.isPlaying) source.Pause();
            }
        }

        public static void ResumeMusic(SoundType musicType)
        {
            if (s_instance != null && s_instance._activeMusicSources.TryGetValue(musicType, out var source))
            {
                if (!source.isPlaying && source.clip != null) source.Play();
            }
        }
    }

    [Serializable]
    public struct SoundList
    {
        [HideInInspector] public string name;
        [Range(0, 1)] public float volume;
        public AudioMixerGroup mixer;
        public AudioClip[] sounds;
    }
}
