using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ability : MonoBehaviour
{
    public int cost = 1;

    protected SwitchCamState switchCamState;

    private void Start()
    {
        switchCamState = Camera.main.GetComponent<SwitchCamState>();
    }

    public virtual void Use()
    {

    }
}
