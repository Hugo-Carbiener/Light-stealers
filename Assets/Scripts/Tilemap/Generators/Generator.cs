using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


/**
 * A Generator is a class inducing changes on a given tilemap to generate pieces of landscape or structures
 */
[System.Serializable]
public abstract class Generator : ScriptableObject {
    public bool initialized { get; protected set; } = false;
    public void Generate()
    {
        Initialize();
        if (!IsInitialized()) return;
        GenerateElement();
    }

    protected abstract void GenerateElement();

    public abstract void Initialize();

    public bool IsInitialized()
    {
        if (!initialized)
        {
            Debug.LogError("Attempting to generate using an uninitialized generator : " + this.ToString());
            return false;
        }
        return true;
    }
}
