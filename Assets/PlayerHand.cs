using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHand : MonoBehaviour
{
    private List<GameObject> playerHand = new();

    [SerializeField] private int startingRedraws = 2;
    private int currentRerollsLeft;

    public int CurrentRerollsLeft { get => currentRerollsLeft; set => currentRerollsLeft = value; }

    private Outline storedOutline = null;
    private GameObject storedhover = null;
    [SerializeField] private float cardHoverStrength = 0.005f;
    [SerializeField] private float abilityHoverStrength = 0.001f;
    [SerializeField] private float cardMaxHoverHeight = 0.1f;
    [SerializeField] private float abilityMaxHoverHeight = 0.05f;

    [SerializeField] private Transform SlotParents;

    bool isRedraw = true;
    public delegate void UIEvent(int score);
    public static event UIEvent OnRedrawNumUpdate;

    [SerializeField] private SwitchCamState switchCamState;
    [SerializeField] private GameObject UiTextBox;
    [SerializeField] private TextMeshProUGUI UiText;

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
        //Handle Outline on hover
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            #region - Outline
            if (hit.transform.TryGetComponent<Outline>(out Outline hitOutline) && storedOutline == null)
            {
                storedOutline = hitOutline;
                storedOutline.enabled = true;
            }
            else if (hit.transform.TryGetComponent<Outline>(out hitOutline) && storedOutline != null)
            {
                storedOutline.enabled = false;
                storedOutline = hitOutline;
                storedOutline.enabled = true;
            }

            //If we are not already outlining something and hover over an outlinable
            if (hit.transform.GetComponentInChildren<Outline>() != null && storedOutline == null)
            {
                storedOutline = hit.transform.GetComponentInChildren<Outline>();
                storedOutline.enabled = true;
            }
            //If we are not already outlining something and hover over an outlinable
            else if (hit.transform.GetComponentInChildren<Outline>() != null && storedOutline != null)
            {
                storedOutline.enabled = false;
                storedOutline = hit.transform.GetComponentInChildren<Outline>();
                storedOutline.enabled = true;
            }
            #endregion
            //Used for Storing Objects that will hover

            //This is specific to the synergy ticket
            else if (hit.transform.parent != null && hit.transform.parent.CompareTag("Ability"))
            {
                storedhover = hit.transform.parent.gameObject;

                UiText.text = hit.transform.parent.GetComponent<Ability>().abilityTxtInfo;
                UiTextBox.SetActive(true);

                if (storedhover.transform.localPosition.y - abilityHoverStrength > -abilityMaxHoverHeight + -0.016)
                {
                    storedhover.transform.localPosition -= new Vector3(0, abilityHoverStrength, 0);
                }
                
            }            
            else if (hit.transform.CompareTag("Ability"))
            {
                storedhover = hit.transform.gameObject;

                UiText.text = hit.transform.GetComponent<Ability>().abilityTxtInfo;
                UiTextBox.SetActive(true);

                if (storedhover.transform.localPosition.y - abilityHoverStrength > -abilityMaxHoverHeight)
                {
                    storedhover.transform.localPosition -= new Vector3(0, abilityHoverStrength, 0);
                }
            }
            else if (hit.transform.CompareTag("Card") && !hit.transform.parent.transform.parent.CompareTag("Decks"))
            {
                storedhover = hit.transform.gameObject;
                if(storedhover.transform.localPosition.y < cardMaxHoverHeight)
                {
                    storedhover.transform.localPosition += new Vector3(0, cardHoverStrength, 0);
                }

            }
        }
        else if (!Physics.Raycast(ray, out hit, Mathf.Infinity) && storedOutline != null)
        {
            storedOutline.enabled = false;
            storedOutline = null;
        }        
        else if (!Physics.Raycast(ray, out hit, Mathf.Infinity) && storedhover != null)
        {
            if(storedhover.name != "SynergyTicket")
            {
                storedhover.transform.localPosition = Vector3.zero;

                UiTextBox.SetActive(false);

                storedhover = null;
            }
            else
            {
                storedhover.transform.localPosition = new Vector3(-0.017f, 0 + -0.016f, 0.003f);

                UiTextBox.SetActive(false);

                storedhover = null;     
            }

        }

        //Handle Selecting Objects
        #region - Clicking Objects
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out  hit, Mathf.Infinity))
            {
                // If it is a card in the hand and is not part of the deck currently
                if(hit.transform.TryGetComponent<BaseCard>(out BaseCard hitCard) && !hit.transform.parent.transform.parent.CompareTag("Decks") && currentRerollsLeft > 0)
                {
                    switch (hitCard.CardDetails.CardType)
                    {
                        case CardType.Start:
                            RedrawCard(hitCard);
                            break;
                        case CardType.Middle:
                            RedrawCard(hitCard);
                            break;
                        case CardType.End:
                            RedrawCard(hitCard);
                            break;
                    }
                }
                //If it is a card in the deck and we have selected a card to replace
                else if (hit.transform.TryGetComponent<BaseCard>(out hitCard) && hit.transform.parent.transform.parent.CompareTag("Decks") && currentRerollsLeft > 0)
                {
                    foreach(GameObject card in playerHand)
                    {
                        if(card.GetComponent<BaseCard>().CardDetails.CardType == hitCard.CardDetails.CardType)
                        {
                            Debug.Log("Found matching type");
                            RedrawCard(card.GetComponent<BaseCard>());
                            break;
                        }
                        
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
                else if (hit.transform.name == "Table.001")
                {
                    hit.transform.GetComponent<PurchaseAbility>().TryPurchaseAbility();
                }
                //Used for clicking abilities.
                else if (hit.transform.CompareTag("Ability"))
                {
                    hit.transform.parent.GetComponent<PurchaseAbility>().UseCurrentAbility();
                }
                //Used for synergy Ticket.
                else if (hit.transform.parent.CompareTag("Ability"))
                {
                    hit.transform.parent.transform.parent.GetComponent<PurchaseAbility>().UseCurrentAbility();
                }
            }
        }
        #endregion
    }

    private void RedrawCard(BaseCard cardToReplace)
    {
        Debug.Log("Intent to redraw a " + cardToReplace.CardDetails.CardType);
        GameObject newcard = CardManager.Instance.RequestNewCard(cardToReplace.CardDetails.CardType, cardToReplace.gameObject);
        playerHand.Insert(playerHand.IndexOf(cardToReplace.gameObject), newcard);
        playerHand.Remove(cardToReplace.gameObject);
        foreach (GameObject card in playerHand)
        {
            SetCardParent(card);
        }
        currentRerollsLeft--;
        OnRedrawNumUpdate(currentRerollsLeft);
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
        currentRerollsLeft += 2;
        OnRedrawNumUpdate(currentRerollsLeft);
    }

    private void OnRoundStart()
    {
        isRedraw = true;
        switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
        switchCamState.canMove = true;
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