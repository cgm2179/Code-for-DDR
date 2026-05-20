using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    public float beat;
    public Lane lane;
    public Conductor conductor;
    public float beatsInAdvance;
    public float durationInBeats;

    public Vector2 spawnPos;
    public Vector2 hitPos;
    public Vector2 removePos;

    public bool isHit = false;
    public bool isHolding = false;
    public bool completed = false;
    private bool releaseJudged = false;

    public float endBeat;

    public float tickInterval = 0.25f;
    private float nextTickBeat;

    public Transform tail;

    SpriteRenderer sr;

    public void Initialize(float beat, Lane lane, Conductor conductor, float beatsInAdvance, float durationInBeats)
    {
        this.beat = beat;
        this.lane = lane;
        this.conductor = conductor;
        this.beatsInAdvance = beatsInAdvance;
        this.durationInBeats = durationInBeats;

        spawnPos = lane.spawnPoint.position;
        hitPos = lane.hitPoint.position;
        removePos = lane.removePoint.position;

        transform.position = spawnPos;

        endBeat = beat + durationInBeats;

        sr = GetComponentInChildren<SpriteRenderer>();
        sr.color = lane.laneColor;

        if (durationInBeats > 0 && tail != null)
        {
            tail.localScale = new Vector3(tail.localScale.x, 0f, 1f);
        }

        SetArrowDirection();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SetArrowDirection()
    {
        float rotation = 0f;

        switch (lane.laneIndex)
        {
            case 0: rotation = 90f; break; // Left
            case 1: rotation = 0f; break;  // Up
            case 2: rotation = 180f; break; // Down
            case 3: rotation = 270f; break;   // Right
        }

        transform.rotation = Quaternion.Euler(0, 0, rotation);
    }

    // Update is called once per frame
    void Update()
    {
        if (completed) return;

        float songBeat = conductor.songPositionInBeats;
        float beatsUntilHit = beat - songBeat;

        // Move head
        Vector2 headPos = spawnPos + (hitPos - spawnPos) * (1f - beatsUntilHit / beatsInAdvance);
        transform.position = new Vector3(headPos.x, headPos.y, 0f);

        // Move and grow tail for hold notes
        if (durationInBeats > 0 && tail != null)
        {
            // Full tail length based on duration in beats
            float fullTailLength = durationInBeats * lane.unitsPerBeat;

            // Current head position
            headPos = transform.position;

            // Distance from head to spawn
            float distanceToSpawn = headPos.y - spawnPos.y; // negative if head below spawn
            float tailLength = Mathf.Min(fullTailLength, Mathf.Abs(distanceToSpawn));

            // Update tail scale (Y-axis)
            tail.localScale = new Vector3(tail.localScale.x, tailLength, 1f);

            // Position tail: center between head and spawn
            Vector2 tailPos = new Vector2(headPos.x, headPos.y + tailLength / 2f);
            tail.position = new Vector3(tailPos.x, tailPos.y, 0f);
        }

        if (isHolding && !releaseJudged)
        {
            while (songBeat >= nextTickBeat && nextTickBeat < endBeat)
            {
                lane.HoldTick(this);
                nextTickBeat += tickInterval;
            }
        }

        // hold notes complete if reach endBeat
        if (isHolding && songBeat >= endBeat && !releaseJudged)
        {
            releaseJudged = true;
            lane.ReleaseJudgement(this);
            Destroy(gameObject);
        }
    }


    public void Hit()
    {
        isHit = true;

        if (durationInBeats > 0)
        {
            isHolding = true;
            nextTickBeat = beat + tickInterval;
        }
        else
            Destroy(gameObject);
    }

    public void ReleaseEarly()
    {
        if (!isHolding || releaseJudged)
            return;

        releaseJudged = true;
        lane.ReleaseJudgement(this);
        Destroy(gameObject);
    }
}
