using System;

namespace Strongly.IntegrationTests;

[Strongly]
readonly partial struct FooValue
{
}

public class Foo
{
    void X()
    {
        Console.WriteLine("3");
    }
}