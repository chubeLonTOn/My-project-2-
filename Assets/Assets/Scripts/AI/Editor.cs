using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AIMovement))]
public class ToggleGizmosEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AIMovement aiMovement = (AIMovement)target;
        DrawDefaultInspector();

        if (GUILayout.Button("Show Path"))
        {
            aiMovement.isPressed = !aiMovement.isPressed;
        }
    }
}
