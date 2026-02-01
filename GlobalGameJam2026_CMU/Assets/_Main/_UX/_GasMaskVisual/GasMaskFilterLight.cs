using UnityEngine;
using System;

public class GasMaskFilterLight : MonoBehaviour
{
    [SerializeField] private GameObject greenLight;   // Active when mask health > 66
    [SerializeField] private GameObject yellowLight;  // Active when mask health is 33–65
    [SerializeField] private GameObject rightLight;   // Red light (naming kept as-is)

    #region Unity Lifecycle

    void OnEnable()
    {
        // Subscribe to mask health updates
        EventManager.currentMaskHealth += OnMaskHealthChanged;
    }

    void OnDisable()
    {
        // Always unsubscribe to avoid memory leaks / null refs
        EventManager.currentMaskHealth -= OnMaskHealthChanged;
    }

    #endregion

    #region Event Callbacks

    /// <summary>
    /// Called whenever the gas mask health changes.
    /// Converts health value into a light color state.
    /// </summary>
    /// <param name="currentHealth">Current gas mask health value</param>
    private void OnMaskHealthChanged(int currentHealth)
    {
        if (currentHealth > 66)
        {
            SetFilterLightColor(LightColor.Green);
        }
        else if (currentHealth >= 33)
        {
            SetFilterLightColor(LightColor.Yellow);
        }
        else
        {
            SetFilterLightColor(LightColor.Red);
        }
    }

    #endregion

    #region Light Control

    /// <summary>
    /// Enables exactly one (or multiple) filter lights based on the selected color.
    /// </summary>
    /// <param name="color">Desired filter light color</param>
    public void SetFilterLightColor(LightColor color)
    {
        switch (color)
        {
            case LightColor.Green:
                greenLight.SetActive(true);
                yellowLight.SetActive(false);
                rightLight.SetActive(false);
                break;

            case LightColor.Yellow:
                greenLight.SetActive(false);
                yellowLight.SetActive(true);
                rightLight.SetActive(false);
                break;

            case LightColor.Red:
                greenLight.SetActive(false);
                yellowLight.SetActive(false);
                rightLight.SetActive(true);
                break;

            case LightColor.None:
                greenLight.SetActive(false);
                yellowLight.SetActive(false);
                rightLight.SetActive(false);
                break;

            case LightColor.All:
                greenLight.SetActive(true);
                yellowLight.SetActive(true);
                rightLight.SetActive(true);
                break;
        }
    }

    #endregion
}

public enum LightColor
{
    Green,
    Yellow,
    Red,
    None,
    All
}
