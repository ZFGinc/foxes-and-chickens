using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CellContent
{
    None,Chiñken,Fox,NotStepping
}

public class map_cell : MonoBehaviour
{
    [Header("Îáúåêòû âèçóàëà")]
    public GameObject Chiken;
    public GameObject Fox;
    public GameObject deadChicken;
    public GameObject ButtonStep;
    public Vector2Int oldStep;
    bool isDeadChicken = false;

    [Space(20)]
    [Header("Äàííûå ÿ÷åéêè")]
    public CellContent me;
    public int x, y;

    public void Set(CellContent val, int x, int y)
    {
        this.x = x;
        this.y = y;
        me = val;

        switch (me)
        {
            case CellContent.None:
                Chiken.SetActive(false);
                Fox.SetActive(false);
                break;
            case CellContent.Chiñken:
                Chiken.SetActive(true);
                Fox.SetActive(false);
                break;
            case CellContent.Fox:
                Chiken.SetActive(false);
                Fox.SetActive(true);
                break;
        }
    }
    public void Set(CellContent val, bool isFoxEat = false)
    {
        if (me == CellContent.Chiñken && val == CellContent.None && isFoxEat) DeadChicken();
        me = val;

        switch (me)
        {
            case CellContent.None:
                Chiken.SetActive(false);
                Fox.SetActive(false);
                break;
            case CellContent.Chiñken:
                Chiken.SetActive(true);
                Fox.SetActive(false);
                break;
            case CellContent.Fox:
                Chiken.SetActive(false);
                Fox.SetActive(true);
                break;
            default: break;
        }
    }
    public void ShowSteps()
    {
        if (me == CellContent.NotStepping || me == CellContent.Fox || me == CellContent.None) return;
        Map.Init.HideAllSteps();
        Map.Init.GetChickensSteps(x, y);
    }
    public void Hide()
    {
        HideSteps();
        me = CellContent.None;
        Chiken.SetActive(false);
    }
    public void HideSteps()
    {
        ButtonStep.SetActive(false);
    }
    public void Step()
    {
        Map.Init.ChickenStep(oldStep.x, oldStep.y, x, y);
    }
    public void DeadChicken()
    {
        deadChicken.SetActive(true);
    }
}
