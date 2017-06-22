using UnityEditor;

namespace UnityStandardAssets.CinematicEffects
{
    public class FXAAEditor : IAntiAliasingEditor
    {
        private string[] presetNames =
        {
            "Extreme performance",
            "Performance",
            "Default",
            "Quality",
            "Extreme quality"
        };


        public void OnEnable(SerializedObject serializedObject, string path)
        {
        }

        public bool OnInspectorGUI(IAntiAliasing target)
        {
            var fxaaTarget = (FXAA)target;
            int selectedPreset = 2;

            if (fxaaTarget.preset.Equals(FXAA.Preset.extremePerformancePreset))
                selectedPreset = 0;
            else if (fxaaTarget.preset.Equals(FXAA.Preset.performancePreset))
                selectedPreset = 1;
            else if (fxaaTarget.preset.Equals(FXAA.Preset.defaultPreset))
                selectedPreset = 2;
            else if (fxaaTarget.preset.Equals(FXAA.Preset.qualityPreset))
                selectedPreset = 3;
            else if (fxaaTarget.preset.Equals(FXAA.Preset.extremeQualityPreset))
                selectedPreset = 4;

            EditorGUI.BeginChangeCheck();
            selectedPreset = EditorGUILayout.Popup("Preset", selectedPreset, presetNames);

            if (EditorGUI.EndChangeCheck())
            {
                if (selectedPreset < 0)
                    selectedPreset = 0;
                else if (selectedPreset > 4)
                    selectedPreset = 4;

                fxaaTarget.preset = FXAA.availablePresets[selectedPreset];
                return true;
            }
            return false;
        }
    }
}
