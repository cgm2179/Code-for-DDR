using UnityEngine;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance;

    public SongData songData;
    public AudioSource musicSource;

    public float secPerBeat;
    public float songPosition;
    public float songPositionInBeats;
    public float dspSongTime;

    private bool songStarted = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // Load audio
        musicSource.clip = songData.audioClip;

        // Calculate beat timing
        secPerBeat = 60f / songData.bpm;
    }

    public void StartSong()
    {
        dspSongTime = (float)AudioSettings.dspTime;

        musicSource.Play();
        songStarted = true;
    }

    void Update()
    {
        if (!songStarted) return;

        // DSP-based timing (very accurate)
        songPosition = (float)(AudioSettings.dspTime - dspSongTime);
        songPositionInBeats = songPosition / secPerBeat;

        // Detect song end
        if (!musicSource.isPlaying)
        {
            GameManager.Instance.EndGame();
            songStarted = false;
        }
    }

    public void ResetSong()
    {
        musicSource.Stop();

        songPosition = 0f;
        songPositionInBeats = 0f;

        dspSongTime = (float)AudioSettings.dspTime;
    }
}