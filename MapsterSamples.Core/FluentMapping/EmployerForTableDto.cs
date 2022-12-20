namespace MapsterSamples.Core.FluentMapping;

using Interfaces;

public sealed record EmployerForTableDto ( string? FullName )
	: IHasFullName;