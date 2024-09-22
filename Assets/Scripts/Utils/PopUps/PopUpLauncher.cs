using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopUpLauncher : MonoBehaviour
{
    private static Dictionary<PopUpBlueprint, List<GameObject>> instantiatedPopUps = new Dictionary<PopUpBlueprint, List<GameObject>>();

    public static void LaunchPopUp(MonoBehaviour monoInstance, PopUpBlueprint blueprint, string text, Vector3 anchor)
    {
        if (!instantiatedPopUps.ContainsKey(blueprint))
        {
            instantiatedPopUps.Add(blueprint, new List<GameObject>());
        }

        GameObject popUpInstance = instantiatedPopUps[blueprint].Find(instance => !instance.activeInHierarchy);
        if (popUpInstance == null)
        {
            popUpInstance = Instantiate(blueprint.popupPrefab);
            instantiatedPopUps[blueprint].Add(popUpInstance);
        }

        blueprint.Init(popUpInstance, text, anchor);
        monoInstance.StartCoroutine(blueprint.LaunchPopUp(popUpInstance, anchor));
    }
}
