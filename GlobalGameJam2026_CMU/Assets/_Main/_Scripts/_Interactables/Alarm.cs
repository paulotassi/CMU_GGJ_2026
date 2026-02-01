using UnityEngine;

public class Alarm : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public AudioClip alarmSound;
    void Start()
    {
        SoundManager.sm.PlaySoundEffect(alarmSound, transform.position, true, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
