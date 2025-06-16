using System;
using System.Reflection;
using UnityEngine;

public static class RenderDocTrigger
{
#if UNITY_EDITOR_WIN
    private static object _renderDocManager;
    private static MethodInfo _triggerCapture;

    static RenderDocTrigger()
    {
        Type type = Type.GetType("UnityEditorInternal.RenderDoc, UnityEditor.dll");
        _renderDocManager = type?.GetProperty("instance", BindingFlags.Static | BindingFlags.Public)?.GetValue(null);
        _triggerCapture = type?.GetMethod("Capture", BindingFlags.Instance | BindingFlags.Public);
    }

    public static void TriggerCapture()
    {
        _triggerCapture?.Invoke(_renderDocManager, null);
        Debug.Log("📸 RenderDoc capture triggered!");
    }
#endif
}
