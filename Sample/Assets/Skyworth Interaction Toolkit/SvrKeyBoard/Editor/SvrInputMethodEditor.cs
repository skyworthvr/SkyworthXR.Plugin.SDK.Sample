using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace Svr.Keyboard
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(SvrInputMethod), true)]
    public class SvrInputMethodEditor : Editor
    {
        private const string kAutoPosition = "m_AutoPosition";
        private const string kbelow_pos = "m_below_pos";
        private const string khorizontal_offset = "m_horizontal_offset";
        SerializedProperty m_AutoPositionSP;
        SerializedProperty m_below_posSP;
        SerializedProperty m_horizontal_offsetSP;

        static GUIContent s_AutoPosition = EditorGUIUtility.TrTextContent("Auto Position");
        static GUIContent s_below_pos = EditorGUIUtility.TrTextContent("Relative Below Position");
        static GUIContent s_horizontal_offset = EditorGUIUtility.TrTextContent("Relative Horizontal Position");

        public override void OnInspectorGUI()
        {
            if (serializedObject == null || serializedObject.targetObject == null)
                return;
            serializedObject.Update();

            if (m_AutoPositionSP == null) m_AutoPositionSP = serializedObject.FindProperty(kAutoPosition);
            if (m_below_posSP == null) m_below_posSP = serializedObject.FindProperty(kbelow_pos);
            if (m_horizontal_offsetSP == null) m_horizontal_offsetSP = serializedObject.FindProperty(khorizontal_offset);

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true));

            EditorGUILayout.PropertyField(m_AutoPositionSP, s_AutoPosition);
            if (m_AutoPositionSP.boolValue)
            {
                EditorGUILayout.PropertyField(m_below_posSP, s_below_pos);
                EditorGUILayout.PropertyField(m_horizontal_offsetSP, s_horizontal_offset);
            }

            EditorGUILayout.EndVertical();
            serializedObject.ApplyModifiedProperties();
        }
    }
}
