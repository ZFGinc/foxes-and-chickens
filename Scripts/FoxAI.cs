using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public static class FoxAI
{
    public static byte[,] map;

    public static void FoxStep(byte[,] _map)
    {
        map = _map;

        Vector2Int[] foxs = GetPositionFoxs();

        if(foxs.Length != 2)
        {
            map[0, 0] = 3;
        }

        List<List<Vector2Int>> AllSteps = new List<List<Vector2Int>>();

        AllSteps.Add(get_step(foxs[0], true));
        AllSteps.Add(get_step(foxs[1], true));

        for (int k = 0; k < AllSteps.Count; k++)
        {
            while (AllSteps[k].Count > 2)
            {
                Vector2Int first = AllSteps[k][0];
                for (int i = 1; i < AllSteps[k].Count; i++)
                {
                    List<Vector2Int> temp = new List<Vector2Int>();
                    temp.Add(first);
                    temp.Add(AllSteps[k][i]);
                    AllSteps.Add(temp);
                }
                AllSteps.RemoveAt(k);
            }
        }

        AllSteps = GetMaximumSteps(AllSteps, map);

        List<Vector2Int> maxStep = new List<Vector2Int>();
        foreach(List<Vector2Int> step in AllSteps)
        {
            if(step.Count > maxStep.Count)
                maxStep = step;
        }
        GoSteps(maxStep);
    }

    private static void GoSteps(List<Vector2Int> steps)
    {
        Map.Init.FoxStepsCount += steps.Count-1;
        Map.Init.ChickensEatCount += steps.Count-1;
        for (int i = 1; i < steps.Count; i++)
        {
            Vector2Int fox = new Vector2Int(steps[i-1].x, steps[i-1].y);
            Vector2Int next = new Vector2Int(steps[i].x, steps[i].y);

            if(fox.x == next.x)
            {
                if(fox.y < next.y)
                {
                    Map.Init.byteMap[fox.x, fox.y] = 0;
                    Map.Init.byteMap[next.x, next.y - 1] = 0;
                    Map.Init.byteMap[next.x, next.y] = 2;
                }
                else
                {
                    Map.Init.byteMap[fox.x, fox.y] = 0;
                    Map.Init.byteMap[next.x, next.y + 1] = 0;
                    Map.Init.byteMap[next.x, next.y] = 2;
                }
            }
            else
            {
                if (fox.x < next.x)
                {
                    Map.Init.byteMap[fox.x, fox.y] = 0;
                    Map.Init.byteMap[next.x - 1, next.y] = 0;
                    Map.Init.byteMap[next.x, next.y] = 2;
                }
                else
                {
                    Map.Init.byteMap[fox.x, fox.y] = 0;
                    Map.Init.byteMap[next.x + 1, next.y] = 0;
                    Map.Init.byteMap[next.x, next.y] = 2;
                }
            }
        }
        if (steps.Count == 1) 
            RandomStep();
        Map.Init.ReDrawMap();
    }

    private static List<List<Vector2Int>> GetMaximumSteps(List<List<Vector2Int>> allSteps, byte[,] map)
    {
        int countSteps = 0;
        for(int k = 0; k < allSteps.Count; k++) {
            List<Vector2Int> step = allSteps[k];
            List<Vector2Int> newSteps = new List<Vector2Int>();
            List<Vector2Int> deleteSteps = new List<Vector2Int>();

            if (step.Count > 1) newSteps = get_step(step[step.Count-1]);

            for(int i = 0; i < newSteps.Count; i++)
                if (step.Contains(newSteps[i])) deleteSteps.Add(newSteps[i]);
            foreach (Vector2Int del in deleteSteps)
                newSteps.Remove(del);

            if (newSteps.Count > 1) 
            {
                int last = newSteps.Count - 1;
                for (int i = 0; i < newSteps.Count; i++)
                {
                    List<Vector2Int> temp = new List<Vector2Int>(step);
                    if (i == last) temp = step;
                    temp.Add(newSteps[i]);
                    if (i != last) allSteps.Add(temp);
                }
                countSteps = newSteps.Count;
            } else if(newSteps.Count > 0) step.Add(newSteps[0]);
        }

        if (countSteps > 0) allSteps = GetMaximumSteps(allSteps, map);

        return allSteps;
    }

    private static List<Vector2Int> get_step(Vector2Int fox, bool isAdd = false)
    {
        List<Vector2Int> steps = new List<Vector2Int>();
        if (isAdd) steps.Add(fox);
        int r = fox.x;
        int f = fox.y;

        if (r > 1 && is_valid(r - 2, f) && map[r - 1, f] == 1 && map[r - 2, f] == 0) steps.Add(new Vector2Int(r - 2, f));
        if (r < 5 && is_valid(r + 2, f) && map[r + 1, f] == 1 && map[r + 2, f] == 0) steps.Add(new Vector2Int(r + 2, f));
        if (f > 1 && is_valid(r, f - 2) && map[r, f - 1] == 1 && map[r, f - 2] == 0) steps.Add(new Vector2Int(r, f - 2));
        if (f < 5 && is_valid(r, f + 2) && map[r, f + 1] == 1 && map[r, f + 2] == 0) steps.Add(new Vector2Int(r, f + 2));

        return steps;
    }

    private static List<Vector2Int> get_step_r(Vector2Int fox)
    {
        List<Vector2Int> steps = new List<Vector2Int>();
        int r = fox.x;
        int f = fox.y;

        if (r > 1 && is_valid(r - 1, f) && map[r - 1, f] == 0) steps.Add(new Vector2Int(r - 1, f));
        if (r < 5 && is_valid(r + 1, f) && map[r + 1, f] == 0) steps.Add(new Vector2Int(r + 1, f));
        if (f > 1 && is_valid(r, f - 1) && map[r, f - 1] == 0) steps.Add(new Vector2Int(r, f - 1));
        if (f < 5 && is_valid(r, f + 1) && map[r, f + 1] == 0) steps.Add(new Vector2Int(r, f + 1));

        return steps;
    }

    private static bool is_valid(int x, int y)
    {
        if ((x == 0 || x == 1 || x == 5 || x == 6) &&
                 (y == 0 || y == 1 || y == 5 || y == 6)) return false;
        else if (x >= 0 && x <= 6 && y >= 0 && y <= 6) return true;
        else return false;
    }

    private static Vector2Int[] GetPositionFoxs()
    {
        List<Vector2Int> foxs = new List<Vector2Int>();

        for(int i = 0; i < 7; i++)
        {
            for(int j = 0; j < 7; j++)
            {
                if(map[i,j] == 2) foxs.Add(new Vector2Int(i,j));
            }
        }

        return foxs.ToArray();
    }

    private static void RandomStep()
    {
        Vector2Int[] foxs = GetPositionFoxs();

        int i = UnityEngine.Random.Range(0, 2);

        List<Vector2Int> curr = get_step_r(foxs[i]);
        
        int id = UnityEngine.Random.Range(0, curr.Count);

        if (curr.Count > 0)
        {
            Map.Init.byteMap[foxs[i].x, foxs[i].y] = 0;
            Map.Init.byteMap[curr[id].x, curr[id].y] = 2;
            Map.Init.FoxStepsCount += 1;
            Map.Init.ReDrawMap();
        }
    }
}
