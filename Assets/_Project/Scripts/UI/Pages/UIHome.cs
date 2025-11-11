using _Project.Scripts.Level;
using _Project.Scripts.UI;
using _Project.Scripts.UI.Items;
using UnityEngine;
using DG.Tweening;
using Zenject;

public class UIHome : UIPage
    {
        [SerializeField] private UILevelCursor[] levelCursors;
        [SerializeField] private RectTransform rtLevel;
        [SerializeField] private Transform trnPlay;

        private LevelManager _levelManager;
        private SignalBus _signalBus;

        [Inject]
        public void Construct(SignalBus signalBus, LevelManager levelManager, UIManager uiManager)
        {
            _signalBus = signalBus;
            _levelManager = levelManager;
            _uiManager = uiManager;
        }
        
        protected override void Start()
        {
            base.Start();

            int currentLevel = _levelManager.CurrentLevelNo + 1;
            
            for( int i = 0; i < levelCursors.Length; i ++){
                levelCursors[ i ].Init( currentLevel + i, currentLevel);
            }

            DOVirtual.DelayedCall(1f, ()=>{
                if( !DOTween.IsTweening(trnPlay) ){
                    trnPlay.DOPunchScale(Vector3.one * 0.04f, 1.5f, 2)
                        .SetEase(Ease.OutFlash)
                        .SetLoops(-1,LoopType.Restart)
                        .SetDelay(0.3f);
                }     
            });
        }

        public void Animate()
        {
            int level = _levelManager.CurrentLevelNo;

            for( int i = 0; i < levelCursors.Length; i ++){
                levelCursors[ i ].Init( level + i, level );
            }

            rtLevel.DOAnchorPosY(-370f, 0.6f).SetEase(Ease.OutFlash).SetDelay(0.5f).OnComplete(()=>{
                levelCursors[1].transform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 2).OnComplete(()=>{
                    trnPlay.DOPunchScale(Vector3.one * 0.04f, 1.5f, 2)
                        .SetEase(Ease.OutFlash)
                        .SetLoops(-1,LoopType.Restart)
                        .SetDelay(0.3f);
                });
                levelCursors[1].Init( level + 1, level + 1 );
                Taptic.Light();
            });
            
            levelCursors[0].transform.DOScale(Vector3.one * 0.1f, 0.9f).SetDelay(0.5f).OnComplete(()=>{
                levelCursors[0].gameObject.SetActive(false);
            });
        }

        public void OnTapPlay()
        {
            _signalBus.Fire<OnPlaySignal>();
        }

        public void OnSetting()
        {
            _uiManager.ShowPopupSetting(false);
        }
    }
