using System;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCallbacks : MonoBehaviour
{
    private static Dictionary<Collider, (Action<Collider>, Action<Collider>)> callbacks =
        new Dictionary<Collider, (Action<Collider>, Action<Collider>)>();

    public static void Register(Collider col, Action<Collider> onEnter, Action<Collider> onExit)
    {
        if (!callbacks.ContainsKey(col))
            callbacks.Add(col, (onEnter, onExit));
    }

    public static void Unregister(Collider col, Action<Collider> onEnter, Action<Collider> onExit)
    {
        if (callbacks.ContainsKey(col))
            callbacks.Remove(col);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (callbacks.TryGetValue(GetComponent<Collider>(), out var cb))
            cb.Item1?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (callbacks.TryGetValue(GetComponent<Collider>(), out var cb))
            cb.Item2?.Invoke(other);
    }
}