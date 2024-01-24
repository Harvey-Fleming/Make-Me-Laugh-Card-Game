using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchCamState : MonoBehaviour
{
    [Header("Top View")]
    [SerializeField] public Vector3 topViewPos;
    [SerializeField] public Vector3 topViewRot;
    [Header("Front View")]
    [SerializeField] private Vector3 frontViewPos;
    [SerializeField] private Vector3 frontViewRot;
    [Header("Left View")]
    [SerializeField] private Vector3 leftViewPos;
    [SerializeField] private Vector3 leftViewRot;
    [Header("Right View")]
    [SerializeField] private Vector3 rightViewPos;
    [SerializeField] private Vector3 rightViewRot;
    [Header("Lights View")]
    [SerializeField] private Vector3 lightsViewPos;
    [SerializeField] private Vector3 lightsViewRot;
    public enum CamView { Top, Front, Left, Right, Lights, CoinSlot};
    public CamView currentView;

    private Vector3 nextCamPos;
    private Quaternion nextCamRot;


    [Header("Move Time")]
    [SerializeField] bool isLerping;
    [SerializeField] private float maxMoveTime;
    [SerializeField] private float timer;

    private void Start()
    {
        currentView = CamView.Front;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            SwitchCamView(CamView.Top);
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            if (currentView == CamView.Right)
            {
                SwitchCamView(CamView.Top);
            }
            else
            {
                SwitchCamView(CamView.Left);
            }

        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            SwitchCamView(CamView.Front);
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            if (currentView == CamView.Left)
            {
                SwitchCamView(CamView.Top);
            }
            else
            {
                SwitchCamView(CamView.Right);
            }
        }

        if (isLerping)
        {
            if(timer < maxMoveTime)
            {
                timer += Time.deltaTime;
            }
            else
            {
                timer = maxMoveTime;
                isLerping = false;
            }

            transform.position = Vector3.Lerp(transform.position, nextCamPos, timer / maxMoveTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, nextCamRot, timer / maxMoveTime);
        }
    }

    public void SwitchCamView(CamView newCamView)
    {
        switch (newCamView)
        {
            case CamView.Top:
                currentView = CamView.Top;
                nextCamPos = topViewPos;
                nextCamRot = Quaternion.Euler(topViewRot);
                timer = 0f;
                isLerping = true;
                break;

            case CamView.Front:
                currentView = CamView.Front;
                nextCamPos = frontViewPos;
                nextCamRot = Quaternion.Euler(frontViewRot);
                timer = 0f;
                isLerping = true;
                break;

            case CamView.Left:
                currentView = CamView.Left;
                nextCamPos = leftViewPos;
                nextCamRot = Quaternion.Euler(leftViewRot);
                timer = 0f;
                isLerping = true;
                break;

            case CamView.Right:
                currentView = CamView.Right;
                nextCamPos = rightViewPos;
                nextCamRot = Quaternion.Euler(rightViewRot);
                timer = 0f;
                isLerping = true;
                break;

            case CamView.Lights:
                currentView = CamView.Lights;
                nextCamPos = lightsViewPos;
                nextCamRot = Quaternion.Euler(lightsViewRot);
                timer = 0f;
                isLerping = true;
                break;
        }
    }
}
