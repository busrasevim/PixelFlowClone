using UnityEngine;
using System;
using _Project.Scripts.Data;
using _Project.Scripts.Level;
using _Project.Scripts.UI;
using Zenject;

public class UIFail : UIPage
{
    private SignalBus _signalBus;
    private LevelManager _levelManager;

    [Inject]
    public void Construct(SignalBus signalBus, LevelManager levelManager)
    {
        _signalBus  = signalBus;
        _levelManager = levelManager;
    }

    public void OnTapPlayAgain()
    {
        _signalBus.Fire(new OnRestartSignal(hard: true));

        Close();
    }

    public void OnTapHome()
    {
        _signalBus.Fire<OnLevelSetupRequestedSignal>();
    }
}