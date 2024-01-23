using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The card Manager will handle the decks as well as dealing to the players hand.
public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    System.Random random;

    private GameObject[] submittedCards = new GameObject[3];

    TurnPhase currentPhase = TurnPhase.Draw;

    [SerializeField] private GameObject startDeckParent;
    [SerializeField] private GameObject middleDeckParent;
    [SerializeField] private GameObject EndDeckParent;
    private GameObject[] deckParents;

    private List<GameObject> StartCardDeck = new();
    private List<GameObject> MiddleCardDeck = new();
    private List<GameObject> EndCardDeck = new();
    private List<List<GameObject>> fullDecks = new();

    public TurnPhase CurrentPhase { get => currentPhase; }

    //TODO - Will handle checking synergies when cards are submitted

    private void Awake()
    {
        random = new();

        if (Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        //Initializes arrays for easier looping when adding all cards to the deck
        if (startDeckParent != null && middleDeckParent != null && EndDeckParent != null)
        {
            deckParents = new GameObject[3] { startDeckParent, middleDeckParent, EndDeckParent };
        }
        else
        {
            Debug.LogError("Could not find a deck parent." + "Error in Card Manager Awake. Line: 27");
        }

        fullDecks = new List<List<GameObject>> { StartCardDeck, MiddleCardDeck, EndCardDeck };

        foreach (GameObject deckparent in deckParents)
        {
            foreach (Transform child in deckparent.transform)
            {
                if (child.TryGetComponent<BaseCard>(out BaseCard card))
                {
                    fullDecks[Array.IndexOf(deckParents, deckparent)].Add(child.gameObject);
                }
            }
        }
    }

    private void Start()
    {
        ShuffleDecks();

        ElevatorAnimation.Instance.SendUp();
    }

    public void OnButtonHit(List<GameObject> hand)
    {
        if(currentPhase == TurnPhase.Draw)
        {
            Debug.Log("End of Draw Phase... Submitting!");
            currentPhase = TurnPhase.Submit;
            for(int i = 0; i < hand.Count; i++)
            {
                submittedCards[i] = hand[i];
            }
            ButtonAnimations.Instance.PushButton();
            ElevatorAnimation.Instance.SendDown();

        }
    }

    //TODO - responsible for shuffling the deck at the start of a round
    private void ShuffleDecks()
    {
        StartCardDeck.Shuffle();
        MiddleCardDeck.Shuffle();
        EndCardDeck.Shuffle();
    }

    #region - Player Hand and Dealing
    //TODO - will handle requests from player to pick up a card

    //Initial Hand will the shuffle the list and get the first card in the list for each deck and return a list of the picked up hand to the player
    public List<GameObject> RequestInitialHand()
    {
        ShuffleDecks();
        List<GameObject> newCardList = new();

        foreach (List<GameObject> deck in fullDecks)
        {
            newCardList.Add(deck[0]);
            deck.Remove(deck[0]);
        }

        return newCardList;
    }

    public GameObject RequestNewCard(CardType typeRequested, GameObject cardToReplace)
    {
        GameObject card;
        
        switch (typeRequested)
        {
            case CardType.Start:
                card = StartCardDeck[0];
                StartCardDeck.Remove(card);
                cardToReplace.transform.parent = startDeckParent.transform;
                cardToReplace.transform.localPosition = Vector3.zero;
                cardToReplace.transform.localRotation = Quaternion.Euler(0, 0, 0);
                StartCardDeck.Add(cardToReplace);
                return card;

            case CardType.Middle:
                card = MiddleCardDeck[0];
                MiddleCardDeck.Remove(card);
                cardToReplace.transform.parent = middleDeckParent.transform;
                cardToReplace.transform.localPosition = Vector3.zero;
                cardToReplace.transform.localRotation = Quaternion.Euler(0, 0, 0);
                MiddleCardDeck.Add(cardToReplace);
                return card;

            case CardType.End:
                card = EndCardDeck[0];
                EndCardDeck.Remove(card);
                cardToReplace.transform.parent = EndDeckParent.transform;
                cardToReplace.transform.localPosition = Vector3.zero;
                cardToReplace.transform.localRotation = Quaternion.Euler(0,0,0);
                EndCardDeck.Add(cardToReplace);
                return card;
            default:
                return null;
        }

    }

    public void ReturnHand(List<GameObject> hand)
    {
        foreach(GameObject card in hand)
        {
            foreach (List<GameObject> deck in fullDecks)
            {
                switch (card.GetComponent<BaseCard>().CardDetails.CardType)
                {
                    case CardType.Start:
                        StartCardDeck.Add(card);
                        card.transform.parent = startDeckParent.transform;
                        break;
                    case CardType.Middle:
                        MiddleCardDeck.Add(card);
                        card.transform.parent = middleDeckParent.transform;
                        break;
                    case CardType.End:
                        EndCardDeck.Add(card);
                        card.transform.parent = EndDeckParent.transform;
                        break;
                }
            }
        }
    }
    #endregion

    private void OnElevatorStop(bool isUp)
    {
        if (isUp && currentPhase is TurnPhase.Draw)
        {
            
        }
        else if (!isUp && currentPhase is TurnPhase.Submit)
        {
            deckParents[0].transform.parent.transform.position -= Vector3.down * 5;
            ElevatorAnimation.Instance.SendUp();
        }
        else if(isUp && currentPhase is TurnPhase.Submit)
        {
            List<Transform> elevatorSlots = ElevatorAnimation.Instance.ElevatorSlots;
            for (int i = 0; i < submittedCards.Length; i++)
            {
                submittedCards[i].transform.parent = elevatorSlots[i];
                submittedCards[i].transform.localPosition = Vector3.zero;
                submittedCards[i].transform.localRotation = Quaternion.Euler(0, 0, 0);
                ElevatorAnimation.Instance.SendDown();
                currentPhase = TurnPhase.Judgement;
            }
        }
        else if(!isUp && currentPhase == TurnPhase.Judgement)
        {
            //Execute Judgement Code
        }
    }

    #region - Event Subscription
    private void OnEnable()
    {
        ElevatorAnimation.OnElevatorStop += OnElevatorStop;
    }

    private void OnDisable()
    {
        ElevatorAnimation.OnElevatorStop -= OnElevatorStop;
    }

    #endregion
}

public enum TurnPhase
{
    Draw,
    Submit,
    Judgement,
    Defeat,
    Victory,
}