using UnityEngine;
using System.Collections;
using System;

public class EvaluationFunction
{
    // Do the logic to evaluate the state of the game !
    public float evaluate(State s)
    {
        float score = 0, hp = 0, units = 0;

        foreach (Unit unit in s.PlayersUnits)
        {
            units++;
            hp += unit.hp;

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

                    if(perfectAttack(s,unit) == true)
                        score -= 10000;
                }
                else if(s.isAttack == true)
                {
                    switch(possibleEnemyAttack(s,unit))
                    {
                        case 0: score += 5000 - s.depth*100;break;
                        
                        case int val:
                            score += 1500 + unit.hp*2 - 250*val - s.depth*50;break;
                    }

                    if(s.unitAttacked.hp <= 0)
                        score += 20000 - s.depth*100;

                    Assassin ass = unit as Assassin;
                    if(ass != null)
                    {
                        if(Math.Abs(unit.x - s.unitAttacked.x) == 1 && Math.Abs(unit.y - s.unitAttacked.y) == 1)
                            score += 5000;
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

        score += hp/units + (float)Math.Pow(units,2);
        units = 0; hp = 0;

        foreach (Unit unit in s.AdversaryUnits)
        {
            units++;
            hp += unit.hp;
        }
        
        score -= hp/units - (float)Math.Pow(units,2);

		return score;
    }

    //  Returns score based on not taking damage(good) vs taking damage(bad) vs dying(vewy bad) on next turn
    
    private bool perfectAttack(State s, Unit guy)
    {
        foreach(Unit fodder in s.AdversaryUnits)
        {
            if(Math.Abs(guy.x - fodder.x) <= 2 && Math.Abs(guy.y - fodder.y) <= 2)
            {
                Assassin ass_A = guy as Assassin;
                Mage mag_A = guy as Mage;
                
                Assassin ass_E = fodder as Assassin;
                Mage mag_E = fodder as Mage;

                if(mag_E != null)
                {
                    return false;
                }
                else if(ass_E != null)
                {
                    if(Math.Abs(guy.x - fodder.x) <= 1 && Math.Abs(guy.y - fodder.y) <= 1)
                        return false;
                }

                
                if(mag_A != null)
                {
                    if(Math.Abs(guy.x - fodder.x) == 1 && guy.y == fodder.y)
                        return false;
                    if(Math.Abs(guy.y - fodder.y) == 1 && guy.x == fodder.x)
                        return false;
                }
                else if(ass_A == null)
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    private float avoidAttacks(State s, Unit guy)
    {
        return 0;
    }

    //  Returns the possible amount of attacks suffered
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
                    continue;
                }

                Mage mag = fodder as Mage;
                if(mag != null)
                {
                    if(Math.Abs(guy.x - fodder.x) <= 2 && guy.y == fodder.y)
                        attacks++;
                    if(Math.Abs(guy.y - fodder.y) <= 2 && guy.x == fodder.x)
                        attacks++;
                    continue;
                }

                if(mag == null && ass == null)
                {
                    if(Math.Abs(guy.x - fodder.x) == 1 && guy.y == fodder.y)
                        attacks++;
                    if(Math.Abs(guy.y - fodder.y) == 1 && guy.x == fodder.x)
                        attacks++;
                }

            }
        }
        return attacks;
    }
}
