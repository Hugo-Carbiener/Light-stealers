using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightModule : MonoBehaviour
{
    [SerializeField] private bool attackable;
    [SerializeField] private Factions faction;
    public bool IsAttackable() { return attackable; }
    public Factions GetFaction() { return faction; }

    /**
     * 
     */
    public bool Attack(Vector2Int location)
    {
        CellData fightCell = TilemapManager.Instance.GetCellData(location);
        if (fightCell == null)
        {
            Debug.LogError(string.Format($"{gameObject.name} attemping to start a fight on null cell ({location.x}, {location.y})"));
            return false;
        }



        if (fightCell.fight == null)
        {
            fightCell.fight = new Fight();
        }

        fightCell.fight.belligerents.Add(this);
        return true;
    }
}
