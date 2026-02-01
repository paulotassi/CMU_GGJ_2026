using System;
using UnityEngine;

public class BloodSplatterScale : MonoBehaviour
{
    [SerializeField] private RectTransform bloodSplatterImage;

    private float maxScale = 4.5f;
    
    private float minScale = 2f;
    
    [SerializeField] private PlayerController player;


    private void Start()
    {
        if (!player)
        {
            player = FindFirstObjectByType<PlayerController>();
        }
    }

    private void Update()
    {
        ScaleBloodSplatterToPlayerHealth();
    }

    
    /// <summary>
    /// Scales the blood splatter image scale (2-4.5) relative to player health (0-100)
    /// </summary>
    private void ScaleBloodSplatterToPlayerHealth()
    {
        
        float lerp = Mathf.Lerp(minScale, maxScale, (float)player.playerHealth / (float)player.startingPlayerHealth);
//        Debug.Log($"{minScale}, {maxScale}, {player.playerHealth}, {player.startingPlayerHealth}, {(float)player.playerHealth / (float)player.startingPlayerHealth}, {lerp}");

        bloodSplatterImage.localScale = new Vector3(lerp, lerp, lerp);
        
    }
}
