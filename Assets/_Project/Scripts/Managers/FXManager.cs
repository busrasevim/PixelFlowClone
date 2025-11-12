using _Project.Scripts.Audio;
using _Project.Scripts.Data;
using _Project.Scripts.Game.Constants;
using _Project.Scripts.Level.Signals;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Managers
{
    public class FXManager : IInitializable
    {
        [Inject] private ParticleLibrary _particleLibrary;
        [Inject] private SignalBus _signalBus;
        [Inject] private AudioManager _audioManager;
        private Transform _psPoolParent;

        public void Initialize()
        {
            _psPoolParent = new GameObject(Constants.ParticleParentGameObjectName).transform;
        }

        public void PlayShooterSelectedFX()
        {
            Taptic.Selection();
            _audioManager.PlaySoundEffect(SoundEffectKey.ShooterSelected);
        }

        public void PlayBulletFireFX()
        {
            Taptic.Medium();
            _audioManager.PlaySoundEffect(SoundEffectKey.BulletFired);
        }

        public void PlayLevelCompletedFX()
        {
            Taptic.Success();
            _audioManager.PlaySoundEffect(SoundEffectKey.LevelCompleted);
        }

        public void PlayLevelFailedFX()
        {
            Taptic.Failure();
            _audioManager.PlaySoundEffect(SoundEffectKey.LevelFailed);
        }
    }

    public class ParticleSystemPool
    {
        private readonly int _poolSize;
        private readonly ParticleSystem[] _particleSystems;

        private int _index;

        public ParticleSystemPool(int poolSize, ParticleSystem particleSystemPrefab, Transform parent)
        {
            _index = 0;
            _poolSize = poolSize;
            _particleSystems = new ParticleSystem[poolSize];

            for (int i = 0; i < _poolSize; i++)
            {
                _particleSystems[i] = Object.Instantiate(particleSystemPrefab, parent);
                _particleSystems[i].Stop();
            }
        }

        public ParticleSystem Play(Vector3 position)
        {
            var ps = _particleSystems[_index];
            ps.transform.position = position;
            ps.Play();

            _index = (_index + 1) % _poolSize;

            return ps;
        }
    }
}