using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PurchaseAbility : MonoBehaviour
{
    private Animator flipAnimator;
    int currentAbility;
    public bool hasAbility;
    [SerializeField] private GameObject[] abilities;
    [SerializeField] private PlayerHand playerHand;

    private void Start()
    {
        flipAnimator = GetComponent<Animator>();

        HideAllAbilities();
    }

    private void Update()
    {
        if (!hasAbility)
        {
            if (Input.GetKeyDown(KeyCode.P) && playerHand.CurrentRerollsLeft >= 1)
            {
                RandomAbility();
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                UseCurrentAbility();
            }
        }
    }

    void UseCurrentAbility()
    {
        abilities[currentAbility].GetComponent<Ability>().Use();
        hasAbility = false;
        flipAnimator.SetTrigger("GoUp");
    }

    void RandomAbility()
    {
        currentAbility = Random.Range(0, abilities.Length);

        for (int i = 0; i < abilities.Length; i++)
        {
            if(i == currentAbility)
            {
                abilities[i].SetActive(true);
                playerHand.CurrentRerollsLeft -= abilities[i].GetComponent<Ability>().cost;
                playerHand.GetComponent<PlayerRedraws>().UpdateRedrawUI(playerHand.CurrentRerollsLeft);
                hasAbility = true;
                flipAnimator.SetTrigger("GoDown");
            }
            else
            {
                abilities[i].SetActive(false);
            }
        }
    }

    public void HideAllAbilities()
    {
        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i].SetActive(false);
        }
    }
}
