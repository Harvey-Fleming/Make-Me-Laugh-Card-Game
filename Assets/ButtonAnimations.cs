using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonAnimations : MonoBehaviour
{
    public static ButtonAnimations Instance;
    Animator animator;

    private void Awake()
    {
        if (Instance != null)
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

    public void PushButton()
    {
        Debug.Log("Button Should go down");
        animator.SetTrigger("pushButton");
        AudioManager.instance.PlayOneShot(FMODEvents.instance.ButtonPress, this.transform.position);
    }
}
