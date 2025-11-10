using System;
using _Project.Scripts.Game;
using _Project.Scripts.Level.Signals;
using _Project.Scripts.Managers;
using UnityEngine;
using Zenject;

public class GameInputController : IInitializable, IDisposable, IInputListener
{
    private InputManager _inputManager;
    private ShooterManager _shooterManager;
    private SignalBus _signalBus;

    private Shooter _selectedShooter;
    private Camera _mainCamera;
    private LayerMask _layerShooter;

    [Inject]
    public void Construct(InputManager inputManager, ShooterManager shooterManager, SignalBus signalBus)
    {
        _inputManager = inputManager;
        _shooterManager = shooterManager;
        _signalBus = signalBus;
    }

    public void OnPressed()
    {
        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerShooter))
        {
            var shooter = hit.collider.gameObject.GetComponentInParent<Shooter>();
            if (shooter == null) return;
            if (!shooter.IsSelectable) return;

            _selectedShooter = shooter;
        }
    }

    public void OnDrag(Vector2 dragVector)
    {
    }

    public void OnReleased(Vector2 dragVector)
    {
        if (_selectedShooter == null) return;

        var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Mathf.Infinity, _layerShooter))
        {
            var shooter = hit.collider.gameObject.GetComponentInParent<Shooter>();
            if (shooter != null || shooter == _selectedShooter)
            {
                _shooterManager.ShooterSelected(shooter);
            }
        }

        _selectedShooter = null;
    }

    public void Initialize()
    {
        _signalBus.Subscribe<OnLevelStartSignal>(OnLevelStart);
        _signalBus.Subscribe<OnLevelEndSignal>(OnLevelEnd);
        _mainCamera = Camera.main;
        _layerShooter = LayerMask.GetMask("Shooter");
    }

    private void OnLevelEnd()
    {
        _inputManager.RemoveListener(this);
    }

    private void OnLevelStart()
    {
        _inputManager.AddListener(this);
    }

    public void Dispose()
    {
        _inputManager.RemoveListener(this);
        _signalBus.Unsubscribe<OnLevelStartSignal>(OnLevelStart);
        _signalBus.Unsubscribe<OnLevelEndSignal>(OnLevelEnd);
    }
}