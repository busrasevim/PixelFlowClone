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

        [TableList(AlwaysExpanded = true)] public List<LevelColorData> levelColors = new();

        [Range(0.01f, 0.5f)] [Tooltip("Renkler arasındaki fark eşiği (daha yüksek = daha az renk grubu)")]
        public float colorThreshold = 0.1f;

        public Texture2D levelTexture;
        public Vector2Int colorCubeGridSize;

        // runtime tarafında kullanılacak geçici renk değerleri (Color Palette Preview için)
        [HideInInspector] public List<LevelColorData> runtimeColorStats = new();

        [Button("Set Cube Size")]
        public void SetCubeSize()
        {
            if (levelTexture == null)
            {
                Debug.LogError("Texture is missing!");
                return;
            }

            colorCubeGridSize = new Vector2Int(levelTexture.width, levelTexture.height);
            Debug.Log($"✅ Cube size set: {colorCubeGridSize.x}x{colorCubeGridSize.y}");
        }

        [Title("Grid Setup")] public Vector2Int shooterGridSize = new Vector2Int(3, 5);

#if UNITY_EDITOR
        [TableMatrix(DrawElementMethod = "DrawElement", SquareCells = true)]
#else
        [TableMatrix(SquareCells = true)]
#endif
        public CellData[,] CellsData;

        [HideInInspector] public int selectedColorIndex = -1;

        [ShowInInspector, ReadOnly, PropertyOrder(-5)]
        [LabelText("🎨 Selected Color")]
        [GUIColor("@GetSelectedColorPreview()")]
        private string SelectedColorLabel =>
            selectedColorIndex >= 0 && selectedColorIndex < levelColors.Count
                ? $"ID: {levelColors[selectedColorIndex].id} | Size: {levelColors[selectedColorIndex].size}"
                : "None Selected";

        private Color GetSelectedColorPreview()
        {
            if (selectedColorIndex < 0 || selectedColorIndex >= levelColors.Count)
                return Color.gray;
            return levelColors[selectedColorIndex].color;
        }

#if UNITY_EDITOR
        [Button("Initialize Grid")]
        public void InitializeGrid()
        {
            CellsData = new CellData[shooterGridSize.x, shooterGridSize.y];
            for (int x = 0; x < shooterGridSize.x; x++)
            for (int y = 0; y < shooterGridSize.y; y++)
                CellsData[x, y] = new CellData(new Vector2Int(x, y));
        }

        private CellData DrawElement(Rect rect, CellData value, int x, int y)
        {
            if (value == null)
                value = new CellData(new Vector2Int(x, y));

            // Hücre arka planını çiz
            UnityEditor.EditorGUI.DrawRect(rect, value.cellColor);

            // Eğer renk atanmışsa (gray değilse), shootCount yaz
            if (value.cellColor != Color.gray)
            {
                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = GetReadableTextColor(value.cellColor) }
                };
                GUI.Label(rect, value.shootCount.ToString(), style);
            }

            // Hücreye tıklama (renk ataması)
            if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                if (selectedColorIndex >= 0 && selectedColorIndex < levelColors.Count)
                {
                    var data = levelColors[selectedColorIndex];

                    if (value.cellColor != data.color)
                    {
                        value.shootCount = 0;
                    }
                    
                    value.cellColor = data.color;
                    value.colorID = data.id;

                    // Eğer shootCount daha önce tanımlı değilse, 0 olarak başlat
                    if (value.shootCount == 0)
                        value.shootCount = 0;
                }

                // Grid anında yenilensin
                GUI.changed = true;
                UnityEditor.EditorUtility.SetDirty(this);
            }

            if (rect.Contains(Event.current.mousePosition) && value.colorID >= 0 &&
                value.colorID < runtimeColorStats.Count && value.cellColor != Color.gray)
            {
                LevelColorData colorData = null;
                foreach (var runtimeColorStat in runtimeColorStats)
                {
                    if (runtimeColorStat.id == value.colorID)
                    {
                        colorData = runtimeColorStat;
                    }
                }

                if (Event.current.type == EventType.KeyDown)
                {
                    if (Event.current.keyCode == KeyCode.Z && colorData.size > 0)
                    {
                        value.shootCount++;
                        colorData.size--;
                        Event.current.Use();
                        GUI.changed = true;
                        UnityEditor.EditorUtility.SetDirty(this);
                    }
                    else if (Event.current.keyCode == KeyCode.X && value.shootCount > 0)
                    {
                        value.shootCount--;
                        colorData.size++;

                        Event.current.Use();
                        GUI.changed = true;
                        UnityEditor.EditorUtility.SetDirty(this);
                    }
                }
            }


            return value;
        }


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
                Debug.LogError("No pixel data found.");
                return;
            }

            levelColors.Clear();
            List<Color> groupedColors = new();

            foreach (Color c in pixels)
            {
                bool found = groupedColors.Any(g => IsSimilarColor(g, c, colorThreshold));
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

            // runtimeColorStats'i sıfırla ve kopyala
            runtimeColorStats = levelColors.Select(c => new LevelColorData(c.id, c.color, c.size)).ToList();
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
            Dictionary<Color, List<int>> shooterData = new();
            List<Color> colors = new();

            foreach (var lc in levelColors)
            {
                var shooterCount = Mathf.FloorToInt(lc.size / 15f);
                var remainder = lc.size % 15;

                shooterData.Add(lc.color, new List<int>());
                for (int x = 0; x < shooterCount; x++)
                    shooterData[lc.color].Add(15);

                if (shooterCount == 0)
                {
                    shooterData[lc.color].Add(lc.size);
                    remainder = 0;
                    totalShooterCount++;
                }

                for (int y = 0; y < remainder; y++)
                {
                    var index = Random.Range(0, shooterCount);
                    var value = shooterData[lc.color][index];
                    value++;
                    shooterData[lc.color][index] = value;
                }

                totalShooterCount += shooterCount;
                colors.Add(lc.color);
            }

            int height = Mathf.CeilToInt(totalShooterCount / (float)width);
            shooterGridSize.y = height;
            InitializeGrid();

            for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
            {
                var cell = CellsData[x, y];
                cell.cellColor = Color.gray;
                cell.shootCount = 0;

                var randomColor = colors.GetRandomElement();
                var dictIndex = Random.Range(0, shooterData[randomColor].Count);
                cell.cellColor = randomColor;
                cell.shootCount = shooterData[randomColor][dictIndex];
                cell.colorID = levelColors.First(l => l.color == randomColor).id;

                shooterData[randomColor].RemoveAt(dictIndex);
                if (shooterData[randomColor].Count == 0)
                    colors.Remove(randomColor);
            }

            Debug.Log("✅ Colors distributed evenly across grid.");
        }

        private bool IsSimilarColor(Color a, Color b, float threshold)
        {
            float dr = a.r - b.r;
            float dg = a.g - b.g;
            float db = a.b - b.b;
            float distance = Mathf.Sqrt(dr * dr + dg * dg + db * db);
            return distance < threshold;
        }

        private Color GetReadableTextColor(Color bg)
        {
            float brightness = (0.299f * bg.r + 0.587f * bg.g + 0.114f * bg.b);
            return brightness > 0.5f ? Color.black : Color.white;
        }

        // 🎨 Renk paleti önizleme
        [Title("Color Palette Preview")] [ShowInInspector, PropertyOrder(-4)] [HideLabel, NonSerialized]
        private bool _drawColorGrid;

        [OnInspectorGUI("DrawColorGrid")]
        private void DrawColorGrid()
        {
            if (runtimeColorStats == null || runtimeColorStats.Count == 0)
            {
                GUILayout.Label("No color stats yet. Run CreateColors first.");
                return;
            }

            const int cellSize = 40;
            const int columns = 6;
            int padding = 4;

            int total = runtimeColorStats.Count;
            int rows = Mathf.CeilToInt(total / (float)columns);

            GUILayout.BeginVertical();
            for (int y = 0; y < rows; y++)
            {
                GUILayout.BeginHorizontal();
                for (int x = 0; x < columns; x++)
                {
                    int index = y * columns + x;
                    if (index >= total)
                        break;

                    var colorData = runtimeColorStats[index];
                    var rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize),
                        GUILayout.Height(cellSize));

                    UnityEditor.EditorGUI.DrawRect(rect, colorData.color);

                    // outline
                    if (index == selectedColorIndex)
                        UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.yellow);
                    else
                        UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.black * 0.4f);

                    // size değeri her daim ortada
                    GUIStyle sizeStyle = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 11,
                        fontStyle = FontStyle.Bold,
                        normal = { textColor = GetReadableTextColor(colorData.color) }
                    };
                    GUI.Label(rect, colorData.size.ToString(), sizeStyle);

                    // seçili yazısı
                    if (index == selectedColorIndex)
                    {
                        Rect selectedRect = new Rect(rect.x, rect.y + 2, rect.width, 14);
                        GUIStyle selectedStyle = new GUIStyle(GUI.skin.label)
                        {
                            alignment = TextAnchor.UpperCenter,
                            fontSize = 10,
                            fontStyle = FontStyle.Bold,
                            normal = { textColor = GetReadableTextColor(colorData.color) }
                        };
                        GUI.Label(selectedRect, "Selected", selectedStyle);
                    }

                    // fare tıklaması
                    if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                    {
                        selectedColorIndex = index;
                        GUI.changed = true;
                    }

                    GUILayout.Space(padding);
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(padding);
            }

            GUILayout.EndVertical();
        }


#endif
    }
}