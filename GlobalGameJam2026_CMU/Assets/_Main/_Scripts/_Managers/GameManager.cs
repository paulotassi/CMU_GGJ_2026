using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float fadeToBlackDuration = 5f;
    private void OnEnable()
    {
        EventManager.playerDeath += endGame;
        EventManager.gameWin += winGame;
    }

    private void OnDisable()
    {
        EventManager.playerDeath -= endGame;
        EventManager.gameWin -= winGame;
    }

    private void winGame()
    {
        Debug.Log("Game will Reset");
        StartCoroutine(gameWinCoroutine());
    }
    private void endGame(float deathEventDuration)
    {
        Debug.Log("Game will Reset");
        StartCoroutine(gameEndCoroutine(deathEventDuration));
    }

    private IEnumerator gameEndCoroutine(float deathEventDuration)
    {
        yield return new WaitForSeconds(deathEventDuration);
        
        
    }

    private IEnumerator gameWinCoroutine()
    {
        yield return new WaitForSeconds(4);
        SceneManager.LoadScene("Win_Credits");


    }

}
