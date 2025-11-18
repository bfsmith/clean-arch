namespace CleanArch.Core.Services;

public interface IRandomNumberService
{
    int NextInt();
}

public class RandomNumberService : IRandomNumberService
{
    public int NextInt()
    {
        return Random.Shared.Next();
    }
}
