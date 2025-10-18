using UnityEngine;

public static class DynamicProbability 
{
    [SerializeField]
    private static float[] weights = { 5f, 1f, 2f, 8f }; // pesos iniciales

    private static System.Random rand = new System.Random();
    [SerializeField]
    private static float[] decayFactor = { 0.6f,0.1f,0.1f }; // cu�nto disminuye el peso del elegido (0.5 = 50%)
  

   
    public static void SetWeights(float[] wgts, float[]dFactor)
    {
        weights=wgts;
        decayFactor=dFactor;
    }
    public static void SetWeights(float[] wgts,float dFactor)
    {
        weights=wgts;
        float[] decayFactor = new float[wgts.Length];
        for(int i = 0;i< wgts.Length; i++)
        {
            decayFactor[i] = dFactor;
        }
    }
    public static int GetRandomIndex()//sucesos equiprobables, generalmente salen todos los tipos
    {
        float totalWeight = 0f;
        foreach (var w in weights) totalWeight += w;

        float r = (float)(rand.NextDouble() * totalWeight);
        float sum = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
            if (r <= sum)
            {
                UpdateWeights(i);
                return i;
            }
        }

        return weights.Length - 1;
    }
    public static int GetRandomIndexArgs(float[] wgts, float dFactor)
    {
        float totalWeight = 0f;
        foreach (var w in wgts) totalWeight += w;

        float r = (float)(rand.NextDouble() * totalWeight);
        float sum = 0f;

        for (int i = 0; i < wgts.Length; i++)
        {
            sum += wgts[i];
            if (r <= sum)
            {
                UpdateWeightsArgs(i, wgts, dFactor);
                return i;
            }
        }

        return wgts.Length - 1;
    }
     private static void UpdateWeightsArgs(int selectedIndex,float[]wgts,float dFactor)
    {
        Debug.Log("Evento"+selectedIndex);
        if (selectedIndex >= wgts.Length||selectedIndex<0) return;
        float oldWeight = wgts[selectedIndex];
        float reducedWeight = oldWeight * dFactor;
        float diff = oldWeight - reducedWeight;

        wgts[selectedIndex] = reducedWeight;

        // Repartir la diferencia entre los dem�s tipos
        float increment = diff / (wgts.Length - 1);
        for (int i = 0; i < wgts.Length; i++)
        {
            if (i != selectedIndex)
                wgts[i] += increment;
        }
    }
    public static int[] RollNTimes(int nTimes)
    {
        int[] indexes = new int[nTimes];
        for(int i = 0; i < nTimes; i++)
        {
            indexes[i] = GetRandomIndex();
        }
        return indexes;
    }
    public static int[] RollNTimesNEP(int nTimes)
    {
        int[] indexes = new int[nTimes];
        for(int i = 0; i < nTimes; i++)
        {
            indexes[i] = GetRandomIndexNEP();
        }
        return indexes;
    }
    public static int GetRandomIndexNEP()//sucesos con probabilidades con pesos, sale el mas comun pero se va distribuyendo la probabilidad para que salga el resto
    {
        float totalWeight = 0f;
        foreach (var w in weights) totalWeight += w;

        float r = (float)(rand.NextDouble() * totalWeight);
        float sum = 0f;

        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
            if (r <= sum)
            {
                UpdateWeightsWProb(i);
                return i;
            }
        }

        return weights.Length - 1;
    }
    private static void UpdateWeights(int selectedIndex)
    {
        if (selectedIndex >= weights.Length||selectedIndex<0) return;
        float oldWeight = weights[selectedIndex];
        float reducedWeight = oldWeight * decayFactor[selectedIndex];
        float diff = oldWeight - reducedWeight;

        weights[selectedIndex] = reducedWeight;

        // Repartir la diferencia entre los dem�s tipos
        float increment = diff / (weights.Length - 1);
        for (int i = 0; i < weights.Length; i++)
        {
            if (i != selectedIndex)
                weights[i] += increment;
        }
    }
    private static void UpdateWeightsWProb(int selectedIndex)
    {
        float oldWeight = weights[selectedIndex];//tomar el peso que tenia
        float reducedWeight = oldWeight * decayFactor[selectedIndex];//aplicar el factor decaimiento
        float diff = oldWeight - reducedWeight;//ver la differencia de lo reducido con el antiguo peso

        weights[selectedIndex] = diff; //el antiguo peso es la diferencia

        // Repartir la diferencia entre los dem�s tipos
        float increment = reducedWeight / (weights.Length - 1);
        for (int i = 0; i < weights.Length; i++)
        {
            if (i != selectedIndex)
                weights[i] += increment;
        }
    }

    public static void PrintProbabilities()
    {
        float sum = 0f;
        foreach (var w in weights) sum += w;

        string probs = "";
        for (int i = 0; i < weights.Length; i++)
        {
            probs += $"Type {i}: {(weights[i] / sum):F2}  ";
        }
        Debug.Log(probs);
    }
}
