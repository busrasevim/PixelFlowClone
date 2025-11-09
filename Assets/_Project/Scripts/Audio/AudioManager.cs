using System.Collections.Generic;
using _Project.Scripts.Signals;
using UnityEngine;
using UnityEngine.Audio;
using Zenject;

namespace _Project.Scripts.Audio
{
    public class AudioManager : MonoBehaviour
    {
        [SerializeField] private SoundEffectLibrary soundEffectLibrary;

        [Min(1), SerializeField] private int oneShotSourceCount = 3;

        [Header("Mixer")]
        [SerializeField] private AudioMixer masterMixer;
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;
        
        private Dictionary<int, SoundEffect> _soundEffectDictionary;
        private AudioSource[] _oneShotSources;
        private Dictionary<int, AudioSource> _loopSources;
        
        private int _oneShotIndex;
        
        private SignalBus _signalBus;

        [Inject] private ISettingsProvider _settingsProvider;

        [Inject]
        private void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
            _signalBus.Subscribe<OnSoundSettingChangedSignal>(OnSoundSettingChanged);
        }
        
        private void Awake()
        {
            _oneShotSources = new AudioSource[oneShotSourceCount];

            for (int i = 0; i < oneShotSourceCount; i++)
            {
                _oneShotSources[i] = CreateAudioSource($"OneShot Source {i}", sfxGroup);
            }

            _loopSources = new Dictionary<int, AudioSource>();
            _soundEffectDictionary = new Dictionary<int, SoundEffect>();
            
            foreach (var s in soundEffectLibrary.soundEffects)
            {
                if (s.soundEffectKey != SoundEffectKey.None)
                    _soundEffectDictionary[(int)s.soundEffectKey] = s;
            }
        }

        private AudioSource CreateAudioSource(string name, AudioMixerGroup output)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            var src = go.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = output;
            return src;
        }

        private AudioSource GetFreeOneShotSource()
        {
            for (int i = 0; i < oneShotSourceCount; i++)
            {
                var src = _oneShotSources[i];
                if (!src.isPlaying)
                    return src;
            }

            var source = _oneShotSources[_oneShotIndex];
            _oneShotIndex = (_oneShotIndex + 1) % oneShotSourceCount;
            return source;
        }

        public void PlaySoundEffect(SoundEffectKey key, float? customPitch = null)
        {
            if (!_settingsProvider.IsSoundEnabled) return;
            
            if(!_soundEffectDictionary.TryGetValue((int)key, out var sfx))
                return;

            if (sfx.loop)
                PlayLooping(key, sfx, customPitch);
            else
            {
                PlayOneShot(sfx, customPitch);
            }
        }

        private void PlayOneShot(SoundEffect sfx, float? customPitch = null)
        {
            var src = GetFreeOneShotSource();
            sfx.Play(src, customPitch);
        }

        private void PlayLooping(SoundEffectKey key, SoundEffect sfx, float? customPitch = null)
        {
            if (_loopSources.TryGetValue((int)key, out var src))
            {
                if(!src.isPlaying)
                    src.Play();
                
                if (customPitch.HasValue)
                    src.pitch = customPitch.Value;
                
                return;
            }

            var group = sfx.Category == SoundCategory.Music ? musicGroup : sfxGroup;

            src = CreateAudioSource($"Loop Source {key}", group);
            _loopSources[(int)key] = src;
            sfx.Play(src, customPitch);
        }

        private void OnSoundSettingChanged(OnSoundSettingChangedSignal signal)
        {
            SetMasterEnabled(signal.IsSoundEnabled);
        }
        
        public void StopLoop(SoundEffectKey key)
        {
            if (_loopSources.TryGetValue((int)key, out var src))
            {
                src.Stop();
                Destroy(src.gameObject);
                _loopSources.Remove((int)key);
            }
        }

        public void StopAllLoops()
        {
            foreach (var kvp in _loopSources)
            {
                if (kvp.Value)
                    Destroy(kvp.Value.gameObject);
            }
            
            _loopSources.Clear();
        }
        
        public void SetLoopPitch(SoundEffectKey key, float pitch)
        {
            if (_loopSources.TryGetValue((int)key, out var src))
            {
                src.pitch = pitch;
            }
        }
        
        public void SetMasterEnabled(bool enabled)  => masterMixer.SetFloat("MasterVolume", enabled ? 0f : -80f);
        public void SetSfxEnabled(bool enabled)     => masterMixer.SetFloat("SFXVolume",    enabled ? 0f : -80f);
        public void SetMusicEnabled(bool enabled)   => masterMixer.SetFloat("MusicVolume",  enabled ? 0f : -80f);

        public static float LinearToDb(float linear) => Mathf.Approximately(linear, 0f) ? -80f : 20f * Mathf.Log10(linear);

        private void OnDestroy()
        {
            _signalBus?.TryUnsubscribe<OnSoundSettingChangedSignal>(OnSoundSettingChanged);
        }
    }
}