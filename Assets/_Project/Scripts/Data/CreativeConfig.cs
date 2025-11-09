using UnityEngine;

[CreateAssetMenu(menuName = "Settings/Creative Config")]
public class CreativeConfig : ScriptableObject
{
    public KeyCode restartKey = KeyCode.R;
    public KeyCode winKey = KeyCode.W;
    public KeyCode failKey = KeyCode.F;
}