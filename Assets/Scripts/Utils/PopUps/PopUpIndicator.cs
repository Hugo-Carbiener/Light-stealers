using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu]
public class PopUpIndicator : ScriptableObject
{
    [Header("Components")]
    [SerializeField] private GameObject componentPrefab;
    [SerializeField] private TextMeshPro text;
    [Header("Durations")]
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private float lifeDuration;
    [Header("Position")]
    [SerializeField] private Vector3 relativeStartPosition;
    [SerializeField] private Vector3 relateiveEndPosition;
    [Header("Text")]
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private int startFontSize;
    [SerializeField] private int endFontSize;

    public GameObject getPrefab() { return componentPrefab;  }

    public void Init(string text, Vector3 anchor)
    {
        this.text.text = text;
        this.text.color = new Color(startColor.r, startColor.g, startColor.b, 0);
        this.text.fontSize = startFontSize;
        componentPrefab.transform.position = anchor + relativeStartPosition;
    }

    public IEnumerator LaunchPopUp(GameObject popupInstance, Vector3 anchor)
    {
        float timer = 0;
        float fadeOutTime = fadeInDuration + lifeDuration;
        float totalTime = fadeInDuration + lifeDuration + fadeOutDuration;
        Color newColor = Color.Lerp(startColor, endColor, timer / totalTime);
        float fontSize = Mathf.Lerp(startFontSize, endFontSize, timer / totalTime);
        Vector3 relativePosition = Vector3.Lerp(relativeStartPosition, relateiveEndPosition, timer / totalTime);

        if (timer < fadeInDuration)
        {
            float progress = timer / fadeInDuration;
            float alpha = Mathf.Lerp(0, 255, progress);
            newColor.a = alpha;
        }

        if (timer > fadeOutTime && timer < totalTime)
        {
            float progress = (timer - fadeInDuration - lifeDuration) / fadeOutDuration;
            float alpha = Mathf.Lerp(255, 0, progress);
            newColor.a = alpha;
        }

        this.text.fontSize = fontSize;
        this.text.color = newColor;
        popupInstance.transform.position = anchor + relativePosition;
        timer += Time.deltaTime;

        if (timer >= totalTime)
        {
            Destroy(popupInstance);
            yield break;
        }
        yield return null;
    }
}
