using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FightModule : MonoBehaviour
{
    [SerializeField] private bool attackable;
    [SerializeField] private Factions faction;
    public bool IsAttackable() { return attackable; }
    public Factions GetFaction() { return faction; }

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

        if (fightCell.fight != null)
        {
            fightCell.fight.belligerents.Add(this);
            return true;
        }

        fightCell.fight = new Fight();
        fightCell.fight.belligerents.Add(this);
        foreach (FightModule fighter in belligerentsOnCell)
        {
            fightCell.fight.belligerents.Add(fighter);
        }
        return true;
    }
}
