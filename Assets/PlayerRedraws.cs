using UnityEngine;
using TMPro;

public class PlayerRedraws : MonoBehaviour
{
    private int playerRedraws;
    [SerializeField] private TMP_Text redrawText;

    public void UpdateRedrawUI(int Redraws)
    {
        playerRedraws = Redraws;
        redrawText.text = playerRedraws.ToString();
    }


    #region - Event Subscription
    private void OnEnable()
    {
        PlayerHand.OnRedrawNumUpdate += UpdateRedrawUI;
    }

    private void OnDisable()
    {
        PlayerHand.OnRedrawNumUpdate -= UpdateRedrawUI;
    }
    #endregion
}
