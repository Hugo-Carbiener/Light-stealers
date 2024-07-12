using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Parent class for behaviors. A behavior holds the logic for executing each task. Each behavior decomposes tasks into a movement and an action.
 */ 
public abstract class Behavior
{
    protected AIAgent agent;

    protected Behavior(AIAgent agent)
    {
        this.agent = agent;
    }

    public void StartBehavior(Task task, Unit unit)
    {
        if (InitMovement(task, unit))
        {
            ExecuteMovement(task, unit);
        } else
        {
            agent.EndTask();
        }
    }

    protected virtual bool InitMovement(Task task, Unit unit)
    {
        // check if the destination is accessible
        unit.GetMovementModule().SetDestination(task.location);
        unit.GetMovementModule().OnArrivalEvent.AddListener(InitAction);
        return true;
    }

    protected void ExecuteMovement(Task task, Unit unit)
    {
        unit.GetMovementModule().StartMovement();
    }
    protected abstract void InitAction(Vector2Int targetCell);
    protected abstract void ExecuteAction(Vector2Int targetCell);
}
