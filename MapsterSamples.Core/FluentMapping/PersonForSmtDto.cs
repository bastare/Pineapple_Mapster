namespace MapsterSamples.Core.FluentMapping;

using Interfaces;

public sealed record PersonForSmtDto ( int Id , string FullName , IEnumerable<StuffForSmtDto> Stuffs ) :
	IHasId,
	IHasFullName;