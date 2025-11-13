using System.Collections;
using System.Collections.Generic;
using _Project.Scripts.Game.Constants;
using _Project.Scripts.Signals;
using UnityEngine;
using Zenject;

public class SettingsManager : ISettingsProvider, IInitializable
{
    private bool _soundEnabled;
    private bool _vibrationEnabled;

    public bool IsSoundEnabled => _soundEnabled;
    public bool IsVibrationEnabled => _vibrationEnabled;

    private readonly SignalBus _signalBus;

    [Inject]
    public SettingsManager(SignalBus signalBus)
    {
        _signalBus = signalBus;
    }

    public void Initialize()
    {
        _soundEnabled = PlayerPrefs.GetInt(Constants.SoundKey, 1) == 1;
        _vibrationEnabled = PlayerPrefs.GetInt(Constants.HapticKey, 1) == 1;

        Taptic.tapticOn = _vibrationEnabled;
    }

    public void SetSound(bool value)
    {
        _soundEnabled = value;
        PlayerPrefs.SetInt(Constants.SoundKey, value ? 1 : 0);
        PlayerPrefs.Save();

        _signalBus.Fire(new OnSoundSettingChangedSignal { IsSoundEnabled = value });
    }

    public void SetVibration(bool value)
    {
        _vibrationEnabled = value;
        Taptic.tapticOn = value;
        PlayerPrefs.SetInt(Constants.HapticKey, value ? 1 : 0);
        PlayerPrefs.Save();
    }
}