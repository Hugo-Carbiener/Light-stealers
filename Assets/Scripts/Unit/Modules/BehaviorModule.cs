using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/**
 * Parent class for behaviors. A behavior holds the logic for executing each task. Each behavior decomposes tasks into a movement and an action.
 */ 
public abstract class BehaviorModule : MonoBehaviour
{
    [SerializeField] protected Unit unit;
    [SerializeField] private List<TaskType> acceptedTasks;

    protected Task assignedTask { get; private set; }

    private void Awake()
    {
        Assert.IsTrue(acceptedTasks != null && acceptedTasks.Count > 0);
        Assert.IsTrue(unit != null);
    }

    public void AssignNewTask(Task task)
    {
        if (!acceptedTasks.Contains(task.type))
        {
            Debug.LogError(string.Format($"Attempting to assign wrong task ({task.type}) to behavior {this.name}"));
            return;
        }
        task.status = Status.ToBeProgrammed;
        assignedTask = task;
        if (InitMovement(task))
        {
            ExecuteMovement(task);
        }
    }

    protected bool InitMovement(Task task)
    {
        // check if the destination is accessible

        unit.GetMovementModule().SetDestination(task.location);
        unit.GetMovementModule().OnArrivalEvent.AddListener(InitAction);
        return true;
    }

    protected void ExecuteMovement(Task task)
    {
        unit.GetMovementModule().StartMovement();
    }
    protected abstract void InitAction(Vector2Int targetCell);
    protected abstract void ExecuteAction(Vector2Int targetCell);
    public bool IsIdle() { return assignedTask == null; }
    public virtual bool GeneratesOwnTasks() { return false; }
    public List<TaskType> GetAcceptedTasks() { return acceptedTasks; }

    public void EndTask()
    {
        assignedTask.Finish();
        assignedTask = null;
    }
}
