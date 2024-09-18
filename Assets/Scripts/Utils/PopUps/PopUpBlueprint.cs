using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using TMPro;

[CreateAssetMenu]
public class PopUpBlueprint : ScriptableObject
{
    [Header("Commponents")]
    public GameObject popupPrefab;
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

    public void Init(GameObject popUpInstance, string textContent, Vector3 anchor)
    {
        Assert.IsNotNull(popUpInstance);
        TextMeshPro text;
        Assert.IsTrue(popUpInstance.gameObject.TryGetComponent(out text));

        text.text = textContent;
        text.color = new Color(startColor.r, startColor.g, startColor.b, 0);
        text.fontSize = startFontSize;
        popUpInstance.transform.position = Vector3.Scale(Camera.main.ScreenToWorldPoint(PixelPerfectUtils.pixelPerfectToFullScreen(anchor)), new Vector3(1,1,0)) + relativeStartPosition;
        popUpInstance.SetActive(true);
    }

    public IEnumerator LaunchPopUp(GameObject popUpInstance, Vector3 anchor)
    {
        Assert.IsNotNull(popUpInstance);
        TextMeshPro text;
        Assert.IsTrue(popUpInstance.gameObject.TryGetComponent(out text));

        float timer = 0;
        float fadeOutTime = fadeInDuration + lifeDuration;
        float totalTime = fadeInDuration + lifeDuration + fadeOutDuration;

        while (timer < totalTime)
        {
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

            text.fontSize = fontSize;
            text.color = newColor;
            popUpInstance.transform.position = Vector3.Scale(Camera.main.ScreenToWorldPoint(PixelPerfectUtils.pixelPerfectToFullScreen(anchor)), new Vector3(1, 1, 0) + relativeStartPosition);
            timer += Time.deltaTime;
            yield return null;
        }
        popUpInstance.SetActive(false);
    }
}
