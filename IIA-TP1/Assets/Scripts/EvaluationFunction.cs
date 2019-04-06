using UnityEngine;
using System.Collections;
using System;

public class EvaluationFunction
{
    // Do the logic to evaluate the state of the game !
    public float evaluate(State s)
    {
        float score = 0;

        foreach (Unit unit in s.PlayersUnits)
        {
            if(unit == s.unitToPermormAction)
            {
                if(s.isMove == true)
                {
                    switch(possibleEnemyAttack(s,unit))
                    {
                        case 0: score += 1000;break;
                        
                        case int val:
                            score -= 250*val;break;
                    }
                }
                else if(s.isAttack == true)
                {
                    switch(possibleEnemyAttack(s,unit))
                    {
                        case 0: score += 5000 - s.depth*100;break;
                        
                        case int val:
                            score += 1500 + unit.hp*2 - 250*val - s.depth*50;break;
                    }
                }
            }

            else
            {
                switch(possibleEnemyAttack(s,unit))
                    {
                        case 0: score += 1000;break;
                        
                        case int val:
                            score -= 150*val + unit.hp*5;break;
                    }
            }


        }

        /*foreach (Unit unit in s.AdversaryUnits)
        {

        }*/
        
		return score;
    }

    //  Returns score based on not taking damage(good) vs taking damage(bad) vs dying(vewy bad) on next turn
    private float avoidAttacks(State s, Unit guy)
    {
        return 0;
    }

    //  Returns the possible amount of attacks
    private int possibleEnemyAttack(State s, Unit guy)
    {
        int attacks = 0;

        foreach(Unit fodder in s.AdversaryUnits)
        {
            if(Math.Abs(guy.x - fodder.x) <= 2 && Math.Abs(guy.y - fodder.y) <= 2)
            {
                Assassin ass = fodder as Assassin;
                if(ass != null)
                {
                    if(Math.Abs(guy.x - fodder.x) <= 1 && Math.Abs(guy.y - fodder.y) <= 1)
                        attacks++;
                }

                Mage mag = fodder as Mage;
                if(mag != null)
                {
                    attacks++;
                }

                if(Math.Abs(guy.x - fodder.x) == 1 && guy.y == fodder.y)
                    attacks++;
                if(Math.Abs(guy.y - fodder.y) == 1 && guy.x == fodder.x)
                    attacks++;

            }
        }
        return attacks;
    }
}
