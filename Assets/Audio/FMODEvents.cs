using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    public static FMODEvents instance { get; private set; }

    [field: Header("Menu SFX")]
    [field: SerializeField] public EventReference testSFX { get; private set; } 

    [field: Header("Music")]
    [field: SerializeField] public EventReference testMusic { get; private set; }

    [field: Header("Clown SFX")]



    [field: Header("UI SFX")]


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene");
        }
        instance = this;
    }
}
