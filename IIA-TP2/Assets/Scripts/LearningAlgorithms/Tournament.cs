using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Tournament : SelectionMethod
{
    public override List<Individual> selectIndividuals(List<Individual> oldpop, int num, double k)
    {
        int i, j;
        System.Random r = new System.Random();
        List<Individual> victors = new List<Individual>();
        while (victors.Count < num)
        {
            i = r.Next(oldpop.Count);
            j = r.Next(oldpop.Count);
            if (r.NextDouble() > k || oldpop[i].Fitness > oldpop[j].Fitness)
                victors.Add(oldpop[i].Clone());
            else
                victors.Add(oldpop[j].Clone());
        }
        return victors;
    }

}
