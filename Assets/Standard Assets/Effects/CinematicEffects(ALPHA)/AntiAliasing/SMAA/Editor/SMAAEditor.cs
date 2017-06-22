using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace UnityStandardAssets.CinematicEffects
{
    public class SMAAEditor : IAntiAliasingEditor
    {
        private List<SerializedProperty> m_TopLevelFields = new List<SerializedProperty>();
        private Dictionary<FieldInfo, List<SerializedProperty>> m_GroupFields = new Dictionary<FieldInfo, List<SerializedProperty>>();

        public void OnEnable(SerializedObject serializedObject, string path)
        {
            var topLevelSettings = typeof(SMAA).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.GetCustomAttributes(typeof(SMAA.TopLevelSettings), false).Any());
            var settingsGroups = typeof(SMAA).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.GetCustomAttributes(typeof(SMAA.SettingsGroup), false).Any());

            foreach (var group in topLevelSettings)
            {
                var searchPath = path + "." + group.Name + ".";

                foreach (var setting in group.FieldType.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    var property = serializedObject.FindProperty(searchPath + setting.Name);
                    if (property != null)
                        m_TopLevelFields.Add(property);
                }
            }

            foreach (var group in settingsGroups)
            {
                var searchPath = path + "." + group.Name + ".";

                foreach (var setting in group.FieldType.GetFields(BindingFlags.Instance | BindingFlags.Public))
                {
                    List<SerializedProperty> settingsGroup;
                    if (!m_GroupFields.TryGetValue(group, out settingsGroup))
                    {
                        settingsGroup = new List<SerializedProperty>();
                        m_GroupFields[group] = settingsGroup;
                    }

                    var property = serializedObject.FindProperty(searchPath + setting.Name);
                    if (property != null)
                        settingsGroup.Add(property);
                }
            }
        }

        public bool OnInspectorGUI(IAntiAliasing target)
        {
            EditorGUI.BeginChangeCheck();

            foreach (var setting in m_TopLevelFields)
                EditorGUILayout.PropertyField(setting);

            foreach (var group in m_GroupFields)
            {
                if (group.Key.FieldType == typeof(SMAA.QualitySettings) && (target as SMAA).settings.quality != SMAA.QualityPreset.Custom)
                    continue;

                bool isExperimental = group.Key.GetCustomAttributes(typeof(SMAA.ExperimentalGroup), false).Length > 0;
                string title = ObjectNames.NicifyVariableName(group.Key.Name);
                if (isExperimental)
                    title += " (Experimental)";

                EditorGUILayout.Space();
                EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                var enabledField = group.Value.FirstOrDefault(x => x.propertyPath == "m_SMAA." + group.Key.Name + ".enabled");
                if (enabledField != null && !enabledField.boolValue)
                {
                    EditorGUILayout.PropertyField(enabledField);
                    EditorGUI.indentLevel--;
                    continue;
                }

                foreach (var field in group.Value)
                    EditorGUILayout.PropertyField(field);

                EditorGUI.indentLevel--;
            }
            return EditorGUI.EndChangeCheck();
        }
    }
}
