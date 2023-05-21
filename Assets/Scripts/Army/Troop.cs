using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Basic component for game agents
 */
public class Troop : MonoBehaviour
{

    private void die(Troop troop)
    {
        troop.gameObject.SetActive(false);
        ArmyManager.Instance.armySize--;
    }
}
