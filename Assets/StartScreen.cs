using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreen : MonoBehaviour
{
    public bool startGame;
    [SerializeField] private Lightswitch lightSwitch;
    [SerializeField] private Vector3 startPos;
    [SerializeField] private Vector3 endPos;
    float timer;
    [SerializeField] private float maxTimer;
    bool isPlaying;
    [SerializeField] private SwitchCamState switchCamState;
    bool isPlayingCoroutine;
    [SerializeField] private Texture2D startCursor;

    private void Start()
    {
        Cursor.SetCursor(startCursor, new Vector2(10, 10), CursorMode.Auto);
    }

    private void Update()
    {
        if (Input.anyKeyDown && !startGame && !isPlayingCoroutine)
        {
            StartCoroutine(StartGame());
        }

        if (isPlaying && timer <= maxTimer)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, endPos, timer / maxTimer);
        }
        else if(isPlaying)
        {
            startGame = true;
            isPlaying = false;
        }
    }

    IEnumerator StartGame()
    {
        isPlayingCoroutine = true;
        yield return new WaitForSeconds(2f);
        AudioManager.instance.PlayOneShot(FMODEvents.instance.LightsOn, transform.position - new Vector3(0, 0, -20f));
        lightSwitch.LightOnOff(true);
        yield return new WaitForSeconds(2f);
        isPlaying = true;
        yield return new WaitForSeconds(2f);
        switchCamState.SwitchCamView(SwitchCamState.CamView.Front);
        ElevatorAnimation.Instance.SendUp();
    }
}
