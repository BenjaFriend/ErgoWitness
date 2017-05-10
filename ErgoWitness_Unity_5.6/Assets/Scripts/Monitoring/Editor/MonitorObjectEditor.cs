using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MonitorObject))]
public class MonitorObjectEditor : Editor {

    MonitorObject m_Target;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        m_Target = (MonitorObject)target;
        DrawImages();

        DrawDefaultInspector();        
    }

    private void DrawImages()
    {
        
        GUILayout.Label("THIS IS A TEST LABEL");

        GUILayout.Space(20);
    }
}
