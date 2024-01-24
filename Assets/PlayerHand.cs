using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    private List<GameObject> playerHand = new();

    [SerializeField] private int startingRedraws = 2;
    private int currentRerollsLeft;

    private BaseCard storedActionCard;

    [SerializeField] private Transform SlotParents;

    bool isRedraw = true;
    public delegate void UIEvent(int score);
    public static event UIEvent OnRedrawNumUpdate;

    private void Start()
    {
        OnRedrawNumUpdate(currentRerollsLeft);
        currentRerollsLeft = startingRedraws;
    }

    private void Update()
    {
        SelectCardRaycast();
    }

    //Handles Selecting Elements with the mouse such as Cards, buttons etc
    //If the player clicks on a card, it is saved in the variable
    //then if they click on a deck, if there is a card saved, it will check if it is the same type and if so, request to redraw that card.
    private void SelectCardRaycast()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
            {
                //Debug.Log("Hit Parent is : " + hit.transform.parent.name);
                //Debug.Log("Hit: " + hit.transform.name);
                Debug.Log("Hit: " + hit.transform.parent.transform.parent);

                // If it is a card in the hand and is not part of the deck currently
                if(hit.transform.TryGetComponent<BaseCard>(out BaseCard hitCard) && !hit.transform.parent.transform.parent.CompareTag("Decks"))
                {
                    switch (hitCard.CardDetails.CardType)
                    {
                        case CardType.Start:
                            storedActionCard = hitCard;
                            Debug.Log("Start Card selected");
                            break;
                        case CardType.Middle:
                            storedActionCard = hitCard;
                            Debug.Log("Middle Card selected");
                            break;
                        case CardType.End:
                            storedActionCard = hitCard;
                            Debug.Log("End Card selected");
                            break;
                    }
                }
                //If it is a card in the deck and we have selected a card to replace
                else if (hit.transform.TryGetComponent<BaseCard>(out hitCard) && storedActionCard != null && hit.transform.parent.transform.parent.CompareTag("Decks"))
                {
                    if (hitCard.CardDetails.CardType == storedActionCard.CardDetails.CardType && currentRerollsLeft > 0)
                    {
                        Debug.Log("Intent to redraw a " + hitCard.CardDetails.CardType);
                        GameObject newcard = CardManager.Instance.RequestNewCard(storedActionCard.CardDetails.CardType, storedActionCard.gameObject);
                        playerHand.Insert(playerHand.IndexOf(storedActionCard.gameObject), newcard);
                        playerHand.Remove(storedActionCard.gameObject);
                        foreach(GameObject card in playerHand)
                        {
                            SetCardParent(card);
                        }
                        currentRerollsLeft--;
                        OnRedrawNumUpdate(currentRerollsLeft);
                    }
                }
                //If we hit the button
                else if(hit.transform.name == "Button")
                {
                    if(CardManager.Instance.CurrentPhase == TurnPhase.Draw)
                    {
                        CardManager.Instance.OnButtonHit(playerHand);
                        ResetCards();
                    }
                }
            }
            else
            {
                storedActionCard = null;
            }
        }
    }

    private void RequestCard()
    {
        List<GameObject> tempcardList = new();
        if (isRedraw)
        {
            OnRedrawNumUpdate(currentRerollsLeft);
            Debug.Log("Request first Hand");
            tempcardList = CardManager.Instance.RequestInitialHand();
            foreach(GameObject card in tempcardList)
            {
                playerHand.Add(card);
                SetCardParent(card);
            }
            isRedraw = false;
        }
        else
        {
            //request other card
        }
    }

    private void SetCardParent(GameObject card)
    {
        card.transform.parent = SlotParents.GetChild(playerHand.IndexOf(card));
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void ResetCards()
    {
        //Will Handle Resetting the list of cards in the players hand
        playerHand.Clear();

        //Reset the player's number of rerolls
        currentRerollsLeft += 1;
        OnRedrawNumUpdate(currentRerollsLeft);
    }

    private void OnRoundStart()
    {
        isRedraw = true;
        RequestCard();
    }

    #region - Event Subscription
    private void OnEnable()
    {
        CardManager.OnRoundStart += OnRoundStart;
    }

    private void OnDisable()
    {
        CardManager.OnRoundStart -= OnRoundStart;
    }

    #endregion
}