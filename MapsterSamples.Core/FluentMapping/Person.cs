namespace MapsterSamples.Core.FluentMapping;

using Interfaces;
using System.Data;

public class Person : IHasFirstLastName, IHasId, IHasMapperInformation
{
	public required int Id { get; init; }

	public required string? FirstName { get; set; }

	public required string? LastName { get; set; }

	public IEnumerable<Stuff> Stuffs { get; set; } = Enumerable.Empty<Stuff> ();

	public DataSetDateTime? CalledAtBeforeMapping { get; set; }

	public DataSetDateTime? CalledAtAfterMapping { get; set; }

	public string? Message { get; set; }
}