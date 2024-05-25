using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehavior : Behavior
{
    public IdleBehavior(AIAgent agent) : base(agent)
    {
    }

    protected override void ExecuteAction(Vector2Int targetCell)
    {
        throw new System.NotImplementedException();
    }

    protected override void InitAction(Vector2Int targetCell)
    {
        throw new System.NotImplementedException();
    }
}
