using System.Collections;
using System.Collections.Generic;
using UnityEngine.Assertions;
using UnityEngine;
using TMPro;
using DG.Tweening;

[CreateAssetMenu]
public class PopUpBlueprint : ScriptableObject
{
    [Header("Commponents")]
    public GameObject popupPrefab;
    [Header("Durations")]
    [SerializeField] private float delayDuration;
    [SerializeField] private float fadeInDuration;
    [SerializeField] private float fadeOutDuration;
    [SerializeField] private float lifeDuration;
    [Header("Position")]
    [SerializeField] private Vector3 relativeStartPosition;
    [SerializeField] private Vector3 relativeEndPosition;
    [Header("Text")]
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;
    [SerializeField] private float startFontSize;

    public void Init(GameObject popUpInstance, string textContent, Vector3 anchor)
    {
        Assert.IsNotNull(popUpInstance);
        TextMeshPro text;
        Assert.IsTrue(popUpInstance.gameObject.TryGetComponent(out text));

        text.text = textContent;
        text.color = new Color(startColor.r, startColor.g, startColor.b, 0);
        text.fontSize = startFontSize;
        popUpInstance.transform.position = Vector3.Scale(Camera.main.ScreenToWorldPoint(PixelPerfectUtils.pixelPerfectToFullScreen(anchor) + relativeStartPosition), new Vector3(1,1,0));
        popUpInstance.SetActive(true);
    }

    public IEnumerator LaunchPopUp(GameObject popUpInstance, Vector3 anchor)
    {
        Assert.IsNotNull(popUpInstance);
        TextMeshPro text;
        Assert.IsTrue(popUpInstance.gameObject.TryGetComponent(out text));

        float totalTime = fadeInDuration + lifeDuration + fadeOutDuration +  delayDuration;
        Sequence sequence = DOTween.Sequence();
        sequence.PrependInterval(delayDuration);
        sequence.Append(text.DOFade(255, fadeInDuration));
        sequence.Insert(0, text.transform.DOMove(Vector3.Scale(Camera.main.ScreenToWorldPoint(PixelPerfectUtils.pixelPerfectToFullScreen(anchor) + relativeEndPosition), new Vector3(1, 1, 0)), totalTime));
        sequence.Insert(0, text.DOColor(endColor, totalTime));
        sequence.Append(text.DOFade(0, fadeOutDuration));
        sequence.OnComplete(() => popUpInstance.SetActive(false));
        yield return null;
    }
}
