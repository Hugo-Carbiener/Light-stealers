using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeBehavior : Behavior
{

    public FleeBehavior(AIAgent agent) : base(agent) {}

    protected override void ExecuteAction(Vector2Int targetCell)
    {
        UnitManager.Instance.DeactivateUnit(agent.GetUnit());
    }

    protected override void InitAction(Vector2Int targetCell)
    {
        ExecuteAction(targetCell);
    }
}
