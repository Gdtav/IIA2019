using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MetaHeuristic
{
    public float mutationProbability;
    public float crossoverProbability;
    public int tournamentSize;
    public bool elitist;
    public int eliteSize;
    public int nCuts;

    public override void InitPopulation()
    {
        //You should implement the code to initialize the population here
        population = new List<Individual>();
        // jncor 
        while (population.Count < populationSize)
        {
            GeneticIndividual new_ind = new GeneticIndividual(topology);
            new_ind.Initialize();
            population.Add(new_ind);
        }
    }

    //The Step function assumes that the fitness values of all the individuals in the population have been calculated.
    public override void Step()
    {
        int n = populationSize;
        Tournament t = new Tournament();
        List<Individual> new_pop = new List<Individual>();

        updateReport(); //called to get some stats
                        // fills the rest with mutations of the best !
        population.Sort();
        if (elitist)
        {
            n -= eliteSize;
            for (int i = populationSize - 1; i > n - 1; i--)
            {
                new_pop.Add(population[i].Clone());
            }
        }
        new_pop.AddRange(t.selectIndividuals(population, n, 0.5));
        for (int i = 0; i < populationSize - 1; i+=2)
        {
            new_pop[i].Crossover(new_pop[i + 1], crossoverProbability, nCuts);
            new_pop[i].Mutate(mutationProbability);
            new_pop[i + 1].Mutate(mutationProbability);
        }
        population = new_pop;
        generation++;
    }
}
