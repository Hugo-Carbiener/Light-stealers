using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Fight : MonoBehaviour
{
    public Dictionary<Factions, Team> teams { get; private set; }

    public Status status { get; private set; }

    private void Update()
    {
        
    }

    public Fight(List<Team> teams)
    {
        status = Status.Pending;
        foreach(Team team in teams)
        {
            this.teams.Add(team.faction, team);
        }
        Init();
    }

    public void Init()
    {
        StartCoroutine("SetupCoroutine");
    }

    private IEnumerator SetupCoroutine()
    {
        float timer = 0;
        float setupDuration = FightManager.Instance.getSetupDuration();
        while (timer < setupDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        status = Status.InProgress;
        StartCoroutine("MainLoopCoroutine");
    }

    private IEnumerator MainLoopCoroutine()
    {
        Factions currentFaction = Extensions.RandomValue<Factions>();
        int turnCount = 0;
        while (TeamsAreAlive())
        {
            for (int i = 0; i < Enum.GetValues(typeof(Factions)).Length; i++)
            {
                Team currentTeam = teams[currentFaction];
                currentTeam.PlayTurn(teams[currentFaction.Next()]);
                currentFaction = currentFaction.Next();
            }
        }
    }

    public void AddFighter(FightModule fighter)
    {
        teams[fighter.GetFaction()].AddFighter(fighter);
    }

    private bool TeamsAreAlive()
    {
        return teams.Values.All(team => team.IsAlive());
    }
}
