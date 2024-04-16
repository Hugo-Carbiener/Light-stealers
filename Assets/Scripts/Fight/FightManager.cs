using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightManager : MonoBehaviour
{
    private static FightManager _instance;
    public static FightManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<FightManager>();
            }

            return _instance;
        }
    }

    [Header("Fight cooldowns")]
    [SerializeField] private float setupDuration;
    [SerializeField] private float actionCooldown;

    public List<Fight> fights { get; private set; }

    private void Awake()
    {
        fights = new List<Fight>();
    }

    public float getSetupDuration() { return setupDuration; }
}
