using UnityEngine;

public static class EventCardGenerator
{
    public static EventCardData GetRandomRewardCard()
    {
        string[] types = { "Sword", "Shield", "Star", "Book" };
        string type = types[Random.Range(0, types.Length)];
        int value = GetValueByProbability(type);
        return new EventCardData(type, value);
    }

    private static int GetValueByProbability(string type)
    {
        float rand = Random.value * 100;

        switch (type)
        {
            case "Sword":
            case "Shield":
                if (rand < 20) return 1;
                if (rand < 30) return 2;
                if (rand < 35) return 3;
                break;
            case "Star":
                if (rand < 3) return 1;
                if (rand < 5) return 2;
                break;
            case "Book":
                if (rand < 10) return 1;
                if (rand < 18) return 2;
                if (rand < 21) return 3;
                if (rand < 23.5f) return 4;
                if (rand < 25f) return 5;
                break;
        }

        return 1;
    }
}