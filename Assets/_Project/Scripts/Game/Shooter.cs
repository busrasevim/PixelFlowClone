using System;
using System.Threading;
using _Project.Scripts.Level;
using Cysharp.Threading.Tasks;
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
        [SerializeField] private GameObject model;

        private ShooterNode _currentShooterNode;
        private ReservedSlot _reservedSlot;
        
        private LayerMask _layerColorCube;
        private int colorID;

        private bool _onConveyor;

        public int ShootCount { get; set; }

        public bool IsSelectable => _currentShooterNode.IsFrontNode();
        
        private CancellationTokenSource _shootCts;

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

            _layerColorCube = LayerMask.GetMask("ColorCube");

            colorID = data.colorID;
        }

        private void SetShootCountText()
        {
            shootCountText.text = ShootCount.ToString();
        }

        public void Selected(SplineComputer conveyorSpline, ShooterManager shooterManager)
        {
            var position = conveyorSpline.GetPointPosition(0);
            transform.DOJump(position, 1f, 1, 0.5f).OnComplete(() =>
            {
                splineFollower.spline = conveyorSpline;
                splineFollower.SetPercent(0f);
                splineFollower.follow = true;
                splineFollower.enabled = true;
                model.transform.localEulerAngles = Vector3.up * -90f;
                StartShootControl(shooterManager);
            });
        }

        private async UniTask StartShootControl(ShooterManager shooterManager)
        {
            _shootCts?.Cancel();
            _shootCts = new CancellationTokenSource();

            _onConveyor = true;
            try
            {
                while (_onConveyor && !_shootCts.IsCancellationRequested)
                {
                    var pos = transform.position;
                    pos.y = 0.282f;
                    var ray = new Ray(pos, -transform.right);

                    if (Physics.Raycast(ray, out var hit, 10f, _layerColorCube))
                    {
                        var colorCube = hit.collider.GetComponentInParent<ColorCube>();
                        if (colorCube != null && colorCube.colorID == colorID)
                        {
                            colorCube.Blast();
                            ShootCount--;
                            SetShootCountText();
                            if (ShootCount == 0)
                            {
                                _onConveyor = false;
                                gameObject.SetActive(false);
                            }
                        }
                    }

                    if (splineFollower.GetPercent() >= 1f)
                    {
                        _onConveyor = false;
                        splineFollower.follow = false;
                        splineFollower.enabled = false;
                        shooterManager.SetReservedSlot(this);
                    }

                    await UniTask.Yield(PlayerLoopTiming.Update, _shootCts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // normal iptal senaryosu
            }
            finally
            {
                _onConveyor = false;
                _shootCts.Dispose();
                _shootCts = null;
            }
        }

        public void SetReservedSlot(ReservedSlot reservedSlot)
        {
            transform.DOJump(reservedSlot.transform.position, 1f, 1, 0.5f);
            transform.DORotate(reservedSlot.transform.rotation.eulerAngles, 0.5f);
            model.transform.DOLocalRotate(Vector3.zero, 0.5f);
            _reservedSlot  = reservedSlot;
        }
    }
}