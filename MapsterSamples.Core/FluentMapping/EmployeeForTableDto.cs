namespace MapsterSamples.Core.FluentMapping;

using Interfaces;

public sealed record EmployeeForTableDto ( string? FullName )
	: IHasFullName;