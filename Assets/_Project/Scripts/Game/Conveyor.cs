using System;
using System.Collections.Generic;
using _Project.Scripts.Pools;
using DG.Tweening;
using Dreamteck.Splines;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public class Conveyor : MonoBehaviour
    {
        [SerializeField] private SplineComputer _spline;
        public SplineComputer SplineComputer => _spline;
        [SerializeField] private TMP_Text shooterCountText;

        private int _conveyorLimit;
        private List<Shooter> _shootersOnConveyor = new List<Shooter>();
        private ConveyorArrow[] _arrows;

        private void Start()
        {
            SetShooterCountText();
        }

        public bool CanGetNewShooter()
        {
            return _shootersOnConveyor.Count < _conveyorLimit;
        }

        public void AddShooter(Shooter shooter)
        {
            if (_shootersOnConveyor.Contains(shooter)) return;

            _shootersOnConveyor.Add(shooter);
            SetShooterCountText();
        }

        public void RemoveShooter(Shooter shooter)
        {
            _shootersOnConveyor.Remove(shooter);
            SetShooterCountText();
        }

        public void SetShooterLimit(int shooterLimit)
        {
            _conveyorLimit = shooterLimit;
        }

        public int GetCurrentShooterCount()
        {
            return _shootersOnConveyor.Count;
        }

        private void SetShooterCountText()
        {
            shooterCountText.text = (_conveyorLimit - _shootersOnConveyor.Count).ToString() + "/" + _conveyorLimit;
        }

        public void PlayConveyorIsFullEffect()
        {
            shooterCountText.DOComplete();
            shooterCountText.transform.DOComplete();
            shooterCountText.DOColor(Color.red, 0.3f);
            shooterCountText.transform.DOShakePosition(
                duration: 0.35f,
                strength: new Vector3(0.05f, 0.05f, 0f),
                vibrato: 12,
                randomness: 60f,
                fadeOut: true
            ).OnComplete(() => { shooterCountText.DOColor(Color.white, 0.2f); });
        }

        public void LevelFailed()
        {
            foreach (var shooter in _shootersOnConveyor)
            {
                shooter.Stop();
            }

            foreach (var arrow in _arrows)
            {
                arrow.Stop();
            }
        }

        public void SetArrows(ObjectPool pool, int arrowCount, float arrowSpeed, Level.Level level)
        {
            _arrows = new ConveyorArrow[arrowCount];

            if (_spline == null || arrowCount <= 0) return;

            // spline'ın gerçek uzunluğunu al
            double splineLength = _spline.CalculateLength();

            // oklar arası world-space mesafesi
            double stepDistance = splineLength / (arrowCount - 1);

            double distance = 0;

            for (int i = 0; i < arrowCount; i++)
            {
                var arrowObj = pool.SpawnFromPool(PoolTags.ConveyorArrow, transform.position, Quaternion.identity);
                var arrow = arrowObj.GetComponent<ConveyorArrow>();

                arrow.transform.SetParent(transform, worldPositionStays: true);

                // Bu distance değerinin spline üzerinde normalized karşılığını bul
                double percent = _spline.Travel(0.0, (float)distance, Spline.Direction.Forward);

                // Oka spline ve yüzdesini ver
                arrow.SetSpline(_spline, percent, arrowSpeed);

                // sonraki oka geç
                distance += stepDistance;

                _arrows[i] = arrow;

                level.SpawnedNewArrows(arrow);
            }
        }
    }
}