using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Interface implemented by behaviors that generate their own tasks when polled by the TaskManager. This is meant for behaviors working on tasks by default rather than by answering to a need. 
 */
public interface ITaskAutoGeneration 
{
    public Task GenerateTask(Unit unit);
}
