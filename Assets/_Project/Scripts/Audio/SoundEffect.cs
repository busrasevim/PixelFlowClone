using UnityEngine;

namespace _Project.Scripts.Audio
{
    [CreateAssetMenu(menuName = "Items/Sound Effect", fileName = "New Sound Effect")]
    public class SoundEffect : ScriptableObject
    {
        public SoundEffectKey soundEffectKey;
        public SoundCategory Category = SoundCategory.SFX; // 🔴 yeni
        public AudioClip clip;

        [Header("Playback Settings")] public bool loop;
        [Range(0, 1f)] public float volume = 1f;
        [Range(-3f, 3f)] public float pitch = 1f;
        [Range(0f, 1f)] public float pitchRandomness = 0f;


        public void Play(AudioSource source, float? customPitch = null)
        {
            if (clip == null) return;

            source.clip = clip;
            source.volume = volume;
            source.loop = loop;
            float finalPitch = customPitch ?? (pitch + Random.Range(-pitchRandomness, pitchRandomness));
            source.pitch = finalPitch;
            source.Play();
        }
    }
}