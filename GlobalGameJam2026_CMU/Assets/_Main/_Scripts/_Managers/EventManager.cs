using UnityEngine;

using System;
using UnityEngine;

public static class EventManager
{
    public static event Action OnPreRoundStart;
    public static event Action OnInRoundStart;
    public static event Action OnPostRoundStart;
    public static event Action OnRoundEnd;
    public static event Action OnCanSlap;
    public static event Action OnFakeTrigger;
    public static event Action<int> OnSlap;
    public static event Action<int> OnFault;
    public static event Action<int> OnReady;

    public static void CanSlap()
    {
        OnCanSlap?.Invoke();
    }

    public static void RoundEnd()
    {
        OnRoundEnd?.Invoke();
    }

    public static void FakeStartTrigger()
    {
        OnFakeTrigger?.Invoke();
    }

    public static void PreRoundStart()
    {
        OnPreRoundStart?.Invoke();
    }

    public static void InRoundStart()
    {
        OnInRoundStart?.Invoke();
    }

    public static void PostRoundStart()
    {
        OnPostRoundStart?.Invoke();
    }

    public static void Slapped(int player)
    {
        OnSlap?.Invoke(player);
    }

    public static void Faulted(int player)
    {
        OnFault?.Invoke(player);
    }

    public static void Readied(int player)
    {
        OnReady?.Invoke(player);
    }
}
