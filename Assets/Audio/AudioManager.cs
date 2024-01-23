using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{

    [Header("Volume")]
    [Range(0, 1)]
    public float masterVolume = 1;
    [Range(0, 1)]
    public float MusicVolume = 1;
    [Range(0, 1)]
    public float SFXVolume = 1;
    [Range(0, 1)]

    private Bus masterBus;
    private Bus musicBus;
    private Bus sfxBus;

    private List<EventInstance> eventInstances;

    private List<StudioEventEmitter> eventEmitters;

    private EventInstance musicEventInstance;
    public static AudioManager instance { get; private set; }


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one Audio Manager in the scene");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        eventEmitters = new List<StudioEventEmitter>();

        masterBus = RuntimeManager.GetBus("bus:/");
        //musicBus = RuntimeManager.GetBus("bus:/MusicBus");
        //sfxBus = RuntimeManager.GetBus("bus:/SFX");
    }

    private void Update()
    {
        masterBus.setVolume(masterVolume);
        //musicBus.setVolume(MusicVolume);
        //sfxBus.setVolume(SFXVolume);
    }

    public void InitialiseMusic(EventReference musicEventReference)
    {
        musicEventInstance = CreateEventInstance(musicEventReference);
        musicEventInstance.start();

    }

    public void PlayOneShot(EventReference sound, Vector3 worldPos)
    {
        RuntimeManager.PlayOneShot(sound, worldPos);
    }

    public EventInstance CreateEventInstance(EventReference eventReference)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public StudioEventEmitter InitializeEventEmitter(EventReference eventReference, GameObject emitterGameObject)
    {
        StudioEventEmitter emitter = emitterGameObject.GetComponent<StudioEventEmitter>();
        emitter.EventReference = eventReference;
        eventEmitters.Add(emitter);
        return emitter;
    }

    private void CleanUp()
    {
        //stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }

        foreach (StudioEventEmitter emitter in eventEmitters)
        {
            emitter.Stop();
        }
    }

    private void OnDestroy()
    {
        CleanUp();
    }
    
}
