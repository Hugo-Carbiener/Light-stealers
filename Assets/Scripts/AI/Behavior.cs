using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Parent class for behaviors. A behavior holds the logic for executing each task.
 */ 
public abstract class Behavior
{
    public abstract Task assignedTask { get; set; }
    public abstract void AssignNewTask();
}
