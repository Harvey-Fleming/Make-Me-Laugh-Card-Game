using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{

    public static FMODEvents instance { get; private set; }

    [field: Header("Menu SFX")]
    [field: SerializeField] public EventReference Music { get; private set; }

    [field: Header("Music")]

    [field: Header("Clown SFX")]

    [field: SerializeField] public EventReference bigClownLaugh { get; private set; }
    [field: SerializeField] public EventReference clownLaugh { get; private set; }
    [field: SerializeField] public EventReference clownGiggle { get; private set; }
    [field: SerializeField] public EventReference clownNotConvinced { get; private set; }
    [field: SerializeField] public EventReference clownFuckinDies { get; private set; }
    [field: SerializeField] public EventReference clownFuckinKillsYou { get; private set; }

    [field: Header("UI SFX")]

    [field: SerializeField] public EventReference cardDraw { get; private set; }
    [field: SerializeField] public EventReference cardPlacing { get; private set; }
    [field: SerializeField] public EventReference cardLiftUp { get; private set; }
    [field: SerializeField] public EventReference ItemFlip { get; private set; }
    [field: SerializeField] public EventReference balloonPop { get; private set; }
    [field: SerializeField] public EventReference laughometerFull { get; private set; }
    [field: SerializeField] public EventReference laughometerMid { get; private set; }
    [field: SerializeField] public EventReference laughometerBad { get; private set; }
    [field: SerializeField] public EventReference lightExplode { get; private set; }
    [field: SerializeField] public EventReference ButtonPress { get; private set; }
    [field: SerializeField] public EventReference CrystalBall { get; private set; }
    [field: SerializeField] public EventReference CoinInsert { get; private set; }
    [field: SerializeField] public EventReference TokenInsert { get; private set; }
    [field: SerializeField] public EventReference LightsOn { get; private set; }


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one FMOD Events instance in the scene");
        }
        instance = this;
    }

    private void Start()
    {

        AudioManager.instance.InitialiseMusic(Music);
    }
}
