using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class FadingText : MonoBehaviour
{

    [SerializeField] private float displayDuration = 2.0f;
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private TMPro.TextMeshProUGUI textMeshPro;
    void OnEnable()
    {
        // Subscribe to mask health updates
        EventManager.TriggerText += DisplayText;
    }

    void OnDisable()
    {
        // Always unsubscribe to avoid memory leaks / null refs
        EventManager.TriggerText -= DisplayText;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void DisplayText(string message)
    {
        Debug.Log("FadingText received message: " + message);
        StartCoroutine(FadeTextRoutine(message));
    }

    private IEnumerator FadeTextRoutine(string message)
    {
        
        textMeshPro.text = message;
        textMeshPro.alpha = 1.0f;

        // Wait for the display duration
        yield return new WaitForSeconds(displayDuration);

        
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            textMeshPro.alpha = Mathf.Lerp(1.0f, 0.0f, elapsedTime / fadeDuration);
            yield return null;
        }

        textMeshPro.alpha = 0.0f;
        textMeshPro.text = "";
    }
}
