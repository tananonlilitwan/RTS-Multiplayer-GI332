using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject winPanel;
    public GameObject losePanel;
    public GameObject teamAPanel;
    public GameObject teamBPanel;

    public void ShowWin(int team)
    {
        winPanel.SetActive(true);
        if (team == 0) teamAPanel.SetActive(false);
        else if (team == 1) teamBPanel.SetActive(false);
    }

    public void ShowLose(int team)
    {
        losePanel.SetActive(true);
        if (team == 0) teamAPanel.SetActive(false);
        else if (team == 1) teamBPanel.SetActive(false);
    }
}