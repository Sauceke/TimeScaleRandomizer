using System.Collections;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BepInEx.Unity.IL2CPP.Utils.Collections;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using UnityEngine;

namespace BepInEx;

public class BaseUnityPlugin : BasePlugin
{
    protected ManualLogSource Logger => Log;

    public override void Load() => Traverse.Create(this).Method("Start").GetValue();

    protected void StartCoroutine(IEnumerator coroutine)
    {
        var go = new GameObject("TimeScaleRandomizerManager");
        GameObject.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave;
        ClassInjector.RegisterTypeInIl2Cpp<Script>();
        var script = go.AddComponent<Script>();
        script.StartCoroutine(coroutine.WrapToIl2Cpp());
    }

    private class Script: MonoBehaviour
    { }
}