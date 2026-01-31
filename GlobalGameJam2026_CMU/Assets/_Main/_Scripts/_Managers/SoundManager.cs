using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
//using System.Diagnostics;

public class SoundManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static SoundManager sm;

    public Transform playerTransform;

    public AudioClip ambientNoise;

    public AudioClip bkgMusic;
    public List<AudioClip> bkgMusicList;


    public AudioClip ending;
    private int musicListSize = 0;
    private int currPlaying = 0;
    private List<AudioSource> tracks = new List<AudioSource>();

    private GameObject currentlyPlaying;

    void Awake()
    {
        if (SoundManager.sm)
        {
            //Might need to become this.GameObject() ?
            Destroy(this);
        }
        else
        {
            sm = this;
        }


    }


    void Start()
    {
        //Uncomment for ambient noise
        //PlaySoundEffect(ambientNoise, transform.position, true);        

        //Uncomment for background music
        currentlyPlaying = PlaySoundEffect(bkgMusic, transform.position, true, false);


        //PlayMultipleTracks(bkgMusicList);
    }

    // Update is called once per frame
    void Update()
    {

    }

    /**
    * Plays a sound effect at a specified position with customizable playback options.
    *
    * @param clip   The audio clip to play.
    * @param pos    The world position where the sound will be played.
    * @param loop   Whether the sound should loop continuously (default: false).
    * @param spatial If true, enables 3D spatial audio (sound depends on listener position).
    * @param volume Playback volume (default: 1.0f).
    *
    * @return The GameObject created to play the sound effect. 
    *         If looping is disabled, it is automatically destroyed after playback.
    */
    public GameObject PlaySoundEffect(AudioClip clip, Vector3 pos, bool loop = false, bool spatial = false, float volume = 1.0f)
    {
        GameObject soundEffect = new GameObject("soundEffect");
        if (spatial)
        {
            soundEffect.transform.position = normalizePosition(pos);
        }


        AudioSource source = soundEffect.AddComponent<AudioSource>();
        source.clip = clip;
        source.spatialBlend = spatial ? 1f : 0f;
        source.volume = volume;

        source.Play();

        if (loop)
            source.loop = true;
        else
            Destroy(soundEffect, source.clip.length + 1f);

        return soundEffect;
    }

    /**
    * Plays a random sound effect from a list of audio clips, using the same parameters
    * as PlaySoundEffect().
    *
    * @param audioClips List of possible audio clips to play from.
    * @param pos        The world position where the sound will be played.
    * @param loop       Whether the sound should loop (default: false).
    * @param spatial    Whether to use 3D spatial sound (default: false).
    * @param volume     Playback volume (default: 1.0f).
    */
    public void PlayRandomSoundEffect(List<AudioClip> audioClips, Vector3 pos, bool loop = false, bool spatial = false, float volume = 1.0f)
    {
        if (audioClips == null || audioClips.Count == 0)
        {
            Debug.LogWarning("PlayRandomSoundEffect: No audio clips provided.");
            return;
        }

        AudioClip randomClip = audioClips[UnityEngine.Random.Range(0, audioClips.Count)];
        PlaySoundEffect(randomClip, pos, loop, spatial, volume);
    }



    public void changeMusic(AudioClip clip)
    {
        Destroy(currentlyPlaying);
        currentlyPlaying = PlaySoundEffect(clip, transform.position, true, false, 0.8f);
    }



    public void PlayMultipleTracks(List<AudioClip> clips)
    {
        musicListSize = 0;
        currPlaying = 0;
        foreach (AudioClip clip in clips)
        {
            //Debug.Log("YO");
            //AudioSource source = PlaySoundEffect(clip, true);
            //tracks.Add(source);
            musicListSize++;
            //source.volume = 0f;
        }
        tracks[currPlaying].volume = 1f;
        UnityEngine.Debug.Log(tracks.Count);
    }

    public void playNextTrack()
    {
        tracks[currPlaying].volume = 0f;
        currPlaying++;
        if (currPlaying >= musicListSize)
        {
            //PlaySoundEffect(ending);
            return;
        }
        tracks[currPlaying].volume = 1f;

    }


    private Vector3 normalizePosition(Vector3 oldPosition)
    {
        Vector3 dir = oldPosition - playerTransform.transform.position;

        Vector3 closerPos = playerTransform.transform.position + dir * 0.015f;

        closerPos.y = playerTransform.transform.position.y;
        Debug.Log("Sound location: " + closerPos);
        return closerPos;
    }


    // public GameObject CastSunSound(Vector3 position)
    // {
    //     return PlaySoundEffect(castSun, position, false, true);
    // }

}
