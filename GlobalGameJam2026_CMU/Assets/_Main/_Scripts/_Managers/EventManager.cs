using System;
using UnityEngine;

public static class EventManager
{

    // Fired when the game starts (ex: after loading finishes / player gains control)
    public static event Action gameStart;

    // Fired when the player wins the game
    public static event Action gameWin;

    // Fired when the player dies
    public static event Action<float> playerDeath;

    // Fired whenever player health changes (send the new current health)
    public static event Action<int> currentPlayerHealth;

    // Fired whenever gas mask health changes (send the new current mask health)
    public static event Action<int> currentMaskHealth;

    // TextEvents
    public static event Action<string> TriggerText;



    #region Invoke Helpers

    // Call this when the game starts
    public static void GameStart()
    {
        gameStart?.Invoke();
    }

    // Call this when the player wins
    public static void GameWin()
    {
        gameWin?.Invoke();
    }

    public static void TextTrigger(string intendedText)
    {
        TriggerText?.Invoke(intendedText);
    }

    // Call this when the player dies
    public static void PlayerDeath(float deathEventDuration)
    {
        Debug.Log("Heard Player Died from the EM");
        playerDeath?.Invoke(deathEventDuration);
    }

    // Call this whenever player health changes
    public static void CurrentPlayerHealth(int value)
    {
        currentPlayerHealth?.Invoke(value);
    }

    // Call this whenever mask health changes
    public static void CurrentMaskHealth(int value)
    {
        currentMaskHealth?.Invoke(value);
    }

    #endregion
}
