namespace MapsterUseCases.Core.Interfaces;

public abstract class Stuff : IHasId
{
	public required int Id { get; init; }
}