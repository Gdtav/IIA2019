using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DeepCopyExtensions;

public class MinMaxAlgorithm: MoveMaker
{
    public EvaluationFunction evaluator;
    private UtilityFunction utilityfunc; 
    public int MaxDepth = 4;
    private PlayerController MaxPlayer;
    private PlayerController MinPlayer;
	private System.Random random;
    private bool AlphaBeta;
    
    public MinMaxAlgorithm(bool AlphaBeta, int MaxDepth, PlayerController MaxPlayer, EvaluationFunction eval, UtilityFunction utilf, PlayerController MinPlayer)
    {
        this.MaxPlayer = MaxPlayer;
        this.MinPlayer = MinPlayer;
        this.evaluator = eval;
        this.utilityfunc = utilf;
        this.AlphaBeta = AlphaBeta;
		this.MaxDepth = MaxDepth;
    }

    public override State MakeMove()
    {
        // The move is decided by the selected state
        return GenerateNewState(); 
    }

    private State GenerateNewState()
    {
        // Creates initial state
        State root = new State(this.MaxPlayer, this.MinPlayer);
		List<State> tree = GeneratePossibleStates(root);
		tree.Sort();
		State bestnode = null;

		foreach(State node in tree)
		{
			if(bestnode == null){
				bestnode = node;
			} 

            if(this.AlphaBeta == false)	
			    node.Score = Min(node);
            else
                node.Score = Min(node,float.MinValue, float.MaxValue);

			if(bestnode.CompareTo(node) < 0)
			{
				bestnode = node;
			}
		}

		this.MaxPlayer.ExpandedNodes = 0;
        return bestnode;
    }

    public float Max(State parent)
    {
        if(utilityfunc.evaluate(parent) != 0)
        {
			return utilityfunc.evaluate(parent);
        }

        if(parent.depth >= this.MaxDepth || this.MaxPlayer.ExpandedNodes >= this.MaxPlayer.MaximumNodesToExpand)
        {
			return evaluator.evaluate(parent);
        }

        List<State> tree = GeneratePossibleStates(parent);
		float max= float.MinValue;

        foreach(State node in tree)
        {	
            max = Math.Max(max, Min(node));
        }

		return max;
    }

    public float Min(State parent)
    {
        if(utilityfunc.evaluate(parent) != 0)
        {
			return utilityfunc.evaluate(parent);
        }

        if(parent.depth >= this.MaxDepth || this.MaxPlayer.ExpandedNodes >= this.MaxPlayer.MaximumNodesToExpand)
        {
			return evaluator.evaluate(parent);
        }

        List<State> tree = GeneratePossibleStates(parent);
		float min = float.MaxValue;

        foreach(State node in tree)
        {
			min = Math.Min(min, Max(node));
        }

		return min;
    }


    public float Max(State parent, float alpha, float beta)
    {
        if(utilityfunc.evaluate(parent) != 0)
        {
			return utilityfunc.evaluate(parent);
        }

        if(parent.depth >= this.MaxDepth || this.MaxPlayer.ExpandedNodes >= this.MaxPlayer.MaximumNodesToExpand)
        {
			return evaluator.evaluate(parent);
        }

        List<State> tree = GeneratePossibleStates(parent);
		float max = float.MinValue;

        foreach(State node in tree)
        {
            max = Math.Max(max, Min(node, alpha, beta));

            if(max >= beta)
                return max;

            alpha = Math.Max(alpha,max);
        }

		return max;
    }

    public float Min(State parent, float alpha, float beta)
    {
        if(utilityfunc.evaluate(parent) != 0)
        {
			return utilityfunc.evaluate(parent);
        }

        if(parent.depth >= this.MaxDepth || this.MaxPlayer.ExpandedNodes >= this.MaxPlayer.MaximumNodesToExpand)
        {
			return evaluator.evaluate(parent);
        }

        List<State> tree = GeneratePossibleStates(parent);
		float min = float.MaxValue;

        foreach(State node in tree)
        {
            min = Math.Min(min, Max(node, alpha, beta));

            if(min <= alpha)
                return min;

            beta = Math.Min(beta,min);
        }

		return min;
    }

    private List<State> GeneratePossibleStates(State state)
    {
        List<State> states = new List<State>();
        //Generate the possible states available to expand
        foreach(Unit currentUnit in state.PlayersUnits)
        {
            // Movement States
            List<Tile> neighbours = currentUnit.GetFreeNeighbours(state);
            foreach (Tile t in neighbours)
            {
                State newState = new State(state, currentUnit, true);
                newState = MoveUnit(newState, t);
                states.Add(newState);
            }
            // Attack states
            List<Unit> attackOptions = currentUnit.GetAttackable(state, state.AdversaryUnits);
            foreach (Unit t in attackOptions)
            {
                State newState = new State(state, currentUnit, false);
                newState = AttackUnit(newState, t);
                states.Add(newState);
            }

        }

        // YOU SHOULD NOT REMOVE THIS
        // Counts the number of expanded nodes;
        this.MaxPlayer.ExpandedNodes += states.Count;
        //

        return states;
    }

    private State MoveUnit(State state,  Tile destination)
    {
        Unit currentUnit = state.unitToPermormAction;
        //First: Update Board
        state.board[(int)destination.gridPosition.x, (int)destination.gridPosition.y] = currentUnit;
        state.board[currentUnit.x, currentUnit.y] = null;
        //Second: Update Players Unit Position
        currentUnit.x = (int)destination.gridPosition.x;
        currentUnit.y = (int)destination.gridPosition.y;
        state.isMove = true;
        state.isAttack = false;
        return state;
    }

    private State AttackUnit(State state, Unit toAttack)
    {
        Unit currentUnit = state.unitToPermormAction;
        Unit attacked = toAttack.DeepCopyByExpressionTree();

        Tuple<float, float> currentUnitBonus = currentUnit.GetBonus(state.board, state.PlayersUnits);
        Tuple<float, float> attackedUnitBonus = attacked.GetBonus(state.board, state.AdversaryUnits);


        attacked.hp += Math.Min(0, (attackedUnitBonus.Item1)) - (currentUnitBonus.Item2 + currentUnit.attack);
        state.unitAttacked = attacked;

        state.board[attacked.x, attacked.y] = attacked;
        int index = state.AdversaryUnits.IndexOf(attacked);
        state.AdversaryUnits[index] = attacked;



        if (attacked.hp <= 0)
        {
            //Board update by killing the unit!
            state.board[attacked.x, attacked.y] = null;
            index = state.AdversaryUnits.IndexOf(attacked);
            state.AdversaryUnits.RemoveAt(index);

        }
        state.isMove = false;
        state.isAttack = true;

        return state;

    }
}
