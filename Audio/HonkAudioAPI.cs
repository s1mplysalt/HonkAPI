using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

namespace HonkAPI.Audio
{
    public static class HonkAudioAPI
    {
        public static void PlaySound(string fmodEvent, Vector3 position)
        {
            if (string.IsNullOrEmpty(fmodEvent)) return;
            try
            {
                AudioManager.TriggerOneShotEvent(fmodEvent, position);
            }
            catch (Exception)
            {
            }
        }

        public static void PlaySound(string fmodEvent)
        {
            if (string.IsNullOrEmpty(fmodEvent)) return;
            try
            {
                AudioManager.TriggerOneShotEvent(fmodEvent);
            }
            catch (Exception)
            {
            }
        }

        public static void PlayOneShot(string soundPath, Vector3 position)
        {
            PlaySound(soundPath, position);
        }

        /// <summary>Plays an FMOD event at a position with a pitch multiplier.</summary>
        public static void PlayPitched(string fmodEvent, Vector3 position, float pitch)
        {
            if (string.IsNullOrEmpty(fmodEvent)) return;
            try
            {
                EventInstance inst = RuntimeManager.CreateInstance(fmodEvent);
                inst.set3DAttributes(RuntimeUtils.To3DAttributes(position));
                inst.setPitch(pitch);
                inst.start();
                inst.release();
            }
            catch (Exception e)
            {
                Debug.LogWarning("[HonkAPI] PlayPitched failed: " + e.Message);
            }
        }

        /// <summary>Plays the goose's own honk sound at a position.</summary>
        public static void PlayHonk(Vector3 position, float pitch = 1f)
        {
            Goose goose = HonkAPI.GooseApi.GooseAPI.Current;
            if (goose == null || goose.gooseHonker == null || string.IsNullOrEmpty(goose.gooseHonker.honkAudioEvent)) return;
            if (Mathf.Approximately(pitch, 1f)) PlaySound(goose.gooseHonker.honkAudioEvent, position);
            else PlayPitched(goose.gooseHonker.honkAudioEvent, position, pitch);
        }

        public static void SetParameter(EventInstance instance, string name, float value)
        {
            try { instance.setParameterValue(name, value); }
            catch (Exception) { }
        }

        public static void SetPitch(EventInstance instance, float pitch)
        {
            try { instance.setPitch(pitch); }
            catch (Exception) { }
        }

        /// <summary>Sets the volume (0..1+) of an FMOD bus, for example "bus:/" for master.</summary>
        public static bool SetBusVolume(string busPath, float volume)
        {
            try
            {
                Bus bus = RuntimeManager.GetBus(busPath);
                return bus.setVolume(Mathf.Max(0f, volume)) == FMOD.RESULT.OK;
            }
            catch (Exception) { return false; }
        }

        public static float GetBusVolume(string busPath)
        {
            try
            {
                Bus bus = RuntimeManager.GetBus(busPath);
                float v; float fv;
                if (bus.getVolume(out v, out fv) == FMOD.RESULT.OK) return v;
            }
            catch (Exception) { }
            return -1f;
        }

        public static void StopAllSounds()
        {
            try
            {
                RuntimeManager.GetBus("bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
            catch (Exception) { }
        }

        public static void MuteMusic(bool mute)
        {
            if (MusicMachine.instance != null)
            {
                MusicMachine.instance.enabled = !mute;
                if (mute)
                {
                    MusicMachine.Stop();
                }
            }
        }

        public static void SetMasterVolume(float volume)
        {
            AudioListener.volume = Mathf.Clamp01(volume);
        }

        public static EventInstance BeginSound(string fmodEvent, Transform target)
        {
            if (string.IsNullOrEmpty(fmodEvent) || target == null) return default(EventInstance);
            try
            {
                return AudioManager.BeginEvent(fmodEvent, target, true);
            }
            catch (Exception)
            {
                return default(EventInstance);
            }
        }

        public static void StopSound(EventInstance instance)
        {
            try
            {
                AudioManager.EndEvent(instance);
            }
            catch (Exception)
            {
            }
        }

        public static void PlayCustomSound(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;
            try
            {
                GameObject temp = new GameObject("HonkCustomAudio");
                temp.transform.position = position;
                AudioSource source = temp.AddComponent<AudioSource>();
                source.clip = clip;
                source.volume = Mathf.Clamp01(volume);
                source.spatialBlend = 1f;
                source.Play();
                UnityEngine.Object.Destroy(temp, clip.length + 0.5f);
            }
            catch (Exception)
            {
            }
        }
    }
}
