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
        [SerializeField] private Transform plateStartPosition;

        private int _conveyorLimit;
        private List<Shooter> _shootersOnConveyor = new List<Shooter>();
        private ConveyorArrow[] _arrows;
        private List<GameObject> _shooterPlates;

        private void Start()
        {
            SetShooterCountText();
        }

        public void Init(ObjectPool pool, int arrowCount, float arrowSpeed, Level.Level level, int shooterLimit)
        {
            this._conveyorLimit = shooterLimit;
            SetArrows(pool, arrowCount, arrowSpeed, level);
            SetPlates(pool, shooterLimit, level);
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

        public void RemoveShooter(Shooter shooter, GameObject plate)
        {
            _shootersOnConveyor.Remove(shooter);
            SetShooterCountText();

            PutPlate(plate);
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

        public void SetPlates(ObjectPool pool, int plateCount, Level.Level level)
        {
            _shooterPlates = new List<GameObject>();
            for (int i = 0; i < plateCount; i++)
            {
                var position = plateStartPosition.position - Vector3.right * (0.1f * i);
                var plate = pool.SpawnFromPool(PoolTags.ShooterPlate, position, plateStartPosition.rotation);
                _shooterPlates.Add(plate);

                level.SpawnedPlate(plate);
            }
        }

        public GameObject GetPlate()
        {
            var plate = _shooterPlates[0];
            _shooterPlates.RemoveAt(0);
            plate.transform.DOKill();
            FixNextPlatesPosition();
            return plate;
        }

        private void PutPlate(GameObject plate)
        {
            plate.transform.SetParent(transform);
            plate.transform.DOMove(plateStartPosition.position - Vector3.right * 0.1f * _shooterPlates.Count, 0.3f);
            plate.transform.DORotate(plateStartPosition.eulerAngles, 0.3f);
            _shooterPlates.Add(plate);
        }

        private void FixNextPlatesPosition()
        {
            for (int i = 0; i < _shooterPlates.Count; i++)
            {
                _shooterPlates[i].transform.DOMove(plateStartPosition.position - Vector3.right * 0.1f * i, 0.1f);
            }
        }
    }
}