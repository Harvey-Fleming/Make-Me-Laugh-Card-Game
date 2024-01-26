using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class wasdUI : MonoBehaviour
{
    Image image;

    [SerializeField] StartScreen startScreen;

    [SerializeField] private TextMeshProUGUI anyKeyText;

    bool hasPressed;

    float cycle;

    void Start()
    {
        image = GetComponent<Image>();
        image.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {

        anyKeyText.color = new Color(1f, 1f, 1f, Mathf.Abs(Mathf.Sin(Time.time) / 2f));

        if (Input.anyKeyDown && anyKeyText.gameObject.activeSelf)
        {
            anyKeyText.gameObject.SetActive(false);
        }

        if (startScreen.startGame && !hasPressed)
        {
            image.enabled = true;

            cycle += Time.deltaTime;

            image.color = new Color(1f, 1f, 1f, Mathf.Abs(Mathf.Sin(cycle) / 2f));
            
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
            {
                hasPressed = true;
            }
        }
        else
        {
            image.enabled = false;
        }
    }
}
