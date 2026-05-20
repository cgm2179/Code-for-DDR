using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

public class JudgementManager : MonoBehaviour
{
    public static JudgementManager Instance;


    public static event Action<JudgementType> OnJudgement;
    public static event Action<int> OnComboChanged;

    public float perfectWindow = 0.1f;
    public float greatWindow = 0.2f;
    public float goodWindow = 0.4f;

    public int score = 0;
    public int combo = 0;
    public int maxCombo = 0;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void JudgeNote(Note note, float timingDifference)
    {
        JudgementType result;

        if (Mathf.Abs(timingDifference) <= perfectWindow)
        {
            result = JudgementType.Perfect;
            score += 30;
            combo++;
            if (combo > maxCombo)
                maxCombo = combo;
        }

        else if (Mathf.Abs(timingDifference) <= greatWindow)
        {
            result = JudgementType.Great;
            score += 10;
            combo++;
            if (combo > maxCombo)
                maxCombo = combo;
        }

        else if (Mathf.Abs(timingDifference) <= goodWindow)
        {
            result = JudgementType.Good;
            score += 5;
            combo++;
            if (combo > maxCombo)
                maxCombo = combo;
        }

        else
        {
            result = JudgementType.Miss;
            combo = 0;
        }

        OnJudgement?.Invoke(result);
        OnComboChanged?.Invoke(combo);

        Debug.Log($"{result} | Timing Diff: {timingDifference:F3} | Combo: {combo} | Score: {score}");
    }

    public void HoldMissed(Note note)
    {
        combo = 0;

        OnJudgement?.Invoke(JudgementType.Miss);
        OnComboChanged?.Invoke(combo);
    }

    public void HoldComplete(Note note)
    {
        score += 200;
        combo += 2;

        OnJudgement?.Invoke(JudgementType.HoldComplete);
        OnComboChanged?.Invoke(combo);
    }

    public void HoldTick()
    {
        score += 10;

        OnJudgement?.Invoke(JudgementType.HoldTick);
        OnComboChanged?.Invoke(combo);
    }

    public void Reset()
    {
        score = 0;
        combo = 0;
        maxCombo = 0;
    }
}
