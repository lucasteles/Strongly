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

public class FloatIdTests
{
    [Fact]
    public void SameValuesAreEqual()
    {
        var id = 123.789f;
        var foo1 = new FloatId(id);
        var foo2 = new FloatId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0, FloatId.Empty.Value);
    }

    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new FloatId(1L);
        var foo2 = new FloatId(2L);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        var id = 12.789f;
        var same1 = new FloatId(id);
        var same2 = new FloatId(id);
        var different = new FloatId(3L);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new FloatId(23L);

        Assert.NotEqual((object) bar, (object) foo);
    }

    [Fact]
    public void CanParseString()
    {
        var value = 1.789f;
        var foo = FloatId.Parse(value.ToString());
        var bar = new FloatId(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => FloatId.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = FloatId.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        var value = 2.789f;
        var result = FloatId.TryParse(value.ToString(), out var foo);
        var bar = new FloatId(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }


    [Fact]
    public void CanSerializeToFloat_WithNewtonsoftJsonProvider()
    {
        var foo = new NewtonsoftJsonFloatId(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedFloat = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedFloat);
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
    public void CanSerializeToFloat_WithSystemTextJsonProvider()
    {
        var foo = new SystemTextJsonFloatId(123L);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedFloat = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedFloat);
    }

    [Fact]
    public void CanDeserializeFromFloat_WithNewtonsoftJsonProvider()
    {
        var value = 123.789f;
        var foo = new NewtonsoftJsonFloatId(value);
        var serializedFloat = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonFloatId>(
                serializedFloat);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromFloat_WithSystemTextJsonProvider()
    {
        var value = 123.789f;
        var foo = new SystemTextJsonFloatId(value);
        var serializedFloat = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonFloatId>(serializedFloat);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToFloat_WithBothJsonConverters()
    {
        var foo = new BothJsonFloatId(123L);

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedFloat1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedFloat2 = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo1, serializedFloat1);
        Assert.Equal(serializedFoo2, serializedFloat2);
    }

    [Fact]
    public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
    {
        var foo = new NoJsonFloatId(123);

        var serialized = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + foo.Value + "}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = new NoJsonFloatId(123);

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected = $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var foo = new NoConverterFloatId(123);

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

        var original = new TestEntity {Id = new EfCoreFloatId(123)};
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

        var results = await connection.QueryAsync<DapperFloatId>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(new DapperFloatId(123), value);
    }

    [Fact]
    public void TypeConverter_CanConvertToAndFrom()
    {
        var value = "123.456";
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonFloatId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonFloatId>(id);
        Assert.Equal(new NoJsonFloatId(123.456f), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void TypeConverter_CanConvertToAndFromFloat()
    {
        var value = 123.456f;
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonFloatId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonFloatId>(id);
        Assert.Equal(new NoJsonFloatId(123.456f), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableFloatId original = default;
        var other = ComparableFloatId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableFloatId original = default;
        var other = EquatableFloatId.Empty;

        var equals1 = (original as IEquatable<EquatableFloatId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableFloatId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothFloatId>>(BothFloatId.Empty);
        Assert.IsAssignableFrom<IComparable<BothFloatId>>(BothFloatId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableFloatId>>(EquatableFloatId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableFloatId>>(ComparableFloatId.Empty);

#pragma warning disable 184
        Assert.False(FloatId.Empty is IComparable<FloatId>);
        Assert.False(FloatId.Empty is IEquatable<FloatId>);
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
                new TestEntity {Id = new EfCoreFloatId(123)});
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
                .Properties<EfCoreFloatId>()
                .HaveConversion<EfCoreFloatId.EfValueConverter>();
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

        var idType = typeof(SwaggerFloatId);
        var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
        schemaFilter.Apply(schema,
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator,
                schemaRepository));

        Assert.Equal("number", schema.Type);
        Assert.Equal("float", schema.Format);
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
                        .HasConversion(new EfCoreFloatId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreFloatId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonFloatId? Id { get; set; }
    }

    [Fact]
    public void AdditionTests()
    {
        var (v1, v2) = (1f, 10f);
        var (t1, t2) = (new FloatMath(v1), new FloatMath(v2));
        var v3 = v1 + v2;
        var t3 = t1 + t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void SubtractionTests()
    {
        var (v1, v2) = (1f, 1f);
        var (t1, t2) = (new FloatMath(v1), new FloatMath(v2));
        var v3 = v1 - v2;
        var t3 = t1 - t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void DivisionTests()
    {
        var (v1, v2) = (10f, 3f);
        var (t1, t2) = (new FloatMath(v1), new FloatMath(v2));
        var v3 = v1 / v2;
        var t3 = t1 / t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void MultiplicationTests()
    {
        var (v1, v2) = (10f, 3f);
        var (t1, t2) = (new FloatMath(v1), new FloatMath(v2));
        var v3 = v1 * v2;
        var t3 = t1 * t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void NegationTests()
    {
        const float v1 = 10f;
        var t1 = new FloatMath(v1);

        var v2 = -v1;
        var t2 = -t1;

        Assert.Equal(t2.Value, v2);
    }

    [Fact]
    public void ConstTest()
    {
        const float v1 = 42;
        const float expected = v1 + 0f + 1f;

        var res = new FloatMath(v1) + FloatMath.Zero + FloatMath.One;
        Assert.Equal(expected, res.Value);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    public void CompareOperator(float a, float b)
    {
        Assert.Equal(a > b, new FloatMath(a) > new FloatMath(b));
        Assert.Equal(a < b, new FloatMath(a) < new FloatMath(b));
        Assert.Equal(a >= b, new FloatMath(a) >= new FloatMath(b));
        Assert.Equal(a <= b, new FloatMath(a) <= new FloatMath(b));
    }
}