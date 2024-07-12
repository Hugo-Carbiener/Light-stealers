using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FractureManager : MonoBehaviour
{
    private static FractureManager _instance;
    public static FractureManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<FractureManager>();
            }

            return _instance;
        }
    }

    public List<CellData> fractures { get; private set; }

    private void Awake()
    {
        fractures = new List<CellData>();
    }

    public CellData GetRandomFracture()
    {
        System.Random rd = new System.Random();
        return fractures[rd.Next(fractures.Count)];
    }
}
