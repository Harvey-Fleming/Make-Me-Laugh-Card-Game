using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergyTicket : Ability
{
    [SerializeField] private Animator synergyAnimator;

    public override void Use()
    {
        GameObject.FindObjectOfType<PlayerHand>().HasSynergy = true;
        StartCoroutine(CamChange());
    }

    IEnumerator CamChange()
    {
        //gameObject.SetActive(false);
        switchCamState.SwitchCamView(SwitchCamState.CamView.CoinSlot);
        synergyAnimator.SetTrigger("Use");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.TokenInsert, transform.position);
        yield return new WaitForSeconds(1f);
        switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
    }
}
