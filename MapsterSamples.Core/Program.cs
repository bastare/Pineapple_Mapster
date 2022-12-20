namespace MapsterSamples.Core;

using Interfaces;
using Mapster;

public sealed class Program
{
	public static void Main ()
	{
		InitMapster ();
		CreateMappings ();

		static void CreateMappings ()
		{
			// ! Create new stuff here
		}

		static void InitMapster ()
		{
			TypeAdapterConfig.GlobalSettings.AllowImplicitDestinationInheritance = true;
			TypeAdapterConfig.GlobalSettings.AllowImplicitSourceInheritance = true;

			TypeAdapterConfig.GlobalSettings.Default.AvoidInlineMapping ( value: true );

			// ! Create new configuration here
			TypeAdapterConfig.GlobalSettings.NewConfig<IHasFirstLastName , IHasFullName> ()
				.Map (
					hasFullName => hasFullName.FullName ,
					hasFirstLastName => string.Join ( " " , hasFirstLastName.FirstName , hasFirstLastName.LastName ) );
		}
	}
}