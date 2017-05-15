using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BroMonitor))]
public class BroMonitor_Editor : Editor
{
    BroMonitor m_Target;
    bool showQuery = false;
    string status = "See your query";

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DrawDefaultInspector();
        m_Target = (BroMonitor)target;

        GUILayout.Label("Packets per query: " + m_Target.PacketPerQuery);

        GUILayout.Space(20);


        showQuery = EditorGUILayout.Foldout(showQuery, status);     

        if (showQuery)
        {

            GUILayout.Label(
                Resources.Load(m_Target.fileLocation_queryTop).ToString() +
                 "TIMESTAMP\n" +
                 Resources.Load(m_Target.fileLocation_queryBottom).ToString());
        }
    }

}
