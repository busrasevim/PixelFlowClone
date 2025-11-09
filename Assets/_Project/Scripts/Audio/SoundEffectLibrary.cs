using UnityEngine;

namespace _Project.Scripts.Audio
{
    [CreateAssetMenu(menuName = "Libraries/Sound Effect Library",fileName = "New Sound Effect Library")]
    public class SoundEffectLibrary : ScriptableObject
    {
        public SoundEffect[] soundEffects;
    }
}