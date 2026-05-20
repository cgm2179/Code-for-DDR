using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public static NoteSpawner Instance;
    public SongData songData;
    public Conductor conductor;
    public LaneManager laneManager;
    public GameObject notePrefab;

    private int nextNoteIndex = 0;

    // Start is called before the first frame update

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (conductor == null)
            conductor = FindObjectOfType<Conductor>();

        if (laneManager == null)
            laneManager = FindObjectOfType<LaneManager>();

        if (songData == null)
            Debug.LogError("SongData not assigned in NoteSpawner!");
        if (notePrefab == null)
            Debug.LogError("NotePrefab not assigned in NoteSpawner!");
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.gameActive)
            return;

        if (Conductor.Instance == null)
            return;

        if (songData == null || laneManager == null || notePrefab == null)
            return;

        if (!Conductor.Instance.musicSource.isPlaying)
            return;

        if (nextNoteIndex >= songData.notes.Length)
            return;

        float songBeat = Conductor.Instance.songPositionInBeats;

        NoteData nextNote = songData.notes[nextNoteIndex];

        if (nextNote.beat <= songBeat + songData.beatsShownInAdvance)
        {
            SpawnNote(nextNote);
            nextNoteIndex++;
        }
    }

    void SpawnNote(NoteData data)
    {
        if (laneManager == null || laneManager.lanes == null)
            return;

        if (data.laneIndex < 0 || data.laneIndex >= laneManager.lanes.Length)
        {
            Debug.LogError("Invalid lane index: " + data.laneIndex);
            return;
        }

        Lane lane = laneManager.lanes[data.laneIndex];

        if (lane == null)
            return;

        GameObject noteObj = Instantiate(
            notePrefab,
            lane.spawnPoint.position,
            Quaternion.identity
        );

        Note note = noteObj.GetComponent<Note>();

        if (note == null)
        {
            Debug.LogError("Note prefab missing Note component!");
            return;
        }

        note.Initialize(
            data.beat,
            lane,
            conductor,
            songData.beatsShownInAdvance,
            data.duration
        );

        lane.RegisterNote(note);
    }

    public void ResetNotes()
    {
        nextNoteIndex = 0;
    }

    public void ClearAllNotes()
    {
        Note[] allNotes = FindObjectsOfType<Note>();

        foreach (var note in allNotes)
        {
            Destroy(note.gameObject);
        }
    }
}
