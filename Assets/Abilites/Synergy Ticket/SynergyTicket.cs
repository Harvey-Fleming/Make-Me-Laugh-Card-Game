using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SynergyTicket : Ability
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Use();
        }
    }

    public override void Use()
    {
        StartCoroutine(CamChange());
    }

    IEnumerator CamChange()
    {
        //gameObject.SetActive(false);
        switchCamState.SwitchCamView(SwitchCamState.CamView.CoinSlot);
        yield return new WaitForSeconds(1f);
        switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
    }
}
