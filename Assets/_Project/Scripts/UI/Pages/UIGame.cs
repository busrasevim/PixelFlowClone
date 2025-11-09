using UnityEngine;
using _Project.Scripts.Level;
using TMPro;
using Zenject;

public class UIGame : UIPage
    {
        [SerializeField] private TextMeshProUGUI lblLevel;
        [SerializeField] private RectTransform rectLevel;
        
        private LevelManager _levelManager;

        [Inject]
        public void Construct(LevelManager levelManager)
        {
            _levelManager = levelManager;
        }
        

        protected override void Start()
        {
            lblLevel.text = $"Level {_levelManager.CurrentLevelNo + 1}";
            base.Start();
        }
    }
