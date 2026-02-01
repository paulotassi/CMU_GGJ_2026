using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class TitleMenu : MonoBehaviour
{
    [Header("Scenes")]
    public string sceneToLoad;

    [Header("Credits UI")]
    public GameObject creditsPanel;          // The full credits panel
    public GameObject creditsFirstButton;    // First button in the credits panel

    [Header("Main Menu Buttons")]
    public Button[] mainMenuButtons;         // All buttons in the main menu
    public GameObject mainMenuFirstButton;   // First button to select when returning from credits

    /// <summary>
    /// Loads the main game scene
    /// </summary>
    /// 


    [SerializeField] private RectTransform arrowImage;
    
    private void Start()
    {
        if (sceneToLoad == "")
        {
            Debug.LogError($"Empty string name of Scene to be loaded in TitleMenu.cs on {gameObject.name}");
        }
        // Make sure a button is selected at start for controller/keyboard navigation
        if (mainMenuFirstButton != null)
        {
            StartCoroutine(SelectButtonNextFrame(mainMenuFirstButton));
        }
    }

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            RectTransform currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>();
            if (currentButton != null)
            {
                arrowImage.anchoredPosition = new Vector2(arrowImage.anchoredPosition.x, currentButton.anchoredPosition.y);
            }
        }
    }

    public void LoadScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }

    /// <summary>
    /// Shows the credits panel and disables main menu buttons
    /// </summary>
    public void ShowCredits()
    {
        if (creditsPanel == null || creditsFirstButton == null) return;

        creditsPanel.SetActive(true);
        SetButtonsInteractable(mainMenuButtons, false);

        // Use a coroutine to ensure the button is active before selecting
        StartCoroutine(SelectButtonNextFrame(creditsFirstButton));
    }

    /// <summary>
    /// Hides the credits panel and restores main menu buttons
    /// </summary>
    public void HideCredits()
    {
        if (creditsPanel == null) return;

        creditsPanel.SetActive(false);
        SetButtonsInteractable(mainMenuButtons, true);

        // Restore selection to main menu first button
        if (mainMenuFirstButton != null)
        {
            StartCoroutine(SelectButtonNextFrame(mainMenuFirstButton));
        }
    }

    /// <summary>
    /// Coroutine to select a button on the next frame
    /// Ensures controller/keyboard navigation works
    /// </summary>
    private IEnumerator SelectButtonNextFrame(GameObject button)
    {
        yield return null; // wait for end of frame
        EventSystem.current.SetSelectedGameObject(null); // clear previous selection
        EventSystem.current.SetSelectedGameObject(button); // set new selection
    }

    /// <summary>
    /// Enables or disables an array of buttons
    /// </summary>
    private void SetButtonsInteractable(Button[] buttons, bool state)
    {
        if (buttons == null) return;
        foreach (var btn in buttons)
        {
            if (btn != null)
                btn.interactable = state;
        }
    }

    /// <summary>
    /// Quits the game (works in editor and build)
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
