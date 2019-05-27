using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm : MetaHeuristic {
	public float mutationProbability;
	public float crossoverProbability;
	public int tournamentSize;
	public bool elitist;

	public override void InitPopulation () {
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
	public override void Step() {
        List<Individual> new_pop = new List<Individual>();

        updateReport(); //called to get some stats
                        // fills the rest with mutations of the best !
        for (int i = 0; i < populationSize; i++)
        {
            GeneticIndividual tmp = (GeneticIndividual)overallBest.Clone();
            tmp.Mutate(mutationProbability);
            new_pop.Add(tmp.Clone());
        }

        population = new_pop;

        generation++;
    }

}
