using UnityEngine;
using System.Collections;
using System;

public class EvaluationFunction
{
    // Do the logic to evaluate the state of the game !
    public float evaluate(State s)
    {
        float score = 0, hp_A = 0, units_A = 0, hp_E = 0, units_E = 0, hp_og = 0, units_og = 0;

        foreach (Unit unit in s.PlayersUnits)
        {
            units_A++;
            hp_A += unit.hp;

            switch(getBonus(s,unit))
            {
                case -1: score -= 500000;break;
                case 0: break;
                case 1: score += 250000;break;
                case 2: score += 500000;break;
            }
           if(unit == s.unitToPermormAction)
            {
                if(s.isMove == true)
                {
                    switch(possibleEnemyAttack(s,unit))
                    {
                        case 0: score += 1000;break;
                        
                        case int val:
                            score -= 10*val;break;
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
                            score += 1500 + unit.hp*2 - 5*val - s.depth*50;break;
                    }

                    if(s.unitAttacked.hp <= 0)
                        score += 20000 - s.depth*100;

                    Assassin ass = unit as Assassin;
                    if(ass != null)
                    {
                        if(Math.Abs(unit.x - s.unitAttacked.x) == 1 && Math.Abs(unit.y - s.unitAttacked.y) == 1)
                            score += 10000;
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

        foreach (Unit unit in s.AdversaryUnits)
        {
            units_E++;
            hp_E += unit.hp;
        }
        
        State x = s;
        while(!x.isRoot)
            x = x.parentState;

        foreach (Unit unit in x.PlayersUnits)
        {
            units_og++;
            hp_og += unit.hp;
        }

        score += 20*(hp_og + hp_A) - (float)Math.Pow(150,Math.Abs(units_A - units_og)) - s.depth*50;

        units_og = 0; hp_og = 0;

        foreach (Unit unit in x.AdversaryUnits)
        {
            units_og++;
            hp_og += unit.hp;
        }

        score += 15*(Math.Abs(hp_E - hp_og)) + (float)Math.Pow(120,Math.Abs(units_E - units_og)) - s.depth*100;

		return score;
    }

    //  Returns score based on not taking damage(good) vs taking damage(bad) vs dying(vewy bad) on next turn
    
    private int getBonus(State s, Unit guy)
    {
        int count = 0;
        if(s.PlayersUnits.Count == 1)
            return 0;

        Assassin ass = guy as Assassin;
        if(ass != null)
            return 1;
        
        foreach(Unit ally in s.PlayersUnits)
        {
            if(Math.Abs(guy.x - ally.x) <= 1 && Math.Abs(guy.y - ally.y) <= 1)
            {
                Assassin assy = ally as Assassin;
                if(assy != null)
                    count++;
            }
        }
        
        if(count == 0)
            return -1;

        return count;
    }
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
                        attacks += ass.attack;
                    continue;
                }

                Mage mag = fodder as Mage;
                if(mag != null)
                {
                    if(Math.Abs(guy.x - fodder.x) <= 2 && guy.y == fodder.y)
                        attacks += mag.attack;
                    if(Math.Abs(guy.y - fodder.y) <= 2 && guy.x == fodder.x)
                        attacks += mag.attack;
                    continue;
                }

                if(mag == null && ass == null)
                {
                    if(Math.Abs(guy.x - fodder.x) == 1 && guy.y == fodder.y)
                        attacks += fodder.attack;
                    if(Math.Abs(guy.y - fodder.y) == 1 && guy.x == fodder.x)
                        attacks += fodder.attack;
                }

            }
        }
        return attacks;
    }
}
