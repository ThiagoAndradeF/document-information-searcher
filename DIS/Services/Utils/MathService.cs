public class MathService{
    public double CalculateCosineSimilarity(float[] vector1, float[] vector2)
    {
        double dotProduct = 0, normA = 0, normB = 0;
        for (int i = 0; i < vector1.Length; i++)
        {
            dotProduct += vector1[i] * vector2[i];
            normA += Math.Pow(vector1[i], 2);
            normB += Math.Pow(vector2[i], 2);
        }
        return dotProduct / (Math.Sqrt(normA) * Math.Sqrt(normB));
    }
}