using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField, Range(0f, 1f)] private float targetVolume = 1f;

    [Header("Fades")]
    [SerializeField] private float fadeOutDuration = 1.5f;
    [SerializeField] private float fadeInDuration = 1.5f;

    [Header("Outdoor Day Music")]
    [SerializeField] private AudioClip[] daytimeTracks;

    [Header("Outdoor Night Music")]
    [SerializeField] private AudioClip[] nighttimeTracks;

    [Header("Home Base Scene Names")]
    [SerializeField] private string[] homeBaseSceneNames;

    [Header("Home Base Day Music")]
    [SerializeField] private AudioClip[] homeBaseDaytimeTracks;

    [Header("Home Base Night Music")]
    [SerializeField] private AudioClip[] homeBaseNighttimeTracks;

    [Header("Zombie Music")]
    [SerializeField] private AudioClip[] zombieTracks;

    private DayNightCycle dayNightCycle;
    private int activeZombieCount;
    private bool wasNight;
    private bool isInHomeBase;
    private bool isTransitioning;
    private Coroutine transitionRoutine;
    private AudioClip[] currentPlaylist;

    // the last clip played from each playlist
    private readonly Dictionary<AudioClip[], AudioClip> lastClipByPlaylist =
        new Dictionary<AudioClip[], AudioClip>();

    // tracks not yet played in the current "round" of each playlist
    private readonly Dictionary<AudioClip[], List<AudioClip>> remainingByPlaylist =
        new Dictionary<AudioClip[], List<AudioClip>>();

    //function: Awake
    //purpose: this function keeps one MusicManager active while scenes change
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            // disable first so Start/Update never run on the duplicate before it is destroyed
            enabled = false;
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    //function: OnDestroy
    //purpose: this function clears the static reference when the real manager is destroyed
    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    //function: Start
    //purpose: this function sets up the audio source and starts the correct music
    private void Start()
    {
        if (musicSource == null)
        {
            musicSource = GetComponent<AudioSource>();
        }

        if (musicSource == null)
        {
            Debug.LogError("MusicManager needs an AudioSource.", this);
            enabled = false;
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;

        musicSource.loop = false;
        UpdateDayNightReference();

        wasNight = dayNightCycle != null && dayNightCycle.IsNight;
        UpdateHomeBaseState(SceneManager.GetActiveScene().name);

        PlayAmbientMusic();
    }

    //function: OnDisable
    //purpose: this function removes the scene event and clears transition state
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (transitionRoutine != null)
        {
            StopCoroutine(transitionRoutine);
        }

        transitionRoutine = null;
        isTransitioning = false;
    }

    //function: OnSceneLoaded
    //purpose: this function updates the music when a new scene finishes loading
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        activeZombieCount = 0;

        UpdateDayNightReference();
        UpdateHomeBaseState(scene.name);

        wasNight = dayNightCycle != null && dayNightCycle.IsNight;

        // only change music if the playlist is different, so the track keeps playing between similar scenes
        RefreshAmbientMusic();
    }

    //function: UpdateDayNightReference
    //purpose: this function finds the DayNightCycle component in the newly loaded scene
    private void UpdateDayNightReference()
    {
        dayNightCycle = FindFirstObjectByType<DayNightCycle>();

        if (dayNightCycle == null)
        {
            Debug.LogWarning("MusicManager: no DayNightCycle in this scene, so day music will be used.", this);
        }
    }

    //function: UpdateHomeBaseState
    //purpose: this function checks whether the loaded scene is a home base scene (ignores capitalization)
    private void UpdateHomeBaseState(string sceneName)
    {
        isInHomeBase = false;

        if (homeBaseSceneNames == null)
        {
            return;
        }

        foreach (string homeBaseSceneName in homeBaseSceneNames)
        {
            if (string.Equals(
                    sceneName,
                    homeBaseSceneName != null ? homeBaseSceneName.Trim() : null,
                    StringComparison.OrdinalIgnoreCase))
            {
                isInHomeBase = true;
                break;
            }
        }
    }

    //function: Update
    //purpose: this function changes music when the time changes or a track ends
    private void Update()
    {
        if (dayNightCycle != null)
        {
            bool isNight = dayNightCycle.IsNight;

            if (isNight != wasNight)
            {
                wasNight = isNight;

                if (activeZombieCount == 0)
                {
                    PlayAmbientMusic();
                }
            }
        }

        if (isTransitioning || AudioListener.pause || !Application.isFocused)
        {
            return;
        }

        if (!musicSource.isPlaying)
        {
            PlayNextTrack();
        }
    }

    //function: GetAmbientPlaylist
    //purpose: this function returns the outdoor or home base playlist that matches the scene and time
    private AudioClip[] GetAmbientPlaylist()
    {
        bool isNight = dayNightCycle != null && dayNightCycle.IsNight;

        if (isInHomeBase)
        {
            return isNight ? homeBaseNighttimeTracks : homeBaseDaytimeTracks;
        }

        return isNight ? nighttimeTracks : daytimeTracks;
    }

    //function: PlayAmbientMusic
    //purpose: this function always picks a new track from the correct ambient playlist
    private void PlayAmbientMusic()
    {
        PlayRandomTrack(GetAmbientPlaylist());
    }

    //function: RefreshAmbientMusic
    //purpose: this function switches music only if the needed playlist is different from the one playing
    private void RefreshAmbientMusic()
    {
        AudioClip[] targetPlaylist = GetAmbientPlaylist();

        if (targetPlaylist == currentPlaylist && (musicSource.isPlaying || isTransitioning))
        {
            return;
        }

        PlayRandomTrack(targetPlaylist);
    }

    //function: PlayNextTrack
    //purpose: this function chooses the next track based on the current game situation
    private void PlayNextTrack()
    {
        if (activeZombieCount > 0)
        {
            PlayRandomTrack(zombieTracks);
        }
        else
        {
            PlayAmbientMusic();
        }
    }

    //function: GetNextShuffledTrack
    //purpose: this function plays every track in a playlist once before any track repeats
    private AudioClip GetNextShuffledTrack(AudioClip[] tracks)
    {
        if (!remainingByPlaylist.TryGetValue(tracks, out List<AudioClip> remaining))
        {
            remaining = new List<AudioClip>();
            remainingByPlaylist[tracks] = remaining;
        }

        // all tracks have played, so start a new round with every track again
        if (remaining.Count == 0)
        {
            foreach (AudioClip clip in tracks)
            {
                if (clip != null)
                {
                    remaining.Add(clip);
                }
            }
        }

        if (remaining.Count == 0)
        {
            return null;
        }

        lastClipByPlaylist.TryGetValue(tracks, out AudioClip lastClip);

        int index = UnityEngine.Random.Range(0, remaining.Count);

        // after a refill, don't let the new round start with the track that just played
        if (remaining.Count > 1 && remaining[index] == lastClip)
        {
            index = (index + 1) % remaining.Count;
        }

        AudioClip selected = remaining[index];
        remaining.RemoveAt(index);
        lastClipByPlaylist[tracks] = selected;

        return selected;
    }

    //function: PlayRandomTrack
    //purpose: this function picks the next shuffled track and fades to it
    private void PlayRandomTrack(AudioClip[] tracks)
    {
        if (tracks == null || tracks.Length == 0)
        {
            return;
        }

        AudioClip selectedTrack = GetNextShuffledTrack(tracks);

        if (selectedTrack == null)
        {
            return;
        }

        currentPlaylist = tracks;

        if (transitionRoutine != null)
        {
            StopCoroutine(transitionRoutine);
        }

        transitionRoutine = StartCoroutine(FadeToTrack(selectedTrack));
    }

    //function: FadeToTrack
    //purpose: this function fades out the current track and fades in the new track
    private IEnumerator FadeToTrack(AudioClip newClip)
    {
        isTransitioning = true;

        if (musicSource.isPlaying)
        {
            yield return FadeVolume(
                musicSource.volume,
                0f,
                fadeOutDuration
            );
        }

        musicSource.Stop();
        musicSource.clip = newClip;
        musicSource.loop = false;
        musicSource.volume = 0f;
        musicSource.Play();

        yield return FadeVolume(
            0f,
            targetVolume,
            fadeInDuration
        );

        transitionRoutine = null;
        isTransitioning = false;
    }

    //function: FadeVolume
    //purpose: this function smoothly changes the music volume
    private IEnumerator FadeVolume(float from, float to, float duration)
    {
        if (duration <= 0f)
        {
            musicSource.volume = to;
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(
                from,
                to,
                elapsed / duration
            );

            yield return null;
        }

        musicSource.volume = to;
    }

    //function: ZombieSpawned
    //purpose: this function begins zombie music when a zombie appears
    public void ZombieSpawned()
    {
        activeZombieCount++;

        if (activeZombieCount == 1)
        {
            PlayRandomTrack(zombieTracks);
        }
    }

    //function: ZombieDefeated
    //purpose: this function returns to the correct ambient music after zombies are gone
    public void ZombieDefeated()
    {
        activeZombieCount = Mathf.Max(
            0,
            activeZombieCount - 1
        );

        if (activeZombieCount == 0)
        {
            PlayAmbientMusic();
        }
    }
}