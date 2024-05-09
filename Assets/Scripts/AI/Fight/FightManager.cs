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

    public List<Fight> fights { get; private set; }

    private void Awake()
    {
        fights = new List<Fight>();
    }

    public void StartFight(List<Team> teams, CellData fightCell)
    {
        Fight fight = new Fight(teams);
        fightCell.fight = fight;
        MainMenuUIManager.Instance.UpdateUIComponent();
        BuildingUIManager.Instance.UpdateVisibility();
        FightManager.Instance.fights.Add(fight);
    }
    public float getSetupDuration() { return setupDuration; }
}
