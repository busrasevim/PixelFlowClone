using System;
using System.Collections.Generic;
using System.Linq;
using _Project.Scripts.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Project.Scripts.Level
{
    [CreateAssetMenu(menuName = "Levels/Level Data", fileName = "Level_")]
    public class LevelData : SerializedScriptableObject
    {
        [Serializable]
        public class LevelColorData
        {
            public int id;
            public Color color;
            public int size;

            public LevelColorData(int id, Color color, int size)
            {
                this.id = id;
                this.color = color;
                this.size = size;
            }
        }
        
        [TableList(AlwaysExpanded = true)]
        public List<LevelColorData> levelColors = new();
        [Range(0.01f, 0.5f)]
        [Tooltip("Renkler arasındaki fark eşiği (daha yüksek = daha az renk grubu)")]
        public float colorThreshold = 0.1f;
        
        public Texture2D levelTexture;
        public Vector2Int colorCubeGridSize;
        
        [Title("Grid Setup")] public Vector2Int shooterGridSize = new Vector2Int(3, 5);
        [TableMatrix(DrawElementMethod = nameof(DrawElement), SquareCells = true)]
        public CellData[,] CellsData;

#if UNITY_EDITOR

        [Button]
        public void InitializeGrid()
        {
            Debug.Log("inited");
            CellsData = new CellData[shooterGridSize.x, shooterGridSize.y];
            for (int x = 0; x < shooterGridSize.x; x++)
            for (int y = 0; y < shooterGridSize.y; y++)
                CellsData[x, y] = new CellData(new Vector2Int(x, y));
        }

#if UNITY_EDITOR
        private CellData DrawElement(Rect rect, CellData value, int x, int y)
        {
            if (value == null)
                value = new CellData(new Vector2Int(x, y));

            // Arka plan rengi
            UnityEditor.EditorGUI.DrawRect(rect, value.cellColor);

            // Eğer hücrede count varsa, onu ortada yaz
            if (value.shootCount > 0)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = GetReadableTextColor(value.cellColor) },
                    fontStyle = FontStyle.Bold
                };

                GUI.Label(rect, value.shootCount.ToString(), style);
            }

            return value;
        }
#endif

        
        [Button("Create Colors")]
        private void CreateColors()
        {
            if (levelTexture == null)
            {
                Debug.LogError("Level texture is missing!");
                return;
            }

            if (!levelTexture.isReadable)
            {
                Debug.LogError($"Texture '{levelTexture.name}' is not readable. Enable Read/Write in import settings.");
                return;
            }

            Color[] pixels = levelTexture.GetPixels();
            if (pixels == null || pixels.Length == 0)
            {
                Debug.LogError("No pixel data found. Check import settings or texture type.");
                return;
            }
            
            Debug.Log(pixels.Length);

            levelColors.Clear();

            List<Color> groupedColors = new();

            foreach (Color c in pixels)
            {
                bool found = false;

                foreach (Color g in groupedColors)
                {
                    if (IsSimilarColor(g, c, colorThreshold))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    groupedColors.Add(c);
            }

            int idCounter = 0;
            foreach (var groupColor in groupedColors)
            {
                int count = pixels.Count(p => IsSimilarColor(p, groupColor, colorThreshold));
                levelColors.Add(new LevelColorData(idCounter, groupColor, count));
                idCounter++;
            }

            levelColors = levelColors.OrderByDescending(c => c.size).ToList();
            Debug.Log($"Detected {levelColors.Count} grouped colors from {levelTexture.name}");
        }


        private bool IsSimilarColor(Color a, Color b, float threshold)
        {
            // Basit Euclidean distance (RGB farkı)
            float dr = a.r - b.r;
            float dg = a.g - b.g;
            float db = a.b - b.b;
            float distance = Mathf.Sqrt(dr * dr + dg * dg + db * db);
            return distance < threshold;
        }
        
        [Button("Distribute Colors To Grid")]
        private void DistributeColorsToGrid()
        {
            if (CellsData == null || CellsData.Length == 0)
            {
                Debug.LogError("Grid not initialized!");
                return;
            }

            
            if (levelColors == null || levelColors.Count == 0)
            {
                Debug.LogError("No colors available! Run CreateColors first.");
                return;
            }

            int width = shooterGridSize.x;

            var totalShooterCount = 0;
            Dictionary<Color, List<int>> shooterData = new Dictionary<Color, List<int>>();

            List<Color> colors = new List<Color>();
            foreach (var lc in levelColors)
            {
                var shooterCount = Mathf.FloorToInt(lc.size / 15f);
                var remainder = lc.size % 15;

                shooterData.Add(lc.color, new List<int>());
                for (int x = 0; x < shooterCount; x++)
                {
                    shooterData[lc.color].Add(15);
                }

                if (shooterCount == 0)
                {
                    shooterData[lc.color].Add(lc.size);
                    remainder = 0;
                }

                for (int y = 0; y < remainder; y++)
                {
                    var index = Random.Range(0, shooterCount);
                    Debug.Log(index);
                    Debug.Log(shooterData[lc.color].Count);
                    var value = shooterData[lc.color][index];
                    value++;
                    shooterData[lc.color][index] = value;
                }
                
                totalShooterCount+=shooterCount;
                colors.Add(lc.color);
            }

            int height = Mathf.CeilToInt(totalShooterCount / (float)width);

            shooterGridSize.y = height;
            
            InitializeGrid();
            // Flatten grid
            List<CellData> allCells = new List<CellData>();
            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var cell = CellsData[x, y];
                cell.cellColor = Color.gray; // reset
                cell.shootCount = 0;
                allCells.Add(cell);

                var randomColor = colors.GetRandomElement();
                var dictIndex = Random.Range(0, shooterData[randomColor].Count);
                cell.cellColor = randomColor;
                cell.shootCount = shooterData[randomColor][dictIndex];
                cell.colorID = levelColors.First(l => l.color == randomColor).id;
                
                shooterData[randomColor].RemoveAt(dictIndex);
                
                if(shooterData[randomColor].Count == 0)
                    colors.Remove(randomColor);
            }

            Debug.Log("✅ Colors distributed evenly across grid.");
        }


        private Color GetReadableTextColor(Color bg)
        {
            // Basit luma hesaplaması
            float brightness = (0.299f * bg.r + 0.587f * bg.g + 0.114f * bg.b);
            return brightness > 0.5f ? Color.black : Color.white;
        }

        [Button]
        public void SetCubeSize()
        {
            var width = levelTexture.width;
            var height = levelTexture.height;
            
            colorCubeGridSize.x = width;
            colorCubeGridSize.y = height;
        }
#endif
    }
}