namespace MapsterUseCases.Core.Interfaces;

public interface IHasFirstLastName
{
	string? FirstName { get; set; }

	string? LastName { get; set; }
}