﻿using UnityEngine;
using System.Collections;
using System;

public class EvaluationFunction
{
    // Do the logic to evaluate the state of the game !
    public float evaluate(State s)
    {
        float score = 0, hp = 0, units = 0;

		foreach(Unit unit in s.PlayersUnits)
        {
            units++;
            hp += unit.hp;
        }

        score += units*hp;
        units = 0; hp = 0;

        foreach(Unit unit in s.AdversaryUnits)
        {
            units++;
            hp += unit.hp;
        }
        
        score -= units*hp;

		if(s.unitAttacked != null)
			score += 2500;
		return score;
    }
}
