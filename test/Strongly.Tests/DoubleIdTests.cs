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

public class DoubleIdTests
{
    [Fact]
    public void RecordHaveEmpty()
    {
        _ = RecordDoubleId.Empty;
    }

    [Fact]
    public void SameValuesAreEqual()
    {
        var id = 123.789;
        var foo1 = new DoubleId(id);
        var foo2 = new DoubleId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0, DoubleId.Empty.Value);
    }

    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new DoubleId(1L);
        var foo2 = new DoubleId(2L);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        var id = 12.789;
        var same1 = new DoubleId(id);
        var same2 = new DoubleId(id);
        var different = new DoubleId(3L);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new DoubleId(23L);

        Assert.NotEqual((object) bar, (object) foo);
    }

    [Fact]
    public void CanParseString()
    {
        var value = 1.789;
        var foo = DoubleId.Parse(value.ToString());
        var bar = new DoubleId(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => DoubleId.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = DoubleId.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        var value = 2.789;
        var result = DoubleId.TryParse(value.ToString(), out var foo);
        var bar = new DoubleId(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }


    [Fact]
    public void CanSerializeToDouble_WithNewtonsoftJsonProvider()
    {
        var foo = new NewtonsoftJsonDoubleId(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedDouble = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedDouble);
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
    public void CanSerializeToDouble_WithSystemTextJsonProvider()
    {
        var foo = new SystemTextJsonDoubleId(123L);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedDouble = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedDouble);
    }

    [Fact]
    public void CanDeserializeFromDouble_WithNewtonsoftJsonProvider()
    {
        var value = 123.789;
        var foo = new NewtonsoftJsonDoubleId(value);
        var serializedDouble = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonDoubleId>(
                serializedDouble);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromDouble_WithSystemTextJsonProvider()
    {
        var value = 123.789;
        var foo = new SystemTextJsonDoubleId(value);
        var serializedDouble = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonDoubleId>(serializedDouble);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToDouble_WithBothJsonConverters()
    {
        var foo = new BothJsonDoubleId(123L);

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedDouble1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedDouble2 = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo1, serializedDouble1);
        Assert.Equal(serializedFoo2, serializedDouble2);
    }

    [Fact]
    public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
    {
        var foo = new NoJsonDoubleId(123);

        var serialized = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":" + foo.Value + "}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = new NoJsonDoubleId(123);

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected = $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var foo = new NoConverterDoubleId(123);

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

        var original = new TestEntity {Id = new EfCoreDoubleId(123)};
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

        var results = await connection.QueryAsync<DapperDoubleId>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(new DapperDoubleId(123), value);
    }

    [Fact]
    public void TypeConverter_CanConvertToAndFrom()
    {
        var value = "123.456";
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonDoubleId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonDoubleId>(id);
        Assert.Equal(new NoJsonDoubleId(123.456d), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void TypeConverter_CanConvertToAndFromDouble()
    {
        var value = 123.456;
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonDoubleId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonDoubleId>(id);
        Assert.Equal(new NoJsonDoubleId(123.456), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableDoubleId original = default;
        var other = ComparableDoubleId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableDoubleId original = default;
        var other = EquatableDoubleId.Empty;

        var equals1 = (original as IEquatable<EquatableDoubleId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableDoubleId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothDoubleId>>(BothDoubleId.Empty);
        Assert.IsAssignableFrom<IComparable<BothDoubleId>>(BothDoubleId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableDoubleId>>(EquatableDoubleId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableDoubleId>>(ComparableDoubleId.Empty);

#pragma warning disable 184
        Assert.False(DoubleId.Empty is IComparable<DoubleId>);
        Assert.False(DoubleId.Empty is IEquatable<DoubleId>);
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
                new TestEntity {Id = new EfCoreDoubleId(123)});
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
                .Properties<EfCoreDoubleId>()
                .HaveConversion<EfCoreDoubleId.EfValueConverter>();
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

        var idType = typeof(SwaggerDoubleId);
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
                        .HasConversion(new EfCoreDoubleId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreDoubleId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonDoubleId? Id { get; set; }
    }

    [Fact]
    public void AdditionTests()
    {
        var (v1, v2) = (1d, 10d);
        var (t1, t2) = (new DoubleMath(v1), new DoubleMath(v2));
        var v3 = v1 + v2;
        var t3 = t1 + t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void SubtractionTests()
    {
        var (v1, v2) = (1d, 1d);
        var (t1, t2) = (new DoubleMath(v1), new DoubleMath(v2));
        var v3 = v1 - v2;
        var t3 = t1 - t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void DivisionTests()
    {
        var (v1, v2) = (10d, 3d);
        var (t1, t2) = (new DoubleMath(v1), new DoubleMath(v2));
        var v3 = v1 / v2;
        var t3 = t1 / t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void MultiplicationTests()
    {
        var (v1, v2) = (10d, 3d);
        var (t1, t2) = (new DoubleMath(v1), new DoubleMath(v2));
        var v3 = v1 * v2;
        var t3 = t1 * t2;
        Assert.Equal(t3.Value, v3);
    }

    [Fact]
    public void NegationTests()
    {
        const double v1 = 10d;
        var t1 = new DoubleMath(v1);

        var v2 = -v1;
        var t2 = -t1;

        Assert.Equal(t2.Value, v2);
    }

    [Fact]
    public void ConstTest()
    {
        const double v1 = 42;
        const double expected = v1 + 0d + 1d;

        var res = new DoubleMath(v1) + DoubleMath.Zero + DoubleMath.One;
        Assert.Equal(expected, res.Value);
    }

    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 1)]
    [InlineData(1, 1)]
    [InlineData(0, 0)]
    public void CompareOperator(double a, double b)
    {
        Assert.Equal(a > b, new DoubleMath(a) > new DoubleMath(b));
        Assert.Equal(a < b, new DoubleMath(a) < new DoubleMath(b));
        Assert.Equal(a >= b, new DoubleMath(a) >= new DoubleMath(b));
        Assert.Equal(a <= b, new DoubleMath(a) <= new DoubleMath(b));
    }
}