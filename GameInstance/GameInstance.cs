using System;
using System.Collections.Generic;

public class GameInstance
{
    private static GameInstance Instance;

    private readonly Dictionary<Type, GameSubInstance> SubInstances;
    private GameInstance()
    {
        SubInstances = new();
    }
    public static GameInstance Get()
    {
        return Instance ??= new();
    }

    public static GameSubInstance Get(Type type)
    {
        bool hasInstance = Get().SubInstances.TryGetValue(type, out GameSubInstance gameSubInstance);

        if (!hasInstance)
        {
            gameSubInstance = Activator.CreateInstance(type) as GameSubInstance;
            Instance.SubInstances.Add(type, gameSubInstance);
        }

        return gameSubInstance;
    }
}
