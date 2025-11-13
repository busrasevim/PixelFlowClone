using UnityEngine;
using _Project.Scripts.Level;
using _Project.Scripts.Managers;
using TMPro;
using Zenject;

public class UIGame : UIPage
    {
        [SerializeField] private TextMeshProUGUI lblLevel;
        [SerializeField] private RectTransform rectLevel;
        
        private LevelManager _levelManager;
        private GameManager  _gameManager;

        [Inject]
        public void Construct(LevelManager levelManager,  GameManager gameManager)
        {
            _levelManager = levelManager;
            _gameManager = gameManager;
        }
        

        protected override void Start()
        {
            lblLevel.text = $"Level {_levelManager.CurrentLevelNo + 1}";
            base.Start();
        }

        public void OnTapToRestart()
        {
            _gameManager.RestartLevel(true);
        }
    }
