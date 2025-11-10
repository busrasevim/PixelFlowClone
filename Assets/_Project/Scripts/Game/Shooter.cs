using _Project.Scripts.Level;
using DG.Tweening;
using Dreamteck.Splines;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public class Shooter : MonoBehaviour, INodeObject
    {
        [SerializeField] private Renderer[] renderers;
        [SerializeField] private TMP_Text shootCountText;
        [SerializeField] private SplineFollower splineFollower;
        
        private ShooterNode _currentShooterNode;

        public int ShootCount { get; set; }

        public bool IsSelectable => _currentShooterNode.IsFrontNode();

        public void Initialize(Node node)
        {
            _currentShooterNode = node as ShooterNode;
        }

        public void Init(CellData data, float followerSpeed)
        {
            foreach (var renderer in renderers)
            {
                renderer.material.color = data.cellColor.linear;
            }

            ShootCount = data.shootCount;
            SetShootCountText();
            
            splineFollower.followSpeed = followerSpeed;
        }

        private void SetShootCountText()
        {
            shootCountText.text = ShootCount.ToString();
        }

        public void Selected(SplineComputer conveyorSpline)
        {
            var position = conveyorSpline.GetPointPosition(0);
            transform.DOJump(position, 1f, 1, 0.5f).OnComplete(() =>
            {
                splineFollower.spline = conveyorSpline;
                splineFollower.enabled = true;
            });
        }
    }
}