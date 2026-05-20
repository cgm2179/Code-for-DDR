using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lane : MonoBehaviour
{
    public int laneIndex;

    public Transform spawnPoint;
    public Transform hitPoint;
    public Transform removePoint;

    public Vector2 direction => (spawnPoint.position - hitPoint.position).normalized;

    public float hitWindow = 0.40f;
    public float unitsPerBeat;

    public SpriteRenderer laneRenderer;
    public Color laneColor = Color.grey;

    private List<Note> ActiveNotes = new List<Note>();
    private Note heldNote;

    private Conductor conductor;
    private SongData songData;
    private InputManager input;

    void Start()
    {
        conductor = FindObjectOfType<Conductor>();
        input = InputManager.Instance;
        songData = FindObjectOfType<NoteSpawner>().songData;

        float distance = Vector2.Distance(spawnPoint.position, hitPoint.position);
        unitsPerBeat = distance / songData.beatsShownInAdvance;

        // Set initial lane color
        if (laneRenderer != null)
            laneRenderer.color = Color.gray;
    } 

    void Update()
    {
        HandleInput();
        CheckMisses();
    }

    void HandleInput()
    {
        if (input.GetLaneDown(laneIndex))
        {
            // Flash immediately on press (feels responsive)
            StartCoroutine(Flash());

            TryHit();
        }

        if (input.GetLaneUp(laneIndex) && heldNote != null)
        {
            heldNote.ReleaseEarly();
            heldNote = null;
        }
    }

    void CheckMisses()
    {
        float songBeat = conductor.songPositionInBeats;

        for (int i = ActiveNotes.Count - 1; i >= 0; i--)
        {
            Note note = ActiveNotes[i];

            if (note == null) continue;
            if (note.isHit) continue;

            if (songBeat - note.beat > hitWindow)
            {
                ActiveNotes.RemoveAt(i);

                JudgementManager.Instance.combo = 0;
                JudgementManager.Instance.JudgeNote(note, songBeat - note.beat);

                Destroy(note.gameObject);
            }
        }
    }

    void TryHit()
    {
        if (ActiveNotes.Count == 0) return;
        if (heldNote != null) return;

        for (int i = 0; i < ActiveNotes.Count; i++)
        {
            Note note = ActiveNotes[i];
            float timingDiff = conductor.songPositionInBeats - note.beat;

            if (Mathf.Abs(timingDiff) <= hitWindow)
            {
                ActiveNotes.RemoveAt(i);

                JudgementManager.Instance.JudgeNote(note, timingDiff);
                note.Hit();

                if (note.durationInBeats > 0)
                    heldNote = note;

                break;
            }
        }
    }

    public void Miss()
    {
        if (heldNote != null)
        {
            JudgementManager.Instance.HoldMissed(heldNote);
            heldNote = null;
        }
    }

    public void RegisterNote(Note note)
    {
        ActiveNotes.Add(note);
    }

    public void ReleaseJudgement(Note note)
    {
        float releaseDiff = conductor.songPositionInBeats - note.endBeat;
        JudgementManager.Instance.JudgeNote(note, releaseDiff);
        heldNote = null;
    }

    public void HoldTick(Note note)
    {
        JudgementManager.Instance.HoldTick();
    }

    IEnumerator Flash()
    {
        if (laneRenderer == null)
            yield break;

        float time = 0f;
        float duration = 0.15f;

        Color flashColor = Color.white * 2f;

        Vector3 originalScale = laneRenderer.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // Color fade
            laneRenderer.color = Color.Lerp(flashColor, Color.grey, t);

            // Scale pulse
            laneRenderer.transform.localScale = Vector3.Lerp(targetScale, originalScale, t);

            yield return null;
        }

        // Reset just in case
        laneRenderer.color = Color.grey;
        laneRenderer.transform.localScale = originalScale;
    }
}
