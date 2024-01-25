using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    public static FMODEvents instance { get; private set; }

    [field: Header("Menu SFX")]

    [field: Header("Music")]

    [field: Header("Clown SFX")]

    [field: SerializeField] public EventReference bigClownLaugh { get; private set; }
    [field: SerializeField] public EventReference clownLaugh { get; private set; }
    [field: SerializeField] public EventReference clownGiggle { get; private set; }
    [field: SerializeField] public EventReference clownNotConvinced { get; private set; }

    [field: Header("UI SFX")]

    [field: SerializeField] public EventReference cardDraw { get; private set; }
    [field: SerializeField] public EventReference cardPlacing { get; private set; }
    [field: SerializeField] public EventReference cardLiftUp { get; private set; }
    [field: SerializeField] public EventReference balloonPop { get; private set; }
    [field: SerializeField] public EventReference laughometerFull { get; private set; }
    [field: SerializeField] public EventReference laughometerMid { get; private set; }
    [field: SerializeField] public EventReference lightExplode { get; private set; }


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene");
        }
        instance = this;
    }
}
