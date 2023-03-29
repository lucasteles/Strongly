using System;

namespace Strongly.IntegrationTests;

[Strongly(StronglyType.Int)]
partial struct FooValue
{
}

public class Foo
{
    void X()
    {
        Console.WriteLine("3");
    }
}