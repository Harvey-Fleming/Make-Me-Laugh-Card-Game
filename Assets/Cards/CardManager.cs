using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The card Manager will handle the decks as well as dealing to the players hand.
public class CardManager : MonoBehaviour
{
    public static CardManager Instance;

    System.Random random;

    [SerializeField] private GameObject startDeckParent;
    [SerializeField] private GameObject middleDeckParent;
    [SerializeField] private GameObject EndDeckParent;
    private GameObject[] deckParents;


    [Header("")]
    [SerializeField] private List<GameObject> StartCardDeck = new();
    [SerializeField] private List<GameObject> MiddleCardDeck = new();
    [SerializeField] private List<GameObject> EndCardDeck = new();
    private List<List<GameObject>> fullDecks = new();

    //TODO - Will handle checking synergies when cards are submitted

    private void Awake()
    {
        random = new();

        if(Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

        //Initializes arrays for easier looping when adding all cards to the deck
        if(startDeckParent != null && middleDeckParent != null && EndDeckParent != null)
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
    }

    //TODO - responsible for shuffling the deck at the start of a round
    private void ShuffleDecks()
    {
        StartCardDeck.Shuffle();
        MiddleCardDeck.Shuffle();
        EndCardDeck.Shuffle();

        foreach(GameObject card in StartCardDeck)
        {
            Debug.Log(card.GetComponent<BaseCard>().CardDetails);
        }
    }

    #region - Player Hand and Dealing
    //TODO - will handle requests from player to pick up a card

    //Initial Hand will the shuffle the list and get the first card in the list for each deck and return a list of the picked up hand to the player
    public List<GameObject> RequestInitialHand()
    {
        ShuffleDecks();
        List<GameObject> newCardList = new();

        foreach(List<GameObject> deck in fullDecks)
        {
            newCardList.Add(deck[0]);
            deck.Add(deck[0]);
            deck.Remove(deck[0]);
        }

        return newCardList;
    }

    public GameObject RequestNewCard(CardType typeRequested, List<GameObject> currentHand)
    {
        GameObject card;
        
        switch (typeRequested)
        {
            case CardType.Start:
                card = StartCardDeck[0];
                StartCardDeck.Remove(card);
                StartCardDeck.Add(card);
                return card;

            case CardType.Middle:
                card = MiddleCardDeck[0];
                MiddleCardDeck.Remove(card);
                MiddleCardDeck.Add(card);
                return card;

            case CardType.End:
                card = EndCardDeck[0];
                EndCardDeck.Remove(card);
                EndCardDeck.Add(card);
                return card;
            default:
                return null;
        }
        #endregion
    }
}