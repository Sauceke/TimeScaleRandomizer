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
    private Manager manager;
    
    protected ManualLogSource Logger => Log;

    public override void Load()
    {
        var go = new GameObject("TimeScaleRandomizerManager");
        GameObject.DontDestroyOnLoad(go);
        go.hideFlags = HideFlags.HideAndDontSave;
        ClassInjector.RegisterTypeInIl2Cpp<Manager>();
        manager = go.AddComponent<Manager>();
        Traverse.Create(this).Method("Start").GetValue();
    }

    protected Coroutine StartCoroutine(IEnumerator coroutine) =>
        manager.StartCoroutine(coroutine.WrapToIl2Cpp());

    protected void StopCoroutine(Coroutine coroutine) =>
        manager.StopCoroutine(coroutine);

    private class Manager: MonoBehaviour
    { }
}