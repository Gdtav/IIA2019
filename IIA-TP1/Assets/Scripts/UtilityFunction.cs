using UnityEngine;
using System.Collections;
using System;

public class UtilityFunction
{

    public float evaluate(State s)
    {
		if(s.PlayersUnits.Count == 0)
            return Int32.MinValue;
        else if(s.AdversaryUnits.Count == 0)
            return Int32.MaxValue;
        return 0;
    }
}
