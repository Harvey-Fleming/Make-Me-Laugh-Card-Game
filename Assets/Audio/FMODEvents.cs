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

    [field: SerializeField] public EventReference bigClownLaugh { get; private set; }
    [field: SerializeField] public EventReference clownLaugh { get; private set; }
    [field: SerializeField] public EventReference clownGiggle { get; private set; }
    [field: SerializeField] public EventReference clownNotConvinced { get; private set; }

    [field: Header("UI SFX")]

    [field: SerializeField] public EventReference cardShuffling { get; private set; }
    [field: SerializeField] public EventReference cardDraw { get; private set; }
    [field: SerializeField] public EventReference cardPlacing { get; private set; }
    [field: SerializeField] public EventReference cardLiftUp { get; private set; }
    [field: SerializeField] public EventReference cardLiftDown { get; private set; }
    [field: SerializeField] public EventReference balloonPop { get; private set; }
    [field: SerializeField] public EventReference laughometerFull { get; private set; }
    [field: SerializeField] public EventReference laughometerMid { get; private set; }
    [field: SerializeField] public EventReference lightExplode { get; private set; }
    [field: SerializeField] public EventReference lightTurnOff { get; private set; }


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene");
        }
        instance = this;
    }
}
