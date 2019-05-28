using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticIndividual : Individual {


	public GeneticIndividual(int[] topology) : base(topology) {
	}

	public override void Initialize () 
	{
		for (int i = 0; i < totalSize; i++) {
			genotype [i] = UnityEngine.Random.Range (-1.0f, 1.0f);
		}
	}

		
	public override void Crossover (Individual partner, float probability, int n)
	{
        GeneticIndividual p = partner as GeneticIndividual;
        System.Random rand = new System.Random();
        int i;
        float temp;
        bool trade = false;
        int[] cut = new int[n];
        cut[0] = rand.Next(totalSize);
        for (i = 1; i < n; i++)
        {
            cut[i] = rand.Next(cut[i-1], totalSize);
        }
        Array.Sort(cut);
        i = 0;
        for (int j = 0; j < totalSize && i < n; j++ ){
            while (j < cut[i])
            {
                if (trade)
                {
                    this.genotype[j] = p.genotype[j];
                }
				else
				{
					p.genotype[j] = this.genotype[j];
				}
                j++;
            }
            trade = !trade;
            i++;
        }

	}

	public override void Mutate (float probability)
	{
		for (int i = 0; i < totalSize; i++) {
			if (UnityEngine.Random.Range (0.0f, 1.0f) < probability) {
				genotype [i] = UnityEngine.Random.Range (-1.0f, 1.0f);
			}
		}
	}

	public override Individual Clone ()
	{
		GeneticIndividual new_ind = new GeneticIndividual(this.topology);

		genotype.CopyTo (new_ind.genotype, 0);
		new_ind.fitness = this.Fitness;
		new_ind.evaluated = false;

		return new_ind;
	}

}
