using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioFadeIn : MonoBehaviour
{
    [Header("Fade Settings")]
    public float fadeDuration = 2f;     // Time to fade in
    public float targetVolume = 1f;     // Final volume level

    AudioSource audioSource;
    float timer;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = 0f;
        audioSource.Play();
    }

    void Update()
    {
        if (timer >= fadeDuration)
        {
            audioSource.volume = targetVolume;
            return;
        }

        timer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / fadeDuration);
        audioSource.volume = Mathf.Lerp(0f, targetVolume, t);
    }
}
