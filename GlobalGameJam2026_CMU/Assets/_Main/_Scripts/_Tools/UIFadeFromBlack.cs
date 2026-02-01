using UnityEngine;
using UnityEngine.UI;

public class UIFadeFromBlack : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 2f;   // Time to fade from black to clear

    Image fadeImage;
    float timer;

    void Awake()
    {
        fadeImage = GetComponent<Image>();

        // Force start fully black
        Color color = fadeImage.color;
        color.a = 1f;
        fadeImage.color = color;
    }

    void Start()
    {
        timer = 0f;
    }

    void Update()
    {
        if (timer >= fadeDuration)
        {
            return;
        }

        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / fadeDuration);

        Color color = fadeImage.color;
        color.a = Mathf.Lerp(1f, 0f, t);
        fadeImage.color = color;
    }
}
