using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFightable
{
    public void OnDeath();
    public FightModule GetFightModule();
    public Vector2Int GetPosition();
}
