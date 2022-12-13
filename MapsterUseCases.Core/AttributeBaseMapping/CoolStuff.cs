namespace MapsterUseCases.Core.AttributeBaseMapping;

using Interfaces;
using Mapster;

public sealed class CoolStuff : Stuff
{
	[AdaptMember ( nameof ( BaseStuff.BaseStuffName ) )]
	public string? CoolStuffName { get; set; }
}