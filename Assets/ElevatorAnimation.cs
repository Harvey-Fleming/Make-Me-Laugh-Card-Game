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

    [SerializeField] private PlayerHP playerHP;

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
        Debug.Log("send up function");
        animator.SetBool("isUp", true);

        if(playerHP.healthPoints > 0)
        {
            AudioManager.instance.PlayOneShot(FMODEvents.instance.cardLiftUp, this.transform.position);
        }
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
