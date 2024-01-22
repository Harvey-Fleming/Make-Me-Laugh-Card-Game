using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private List<GameObject> playerHand = new();
    [SerializeField] private int maxHandSize = 5;
    [SerializeField] private int maxRerollsPerRound = 2;
    private int currentRerollsLeft;

    [SerializeField] private Transform SlotParents;



    bool isFirstround = true;

    private void Start()
    {
        RequestCard();
    }

    private void RequestCard()
    {
        List<GameObject> tempcardList = new();
        if (isFirstround)
        {
            Debug.Log("Request first Hand");
            tempcardList = CardManager.Instance.RequestInitialHand();
            foreach(GameObject card in tempcardList)
            {
                playerHand.Add(card);
                card.transform.position = SlotParents.GetChild(playerHand.IndexOf(card)).position;
                card.transform.rotation = SlotParents.GetChild(playerHand.IndexOf(card)).rotation;
            }
            isFirstround = false;
        }
        else
        {
            //request other card
        }
    }

    private void ResetCards()
    {
        //Will Handle Resetting the list of cards in the players hand


        //Reset the player's number of rerolls
        currentRerollsLeft = maxRerollsPerRound;
    }

    public void OnCardButtonPressed(int index)
    {
        CardType type;

        switch (index)
        { 
            case 0:
                type = CardType.Start;
                break;            
            case 1:
                type = CardType.Middle;
                break;            
            case 2:
                type = CardType.End;
                break;
            default:
                type = CardType.Start;
                break;
        }

        if (playerHand.Count < maxHandSize)
        {
            GameObject newCard = CardManager.Instance.RequestNewCard(type, playerHand);
            playerHand.Add(newCard);
            newCard.transform.position = SlotParents.GetChild(playerHand.IndexOf(newCard)).position;
            newCard.transform.rotation = SlotParents.GetChild(playerHand.IndexOf(newCard)).rotation;
        }
    }
}