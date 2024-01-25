using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorAnimation : MonoBehaviour
{
    public static ElevatorAnimation Instance;

    [SerializeField] private List<Transform> elevatorSlots = new();

    Animator animator;

    public List<Transform> ElevatorSlots { get => elevatorSlots;}

    public delegate void ElevatorEvent(bool isUp);
    public static event ElevatorEvent OnElevatorStop;

    private void Awake()
    {
        if(Instance != null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SendDown()
    {
        animator.SetBool("isUp", false);

    }    

    public void SendUp()
    {
        animator.SetBool("isUp", true);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.cardLiftUp, this.transform.position);
    }

    public void ElevatorUp()
    {
        OnElevatorStop(true);
    }

    public void ElevatorDown()
    {
        OnElevatorStop(false);
    }
}
