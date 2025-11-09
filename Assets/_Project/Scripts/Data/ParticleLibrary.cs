using UnityEngine;

namespace _Project.Scripts.Data
{
    [CreateAssetMenu(menuName = "Libraries/Particle Library", fileName = "New Particle Library")]
    public class ParticleLibrary : ScriptableObject
    {
        public ParticleSystem levelCompletedPS;
    }
}
