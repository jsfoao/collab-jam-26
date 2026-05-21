using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TransformAnimator))]
public class TransformAnimatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        TransformAnimator animator = (TransformAnimator)target;

        GUILayout.Space(10);
        if (GUILayout.Button("Start Forward"))
        {
            animator.StartForwardAnim();
        }
        if (GUILayout.Button("Start Reverse"))
        {
            animator.StartReverseAnim();
        }
    }
}