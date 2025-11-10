using _Project.Scripts.Level;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public class Shooter : MonoBehaviour, INodeObject
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private TMP_Text shootCountText;

        public int ShootCount { get; set; }
        public void Initialize(Node node)
        {
        
        }

        public void Init(CellData data)
        {
            foreach (var renderer in renderers)
            {
                renderer.material.color = data.cellColor;
            }

            ShootCount = data.shootCount;
            SetShootCountText();

            transform.eulerAngles = Vector3.up * 180f;
        }

        private void SetShootCountText()
        {
            shootCountText.text = ShootCount.ToString();
        }
    }
}
