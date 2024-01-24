using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The card Manager will handle the decks as well as dealing to the players hand.
public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    private int ClownLives = 6;

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

    public delegate void ScoreEvent(int score);
    public static event ScoreEvent SubmitScore;

    public delegate void GameEvent();
    public static event GameEvent OnRoundStart;


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
    //Initial Hand will the shuffle the list and get the first card in the list for each deck and return a list of the picked up hand to the player
    public List<GameObject> RequestInitialHand()
    {
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
            OnRoundStart();
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
                SetCardParent(submittedCards[i], elevatorSlots[i]);
                ElevatorAnimation.Instance.SendDown();
                currentPhase = TurnPhase.Judgement;
            }
        }
        else if(!isUp && currentPhase == TurnPhase.Judgement)
        {
            //Execute Judgement Code
            CalculateScore();

        }
    }

    private void SetCardParent(GameObject card, Transform parent)
    {
        card.transform.parent = parent;
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    private void CalculateScore()
    {
        int score = 0;

        //Check if first card relates to end card
        foreach(CardObject synergy in submittedCards[0].GetComponent<BaseCard>().CardDetails.WeakSynergies)
        {
            if(synergy.name == submittedCards[2].GetComponent<BaseCard>().CardDetails.name)
            {
                Debug.Log("Weak End Synergy Found");
                score++;
            }
        }      
        
        foreach(CardObject synergy in submittedCards[0].GetComponent<BaseCard>().CardDetails.StrongSynergies)
        {
            if(synergy.name == submittedCards[2].GetComponent<BaseCard>().CardDetails.name)
            {
                Debug.Log("Strong End Synergy Found");
                score += 2;
            }
        }

        for(int i = 0; i < 3; i += 2)
        {
            //Check if first card relates to end card
            foreach (CardObject synergy in submittedCards[1].GetComponent<BaseCard>().CardDetails.WeakSynergies)
            {
                if (synergy.name == submittedCards[i].GetComponent<BaseCard>().CardDetails.name)
                {
                    Debug.Log("Weak Synergy Found");
                    score++;
                }
            }
            foreach (CardObject synergy in submittedCards[1].GetComponent<BaseCard>().CardDetails.StrongSynergies)
            {
                if (synergy.name == submittedCards[i].GetComponent<BaseCard>().CardDetails.name)
                {
                    Debug.Log("Strong Synergy Found");
                    score += 2;
                }
            }
        }
        Debug.Log("Score is " + score);

        SubmitScore(score);

        //Execute Tension + Sound Effect

        //Result based on Score
        switch (score)
        {
            case 0 | 1:
                Debug.Log("You got nobody laughing");
                break;            
            case 2 | 3:
                Debug.Log("Clown Lost 1 Life");
                ClownLives--;
                break;            
            case >= 4:
                Debug.Log("Clown Loses 2 Lives!!!");
                ClownLives -= 2;
                break;
        }

        //Add Cards back to Deck
        StartCardDeck.Add(submittedCards[0]);
        SetCardParent(submittedCards[0], startDeckParent.transform);
        MiddleCardDeck.Add(submittedCards[1]);
        SetCardParent(submittedCards[1], middleDeckParent.transform);
        EndCardDeck.Add(submittedCards[2]);
        SetCardParent(submittedCards[2], EndDeckParent.transform);

        for(int i = 0; i < submittedCards.Length; i++)
        {
            submittedCards[i] = null;
        }

        if (ClownLives <= 0)
        {
            currentPhase = TurnPhase.Victory;
            Debug.Log("Clown has been defeated");
        }
        else
        {
            deckParents[0].transform.parent.transform.position += Vector3.down * 5;
            ElevatorAnimation.Instance.SendUp();
            currentPhase = TurnPhase.Draw;
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