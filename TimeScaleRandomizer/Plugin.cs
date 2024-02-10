using System.Collections;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace TimeScaleRandomizer
{
    [BepInPlugin("Sauceke.TimeScaleRandomizer", "Time Scale Randomizer", "1.0.0")]
    internal class Plugin : BaseUnityPlugin
    {
        private ConfigEntry<bool> enabled;
        private ConfigEntry<float> timeScaleMin;
        private ConfigEntry<float> timeScaleMax;
        private ConfigEntry<float> waitSecsMin;
        private ConfigEntry<float> waitSecsMax;
        private ConfigEntry<float> changeSecsMin;
        private ConfigEntry<float> changeSecsMax;

        private void Start()
        {
            const string title = "Randomization Settings";
            enabled = Config.Bind(
                section: title,
                key: "Enabled",
                defaultValue: true,
                configDescription: new ConfigDescription(
                    description: "Whether to enable time scale randomization.",
                    tags: new ConfigurationManagerAttributes { Order = 1000 }));
            Config.BindRange(
                section: title,
                key: "Time Scale Range",
                lowerDefaultValue: 0.75f,
                upperDefaultValue: 1.25f,
                description: "Range of speeds to select from.",
                acceptableValues: new AcceptableValueRange<float>(1 / 3f, 3f),
                lower: out timeScaleMin,
                upper: out timeScaleMax,
                format: "P0");
            Config.BindRange(
                section: title,
                key: "Waiting Time Range (seconds)",
                lowerDefaultValue: 5f,
                upperDefaultValue: 15f,
                description: "Range of how much to wait before changing the time scale.",
                acceptableValues: new AcceptableValueRange<float>(0f, 30f),
                lower: out waitSecsMin,
                upper: out waitSecsMax);
            Config.BindRange(
                section: title,
                key: "Change Time Range (seconds)",
                lowerDefaultValue: 1f,
                upperDefaultValue: 5f,
                description: "Range of how long it will take to gradually change the time scale.",
                acceptableValues: new AcceptableValueRange<float>(0f, 30f),
                lower: out changeSecsMin,
                upper: out changeSecsMax);
            StartCoroutine(Run());
        }
        
        private IEnumerator Run()
        {
            while (true)
            {
                if (!enabled.Value)
                {
                    Time.timeScale = 1f;
                    yield return new WaitForSeconds(1f);
                    continue;
                }
                if (Time.timeScale == 0f)
                {
                    yield return new WaitForSeconds(1f);
                    continue;
                }
                float waitSecs = UnityEngine.Random.Range(waitSecsMin.Value, waitSecsMax.Value);
                yield return new WaitForSeconds(waitSecs * Time.timeScale);
                float newTimeScale = UnityEngine.Random
                    .Range(timeScaleMin.Value, timeScaleMax.Value);
                float changeSecs = UnityEngine.Random
                    .Range(changeSecsMin.Value, changeSecsMax.Value);
                yield return ChangeTimeScale(newTimeScale, changeSecs);
            }
        }

        private static IEnumerator ChangeTimeScale(float targetTimeScale, float durationSecs)
        {
            float startTimeScale = Time.timeScale;
            const float refreshTimeSecs = 0.1f;
            int iterations = Mathf.RoundToInt(durationSecs / refreshTimeSecs);
            for (int i = 0; i < iterations; i++)
            {
                yield return new WaitForSeconds(refreshTimeSecs * Time.timeScale);
                Time.timeScale = Mathf.Lerp(startTimeScale, targetTimeScale, (i + 1f) / iterations);
            }
        }
    }
}