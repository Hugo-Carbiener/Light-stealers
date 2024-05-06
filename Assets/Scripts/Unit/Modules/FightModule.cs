using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class FightModule : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private IFightable actor;
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
        if (actor == null) actor = (IFightable) gameObject.GetComponent(typeof(IFightable));
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

        if (ennemiesOnCell.Count == 0)
        {
            Debug.LogError(string.Format($"{gameObject.name} attemping to start a fight on cell with no opponents"));
            return false;
        }

        // create a new fight if no other exists
        if (fightCell.fight == null)
        {
            List<Team> teams = new List<Team>();
            foreach (Factions currentFaction in Factions.GetValues(typeof(Factions)))
            {
                teams.Add(new Team(currentFaction, belligerentsOnCell.Where(fighter => fighter.faction == currentFaction).ToList()));
            }
            FightManager.Instance.StartFight(teams, fightCell);
            Debug.Log(this.gameObject.name + " started a fight on " + fightCell.coordinates + " with " + teams);
            return true;
        }

        // join an existing fight
        fightCell.fight.AddFighter(this);
        Debug.Log(this.gameObject.name + " joined a fight on " + fightCell.coordinates);
        return true;
    }

    private void Attack(FightModule ennemyFighter)
    {
        Debug.Log("FIGHT : " + this.gameObject.name + " attacks " + ennemyFighter.gameObject.name + " for " + attack + "hp");
        ennemyFighter.health -= attack;
    }

    public IEnumerator PlayTurn(Team ennemyTeam)
    {
        Debug.Log("FIGHT : " + this.gameObject.name + " starts their turn. (" + health + "hp)");

        if (!IsAlive()) yield break;
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

    public void OnFightEnd()
    {
        if (health <= 0)
        {
            actor.OnDeath();
        }
    }

    public bool IsAlive()
    {
        if (health <= 0) Debug.Log("FIGHT : " + this.gameObject.name + " was unalived, skipping their turn. (" + health + "hp)");
        return health > 0;
    }
}
