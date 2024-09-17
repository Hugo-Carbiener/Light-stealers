using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/**
 * IFightable module watching over everything related to the fight.
 */
public class FightModule : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private IFightable actor;
    [SerializeField] private Factions faction;
    [SerializeField] private bool attackable;

    [Header("Fight values")]
    [SerializeField] private float turnDuration;
    public BoundCounter health { get; private set; }
    [SerializeField] private int maxHealth;
    [SerializeField] private int attack;

    public UnityEvent<FightModule> OnAttack {get; private set; } = new UnityEvent<FightModule>();
    public UnityEvent<int> OnDamaged { get; private set; } = new UnityEvent<int>();
    public UnityEvent OnDeath { get; private set; } = new UnityEvent();
    public UnityEvent OnFlee { get; private set; } = new UnityEvent();


    private void Awake()
    {
        if (actor == null) actor = (IFightable) gameObject.GetComponent(typeof(IFightable));
        health = new BoundCounter(maxHealth, maxHealth);
        health.OnMinValueReachedOrExceeded.AddListener(actor.OnDeath);
    }

    private void Start()
    {
        turnDuration = Math.Max(FightManager.Instance.getMinFightTurnDuration(), turnDuration);
    }

    /**
     * Starts a fight on a tile if there is an attackable opponent or joins an existing fight.
     */
    public bool Attack(Vector2Int location, Task attackTask)
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
            FightManager.Instance.StartFight(teams, fightCell, attackTask);
            Debug.Log(this.gameObject.name + " started a fight on " + fightCell.coordinates + " with " + teams);
            return true;
        }

        // join an existing fight
        fightCell.fight.AddFighter(this);
        Debug.Log(this.gameObject.name + " joined a fight on " + fightCell.coordinates);
        return true;
    }
    
    private void Attack(FightModule enemyFighter)
    {
        // execute attack
        enemyFighter.health -= attack;
        // events
        this.OnAttack.Invoke(enemyFighter);
        enemyFighter.OnDamaged.Invoke(attack);

        Debug.Log("FIGHT : " + this.gameObject.name + " attacks " + enemyFighter.gameObject.name + " for " + attack + "hp");
    }

    /**
     * Plays the turn of a IFightable. If the fighter is still alive, they inflict their attack to a random ennemy and stall the fight for a short duration.
     */
    public IEnumerator PlayTurn(Team ennemyTeam)
    {
        if (!IsAlive()) yield break;

        System.Random random = new System.Random();
        List<FightModule> validTargets = ennemyTeam.fighters.Where(fighter => fighter.IsValidTarget()).ToList();
        if (validTargets.Count == 0) yield break;

        int randomIndex = random.Next(validTargets.Count);
        FightModule ennemyFighter = validTargets[randomIndex];
        Attack(ennemyFighter);
        Debug.Log("FIGHT : " + this.gameObject.name + " (" + health + ") attacked " + ennemyFighter.gameObject.name + " (" + health + "hp)");

        float timer = 0;
        while (timer < turnDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
    }

    public bool IsAttackable() { return attackable; }
    public IFightable GetActor() { return actor;  }
    public Factions GetFaction() { return faction; }

    public bool IsAlive()
    {
        if (health.IsMined()) Debug.Log("FIGHT : " + this.gameObject.name + " was unalived, skipping their turn. (" + health + "hp)");
        return !health.IsMined();
    }

    public bool IsValidTarget()
    {
        return IsAlive();
    }
}
