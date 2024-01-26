using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//The card Manager will handle the decks as well as dealing to the players hand.
public class CardManager : MonoBehaviour
{
    public static CardManager Instance;
    [SerializeField] public int ClownLives = 6;

    System.Random random;

    private GameObject[] submittedCards = new GameObject[3];

    TurnPhase currentPhase = TurnPhase.Draw;

    [SerializeField] private GameObject startDeckParent;
    [SerializeField] private GameObject middleDeckParent;
    [SerializeField] private GameObject EndDeckParent;
    [SerializeField] private GameObject LaughoMeter;
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

    [SerializeField] private GameObject[] lightBulbs;
    [SerializeField] private GameObject[] brokenLightBulbs;

    [SerializeField] private SwitchCamState switchCamState;

    [SerializeField] private PlayerHP playerHP;

    [SerializeField] private Transform clown;

    public enum ClownStates { Angry, BigLaugh, Chuckle, Idle, Wait, Laugh, Die}

    private ClownStates currentClownState;

    [SerializeField] private Animator clownAnimator;

    [SerializeField] private ClownDeathSequence clownDeath;


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


        SetClownState(ClownStates.Idle);
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

            switchCamState.canMove = false;
            switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
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

    public GameObject RequestSynergyCard(GameObject cardRequested, GameObject cardToReplace)
    {
        switch (cardRequested.GetComponent<BaseCard>().CardDetails.CardType)
        {
            case CardType.Start:
                StartCardDeck.Remove(cardRequested);
                SetCardParent(cardToReplace, startDeckParent.transform);
                StartCardDeck.Add(cardToReplace);
                return cardRequested;

            case CardType.Middle:
                MiddleCardDeck.Remove(cardRequested);
                SetCardParent(cardToReplace, middleDeckParent.transform);
                MiddleCardDeck.Add(cardToReplace);
                return cardRequested;

            case CardType.End:
                EndCardDeck.Remove(cardRequested);
                SetCardParent(cardToReplace, EndDeckParent.transform);
                EndCardDeck.Add(cardToReplace);
                return cardRequested;
            default:
                return null;
        }

    }

    public GameObject FindSynergyCard(CardType typetoSearch, CardObject synergyToFind, GameObject cardToReplace)
    {
        Debug.Log("Synergy to find is" + synergyToFind.name);
        switch (typetoSearch)
        {
            case CardType.Start:
                foreach (GameObject deckcard in StartCardDeck)
                {
                    if (deckcard.GetComponent<BaseCard>().CardDetails == synergyToFind)
                    {
                        Debug.Log("Found the correct Synergy");
                        return RequestSynergyCard(deckcard, cardToReplace);
                    }
                }
                Debug.Log("No Found the correct Synergy");
                return StartCardDeck[random.Next(0, StartCardDeck.Count)];
            case CardType.Middle:
                foreach (GameObject deckcard in MiddleCardDeck)
                {
                    if (deckcard.GetComponent<BaseCard>().CardDetails == synergyToFind)
                    {
                        Debug.Log("Found the correct Synergy");
                        return RequestSynergyCard(deckcard, cardToReplace);

                    }
                }
                Debug.Log("No Found the correct Synergy");
                return MiddleCardDeck[random.Next(0, MiddleCardDeck.Count)];
            case CardType.End:
                foreach (GameObject deckcard in EndCardDeck)
                {
                    if (deckcard.GetComponent<BaseCard>().CardDetails == synergyToFind)
                    {
                        Debug.Log("Found the correct Synergy");
                        return RequestSynergyCard(deckcard, cardToReplace);
                    }
                }
                Debug.Log("No Found the correct Synergy");
                return EndCardDeck[random.Next(0, EndCardDeck.Count)];
            default:
                Debug.Log("Default State");
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
                        SetCardParent(card, startDeckParent.transform);
                        break;
                    case CardType.Middle:
                        MiddleCardDeck.Add(card);
                        SetCardParent(card, middleDeckParent.transform);
                        break;
                    case CardType.End:
                        EndCardDeck.Add(card);
                        SetCardParent(card, EndDeckParent.transform);
                        break;
                }
            }
        }
    }
    #endregion

    #region - FlipCards
    public void StartFlipCards()
    {
        StartCoroutine(DisplayTopCards());
    }

    public IEnumerator DisplayTopCards()
    {
        FlipDecks(false);
        yield return new WaitForSeconds(5f);
        FlipDecks(true);
    }

    public void FlipDecks(bool hasShown)
    {
        if (!hasShown)
        {
            foreach (List<GameObject> deck in fullDecks)
            {
                deck[0].transform.localRotation = Quaternion.Euler(0, 180, 0);
                deck[0].transform.localPosition = new Vector3(0, 0, 0.25f);
            }
        }
        else
        {
            foreach (List<GameObject> deck in fullDecks)
            {
                deck[0].transform.localRotation = Quaternion.Euler(0, 0, 0);
                deck[0].transform.localPosition = new Vector3(0, 0, 0);
            }
        }
    }
    #endregion

    private void OnElevatorStop(bool isUp)
    {
        if (isUp && currentPhase is TurnPhase.Draw)
        {
            OnRoundStart();

            switchCamState.canMove = true;
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
            switchCamState.SwitchCamView(SwitchCamState.CamView.Right);
            switchCamState.canMove = false;
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
            case 0:
                Debug.Log("You got nobody laughing");
                
                if(ClownLives <= 3)
                {
                    SetClownState(ClownStates.Angry);
                }
                else
                {
                    SetClownState(ClownStates.Wait);
                }
                StartCoroutine(PlayerDamageScene());
                break;
            case 1:
                Debug.Log("Slight Chuckle");
                SetClownState(ClownStates.Chuckle);
                StartCoroutine(PlayerDamageScene());
                break;
            case 2:
            case 3:
                Debug.Log("Clown Lost 1 Life");
                ClownLives--;
                SetClownState(ClownStates.Laugh);
                AudioManager.instance.PlayOneShot(FMODEvents.instance.laughometerMid, LaughoMeter.transform.position);
                StartCoroutine(ClownDamageScene());
                break;            
            case >= 4:
                Debug.Log("Clown Loses 2 Lives!!!");
                ClownLives -= 2;
                SetClownState(ClownStates.BigLaugh);
                AudioManager.instance.PlayOneShot(FMODEvents.instance.laughometerFull, LaughoMeter.transform.position);
                StartCoroutine(ClownDamageScene());
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
        }
    }

    public void UpdateClownHealthUI()
    {
        for (int i = 0; i < lightBulbs.Length; i++)
        {
            if(i >= ClownLives)
            {
                lightBulbs[i].SetActive(false);
                AudioManager.instance.PlayOneShot(FMODEvents.instance.lightExplode, lightBulbs[i].transform.position);
                brokenLightBulbs[i].SetActive(true);
                AudioManager.instance.musicEventInstance.setParameterByName("ClownHealth", ClownLives);
            }
            else
            {
                lightBulbs[i].SetActive(true);
                brokenLightBulbs[i].SetActive(false);
            }
        }
    }

    public void SetClownState(ClownStates clownState)
    {
        switch (clownState)
        {
            case ClownStates.Angry:
                Debug.Log("Clown is angry");
                clownAnimator.SetTrigger("Angry");
                AudioManager.instance.PlayOneShot(FMODEvents.instance.clownNotConvinced, clown.position);
                break;
            case ClownStates.BigLaugh:
                Debug.Log("Clown is big laughing");
                clownAnimator.SetTrigger("BigLaugh");
                AudioManager.instance.PlayOneShot(FMODEvents.instance.bigClownLaugh, clown.position);
                break;
            case ClownStates.Chuckle:
                Debug.Log("Clown is chuckling");
                clownAnimator.SetTrigger("Chuckle");
                AudioManager.instance.PlayOneShot(FMODEvents.instance.clownGiggle, clown.position);
                break;
            case ClownStates.Idle:
                Debug.Log("Clown is idle");
                clownAnimator.SetTrigger("Idle");
                break;
            case ClownStates.Wait:
                Debug.Log("Clown is waiting");
                clownAnimator.SetTrigger("Wait");
                break;
            case ClownStates.Laugh:
                Debug.Log("Clown is laughing");
                clownAnimator.SetTrigger("Laugh");
                AudioManager.instance.PlayOneShot(FMODEvents.instance.clownLaugh, clown.position);
                break;
            case ClownStates.Die:
                clownAnimator.SetTrigger("Die");
                break;
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


    IEnumerator ClownDamageScene()
    {
        switchCamState.canMove = false;
        switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
        yield return new WaitForSeconds(2f);
        switchCamState.SwitchCamView(SwitchCamState.CamView.Right);
        yield return new WaitForSeconds(2f);
        switchCamState.SwitchCamView(SwitchCamState.CamView.Lights);
        yield return new WaitForSeconds(1f);
        UpdateClownHealthUI();
        if(ClownLives <= 0)
        {
            ClownDeath();
        }
        else
        {
            yield return new WaitForSeconds(2f);
            switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
            switchCamState.canMove = true;

            if (ClownLives <= 3 && ClownLives > 0)
            {
                SetClownState(ClownStates.Angry);
            }

            ElevatorAnimation.Instance.SendUp();
            currentPhase = TurnPhase.Draw;
        }
    }

    IEnumerator PlayerDamageScene()
    {
        switchCamState.canMove = false;
        switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
        yield return new WaitForSeconds(2f);
        switchCamState.SwitchCamView(SwitchCamState.CamView.Right);
        yield return new WaitForSeconds(2f);
        switchCamState.SwitchCamView(SwitchCamState.CamView.Left);
        yield return new WaitForSeconds(1f);
        playerHP.TakeDamage();
        SetClownState(ClownStates.Idle);
        yield return new WaitForSeconds(2f);
        switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
        switchCamState.canMove = true;
        ElevatorAnimation.Instance.SendUp();
        currentPhase = TurnPhase.Draw;
    }

    public void ClownDeath()
    {
        if (ClownLives <= 0)
        {
            StartCoroutine(clownDeath.DeathSequence());

            Debug.Log("Clown has been defeated");
        }
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