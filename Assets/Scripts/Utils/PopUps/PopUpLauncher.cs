using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpLauncher : MonoBehaviour
{
    public static void LaunchPopUp(MonoBehaviour monoInstance, PopUpIndicator popup, string text, Vector3 anchor)
    {
        popup.Init(text, anchor);
        GameObject prefab = popup.getPrefab();
        GameObject popupInstance = Instantiate(prefab);
        monoInstance.StartCoroutine(popup.LaunchPopUp(popupInstance, anchor));
    }
}
