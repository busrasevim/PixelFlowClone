using Dreamteck.Splines;
using UnityEngine;

namespace _Project.Scripts.Game
{
    public class Conveyor : MonoBehaviour
    {
        [SerializeField] private SplineComputer _spline;
        public SplineComputer SplineComputer => _spline;
    }
}
