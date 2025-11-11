using _Project.Scripts.Game;
using _Project.Scripts.Level;
using _Project.Scripts.Managers;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class ShooterManager
{
    private LevelManager _levelManager;
    private GameManager _gameManager;

    [Inject]
    public void Construct(LevelManager levelManager, GameManager gameManager)
    {
        _levelManager = levelManager;
        _gameManager = gameManager;
    }

    public void ShooterSelected(Shooter shooter)
    {
        var conveyor = _levelManager.CurrentLevel.Conveyor;
        if (!conveyor.CanGetNewShooter()) return;

        conveyor.AddShooter(shooter);

        if (shooter.CurrentShooterNode != null)
        {
            shooter.CurrentShooterNode.SetEmpty(shooter);
            _levelManager.CurrentLevel.ShooterGridSystem.TransferShooters(shooter.CurrentShooterNode.GridPosition.x);
        }
        
        shooter.Selected(conveyor.SplineComputer, this);
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

    public void RemoveShooterFromConveyor(Shooter shooter)
    {
        _levelManager.CurrentLevel.Conveyor.RemoveShooter(shooter);
    }

    public void ColorCubeBlasted()
    {
        if (_levelManager.CurrentLevel.colorCubeGridSystem.IsPictureComplete())
        {
            _gameManager.EndLevel(true);
        }
    }
}