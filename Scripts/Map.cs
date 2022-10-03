using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public GameObject[] MAP_CELLS;

    public byte[,] byteMap = new byte[7, 7];
    public map_cell[,] map = new map_cell[7, 7];

    public static Map Init;
    [HideInInspector] public int ChickenStepsCount = 0;
    [HideInInspector] public int FoxStepsCount = 0;
    [HideInInspector] public int ChickensEatCount = 0;


    void Start()
    {
        Init = this;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if ((i == 0 || i == 1 || i == 5 || i == 6) &&
                    (j == 0 || j == 1 || j == 5 || j == 6)) CreateObject(1, i, j, CellContent.NotStepping);
                else
                {
                    if (i == 2 && (j == 2 || j == 4)) CreateObject(0, i, j, CellContent.Fox);
                    else if (i >= 3) CreateObject(0, i, j, CellContent.Chiñken);
                    else CreateObject(0, i, j);
                }
            }
        }
    }

    public void HardMap()
    {
        byteMap = new byte[7,7]{
            { 3, 3, 1, 1, 1, 3, 3 },
            { 3, 3, 1, 1, 1, 3, 3 },
            { 0, 0, 0, 2, 1, 1, 0 },
            { 0, 0, 0, 0, 0, 0, 1 },
            { 0, 0, 0, 2, 0, 0, 0 },
            { 3, 3, 0, 0, 0, 3, 3 },
            { 3, 3, 0, 0, 0, 3, 3 }
        };
        ReDrawMap();
    }
    private void CreateObject(int id, int x = -1, int y = -1, CellContent cont = CellContent.None)
    {
        var obj = Instantiate(MAP_CELLS[id]);
        obj.transform.parent = this.transform;
        obj.transform.localScale = Vector3.one;
        map[x, y] = obj.GetComponent<map_cell>();
        byteMap[x, y] = (byte)((cont == CellContent.None) ? 0 : (cont == CellContent.NotStepping) ? 3 : (cont == CellContent.Chiñken) ? 1 : 2);
        if (id == 0) obj.GetComponent<map_cell>().Set(cont, x, y);
        if (y > 1 && y < 5 && x < 3) map[x, y].GetComponent<Image>().color = new Color(0.4f, 1f, 0.8f, 0.3921569f);
    }
    public bool GetChickensSteps(int i, int j)
    {
        bool isHase = false;

        if (j - 1 >= 0 && byteMap[i, j - 1] == 0)
        {
            map[i, j - 1].oldStep = new Vector2Int(i, j);
            map[i, j - 1].ButtonStep.SetActive(true);
            isHase = true;
        }
        if (j + 1 <= 6 && byteMap[i, j + 1] == 0)
        {
            map[i, j + 1].oldStep = new Vector2Int(i, j);
            map[i, j + 1].ButtonStep.SetActive(true);
            isHase = true;
        }
        if (i - 1 >= 0 && byteMap[i - 1, j] == 0)
        {
            map[i - 1, j].oldStep = new Vector2Int(i, j);
            map[i - 1, j].ButtonStep.SetActive(true);
            isHase = true;
        }
        return isHase;
    }
    public void UpdateMap(byte[,] newMap)
    {
        byteMap = newMap;

        ReDrawMap();
    }
    public void ReDrawMap()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                CellContent val = (byteMap[i, j] == 0) ? CellContent.None
                                                      : (byteMap[i, j] == 1) ? CellContent.Chiñken
                                                      : (byteMap[i, j] == 2) ? CellContent.Fox
                                                      : CellContent.NotStepping;
                map[i, j].Set(val, true);
            }
        }
    }
    public void HideAllSteps()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if ((i == 0 || i == 1 || i == 5 || i == 6) &&
                    (j == 0 || j == 1 || j == 5 || j == 6)) continue;
                
                map[i, j].HideSteps();
            }
        }
    }
    private void HideDeadChickens()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if ((i == 0 || i == 1 || i == 5 || i == 6) &&
                    (j == 0 || j == 1 || j == 5 || j == 6)) continue;

                map[i, j].deadChicken.SetActive(false);
            }
        }
    }
    public void ChickenStep(int x0, int y0, int x1, int y1)
    {
        byteMap[x0, y0] = 0;
        byteMap[x1, y1] = 1;
        map[x1, y1].Set(CellContent.Chiñken);
        map[x0, y0].Hide();
        HideAllSteps();
        HideDeadChickens();
        ChickenStepsCount++;
        FoxAI.FoxStep(byteMap);

        if (CheckWin()) UIData.Init.ShowWinWindow();
        else if (isLoss()) UIData.Init.ShowLoassWindow();
        else if (isDraw()) UIData.Init.ShowDrawWindow();
    }
    private bool CheckWin()
    {
        for (int i = 0; i < 3; i++)
        {
            for (int j = 2; j < 5; j++)
            {
                if (byteMap[i, j] == 0 || byteMap[i, j] == 2) return false;
            }
        }
        return true;
    }
    private bool isLoss()
    {
        if (ChickensEatCount > 11) return true;
        else return false;
    }
    private bool isDraw()
    {
        bool isDraw = true;
        for (int i = 0; i < 7; i++)
        {
            for (int j = 0; j < 7; j++)
            {
                if(byteMap[i,j] == 1 && GetChickensSteps(i,j)) { isDraw = false; break; }
            }
            if (!isDraw) break;
        }
        HideAllSteps();
        return isDraw;
    }
}
