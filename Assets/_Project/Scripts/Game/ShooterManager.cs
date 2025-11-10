using _Project.Scripts.Game;
using _Project.Scripts.Level;
using DG.Tweening;
using UnityEngine;
using Zenject;

public class ShooterManager
{
    private LevelManager _levelManager;

    [Inject]
    public void Construct(LevelManager levelManager)
    {
        _levelManager = levelManager;
    }
    
    public void ShooterSelected(Shooter shooter)
    {
        Debug.Log(_levelManager);
        Debug.Log(_levelManager.CurrentLevel);
        Debug.Log(_levelManager.CurrentLevel.Conveyor);
        Debug.Log(_levelManager.CurrentLevel.Conveyor.SplineComputer);
        shooter.Selected(_levelManager.CurrentLevel.Conveyor.SplineComputer);
    }
}
