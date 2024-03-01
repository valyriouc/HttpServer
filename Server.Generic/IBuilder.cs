namespace Server.Generic;

/// <summary>
/// Abstract interface for a builder 
/// </summary>
/// <typeparam name="TResult"></typeparam>
public interface IBuilder<TResult>
{
    public TResult Build();
}
