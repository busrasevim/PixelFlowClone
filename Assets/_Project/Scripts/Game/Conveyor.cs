using System.Collections.Generic;
using Dreamteck.Splines;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public class Conveyor : MonoBehaviour
    {
        [SerializeField] private SplineComputer _spline;
        public SplineComputer SplineComputer => _spline;

        private int _conveyorLimit;
        private List<Shooter> _shootersOnConveyor = new List<Shooter>();

        public bool CanGetNewShooter()
        {
            return _shootersOnConveyor.Count < _conveyorLimit;
        }

        public void AddShooter(Shooter shooter)
        {
            if (_shootersOnConveyor.Contains(shooter)) return;

            _shootersOnConveyor.Add(shooter);
        }

        public void RemoveShooter(Shooter shooter)
        {
            _shootersOnConveyor.Remove(shooter);
        }

        public void SetShooterLimit(int shooterLimit)
        {
            _conveyorLimit  = shooterLimit;
        }

        public int GetCurrentShooterCount()
        {
            return _shootersOnConveyor.Count;
        }
    }
}