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

    private void endGame(float deathEventDuration)
    {
        StartCoroutine(gameEndCoroutine(deathEventDuration));
    }

    private IEnumerator gameEndCoroutine(float deathEventDuration)
    {
        yield return new WaitForSeconds(deathEventDuration);
        //Load Game Start Scene
        Debug.Log("Game will Reset");
    }

}
