using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ScoringSystem
{
    private static int boxesBroken;
    private static int wumpaScore;
    private static int lifeScore;

    public static void AddABox()
    {
        boxesBroken += 1;
    }

    public static void RemoveABox()
    {
        boxesBroken -= 1;
    }

    public static void AddAWumpa()
    {
        wumpaScore += 1;
        if(wumpaScore >= 100)
        {
            wumpaScore -= 100;
            lifeScore += 1;
        }
    }

    public static void AddALife()
    {
        lifeScore += 1;
    }

    public static int BoxesBroken()
    {
        return boxesBroken;
    }

    public static int Wumpas()
    {
        return wumpaScore;
    }

    public static int Lives()
    {
        return lifeScore;
    }
}
