using _Project.Scripts.Game;
using _Project.Scripts.Level;
using _Project.Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class ShooterManager
{
    private LevelManager _levelManager;
    private GameManager  _gameManager;

    [Inject]
    public void Construct(LevelManager levelManager, GameManager gameManager)
    {
        _levelManager = levelManager;
        _gameManager = gameManager;
    }
    
    public void ShooterSelected(Shooter shooter)
    {
        shooter.Selected(_levelManager.CurrentLevel.Conveyor.SplineComputer, this);
    }

    public void SetReservedSlot(Shooter shooter)
    {
        var reservedSlot = _levelManager.CurrentLevel.GetAvailableReservedSlot();
        if (reservedSlot == null)
        {
            _gameManager.EndLevel(false);
            return;
        }

        shooter.SetReservedSlot(reservedSlot);
        reservedSlot.AssignNodeObject(shooter);
    }
}
