using UnityEngine;

namespace _Project.Scripts.Game
{
    public class ColorCube : MonoBehaviour, INodeObject
    {
        [SerializeField] private Renderer cubeRenderer;
        public void Initialize(Node node)
        {
        
        }

        public void Init(Color pixelColor)
        {
            cubeRenderer.material.color = pixelColor.linear;
        }
    }
}
