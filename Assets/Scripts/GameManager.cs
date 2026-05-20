using UnityEngine;
using UnityEngine.Video;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject startPanel;
    public GameObject endPanel;

    public Conductor conductor;
    public NoteSpawner spawner;
    public VideoPlayer videoPlayer;

    public static bool gameActive;

    private bool gameStarted = false;

    public bool isPlaying = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        startPanel.SetActive(true);
        endPanel.SetActive(false);

        // pause everything at start
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (InputManager.Instance == null)
            return;

        if (!gameStarted)
        {
            if (InputManager.Instance.GetStartPressed())
            {
                StartGame();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EndGame();
        }

        if (!gameStarted)
        {
            if (InputManager.Instance.GetStartPressed())
            {
                gameStarted = true;
                StartGame();
            }
        }
    }

    public void StartGame()
    {
        gameActive = true;

        FindObjectOfType<ComboUI>().isActive = true;
        FindObjectOfType<JudgementUI>().isActive = true;

        UIManager.Instance.ShowGame();

        gameStarted = true;

        Time.timeScale = 1f;
        isPlaying = true;

        conductor.StartSong();

        videoPlayer.time = 0;
        videoPlayer.Play();
    }

    public void EndGame()
    {
        FindObjectOfType<ComboUI>().isActive = false;
        FindObjectOfType<JudgementUI>().isActive = false;

        conductor.musicSource.Stop();

        isPlaying = false;

        videoPlayer.Stop();

        Time.timeScale = 0f;

        UIManager.Instance.ShowResultsScreen();

        UIManager.Instance.ShowResults(
            JudgementManager.Instance.score,
            JudgementManager.Instance.maxCombo
        );

        StartCoroutine(RestartRoutine());

        gameActive = false;

    }

    public void RestartRun()
    {
        StartCoroutine(RestartRoutine());
    }

    IEnumerator RestartRoutine()
    {
        // show results briefly
        yield return new WaitForSecondsRealtime(15f);

        ResetGameState();

        UIManager.Instance.ShowStart();
    }

    void ResetGameState()
    {
        FindObjectOfType<ComboUI>().ResetUI();
        FindObjectOfType<JudgementUI>().ResetUI();

        JudgementManager.Instance.Reset();

        NoteSpawner.Instance.ClearAllNotes();
        NoteSpawner.Instance.ResetNotes();

        Conductor.Instance.ResetSong();

        UIManager.Instance.HideResults();

        gameStarted = false;
    }
}

