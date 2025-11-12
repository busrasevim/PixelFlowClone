using _Project.Scripts.Data;
using _Project.Scripts.Game;
using _Project.Scripts.Level;
using _Project.Scripts.Managers;
using _Project.Scripts.Pools;
using UnityEngine;
using Zenject;

public class ShooterManager
{
    private LevelManager _levelManager;
    private GameManager _gameManager;
    private GameSettings _gameSettings;
    private ObjectPool _objectPool;

    public bool OnLastShooterEffect { get; set; }

    [Inject]
    public void Construct(LevelManager levelManager, GameSettings gameSettings, ObjectPool objectPool)
    {
        _levelManager = levelManager;
        _gameSettings = gameSettings;
        _objectPool = objectPool;
    }

    public void ShooterSelected(Shooter shooter)
    {
        var conveyor = _levelManager.CurrentLevel.Conveyor;
        if (!conveyor.CanGetNewShooter())
        {
            conveyor.PlayConveyorIsFullEffect();
            return;
        }

        conveyor.AddShooter(shooter);

        if (shooter.CurrentShooterNode != null)
        {
            shooter.CurrentShooterNode.SetEmpty(shooter);
            _levelManager.CurrentLevel.ShooterGridSystem.TransferShooters(shooter.CurrentShooterNode.GridPosition.x);
        }

        shooter.Selected(conveyor.SplineComputer, this, _objectPool, _gameSettings.bulletSpeed,
            _gameSettings.bulletFireEase);

        _levelManager.CurrentLevel.ReservedSlotGridSystem.SetWarningEffect();
        _levelManager.CurrentLevel.ReservedSlotGridSystem.TransferShooters();
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
        _levelManager.CurrentLevel.ReservedSlotGridSystem.SetWarningEffect();
    }

    //hızlan
    //bitince konumu sıfırla devam etsin aynen

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

    public void ControlLastShooters()
    {
        if (OnLastShooterEffect) return;

        var count = _levelManager.CurrentLevel.ShooterGridSystem.GetCurrentShooterCount();
        count += _levelManager.CurrentLevel.ReservedSlotGridSystem.GetCurrentShooterCount();
        count += _levelManager.CurrentLevel.Conveyor.GetCurrentShooterCount();

        if (count <= _gameSettings.conveyorShooterLimit)
        {
            OnLastShooterEffect = true;
            var shooters = Object.FindObjectsByType<Shooter>(FindObjectsSortMode.None);
            foreach (var shooter in shooters)
            {
                shooter.SetSpeed(_gameSettings.lastShooterEffectFastSpeed);
            }
        }
    }

    public void ResetSystem()
    {
        OnLastShooterEffect = false;
    }

    public void SetGameManager(GameManager gameManager)
    {
        _gameManager = gameManager;
    }
}