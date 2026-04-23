using System;
using Unity.Netcode;

[Serializable]
public class Stat : NetworkBehaviour
{
    public int TotalOrder = 10;
    public int MaxFailCount = 5;

    public NetworkVariable<int> SentOrder = new();
    public NetworkVariable<int> MistakeFail = new();

    public int GetStarScore()
    {
        // If they failed too many times, they get 0 stars (Optional check)
        if (MistakeFail.Value >= MaxFailCount)
        {
            return 0;
        }

        // Calculate the ratio of mistakes to the maximum allowed
        // 0 mistakes = 3 stars
        // Low mistakes = 2 stars
        // Near limit = 1 star
        float mistakeRatio = (float)MistakeFail.Value / MaxFailCount;

        if (mistakeRatio == 0)
        {
            return 3;
        }
        else if (mistakeRatio < 0.5f) // Less than 50% of max fails reached
        {
            return 2;
        }
        else // They are close to the fail limit
        {
            return 1;
        }
    }
}