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
        Statistics.text = "Статистика игры:\n" +
                          "\nХодов куриц: " + Map.Init.ChickenStepsCount.ToString() +
                          "\nХодов лис:" + Map.Init.FoxStepsCount.ToString() +
                          "\nСъедено куриц:" + Map.Init.ChickensEatCount.ToString();
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
