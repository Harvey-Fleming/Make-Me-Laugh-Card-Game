using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ClownDeathSequence : MonoBehaviour
{
    [SerializeField] private CardManager cardManager;
    [SerializeField] GameObject redLight;
    [SerializeField] private Lightswitch lightSwitch;
    float timer;
    [SerializeField] private float maxTimer;
    [SerializeField] private SwitchCamState switchCamState;
    [SerializeField] private GameObject livingClown;
    [SerializeField] private GameObject deadClown;

    public IEnumerator DeathSequence()
    {
        Debug.Log("clown is dying");
        timer = 0f;
        lightSwitch.LightOnOff(false);
        switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
        yield return new WaitForSeconds(1f);
        switchCamState.isDying = true;
        cardManager.SetClownState(CardManager.ClownStates.Die);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.clownFuckinDies, transform.position);
        yield return new WaitForSeconds(1f);
        redLight.SetActive(true);
        yield return new WaitForSeconds(5f);
        livingClown.SetActive(false);
        deadClown.SetActive(true);
        lightSwitch.LightOnOff(true);
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }
}