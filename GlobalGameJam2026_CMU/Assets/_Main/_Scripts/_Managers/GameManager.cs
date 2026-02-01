using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float fadeToBlackDuration = 5f;
    private void OnEnable()
    {
        EventManager.playerDeath += endGame;
    }

    private void OnDisable()
    {
        EventManager.playerDeath -= endGame;
    }

    private void endGame()
    {
        StartCoroutine(gameEndCoroutine());
    }

    private IEnumerator gameEndCoroutine()
    {
        yield return new WaitForSeconds(fadeToBlackDuration);
    }

}
