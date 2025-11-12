using System.Collections.Generic;
using _Project.Scripts.Level;
using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public class ColorCube : MonoBehaviour, INodeObject, IPoolObject
    {
        [SerializeField] private Renderer cubeRenderer;
        [SerializeField] private Collider cubeCollider;
        public Transform getBulletTr;
        public int colorID;

        public ColorCubeNode CurrentNode { get; set; }

        private static MaterialPropertyBlock _mpb;

        public void Initialize(Node node)
        {
            CurrentNode = node as ColorCubeNode;
        }

        public void Init(Color pixelColor, List<LevelData.LevelColorData> levelColors, float threshold)
        {
            if (_mpb == null)
                _mpb = new MaterialPropertyBlock();

            cubeRenderer.GetPropertyBlock(_mpb);
            _mpb.SetColor("_Color", pixelColor.linear);
            cubeRenderer.SetPropertyBlock(_mpb);
            
            colorID = FindClosestColorID(pixelColor, levelColors, threshold);
        }

        private int FindClosestColorID(Color pixelColor, List<LevelData.LevelColorData> levelColors, float threshold)
        {
            float minDistance = float.MaxValue;
            int bestID = -1;

            foreach (var lc in levelColors)
            {
                float dr = pixelColor.r - lc.color.r;
                float dg = pixelColor.g - lc.color.g;
                float db = pixelColor.b - lc.color.b;
                float distance = Mathf.Sqrt(dr * dr + dg * dg + db * db);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestID = lc.id;
                }
            }

            return bestID;
        }

        public void Blast(ShooterManager shooterManager)
        {
            CurrentNode.SetEmpty(this);
            transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack)
                .OnComplete(() => { gameObject.SetActive(false); });
            
            shooterManager.ColorCubeBlasted();
        }

        public void Reserve()
        {
            cubeCollider.enabled = false;
        }

        public void Init()
        {
            
        }

        public void Reset()
        {
            transform.localScale = Vector3.one;
            cubeCollider.enabled = true;
            colorID = -1;
            CurrentNode = null;
        }
    }
}