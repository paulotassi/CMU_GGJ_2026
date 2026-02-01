using UnityEngine;
using UnityEngine.SceneManagement;

public class UIScrollUp : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 50f;     // Units per second (UI units)
    public float scrollDuration = 5f;   // Total time to scroll

    [Header("Scene Transition")]
    public string titleSceneName = "TitleScene";

    RectTransform rectTransform;
    float timer;
    bool isScrolling;
    bool waitingForInput;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        StartScroll();
    }

    public void StartScroll()
    {
        timer = 0f;
        isScrolling = true;
        waitingForInput = false;
    }

    void Update()
    {
        // --- SCROLL PHASE ---
        if (isScrolling)
        {
            timer += Time.deltaTime;

            if (timer >= scrollDuration)
            {
                isScrolling = false;
                waitingForInput = true;
                return;
            }

            rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            return;
        }

        // --- INPUT WAIT PHASE ---
        if (waitingForInput)
        {

                SceneManager.LoadScene(titleSceneName);
            
        }
    }
}
