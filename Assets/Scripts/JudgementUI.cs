using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JudgementUI : MonoBehaviour
{
    public TextMeshProUGUI judgementText;
    public float displayTime = 0.5f;

    public bool isActive;

    Coroutine currentRoutine;

    void OnEnable()
    {
        JudgementManager.OnJudgement += ShowJudgement;
    }

    void OnDisable()
    {
        JudgementManager.OnJudgement -= ShowJudgement;
    }

    void ShowJudgement(JudgementType type)
    {
        if (!isActive) return;

        if (type == JudgementType.HoldTick)
            return;

        if (type == JudgementType.HoldComplete)
            return;

        if (currentRoutine != null)
            StopCoroutine(currentRoutine);

        currentRoutine = StartCoroutine(ShowRoutine(type));
    }

    IEnumerator ShowRoutine(JudgementType type)
    {
        judgementText.text = type.ToString();

        // Set color based on judgement
        judgementText.color = GetColor(type);

        float time = 0f;
        float duration = 0.4f;

        while (time < duration)
        {
            time += Time.deltaTime;

            float t = time / duration;

            // scale pop
            float scale = Mathf.Lerp(1.5f, 1f, t);
            judgementText.transform.localScale = Vector3.one * scale;

            // fade out
            judgementText.alpha = 1f - t;

            yield return null;
        }

        judgementText.alpha = 0f;
    }

    Color GetColor(JudgementType type)
    {
        switch (type)
        {
            case JudgementType.Perfect: return Color.yellow;
            case JudgementType.Great: return Color.green;
            case JudgementType.Good: return Color.cyan;
            case JudgementType.Miss: return Color.red;
            default: return Color.white;
        }
    }

    public void ResetUI()
    {
        if (currentRoutine != null)
        {
            StopCoroutine(currentRoutine);
            currentRoutine = null;
        }

        judgementText.text = "";
        judgementText.alpha = 0f;
        judgementText.transform.localScale = Vector3.one;
    }
}
