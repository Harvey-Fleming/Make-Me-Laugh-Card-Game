using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerRedraws : MonoBehaviour
{
    [SerializeField] private int playerRedraws;
    [SerializeField] private Text redrawText;

    private void Update()
    {
        UpdateRedrawUI();
    }

    public void UpdateRedrawUI()
    {
        redrawText.text = playerRedraws.ToString();
    }
}
