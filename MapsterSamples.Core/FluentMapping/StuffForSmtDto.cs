namespace MapsterSamples.Core.FluentMapping;

using Interfaces;

public sealed record StuffForSmtDto ( int Id , string StuffName )
	: IHasId;