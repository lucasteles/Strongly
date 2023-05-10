using System;
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

public class DecimalIdTests
{
    [Fact]
    public void SameValuesAreEqual()
    {
        var id = 123.789M;
        var foo1 = new DecimalId(id);
        var foo2 = new DecimalId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0, DecimalId.Empty.Value);
    }

    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new DecimalId(1L);
        var foo2 = new DecimalId(2L);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        var id = 12.789M;
        var same1 = new DecimalId(id);
        var same2 = new DecimalId(id);
        var different = new DecimalId(3L);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new DecimalId(23L);

        Assert.NotEqual((object) bar, (object) foo);
    }

    [Fact]
    public void CanParseString()
    {
        var value = 1.789M;
        var foo = DecimalId.Parse(value.ToString());
        var bar = new DecimalId(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => DecimalId.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = DecimalId.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        var value = 2.789M;
        var result = DecimalId.TryParse(value.ToString(), out var foo);
        var bar = new DecimalId(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }


    [Fact]
    public void CanSerializeToDecimal_WithNewtonsoftJsonProvider()
    {
        var foo = new NewtonsoftJsonDecimalId(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedDecimal = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedDecimal);
    }

    [Fact]
    public void CanSerializeToNullableInt_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId {Id = null};

        var json = NewtonsoftJsonSerializer.SerializeObject(entity);
        var deserialize =
            NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

    [Fact]
    public void CanSerializeToDecimal_WithSystemTextJsonProvider()
    {
        var foo = new SystemTextJsonDecimalId(123L);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedDecimal = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedDecimal);
    }

    [Fact]
    public void CanDeserializeFromDecimal_WithNewtonsoftJsonProvider()
    {
        var value = 123.789M;
        var foo = new NewtonsoftJsonDecimalId(value);
        var serializedDecimal = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonDecimalId>(
                serializedDecimal);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromDecimal_WithSystemTextJsonProvider()
    {
        var value = 123.789M;
        var foo = new SystemTextJsonDecimalId(value);
        var serializedDecimal = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonDecimalId>(serializedDecimal);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToDecimal_WithBothJsonConverters()
    {
        var foo = new BothJsonDecimalId(123L);

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedDecimal1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedDecimal2 = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo1, serializedDecimal1);
        Assert.Equal(serializedFoo2, serializedDecimal2);
    }

    [Fact]
    public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
    {
        var foo = new NoJsonDecimalId(123);

        var serialized = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + foo.Value + "}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = new NoJsonDecimalId(123);

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected = $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var foo = new NoConverterDecimalId(123);

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

        var original = new TestEntity {Id = new EfCoreDecimalId(123)};
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

        var results = await connection.QueryAsync<DapperDecimalId>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(new DapperDecimalId(123), value);
    }

    [Fact]
    public void TypeConverter_CanConvertToAndFrom()
    {
        var value = "123.456";
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonDecimalId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonDecimalId>(id);
        Assert.Equal(new NoJsonDecimalId(123.456M), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void TypeConverter_CanConvertToAndFromDecimal()
    {
        var value = 123.456M;
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonDecimalId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonDecimalId>(id);
        Assert.Equal(new NoJsonDecimalId(123.456M), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableDecimalId original = default;
        var other = ComparableDecimalId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableDecimalId original = default;
        var other = EquatableDecimalId.Empty;

        var equals1 = (original as IEquatable<EquatableDecimalId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableDecimalId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothDecimalId>>(BothDecimalId.Empty);
        Assert.IsAssignableFrom<IComparable<BothDecimalId>>(BothDecimalId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableDecimalId>>(EquatableDecimalId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableDecimalId>>(ComparableDecimalId.Empty);

#pragma warning disable 184
        Assert.False(DecimalId.Empty is IComparable<DecimalId>);
        Assert.False(DecimalId.Empty is IEquatable<DecimalId>);
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
                new TestEntity {Id = new EfCoreDecimalId(123)});
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
                .Properties<EfCoreDecimalId>()
                .HaveConversion<EfCoreDecimalId.EfValueConverter>();
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

        var idType = typeof(SwaggerDecimalId);
        var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
        schemaFilter.Apply(schema,
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator,
                schemaRepository));

        Assert.Equal("number", schema.Type);
        Assert.Equal("double", schema.Format);
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
                        .HasConversion(new EfCoreDecimalId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreDecimalId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonDecimalId? Id { get; set; }
    }

    [Fact]
    public void AdditionTests()
    {
        var (v1, v2) = (1M, 10M);
        var (t1, t2) = (new DecimalMath(v1), new DecimalMath(v2));
        var v3 = v1 + v2;
        var t3 = t1 + t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void SubtractionTests()
    {
        var (v1, v2) = (1M, 10M);
        var (t1, t2) = (new DecimalMath(v1), new DecimalMath(v2));
        var v3 = v1 - v2;
        var t3 = t1 - t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void DivisionTests()
    {
        var (v1, v2) = (10M, 3M);
        var (t1, t2) = (new DecimalMath(v1), new DecimalMath(v2));
        var v3 = v1 / v2;
        var t3 = t1 / t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void MultiplicationTests()
    {
        var (v1, v2) = (10M, 3M);
        var (t1, t2) = (new DecimalMath(v1), new DecimalMath(v2));
        var v3 = v1 * v2;
        var t3 = t1 * t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void NegationTests()
    {
        const decimal v1 = 10M;
        var t1 = new DecimalMath(v1);

        var v2 = -v1;
        var t2 = -t1;

        Assert.Equal(t2.Value, v2);
    }

    [Fact]
    public void ConstTest()
    {
        const decimal v1 = 42M;
        const decimal expected = v1 + decimal.Zero + decimal.One;

        var res = new DecimalMath(v1) + DecimalMath.Zero + DecimalMath.One;
        Assert.Equal(expected, res.Value);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    public void CompareOperator(decimal a, decimal b)
    {
        Assert.Equal(a > b, new DecimalMath(a) > new DecimalMath(b));
        Assert.Equal(a < b, new DecimalMath(a) < new DecimalMath(b));
        Assert.Equal(a >= b, new DecimalMath(a) >= new DecimalMath(b));
        Assert.Equal(a <= b, new DecimalMath(a) <= new DecimalMath(b));
    }
}