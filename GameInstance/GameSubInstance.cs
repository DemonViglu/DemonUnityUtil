using Unity.VisualScripting;

public class GameSubInstance
{
    public static GameSubInstance GetInstance(System.Type type)
    {
        return GameInstance.Get(type);
    }
}
