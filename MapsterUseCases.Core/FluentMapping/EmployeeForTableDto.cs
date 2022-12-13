namespace MapsterUseCases.Core.FluentMapping;

using Interfaces;

public sealed record EmployeeForTableDto ( string? FullName )
	: IHasFullName;