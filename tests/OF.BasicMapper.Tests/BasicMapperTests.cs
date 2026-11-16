using Xunit;
using Mapper = OF.BasicMapper.BasicMapper;

namespace OF.BasicMapper.Tests;

public class BasicMapperTests
{
    private readonly IBasicMapper _mapper = new Mapper();

    private static Person SamplePerson() => new()
    {
        Id = 7,
        Name = "Ada",
        Email = "ada@example.com",
        CreatedAt = new DateTime(2024, 1, 2, 3, 4, 5, DateTimeKind.Utc),
        Tags = new List<string> { "a", "b" },
    };

    // ---- Core behaviour (unchanged from 2.x) ----

    [Fact]
    public void MapFields_New_CopiesAllMatchingProperties()
    {
        var src = SamplePerson();

        var dto = _mapper.MapFields<PersonDto>(src);

        Assert.Equal(src.Id, dto.Id);
        Assert.Equal(src.Name, dto.Name);
        Assert.Equal(src.Email, dto.Email);
        Assert.Equal(src.CreatedAt, dto.CreatedAt);
        Assert.Equal("untouched", dto.ExtraOnDto);
    }

    [Fact]
    public void MapFields_New_IgnoresPropertiesMissingOnTarget()
    {
        var dto = _mapper.MapFields<PartialDto>(SamplePerson());

        Assert.Equal(7, dto.Id);
        Assert.Equal("Ada", dto.Name);
    }

    [Fact]
    public void MapFields_Existing_UpdatesAndReturnsSameInstance()
    {
        var target = new PersonDto { ExtraOnDto = "keep" };

        var result = _mapper.MapFields<PersonDto>(SamplePerson(), target);

        Assert.Same(target, result);
        Assert.Equal("Ada", target.Name);
        Assert.Equal("keep", target.ExtraOnDto);
    }

    [Fact]
    public void MapFields_WithSkip_LeavesSkippedPropertiesAlone()
    {
        var dto = _mapper.MapFields<PersonDto>(SamplePerson(), new List<string> { "Email", "Id" });

        Assert.Equal(0, dto.Id);
        Assert.Null(dto.Email);
        Assert.Equal("Ada", dto.Name);
    }

    [Fact]
    public void MapFields_ExistingWithSkip_LeavesSkippedPropertiesAlone()
    {
        var target = new PersonDto { Email = "keep@example.com" };

        _mapper.MapFields<PersonDto>(SamplePerson(), target, new List<string> { "Email" });

        Assert.Equal("keep@example.com", target.Email);
        Assert.Equal("Ada", target.Name);
    }

    [Fact]
    public void MapFields_NullSkipList_IsTreatedAsEmpty()
    {
        var dto = _mapper.MapFields<PersonDto>(SamplePerson(), (List<string>)null);

        Assert.Equal("Ada", dto.Name);
    }

    [Fact]
    public void MapFields_SkipIsCaseSensitive()
    {
        var dto = _mapper.MapFields<PersonDto>(SamplePerson(), new List<string> { "name" });

        Assert.Equal("Ada", dto.Name);
    }

    [Fact]
    public void MapFields_PropertyNamesAreCaseSensitive()
    {
        var target = _mapper.MapFields<CaseTarget>(new CaseSource { name = "x" });

        Assert.Equal("unchanged", target.Name);
    }

    [Fact]
    public void MapFields_IsShallowCopy()
    {
        var src = SamplePerson();

        var dto = _mapper.MapFields<PersonDto>(src);

        Assert.Same(src.Tags, dto.Tags);
    }

    [Fact]
    public void MapFields_NullSourceValue_IsCopiedAsNull()
    {
        var src = SamplePerson();
        src.Name = null;

        var dto = _mapper.MapFields<PersonDto>(src, new PersonDto { Name = "old" });

        Assert.Null(dto.Name);
    }

    [Fact]
    public void MapFields_WorksWithAnonymousSource()
    {
        var dto = _mapper.MapFields<PartialDto>(new { Id = 3, Name = "anon", Other = true });

        Assert.Equal(3, dto.Id);
        Assert.Equal("anon", dto.Name);
    }

    [Fact]
    public void MapFields_DerivedValue_IsAssignedToBaseTypedProperty()
    {
        var dog = new Dog { Sound = "woof" };

        var result = _mapper.MapFields<HasAnimal>(new HasDog { Pet = dog });

        Assert.Same(dog, result.Pet);
    }

    [Fact]
    public void MapFields_BaseTypedValue_IsNotAssignedToDerivedTypedProperty()
    {
        var result = _mapper.MapFields<HasDog>(new HasAnimal { Pet = new Animal() });

        Assert.Null(result.Pet);
    }

    [Fact]
    public void MapFields_NonNullableToNullable_IsCopied()
    {
        var result = _mapper.MapFields<NonNullableTarget>(new NullableSource { Score = 5 });

        Assert.Equal(5, result.Score);
    }

    [Fact]
    public void MapFields_NullableWithValueToNonNullable_IsCopied()
    {
        var result = _mapper.MapFields<NonNullableTarget>(new NullableSource { Age = 30 });

        Assert.Equal(30, result.Age);
    }

    [Fact]
    public void MapFields_NullNullableToNonNullable_WritesDefault()
    {
        // Same as 2.x: reflection writes default(int) when the source value is null.
        var result = _mapper.MapFields<NonNullableTarget>(new NullableSource { Age = null });

        Assert.Equal(0, result.Age);
    }

    [Fact]
    public void MapFields_StructDestination_ReturnsMappedValue()
    {
        var p = _mapper.MapFields<PointStruct>(new PointClass { X = 1, Y = 2 });

        Assert.Equal(1, p.X);
        Assert.Equal(2, p.Y);
    }

    [Fact]
    public void MapFields_ExistingStructDestination_ReturnsMappedValue()
    {
        var p = _mapper.MapFields<PointStruct>(new PointClass { X = 3, Y = 4 }, new PointStruct());

        Assert.Equal(3, p.X);
        Assert.Equal(4, p.Y);
    }

    // ---- Cases that threw in 2.0.1 ----

    [Fact]
    public void MapFields_ReadOnlyTargetProperty_IsSkipped()
    {
        var result = _mapper.MapFields<ReadOnlyTarget>(SamplePerson());

        Assert.Equal(7, result.Id);
        Assert.Equal("fixed", result.Name);
    }

    [Fact]
    public void MapFields_PrivateSetterOnTarget_IsSkipped()
    {
        var result = _mapper.MapFields<PrivateSetterTarget>(SamplePerson());

        Assert.Equal(7, result.Id);
        Assert.Equal("original", result.Name);
    }

    [Fact]
    public void MapFields_IncompatibleTypes_AreSkipped()
    {
        var result = _mapper.MapFields<StringIdTarget>(SamplePerson());

        Assert.Null(result.Id);
        Assert.Equal("Ada", result.Name);
    }

    [Fact]
    public void MapFields_WriteOnlySourceProperty_IsSkipped()
    {
        var result = _mapper.MapFields<PartialDto>(new WriteOnlySource { Id = 9, Name = "w" });

        Assert.Equal(0, result.Id);
        Assert.Equal("w", result.Name);
    }

    [Fact]
    public void MapFields_Indexers_AreIgnored()
    {
        var result = _mapper.MapFields<IndexedB>(new IndexedA { Id = 4 });

        Assert.Equal(4, result.Id);
    }

    [Fact]
    public void MapFields_HiddenProperty_UsesMostDerivedDeclaration()
    {
        var result = _mapper.MapFields<PartialDto>(new DerivedWithIntId { Id = 11 });

        Assert.Equal(11, result.Id);
    }

    [Fact]
    public void MapFields_StaticProperties_AreIgnored()
    {
        StaticPropTarget.Counter = 0;

        _mapper.MapFields<StaticPropTarget>(new StaticPropSource { Id = 1 });

        Assert.Equal(0, StaticPropTarget.Counter);
    }

    // ---- Argument validation ----

    [Fact]
    public void MapFields_NullSource_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => _mapper.MapFields<PersonDto>(null));
        Assert.Equal("from", ex.ParamName);
    }

    [Fact]
    public void MapFields_NullDestination_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(() => _mapper.MapFields<PersonDto>(SamplePerson(), (object)null));
        Assert.Equal("to", ex.ParamName);
    }

    [Fact]
    public void MapFields_DestinationNotOfTypeT_ThrowsBeforeMutating()
    {
        var target = new PartialDto();

        Assert.Throws<ArgumentException>(() => _mapper.MapFields<PersonDto>(SamplePerson(), target));
        Assert.Equal(0, target.Id);
    }

    [Fact]
    public void MapFields_TypeWithoutParameterlessCtor_ThrowsHelpfulError()
    {
        var ex = Assert.Throws<InvalidOperationException>(() => _mapper.MapFields<NoDefaultCtor>(SamplePerson()));
        Assert.Contains("parameterless constructor", ex.Message);
    }

    [Fact]
    public void MapFields_TypeWithoutParameterlessCtor_WorksWithExistingInstance()
    {
        var result = _mapper.MapFields<NoDefaultCtor>(SamplePerson(), new NoDefaultCtor(0));

        Assert.Equal(7, result.Id);
    }

    // ---- Concurrency ----

    [Fact]
    public void MapFields_IsThreadSafe()
    {
        var mapper = new Mapper();

        Parallel.For(0, 2000, i =>
        {
            var dto = mapper.MapFields<PersonDto>(new Person { Id = i, Name = "n" + i });
            Assert.Equal(i, dto.Id);
            Assert.Equal("n" + i, dto.Name);
        });
    }
}
