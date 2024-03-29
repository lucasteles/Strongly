﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Strongly.IntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Strongly.IntegrationTests;

public class ShortIdTests
{
    [Fact]
    public void RecordHaveEmpty()
    {
        _ = RecordShortId.Empty;
    }

    [Fact]
    public void SameValuesAreEqual()
    {
        short id = 123;
        var foo1 = new ShortId(id);
        var foo2 = new ShortId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0, ShortId.Empty.Value);
    }


    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new ShortId(1);
        var foo2 = new ShortId(2);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        short id = 12;
        var same1 = new ShortId(id);
        var same2 = new ShortId(id);
        var different = new ShortId(3);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new ShortId(23);

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object)bar, (object)foo);
    }

    [Fact]
    public void CanParseString()
    {
        short value = 1;
        var foo = ShortId.Parse(value.ToString());
        var bar = new ShortId(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => ShortId.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = ShortId.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        short value = 2;
        var result = ShortId.TryParse(value.ToString(), out var foo);
        var bar = new ShortId(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }


    [Fact]
    public void CanSerializeToShort_WithNewtonsoftJsonProvider()
    {
        var foo = new NewtonsoftJsonShortId(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedShort = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedShort);
    }

    [Fact]
    public void CanSerializeToNullableShort_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId { Id = null };

        var json = NewtonsoftJsonSerializer.SerializeObject(entity);
        var deserialize =
            NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

    [Fact]
    public void CanSerializeToShort_WithSystemTextJsonProvider()
    {
        var foo = new SystemTextJsonShortId(123);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedShort = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedShort);
    }

    [Fact]
    public void CanDeserializeFromShort_WithNewtonsoftJsonProvider()
    {
        short value = 123;
        var foo = new NewtonsoftJsonShortId(value);
        var serializedShort = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonShortId>(serializedShort);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromShort_WithSystemTextJsonProvider()
    {
        short value = 123;
        var foo = new SystemTextJsonShortId(value);
        var serializedShort = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonShortId>(serializedShort);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToShort_WithBothJsonConverters()
    {
        var foo = new BothJsonShortId(123);

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedShort1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedShort2 = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo1, serializedShort1);
        Assert.Equal(serializedFoo2, serializedShort2);
    }

    [Fact]
    public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
    {
        var foo = new NoJsonShortId(123);

        var serialized = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + foo.Value + "}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = new NoJsonShortId(123);

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected = $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var foo = new NoConverterShortId(123);

        var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
        var systemText = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + foo.Value + "}";

        Assert.Equal(expected, newtonsoft);
        Assert.Equal(expected, systemText);
    }

    [Fact]
    public void WhenEfValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        var original = new TestEntity { Id = new EfCoreShortId(123) };
        using (var context = new TestDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities.Add(original);
            context.SaveChanges();
        }

        using (var context = new TestDbContext(options))
        {
            var all = context.Entities.ToList();
            var retrieved = Assert.Single(all);
            Assert.Equal(original.Id, retrieved.Id);
        }
    }

    [Fact]
    public async Task WhenDapperValueConverterUsesValueConverter()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var results = await connection.QueryAsync<DapperShortId>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(new DapperShortId(123), value);
    }

    [Theory]
    [InlineData((short)123)]
    [InlineData("123")]
    public void TypeConverter_CanConvertToAndFrom(object value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonShortId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonShortId>(id);
        Assert.Equal(new NoJsonShortId(123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableShortId original = default;
        var other = ComparableShortId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableShortId original = default;
        var other = EquatableShortId.Empty;

        var equals1 = (original as IEquatable<EquatableShortId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableShortId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsShorterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothShortId>>(BothShortId.Empty);
        Assert.IsAssignableFrom<IComparable<BothShortId>>(BothShortId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableShortId>>(EquatableShortId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableShortId>>(ComparableShortId.Empty);

#pragma warning disable 184
        Assert.False(ShortId.Empty is IComparable<ShortId>);
        Assert.False(ShortId.Empty is IEquatable<ShortId>);
#pragma warning restore 184
    }


#if NET6_0_OR_GREATER
    [Fact]
    public void WhenConventionBasedEfValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<ConventionsDbContext>()
            .UseSqlite(connection)
            .Options;

        using (var context = new ConventionsDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities.Add(
                new TestEntity { Id = new EfCoreShortId(123) });
            context.SaveChanges();
        }

        using (var context = new ConventionsDbContext(options))
        {
            var all = context.Entities.ToList();
            Assert.Single(all);
        }
    }

    public class ConventionsDbContext : DbContext
    {
        public DbSet<TestEntity> Entities { get; set; }

        public ConventionsDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void ConfigureConventions(
            ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .Properties<EfCoreShortId>()
                .HaveConversion<EfCoreShortId.EfValueConverter>();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TestEntity>(builder =>
                {
                    builder
                        .Property(x => x.Id)
                        .ValueGeneratedNever();
                });
        }
    }
#endif

#if NET5_0_OR_GREATER
    [Fact]
    public void CanShowImplementationTypeExample_WithSwaggerSchemaFilter()
    {
        var schemaGenerator = new Swashbuckle.AspNetCore.SwaggerGen.SchemaGenerator(
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaGeneratorOptions(),
            new Swashbuckle.AspNetCore.SwaggerGen.JsonSerializerDataContractResolver(
                new System.Text.Json.JsonSerializerOptions()));
        var provider = Microsoft.Extensions.DependencyInjection
            .ServiceCollectionContainerBuilderExtensions.BuildServiceProvider(
                new Microsoft.Extensions.DependencyInjection.ServiceCollection());
        var schemaFilter =
            new Swashbuckle.AspNetCore.Annotations.AnnotationsSchemaFilter(provider);
        var schemaRepository = new Swashbuckle.AspNetCore.SwaggerGen.SchemaRepository();

        var idType = typeof(SwaggerShortId);
        var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
        schemaFilter.Apply(schema,
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator,
                schemaRepository));

        Assert.Equal("integer", schema.Type);
        Assert.Equal(short.MinValue, schema.Minimum);
        Assert.Equal(short.MaxValue, schema.Maximum);
    }
#endif
    public class TestDbContext : DbContext
    {
        public DbSet<TestEntity> Entities { get; set; }

        public TestDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<TestEntity>(builder =>
                {
                    builder
                        .Property(x => x.Id)
                        .HasConversion(new EfCoreShortId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreShortId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonShortId? Id { get; set; }
    }
}