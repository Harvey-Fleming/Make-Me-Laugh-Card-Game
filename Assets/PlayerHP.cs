using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHP : MonoBehaviour
{
    [SerializeField] public int healthPoints;
    [SerializeField] private int maxHealthPoints;
    [SerializeField] Lightswitch lightSwitch;

    [SerializeField] private GameObject[] balloons;
    [SerializeField] private SwitchCamState switchCamState;

    private void Start()
    {
        healthPoints = maxHealthPoints;
    }

    public void TakeDamage()
    {
        healthPoints--;

        Debug.Log("Lower HP");

        if(balloons[0].activeSelf == true)
        {
            balloons[0].GetComponent<Animator>().SetTrigger("Popped");
        }
        else
        {
            balloons[1].GetComponent<Animator>().SetTrigger("Popped");
            switchCamState.SwitchCamView(SwitchCamState.CamView.Left);
            StartCoroutine(PopDelay());
        }
    }

    IEnumerator PopDelay()
    {
        yield return new WaitForSeconds(0.5f);
        lightSwitch.LightOnOff(false);
        AudioManager.instance.musicEventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        yield return new WaitForSeconds(0.5f);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.clownFuckinKillsYou, transform.position);
        yield return new WaitForSeconds(6f);
        SceneManager.LoadScene(0);
    }
}
