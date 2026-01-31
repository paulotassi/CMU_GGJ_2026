using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasMaskFilterLight : MonoBehaviour
{
    [SerializeField] private GameObject greenLight;
    [SerializeField] private GameObject yellowLight;
    [SerializeField] private GameObject rightLight;

    
    
    
    /// <summary>
    /// just for testing
    /// </summary>
    /// <returns></returns>
    IEnumerator Start()
    {
        while (true)
        {
            SetFilterLightColor(LightColor.Green);
            yield return new WaitForSeconds(1f);
            SetFilterLightColor(LightColor.Yellow);
            yield return new WaitForSeconds(1f);
            SetFilterLightColor(LightColor.Red);
            yield return new WaitForSeconds(1f);
            
        }

    }
    
    /// <summary>
    /// Set which light on the filter to be on. Use enum LightColor either green, yellow, red, none, or all
    /// </summary>
    /// <param name="color"></param>
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
}

public enum LightColor
{
    Green,
    Yellow,
    Red,
    None,
    All
}