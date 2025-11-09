using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UISetting : UIPopup
{
    [SerializeField] private Image imgSoundBtn;
    [SerializeField] private Image imgHapticBtn;
    [SerializeField] private Sprite[] sprButton;

    [Inject] private ISettingsProvider _settingsProvider;

    protected override void Start()
    {
        base.Start();
        UpdateButtonSprites();
    }

    public void OnToggleSoundBtn()
    {
        bool newValue = !_settingsProvider.IsSoundEnabled;
        _settingsProvider.SetSound(newValue);
        UpdateButtonSprites();
    }

    public void OnToggleHapticBtn()
    {
        bool newValue = !_settingsProvider.IsVibrationEnabled;
        _settingsProvider.SetVibration(newValue);
        UpdateButtonSprites();
    }

    public override void OnTapClose()
    {
        _uiManager.ShowHome();
        Close();
    }

    public void OnTapHomeBtn()
    {
        _uiManager.ShowHome();
        Close();
    }

    private void UpdateButtonSprites()
    {
        imgSoundBtn.sprite = sprButton[_settingsProvider.IsSoundEnabled ? 1 : 0];
        imgHapticBtn.sprite = sprButton[_settingsProvider.IsVibrationEnabled ? 1 : 0];
    }
}