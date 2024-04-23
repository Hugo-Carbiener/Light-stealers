using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FightModule : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private bool attackable;
    [SerializeField] private Factions faction;

    [Header("Fight values")]
    [SerializeField] private float turnDuration;
    public int health { get; private set; }
    [SerializeField] private int maxHealth;
    [SerializeField] private int attack;
    public bool IsAttackable() { return attackable; }
    public Factions GetFaction() { return faction; }

    private void Awake()
    {
        health = maxHealth;
    }

    /**
     * Starts a fight on a tile if there is an attackable opponent or joins an existing fight.
     */
    public bool Attack(Vector2Int location)
    {
        CellData fightCell = TilemapManager.Instance.GetCellData(location);
        if (fightCell == null)
        {
            Debug.LogError(string.Format($"{gameObject.name} attemping to start a fight on null cell ({location.x}, {location.y})"));
            return false;
        }

        List<FightModule> belligerentsOnCell = FightUtils.GetBelligerentsOnCell(location);
        List<FightModule> ennemiesOnCell = belligerentsOnCell.Where(fighter => fighter.faction != faction).ToList();

        if (ennemiesOnCell == null || ennemiesOnCell.Count == 0)
        {
            Debug.LogError(string.Format($"{gameObject.name} attemping to start a fight on cell with no opponents"));
            return false;
        }

        // create a new fight if no other exists
        if (fightCell.fight == null)
        {
            List<Team> teams = new List<Team>();
            foreach (Factions faction in Factions.GetValues(typeof(Factions)))
            {
                teams.Add(new Team(faction, belligerentsOnCell.Where(fighter => fighter.faction == faction).ToList()));
            }
            Fight fight = new Fight(teams);
            fightCell.fight = fight;
            FightManager.Instance.fights.Add(fight);
            return true;
        }

        // join an existing fight
        fightCell.fight.AddFighter(this);
        return true;
    }

    private void Attack(FightModule ennemyFighter)
    {
        ennemyFighter.health -= attack;
    }

    public IEnumerator PlayTurn(Team ennemyTeam)
    {
        if (!IsAlive()) yield return null;

        System.Random random = new System.Random();
        int randomIndex = random.Next(ennemyTeam.fighters.Count());
        FightModule ennemyFighter = ennemyTeam.fighters[randomIndex];
        Attack(ennemyFighter);

        float timer = 0;
        while (timer < turnDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public bool IsAlive()
    {
        return health > 0;
    }
}
