using System;
using System.Collections.Generic;
using _Project.Scripts.UI.Pages;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI
{
    public class UIManager : MonoBehaviour
    {
        [Header("Pages")] [SerializeField] private GameObject preHome;
        [SerializeField] private GameObject preGame;
        [SerializeField] private GameObject preCompleted;
        [SerializeField] private GameObject preFail;

        [Header("Popups")] [SerializeField] private GameObject preSetting;

        [Inject] private DiContainer _container;

        private Transform _root;
        private readonly HashSet<GameObject> _spawned = new HashSet<GameObject>();

        private void Awake()
        {
            _root = transform;
        }

        private GameObject Show(GameObject prefab, bool isPage)
        {
            if (prefab == null) return null;

            if (isPage) HideAllPages();

            var go = Instantiate(prefab, _root);
            _container.InjectGameObject(go);
            _spawned.Add(go);
            return go;
        }

        private void HideAllPages()
        {
            foreach (var go in _spawned)
            {
                if (go != null) Destroy(go);
            }

            _spawned.Clear();
        }

        public void Hide(GameObject instance)
        {
            if (instance == null) return;
            if (_spawned.Remove(instance)) Destroy(instance);
        }

        public void ShowHome()
        {
            Show(preHome, isPage: true);
        }

        public UIGame ShowGame()
        {
            var go = Show(preGame, isPage: true);
            return go != null && TryGetComponent<UIGame>(out var ui) ? ui : null;
        }

        public void ShowCompleted(int earnedCoin, Action onCompleted = null)
        {
            var go = Show(preCompleted, isPage: true);
            if (go != null && TryGetComponent<UICompleted>(out var completed))
                completed.Build(earnedCoin, onCompleted);
        }

        public void ShowFail()
        {
            Show(preFail, isPage: true);
        }

        public void ShowPopupSetting(bool isPaused)
        {
            Show(preSetting, isPage: false);
        }
    }
}