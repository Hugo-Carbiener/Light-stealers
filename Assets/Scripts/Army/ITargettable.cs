using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITargettable
{
    public FightModule GetFightModule();
    public Vector2Int GetPosition();
}
