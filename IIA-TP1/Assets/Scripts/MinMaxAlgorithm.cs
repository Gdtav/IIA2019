﻿using System;
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
    private bool AlphaBeta;
    
    public MinMaxAlgorithm(bool AlphaBeta, PlayerController MaxPlayer, EvaluationFunction eval, UtilityFunction utilf, PlayerController MinPlayer)
    {
        this.MaxPlayer = MaxPlayer;
        this.MinPlayer = MinPlayer;
        this.evaluator = eval;
        this.utilityfunc = utilf;
        this.AlphaBeta = AlphaBeta;
    }

    public override State MakeMove()
    {
        // The move is decided by the selected state
        return GenerateNewState(); 
    }

    private State GenerateNewState()
    {
        // Creates initial state
        State initialState = new State(this.MaxPlayer, this.MinPlayer);
        return MinMax(initialState,true);
    }

    // Root is the state of entry
    public State MinMax(State root)
    {      
        List<State> tree = GeneratePossibleStates(root);

        State bestnode = null;

        foreach(State node in tree)
        {
            if(bestnode == null){
                bestnode = node;
                continue;
            } 

            Max(node);
            
            if(root.CompareTo(node) < 0)
                bestnode = node;
        }

        return bestnode;
    }

    public void Max(state parent)
    {
        if(utilityfunc.evaluate(parent) != 0)
        {
            parent.Score = utilityfunc.evaluate(parent);
            return;
        }

        if(parent.depth >= this.MaxDepth)
        {
            parent.Score = evaluator.evaluate(parent);
            return;
        }

        List<State> tree = GeneratePossibleStates(parent);
        foreach(State node in tree)
        {
            Min(node);
            if(parent.CompareTo(node) > 0)
                parent.Score = node.Score;
        }
    }

    public void Min(state parent)
    {
        if(utilityfunc.evaluate(parent) != 0)
        {
            parent.Score = utilityfunc.evaluate(parent);
            return;
        }

        if(parent.depth >= this.MaxDepth)
        {
            parent.Score = evaluator.evaluate(parent);
            return;
        }

        List<State> tree = GeneratePossibleStates(parent);
        foreach(State node in tree)
        {
            Max(node);
            if(parent.CompareTo(node) < 0)
                parent.Score = node.Score;
        }
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
