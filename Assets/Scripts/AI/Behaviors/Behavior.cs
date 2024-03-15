using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Parent class for behaviors. A behavior holds the logic for executing each task.
 */ 
public abstract class Behavior : ScriptableObject
{
    [SerializeField] private List<TaskType> acceptedTasks;
    protected abstract void SetTaskLocation(Task task);
    public bool InitMovementForTask(Task task, MovementModule movementModule)
    {
        SetTaskLocation(task);
        if (task.location == null)
        {
            Debug.LogError(string.Format($"Monster ({movementModule.currentCell}) attempting attack on null cell"));
            return false;
        }

        // check if the destination is accessible

        movementModule.SetDestination(task.location.coordinates);
        movementModule.OnArrival += Execute(task);
        return true;

    }

    public abstract void Execute(Task task);
    public List<TaskType> GetAcceptedTasks() { return acceptedTasks; }
}
