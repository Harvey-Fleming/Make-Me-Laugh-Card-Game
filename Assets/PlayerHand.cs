using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerHand : MonoBehaviour
{
    private List<GameObject> playerHand = new();

    [SerializeField] private int startingRedraws = 2;
    private int currentRerollsLeft;

    private Outline storedOutline = null;
    private GameObject storedhover = null;
    [SerializeField] private float cardHoverStrength = 0.005f;
    [SerializeField] private float abilityHoverStrength = 0.001f;
    [SerializeField] private float cardMaxHoverHeight = 0.1f;
    [SerializeField] private float abilityMaxHoverHeight = 0.05f;

    [SerializeField] private Transform SlotParents;
    private bool hasSynergy;
    private int synergyStrength = 0;

    System.Random random = new();

    bool isRedraw = true;
    public delegate void UIEvent(int score);
    public static event UIEvent OnRedrawNumUpdate;

    [SerializeField] private SwitchCamState switchCamState;
    [SerializeField] private GameObject UiTextBox;
    [SerializeField] private TextMeshProUGUI UiText;

    public int CurrentRerollsLeft { get => currentRerollsLeft; set => currentRerollsLeft = value; }
    public bool IsRedraw { get => isRedraw; set => isRedraw = value; }
    public bool HasSynergy { get => hasSynergy; set => hasSynergy = value; }
    public int SynergyStrength { get => synergyStrength; set => synergyStrength = value; }

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

            #region - Hover Over
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
        #endregion

        //Handle Selecting Objects
        #region - Clicking Objects
        if (Input.GetMouseButtonDown(0))
        {
            if (Physics.Raycast(ray, out  hit, Mathf.Infinity))
            {
                // If it is a card in the hand and is not part of the deck currently
                if(hit.transform.TryGetComponent<BaseCard>(out BaseCard hitCard) && !hit.transform.parent.transform.parent.CompareTag("Decks"))
                {
                    if (hasSynergy | currentRerollsLeft > 0)
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
                }
                //If it is a card in the deck and we have selected a card to replace
                else if (hit.transform.TryGetComponent<BaseCard>(out hitCard) && hit.transform.parent.transform.parent.CompareTag("Decks"))
                {
                    if (hasSynergy | currentRerollsLeft > 0)
                    {
                        foreach (GameObject card in playerHand)
                        {
                            if (card.GetComponent<BaseCard>().CardDetails.CardType == hitCard.CardDetails.CardType)
                            {
                                Debug.Log("Found matching type");
                                RedrawCard(card.GetComponent<BaseCard>());
                                break;
                            }

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
                else if (hit.transform.parent != null && hit.transform.parent.CompareTag("Ability"))
                {
                    hit.transform.parent.transform.parent.GetComponent<PurchaseAbility>().UseCurrentAbility();
                }
            }
        }
        #endregion
    }

    private void RedrawCard(BaseCard cardToReplace)
    {
        GameObject newcard = null;
        if (!hasSynergy)
        {
            newcard = CardManager.Instance.RequestNewCard(cardToReplace.CardDetails.CardType, cardToReplace.gameObject);
            currentRerollsLeft--;
        }
        else if (hasSynergy)
        {
            switch (cardToReplace.CardDetails.CardType)
            {
                case CardType.Start:
                    newcard = CheckSynergies(CardType.Start, 1, cardToReplace);
                    hasSynergy = false;
                    break;
                case CardType.Middle:
                    List<GameObject> tempSynergyList = new();
                    List<GameObject> tempweakSynergyList = new();
                    List<GameObject> tempstrongSynergyList = new();

                    GameObject synergy1 = CheckSynergies(CardType.Middle, 0, cardToReplace);
                    GameObject synergy2 = CheckSynergies(CardType.Middle, 2, cardToReplace);

                    Debug.Log(synergy1);
                    Debug.Log(synergy2);

                    if (synergy1 != null) tempSynergyList.Add(synergy1);
                    if (synergy1 != null) tempSynergyList.Add(synergy2);
                    Debug.Log("Temp synergy list count is " + tempSynergyList.Count);

                    foreach(GameObject synergy in tempSynergyList)
                    {
                        Debug.Log(synergy);
                    }

                    if(tempSynergyList.Count > 0)
                    {
                        foreach(GameObject synergyCard in tempSynergyList)
                        {
                            if(synergyStrength == 2)
                            {
                                Debug.Log("Strong Synergy found");
                                tempstrongSynergyList.Add(synergyCard);
                            }
                            else if(synergyStrength == 1)
                            {
                                Debug.Log("Weak Synergy found");
                                tempweakSynergyList.Add(synergyCard);
                            }
                        }

                        if (tempstrongSynergyList.Count > 0)
                        {
                            Debug.Log("Strong Synergy List is not empty");
                            if (tempweakSynergyList.Count == 1)
                            {
                                newcard = tempstrongSynergyList[0];
                            }
                            else
                            {
                                Debug.Log(random.Next(0, tempSynergyList.Count));
                                newcard = tempstrongSynergyList[random.Next(0, tempstrongSynergyList.Count)];

                            }
                        }
                        else if (tempweakSynergyList.Count > 0)
                        {
                            Debug.Log("Weak Synergy List is not empty");
                            if (tempweakSynergyList.Count == 1)
                            {
                                
                                newcard = tempweakSynergyList[0];
                            }
                            else
                            {
                                int storedint = random.Next(0, tempSynergyList.Count);
                                
                                Debug.Log(storedint);
                                newcard = tempweakSynergyList[storedint];
                            }
                        }
                        else
                        {
                            Debug.Log("No Other Synergy List has elements");
                            int storedint = random.Next(0, tempSynergyList.Count);
                            Debug.Log(storedint);

                            if (tempSynergyList.Count == 1)
                            {
                                
                                newcard = tempSynergyList[0];
                            }
                            else
                            {
                                newcard = tempSynergyList[storedint];
                            }

                            
                        }
                        Debug.Log("New card is" + newcard.name);
                        hasSynergy = false;
                    }
                    else
                    {
                        Debug.LogWarning("No Synergies found");
                        return;
                    }
                    break;


                case CardType.End:
                    newcard = CheckSynergies(CardType.End, 1, cardToReplace);
                    hasSynergy = false;
                    break;

            }
        }
        playerHand.Insert(playerHand.IndexOf(cardToReplace.gameObject), newcard);
        playerHand.Remove(cardToReplace.gameObject);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.cardDraw, transform.position);
        foreach (GameObject card in playerHand)
        {
            SetCardParent(card);
        }
        OnRedrawNumUpdate(currentRerollsLeft);
    }

    private GameObject CheckSynergies(CardType typeToFind, int indexToCheck, BaseCard cardToReplace)
    {
        BaseCard cardToCheck = playerHand[indexToCheck].GetComponent<BaseCard>();

        List<CardObject> tempweakSynergyList = new();
        List<CardObject> tempstrongSynergyList = new();

        foreach (CardObject synergy in cardToCheck.CardDetails.StrongSynergies)
        {
            if (synergy.CardType == typeToFind && synergy.name != cardToReplace.CardDetails.name)
            {
                tempstrongSynergyList.Add(synergy);
            }
        }        
        
        foreach (CardObject synergy in cardToCheck.CardDetails.WeakSynergies)
        {
            if (synergy.CardType == typeToFind && synergy.name != cardToReplace.CardDetails.name)
            {
                tempweakSynergyList.Add(synergy);
            }
        }

        if (tempstrongSynergyList.Count > 0)
        {
            List<CardObject> templist = new();
            Debug.Log("Card has " + cardToCheck.CardDetails.StrongSynergies.Length + " synergies");
            foreach (CardObject synergy in cardToCheck.CardDetails.StrongSynergies)
            {
                if (synergy.CardType == typeToFind && synergy.name != cardToReplace.CardDetails.name)
                {
                    templist.Add(synergy);
                }
                else if(synergy.CardType == typeToFind && synergy.name == cardToReplace.CardDetails.name)
                {
                    Debug.Log("Only Same Card is found in the strong synergy list");
                    return null;
                }
            }
            Debug.Log("Temp list has " + templist.Count + " elements");
            if (templist.Count > 0)
            {
                SynergyStrength = 2;
                return CardManager.Instance.FindSynergyCard(typeToFind, templist[random.Next(0, templist.Count - 1)], cardToReplace.gameObject);
            }
            else if (templist.Count == 1)
            {
                SynergyStrength = 2;
                return CardManager.Instance.FindSynergyCard(typeToFind, templist[0], cardToReplace.gameObject);
            }
            else
            {
                Debug.LogWarning("Strong Synergy Check Synergy in player hand has returned null");
                return null;
            }
        }
        else if (tempweakSynergyList.Count > 0)
        {
            List<CardObject> templist = new();
            foreach (CardObject synergy in cardToCheck.CardDetails.WeakSynergies)
            {
                if (synergy.CardType == typeToFind)
                {
                    templist.Add(synergy);
                }
            }
            if (templist.Count > 0)
            {
                GameObject.FindObjectOfType<PlayerHand>().SynergyStrength = 1;
                return CardManager.Instance.FindSynergyCard(typeToFind, templist[random.Next(0, templist.Count - 1)], cardToReplace.gameObject);
            }
            else if (templist.Count == 1)
            {
                SynergyStrength = 1;
                return CardManager.Instance.FindSynergyCard(typeToFind, templist[0], cardToReplace.gameObject);
            }
            else
            {
                Debug.LogWarning("Weak Synergy Check Synergy in player hand has returned null");
                return null;
            }
        }
        else
        {
            SynergyStrength = 0;
            return CardManager.Instance.RequestNewCard(typeToFind, cardToReplace.gameObject);
        }
    }

    public void RequestCard()
    {
        List<GameObject> tempcardList = new();
        if (isRedraw)
        {
            if (playerHand.Count > 0)
            {
                CardManager.Instance.ReturnHand(playerHand);
                playerHand.Clear();
            }

            OnRedrawNumUpdate(currentRerollsLeft);
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