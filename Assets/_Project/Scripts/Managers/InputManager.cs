using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.Managers
{
    public class InputManager : IInitializable, ITickable
    {
        private List<IInputListener> _listeners;
    
        private Vector3 _pivotPosition;
        private float _dragUnit;
        private float dragUnitMultiplier = 1f / 12f;
        private bool _isPointerDown;

        public void Initialize()
        {
            _listeners = new List<IInputListener>();
            _dragUnit = Screen.width * dragUnitMultiplier;
        }
    
        public void Tick()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _pivotPosition = Input.mousePosition;
                _isPointerDown  = true;
                NotifyPressed();
            }else if (Input.GetMouseButton(0) && _isPointerDown)
            {
                var delta = Input.mousePosition - _pivotPosition;
                var modifiedDelta = delta / _dragUnit;
                NotifyDragged(modifiedDelta);
            }else if (Input.GetMouseButtonUp(0) && _isPointerDown)
            {
                var delta = Input.mousePosition - _pivotPosition;
                var modifiedDelta = delta / _dragUnit;
                NotifyReleased(modifiedDelta);
                _isPointerDown = false;
            }
        }

        private void NotifyPressed()
        {
            for (var i = 0; i < _listeners.Count; i++)
            {
                var listener = _listeners[i];
                if (listener == null) return;

                listener.OnPressed();
            }
        }

        private void NotifyDragged(Vector2 dragVector)
        {
            foreach (var listener in _listeners)
            {
                listener.OnDrag(dragVector);
            }
        }

        private void NotifyReleased(Vector2 dragVector)
        {
            foreach (var listener in _listeners)
            {
                listener.OnReleased(dragVector);
            }
        }

        public Vector3 GetPivotPosition()
        {
            return _pivotPosition;
        }
        
        public void ResetPivotPosition()
        {
            _pivotPosition = Input.mousePosition;
        }
    
        public void AddListener(IInputListener listener)
        {
            if(_listeners.Contains(listener)) return;
        
            _listeners.Add(listener);
        }

        public void RemoveListener(IInputListener listener)
        {
            if (_listeners.Contains(listener))
                _listeners.Remove(listener);
        }
    }

    public interface IInputListener
    {
        void OnPressed();
        void OnDrag(Vector2 dragVector);
        void OnReleased(Vector2 dragVector);
    }
}