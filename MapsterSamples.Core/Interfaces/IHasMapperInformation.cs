namespace MapsterSamples.Core.Interfaces;

using System.Data;

public interface IHasMapperInformation
{
	DataSetDateTime? CalledAtBeforeMapping { get; }

	DataSetDateTime? CalledAtAfterMapping { get; }

	string? Message { get; }
}