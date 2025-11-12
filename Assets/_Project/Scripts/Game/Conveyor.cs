using System;
using System.Collections.Generic;
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
            _conveyorLimit  = shooterLimit;
        }

        public int GetCurrentShooterCount()
        {
            return _shootersOnConveyor.Count;
        }

        private void SetShooterCountText()
        {
            shooterCountText.text = _shootersOnConveyor.Count.ToString() + "/" + _conveyorLimit;
        }
    }
}