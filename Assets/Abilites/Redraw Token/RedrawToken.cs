using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedrawToken : Ability
{
    [SerializeField] private Animator redrawTicketAnimator;

    public override void Use()
    {
        StartCoroutine(CamChange());
    }

    IEnumerator CamChange()
    {
        //gameObject.SetActive(false);
        switchCamState.SwitchCamView(SwitchCamState.CamView.CoinSlot);
        redrawTicketAnimator.SetTrigger("Use");
        yield return new WaitForSeconds(1f);
        switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
    }
}
