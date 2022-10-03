using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIData : MonoBehaviour
{
    public TMP_Text Statistics;
    public GameObject PanwelWin;
    public GameObject PanwelLoss;
    public GameObject PanwelDraw;
    public static UIData Init;

    private void Start()
    {
        Init = this;
        PanwelWin.SetActive(false);
    }

    private void FixedUpdate()
    {
        Statistics.text = "���������� ����:\n" +
                          "\n����� �����: " + Map.Init.ChickenStepsCount.ToString() +
                          "\n����� ���:" + Map.Init.FoxStepsCount.ToString() +
                          "\n������� �����:" + Map.Init.ChickensEatCount.ToString();
    }

    public void ShowWinWindow()
    {
        PanwelWin.SetActive(true);
    }

    public void ShowLoassWindow()
    {
        PanwelLoss.SetActive(true);
    }

    public void ShowDrawWindow()
    {
        PanwelDraw.SetActive(true);
    }

    public void HardGame()
    {
        Map.Init.HardMap();
    }


    public void RestartGame()
    {
        Application.LoadLevel(0);
    }
}
