using System.Collections;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public TextMeshProUGUI completeText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;

    public RectTransform spinImage;
    private CanvasGroup spinGroup;

    public GameObject startPanel;
    public GameObject endPanel;

    void Awake()
    {
        Instance = this;
        spinGroup = spinImage.GetComponent<CanvasGroup>();
    }

    public void ShowResults(int score, int maxCombo)
    {
        // ALWAYS reset BEFORE setting text
        ResetUI();

        // set text AFTER reset
        scoreText.text = "Score: " + score;
        comboText.text = "Max Combo: " + maxCombo;

        StartCoroutine(AnimateResults());
    }

    public void ShowStart()
    {
        startPanel.SetActive(true);
        endPanel.SetActive(false);
    }

    public void ShowGame()
    {
        startPanel.SetActive(false);
        endPanel.SetActive(false);
    }

    public void ShowResultsScreen()
    {
        startPanel.SetActive(false);
        endPanel.SetActive(true);
    }

    void ResetUI()
    {
        SetScale(completeText.transform, 0);
        SetScale(scoreText.transform, 0);
        SetScale(comboText.transform, 0);

        completeText.gameObject.SetActive(true);
        scoreText.gameObject.SetActive(true);
        comboText.gameObject.SetActive(true);

        spinImage.localScale = Vector3.one;
        spinImage.rotation = Quaternion.Euler(0, 0, -180f); // start rotated
        spinGroup.alpha = 0f;
    }

    IEnumerator AnimateSpinImage()
    {
        float duration = 8f;
        float t = 0f;

        float startRot = -2520f;
        float endRot = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / duration;

            // ease-out for smoother feel
            float eased = 1 - Mathf.Pow(1 - p, 3);

            float rot = Mathf.Lerp(startRot, endRot, eased);
            spinImage.rotation = Quaternion.Euler(0, 0, rot);

            spinGroup.alpha = eased;

            yield return null;
        }

        spinImage.rotation = Quaternion.identity;
        spinGroup.alpha = 1f;
    }


    IEnumerator AnimateResults()
    {
        // start spin + fade AT SAME TIME as COMPLETE
        StartCoroutine(AnimateSpinImage());

        yield return AnimatePop(completeText.transform);

        yield return new WaitForSecondsRealtime(0.3f);

        yield return AnimatePop(scoreText.transform);

        yield return new WaitForSecondsRealtime(0.2f);

        yield return AnimatePop(comboText.transform);
    }

    IEnumerator AnimatePop(Transform obj)
    {
        float duration = 0.25f;
        float t = 0f;

        Vector3 start = Vector3.zero;
        Vector3 mid = Vector3.one * 1.2f;
        Vector3 end = Vector3.one;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = t / duration;

            if (p < 0.7f)
                obj.localScale = Vector3.Lerp(start, mid, p / 0.7f);
            else
                obj.localScale = Vector3.Lerp(mid, end, (p - 0.7f) / 0.3f);

            yield return null;
        }

        obj.localScale = end;
    }

    void SetScale(Transform t, float s)
    {
        t.localScale = Vector3.one * s;
    }

    public void HideResults()
    {
        completeText.gameObject.SetActive(false);
        scoreText.gameObject.SetActive(false);
        comboText.gameObject.SetActive(false);
    }
}