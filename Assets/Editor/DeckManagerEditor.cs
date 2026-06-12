#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DrawPileManager))]
public class DrawPileManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        DrawPileManager drawPileManager = (DrawPileManager)target;

        if (GUILayout.Button("Draw Next Card"))
        {
            HandManager handManager = Object.FindFirstObjectByType<HandManager>();

            if (handManager != null)
            {
                drawPileManager.DrawCard(handManager);
            }
            else
            {
                Debug.LogWarning("No HandManager found in the scene.");
            }
        }
    }
}
#endif