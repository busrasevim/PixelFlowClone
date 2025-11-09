using DG.Tweening;
using UnityEngine;

namespace _Project.Scripts.Utils
{
    public static class CanvasHelper
    {
        public static void Hide(this CanvasGroup canvasGroup)
        {
            canvasGroup.gameObject.SetActive(false);
            canvasGroup.SetHidden(true);
        }

        public static void Show(this CanvasGroup canvasGroup)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.SetHidden(false);
        }

        public static void SetHidden(this CanvasGroup canvasGroup, bool isHidden)
        {
            canvasGroup.alpha = isHidden ? 0f : 1f;
            canvasGroup.interactable = !isHidden;
            canvasGroup.blocksRaycasts = !isHidden;
        }

        public static void Hide(this CanvasGroup canvasGroup, float duration, float delay = 0f)
        {
            canvasGroup.DOFade(0f, duration).SetDelay(delay).SetUpdate(true).OnComplete(() =>
            {
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                canvasGroup.gameObject.SetActive(false);
            });
        }

        public static void Show(this CanvasGroup canvasGroup, float duration, float delay = 0f)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.DOFade(1f, duration).SetDelay(delay).SetUpdate(true).OnComplete(() =>
            {
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;
            });
        }
    }
}