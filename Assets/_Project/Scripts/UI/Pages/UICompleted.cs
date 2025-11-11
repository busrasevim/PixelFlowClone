using System;
using DG.Tweening;
using UnityEngine;
using Zenject;

namespace _Project.Scripts.UI.Pages
{
    public class UICompleted : UIPage
    {
        [SerializeField] private ParticleSystem[] confettis;
        [SerializeField] private GameObject goPage;
        [SerializeField] private GameObject goButton;

        private int _earnedCoin;
        private Action _onCompleted;

        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        protected override void Awake()
        {
            goPage.SetActive(false);
            base.Awake();
        }

        protected override void Start()
        {
            foreach (var confetti in confettis)
                confetti.Play();

            Taptic.Success();

            DOVirtual.DelayedCall(1f, () =>
            {
                goPage.SetActive(true);
                base.Start();
            });
        }

        public void Build(int earnedCoin, Action onCompleted = null)
        {
            _onCompleted = onCompleted;
            _earnedCoin = earnedCoin;
        }

        public void OnTapContinue()
        {
            goButton.SetActive(false);

            _onCompleted?.Invoke();

            _signalBus.Fire<OnLevelSetupRequestedSignal>();
        }
    }
}