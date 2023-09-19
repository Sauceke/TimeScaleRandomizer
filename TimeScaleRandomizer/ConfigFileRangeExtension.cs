using BepInEx.Configuration;
using UnityEngine;

namespace TimeScaleRandomizer
{
    public static class ConfigFileRangeExtension
    {
        public static void BindRange(this ConfigFile configFile, string section, string key,
            float lowerDefaultValue, float upperDefaultValue, string description,
            AcceptableValueRange<float> acceptableValues,
            out ConfigEntry<float> lower, out ConfigEntry<float> upper, string format = "F2")
        {
            var lowerEntry = configFile.Bind(
                section: section,
                key: key + " (Min)",
                defaultValue: lowerDefaultValue,
                configDescription: new ConfigDescription(
                    description: "Minimum value of: " + description,
                    acceptableValues: acceptableValues,
                    tags: new ConfigurationManagerAttributes { Browsable = false }));
            var upperEntry = configFile.Bind(
                section: section,
                key: key + " (Max)",
                defaultValue: upperDefaultValue,
                configDescription: new ConfigDescription(
                    description: "Maximum value of: " + description,
                    acceptableValues: acceptableValues,
                    tags: new ConfigurationManagerAttributes { Browsable = false }));
            void Drawer(ConfigEntryBase entry) =>
                RangeDrawer(lowerEntry, upperEntry, acceptableValues, format);
            configFile.Bind(
                section: section,
                key: key,
                defaultValue: "Ignore this",
                configDescription: new ConfigDescription(
                    description: description,
                    tags: new ConfigurationManagerAttributes
                    {
                        CustomDrawer = Drawer,
                        HideDefaultButton = true
                    }));
            lower = lowerEntry;
            upper = upperEntry;
        }
        
        private static void RangeDrawer(ConfigEntry<float> lowerEntry,
            ConfigEntry<float> upperEntry, AcceptableValueRange<float> acceptableValues,
            string format)
        {
            // Allocate a fixed space to the value labels so the slider won't get displaced while
            // being used
            var minLabel = new GUIContent(acceptableValues.MinValue.ToString(format));
            var maxLabel = new GUIContent(acceptableValues.MaxValue.ToString(format));
            float labelWidth = Mathf.Max(GUI.skin.label.CalcSize(minLabel).x,
                GUI.skin.label.CalcSize(maxLabel).x);
            float lower = lowerEntry.Value;
            float upper = upperEntry.Value;
            GUILayout.BeginHorizontal();
            {
                GUILayout.Label(lower.ToString(format), GUILayout.Width(labelWidth));
                RangeSlider.Create(ref lower, ref upper,
                    acceptableValues.MinValue, acceptableValues.MaxValue);
                GUILayout.Label(upper.ToString(format), GUILayout.Width(labelWidth));
                if (GUILayout.Button("Reset", GUILayout.ExpandWidth(false)))
                {
                    lower = (float)lowerEntry.DefaultValue;
                    upper = (float)upperEntry.DefaultValue;
                }
            }
            GUILayout.EndHorizontal();
            lowerEntry.Value = lower;
            upperEntry.Value = upper;
        }
    }
}