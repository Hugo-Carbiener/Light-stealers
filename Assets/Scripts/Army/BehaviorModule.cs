using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Unit module in charge of holding information regarding the current set behavior
 */
public class BehaviorModule : MonoBehaviour
{
    public Behavior currentBehavior { get; private set; } = null;
    public Task currentTask { get; private set; } = null;

    public void StartNewTask(Task task)
    {
        if (currentBehavior == null)
        {
            Debug.LogError("Attempting to assign new task on a unit without behavior : " + gameObject.name);
            return;
        }

        if (!currentBehavior.GetAcceptedTasks().Contains(task.type))
        {
            Debug.LogError(string.Format($"Attempting to assign new task on a unit with wrong behavior : object :{gameObject.name}, task : {task.type}, accepted tasks : {currentBehavior.GetAcceptedTasks().ToString()}"));
            return;
        }

        currentBehavior.InitMovementForTask();
    }
}
