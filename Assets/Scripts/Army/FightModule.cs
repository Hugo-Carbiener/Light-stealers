using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightModule : MonoBehaviour
{
    [SerializeField] private bool attackable;

    public bool IsAttackable() { return attackable; }
}
