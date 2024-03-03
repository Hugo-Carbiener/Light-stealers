using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorsManager : MonoBehaviour 
{
    private static GeneratorsManager _instance;
    public static GeneratorsManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<GeneratorsManager>();
            }

            return _instance;
        }
    }

    [Header("Generators")]
    [SerializeField] 
    [SerializeReference]
    private List<Generator> generators;

    public void ExecuteGenerator(Generator generator)
    {
        generator.Generate();
    }

    public void ExecuteGenerators()
    {
        foreach (Generator generator in generators)
        {
            generator.Generate();
        }
    }
}
