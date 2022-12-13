namespace MapsterUseCases;

using Core.AttributeBaseMapping;
using Core.Interfaces;
using Mapster;
using MapsterUseCases.Core.FluentMapping;
using System.Data;
using Xunit;

public sealed class MapsterTest
{
	public MapsterTest ()
	{
		InitMapster ();

		static void InitMapster ()
		{
			TypeAdapterConfig.GlobalSettings.AllowImplicitDestinationInheritance = true;
			TypeAdapterConfig.GlobalSettings.AllowImplicitSourceInheritance = true;

			TypeAdapterConfig.GlobalSettings.Default.AvoidInlineMapping ( value: true );

			TypeAdapterConfig.GlobalSettings.NewConfig<IHasFirstLastName , IHasFullName> ()
				.Map (
					hasFullName => hasFullName.FullName ,
					hasFirstLastName => string.Join ( " " , hasFirstLastName.FirstName , hasFirstLastName.LastName ) );

			TypeAdapterConfig.GlobalSettings.NewConfig<Employer , Person> ()
				.BeforeMapping ( ( employee , person ) =>
				  {
					  // ! You can call services from DI
					  // var serviceFromDI = MapContext.Current.GetService(typeof(SomeService));

					  employee.CalledAtBeforeMapping = DataSetDateTime.Utc;
					  employee.Message = "Howdy from BeforeMapping";

					  person.CalledAtBeforeMapping = DataSetDateTime.Utc;
					  person.Message = "Howdy from BeforeMapping";
				  } )
				.AfterMapping ( ( employee , person ) =>
				  {
					  // ! You can call services from DI
					  // var serviceFromDI = MapContext.Current.GetService(typeof(SomeService));

					  employee.CalledAtAfterMapping = DataSetDateTime.Utc;
					  employee.Message = string.Join ( "; " , employee.Message , "Howdy from AfterMapping" );

					  person.CalledAtAfterMapping = DataSetDateTime.Utc;
					  person.Message = string.Join ( "; " , person.Message , "Howdy from AfterMapping" );
				  } )
				.AfterMappingAsync ( ( employee , person ) =>
				  {
					  // ! You can call services from DI
					  // var serviceFromDI = MapContext.Current.GetService(typeof(SomeService));

					  employee.CalledAtAfterMapping = DataSetDateTime.Utc;
					  employee.Message = string.Join ( "; " , employee.Message , "Howdy from AfterMappingAsync" );

					  person.CalledAtAfterMapping = DataSetDateTime.Utc;
					  person.Message = string.Join ( "; " , person.Message , "Howdy from AfterMappingAsync" );

					  return Task.CompletedTask;
				  } );

			// ! Condition mapping
			TypeAdapterConfig.GlobalSettings.NewConfig<Stuff , StuffForSmtDto> ()
				.Map (
					stuffForSmt => stuffForSmt.StuffName ,
					stuff => ( ( BaseStuff ) stuff ).BaseStuffName ,
					shouldMap: stuff => stuff is BaseStuff )

				.Map (
					stuffForSmt => stuffForSmt.StuffName ,
					stuff => ( ( CoolStuff ) stuff ).CoolStuffName ,
					shouldMap: stuff => stuff is CoolStuff );
		}
	}

	[Fact]
	public void Should_Be_Mapped_By_Using_Attribute_Configuration ()
	{
		const string BaseStuffName = "Some base stuff";
		const int BaseStuffId = 1;

		const string CoolStuffName = "Some cool stuff";
		const int CoolStuffId = -1;

		var baseStuff = new BaseStuff
		{
			Id = BaseStuffId ,
			BaseStuffName = BaseStuffName
		};

		var coolStuff = new CoolStuff
		{
			Id = CoolStuffId ,
			CoolStuffName = CoolStuffName
		};

		var newMappedCoolStuffFromBaseStuff = baseStuff.Adapt<CoolStuff> ();
		var newMappedBaseStuffFromCoolStuff = coolStuff.Adapt<BaseStuff> ();

		Assert.True ( newMappedCoolStuffFromBaseStuff is { CoolStuffName: BaseStuffName, Id: BaseStuffId } );
		Assert.True ( newMappedBaseStuffFromCoolStuff is { BaseStuffName: CoolStuffName, Id: CoolStuffId } );
	}

	[Fact]
	public void Should_Be_ReAssigned_By_Using_Attribute_Configuration ()
	{
		const string BaseStuffName = "Some base stuff";
		const int BaseStuffId = 1;

		const string NewBaseStuffName = "New base stuff";
		const int NewBaseStuffId = 2;

		const string CoolStuffName = "Some cool stuff";
		const int CoolStuffId = -1;

		const string NewCoolStuffName = "New cool stuff";
		const int NewCoolStuffId = -2;

		var baseStuff = new BaseStuff
		{
			Id = BaseStuffId ,
			BaseStuffName = BaseStuffName
		};

		var coolStuff = new CoolStuff
		{
			Id = CoolStuffId ,
			CoolStuffName = CoolStuffName
		};

		TypeAdapter.Adapt (
			source: new CoolStuff { Id = NewCoolStuffId , CoolStuffName = NewCoolStuffName } ,
			destination: baseStuff );

		TypeAdapter.Adapt (
			source: new BaseStuff { Id = NewBaseStuffId , BaseStuffName = NewBaseStuffName } ,
			destination: coolStuff );

		Assert.True ( baseStuff is { Id: NewCoolStuffId, BaseStuffName: NewBaseStuffName } );
		Assert.True ( coolStuff is { Id: NewBaseStuffId, CoolStuffName: NewCoolStuffName } );
	}

	[Fact]
	public void Should_Be_Mapped_By_Interface_Configurations ()
	{
		var person = new Person ()
		{
			Id = 1 ,
			FirstName = "Tony" ,
			LastName = "Boi"
		};

		var employeeForTable = person.Adapt<EmployeeForTableDto> ();

		var isMappedProperly =
			employeeForTable.FullName == string.Join ( " " , person.FirstName , person.LastName );

		Assert.True ( isMappedProperly );
	}

	[Fact]
	public async Task Should_Do_Stuff_Before_And_After_Mapping ()
	{
		var employer = new Employer
		{
			Id = 0 ,
			FirstName = "Bob" ,
			LastName = "Thomson"
		};

		var person =
			await employer.BuildAdapter ()
				.AdaptToTypeAsync<Person> ();

		Assert.NotNull ( person.CalledAtAfterMapping );
		Assert.NotNull ( person.CalledAtBeforeMapping );
		Assert.NotEmpty ( person.Message! );
	}

	[Fact]
	public void Should_Do_Complex_Mapping ()
	{
		var employer = new Employee
		{
			Id = 0 ,
			FirstName = "Bob" ,
			LastName = "Thomson" ,
			Stuffs = new Stuff[]
			{
				new CoolStuff { CoolStuffName = "Cool box", Id = 0 },
				new BaseStuff { BaseStuffName = "Box", Id = 1 },
			}
		};

		var personDto = employer.Adapt<PersonForSmtDto> ();

		var hasProperlyMappedName =
			personDto.FullName == string.Join ( " " , employer.FirstName , employer.LastName );

		var hasProperlyMappedStuffs =
			personDto.Stuffs.All ( stuffDto => string.IsNullOrEmpty ( stuffDto.StuffName ) );

		Assert.True ( hasProperlyMappedName );
		Assert.True ( hasProperlyMappedStuffs );
	}
}