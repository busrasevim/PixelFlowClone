using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using _Project.Scripts.UI;
using Zenject;

[RequireComponent(typeof(CanvasScaler))]
public class UIPage : MonoBehaviour
{
    protected CanvasScaler _canvasScaler;
    protected RectTransform _rectTransform;
    protected UIManager _uiManager;

    [HideInInspector] public Action OnClose;

    [Inject]
    public void Construct(UIManager uiManager)
    {
        _uiManager = uiManager;
    }

    protected virtual void Awake()
    {
        _canvasScaler = GetComponent<CanvasScaler>();
        _rectTransform = transform.Find("Page")?.GetComponent<RectTransform>();

        if (_canvasScaler == null || _rectTransform == null)
            Debug.LogWarning($"[{name}] UIPage: Missing CanvasScaler or Page RectTransform reference.");

        var refSize = _canvasScaler.referenceResolution;
        _rectTransform.DOLocalMoveX(-refSize.x, 0f);

        float aspect = (float)Screen.height / Screen.width;

        if (aspect < 1.6f)
            _canvasScaler.matchWidthOrHeight = 1f;
    }

    protected virtual void Start()
    {
        _rectTransform.DOLocalMoveX(0f, 0.2f).SetEase(Ease.OutFlash);
    }

    public void OnTapClose()
    {
        Close();
    }

    public virtual void Close()
    {
        var refSize = _canvasScaler.referenceResolution;

        _rectTransform.DOLocalMoveX(-refSize.x, 0.2f).SetEase(Ease.OutFlash).OnComplete(() =>
        {
            OnClose?.Invoke();
            Destroy(gameObject);
        });
    }
}