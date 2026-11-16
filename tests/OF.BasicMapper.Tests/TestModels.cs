namespace OF.BasicMapper.Tests;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; }
}

public class PersonDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<string> Tags { get; set; }
    public string ExtraOnDto { get; set; } = "untouched";
}

public class PartialDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

public class ReadOnlyTarget
{
    public int Id { get; set; }
    public string Name => "fixed";
}

public class PrivateSetterTarget
{
    public int Id { get; set; }
    public string Name { get; private set; } = "original";
}

public class StringIdTarget
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class WriteOnlySource
{
    private int _id;
    public int Id { set => _id = value; }
    public string Name { get; set; }
}

public class IndexedA
{
    public int Id { get; set; }
    public int this[int i] { get => i; set { } }
}

public class IndexedB
{
    public int Id { get; set; }
    public int this[int i] { get => i; set { } }
}

public class BaseWithStringId
{
    public string Id { get; set; }
}

public class DerivedWithIntId : BaseWithStringId
{
    public new int Id { get; set; }
}

public class StaticPropSource
{
    public static int Counter { get; set; } = 42;
    public int Id { get; set; }
}

public class StaticPropTarget
{
    public static int Counter { get; set; }
    public int Id { get; set; }
}

public class NullableSource
{
    public int? Age { get; set; }
    public int Score { get; set; }
}

public class NonNullableTarget
{
    public int Age { get; set; } = -1;
    public int? Score { get; set; }
}

public class Animal
{
    public string Sound { get; set; }
}

public class Dog : Animal { }

public class HasDog { public Dog Pet { get; set; } }
public class HasAnimal { public Animal Pet { get; set; } }

public class NoDefaultCtor
{
    public NoDefaultCtor(int id) { Id = id; }
    public int Id { get; set; }
}

public struct PointStruct
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class PointClass
{
    public int X { get; set; }
    public int Y { get; set; }
}

public class CaseSource { public string name { get; set; } }
public class CaseTarget { public string Name { get; set; } = "unchanged"; }
