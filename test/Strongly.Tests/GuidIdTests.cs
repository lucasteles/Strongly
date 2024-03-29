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

public class GuidIdTests
{
    [Fact]
    public void SameValuesAreEqual()
    {
        var id = Guid.NewGuid();
        var foo1 = new GuidId1(id);
        var foo2 = new GuidId1(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(GuidId1.Empty.Value, Guid.Empty);
    }

    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = GuidId1.New();
        var foo2 = GuidId1.New();

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        var id = Guid.NewGuid();
        var same1 = new GuidId1(id);
        var same2 = new GuidId1(id);
        var different = GuidId1.New();

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = GuidId1.New();

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object) bar, (object) foo);
    }

    [Fact]
    public void CantCreateEmptyGeneratedId1()
    {
        var foo = new GuidId1();
        var bar = new GuidId2();

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object) bar, (object) foo);
    }

    [Fact]
    public void CanParseString()
    {
        var value = Guid.NewGuid();
        var foo = GuidId1.Parse(value.ToString());
        var bar = new GuidId1(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => GuidId1.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = GuidId1.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        var value = Guid.NewGuid();
        var result = GuidId1.TryParse(value.ToString(), out var foo);
        var bar = new GuidId1(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }


    [Fact]
    public void CanSerializeToGuid_WithTypeConverter()
    {
        var foo = NewtonsoftJsonGuidId.New();

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedGuid = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        Assert.Equal(serializedFoo, serializedGuid);
    }

    [Fact]
    public void CanSerializeToGuid_WithSystemTextJsonProvider()
    {
        var foo = SystemTextJsonGuidId.New();

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedGuid = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo, serializedGuid);
    }

    [Fact]
    public void CanDeserializeFromGuid_WithNewtonsoftJsonProvider()
    {
        var value = Guid.NewGuid();
        var foo = new NewtonsoftJsonGuidId(value);
        var serializedGuid = NewtonsoftJsonSerializer.SerializeObject(value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonGuidId>(serializedGuid);

        Assert.Equal(foo, deserializedFoo);
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
    public void CanDeserializeFromGuid_WithSystemTextJsonProvider()
    {
        var value = Guid.NewGuid();
        var foo = new SystemTextJsonGuidId(value);
        var serializedGuid = SystemTextJsonSerializer.Serialize(value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonGuidId>(serializedGuid);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToGuid_WithBothJsonConverters()
    {
        var foo = BothJsonGuidId.New();

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedGuid1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedGuid2 = SystemTextJsonSerializer.Serialize(foo.Value);

        Assert.Equal(serializedFoo1, serializedGuid1);
        Assert.Equal(serializedFoo2, serializedGuid2);
    }

    [Fact]
    public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
    {
        var foo = NoJsonGuidId.New();

        var serialized = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":\"" + foo.Value + "\"}";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
    {
        var foo = NoJsonGuidId.New();

        var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

        var expected = $"\"{foo.Value}\"";

        Assert.Equal(expected, serialized);
    }

    [Fact]
    public void WhenNoTypeConverter_SerializesWithValueProperty()
    {
        var foo = NoConverterGuidId.New();

        var newtonsoft = SystemTextJsonSerializer.Serialize(foo);
        var systemText = SystemTextJsonSerializer.Serialize(foo);

        var expected = "{\"Value\":\"" + foo.Value + "\"}";

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

        using (var context = new TestDbContext(options))
        {
            context.Database.EnsureCreated();
            context.Entities.Add(
                new TestEntity {Id = EfCoreGuidId.New()});
            context.SaveChanges();
        }

        using (var context = new TestDbContext(options))
        {
            var all = context.Entities.ToList();
            Assert.Single(all);
        }
    }

    [Fact]
    public async Task WhenDapperValueConverterUsesValueConverter()
    {
        await using var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var results =
            await connection.QueryAsync<DapperGuidId>(
                "SELECT '5640dad4-862a-4738-9e3c-c76dc227eb66'");

        var value = Assert.Single(results);
        Assert.Equal(value,
            new DapperGuidId(Guid.Parse("5640dad4-862a-4738-9e3c-c76dc227eb66")));
    }

    [Theory]
    [InlineData("78104553-f1cd-41ec-bcb6-d3a8ff8d994d")]
    public void TypeConverter_CanConvertToAndFrom(string value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonGuidId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonGuidId>(id);
        Assert.Equal(new NoJsonGuidId(Guid.Parse(value)), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableGuidId original = default;
        var other = ComparableGuidId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableGuidId original = default;
        var other = EquatableGuidId.Empty;

        var equals1 = (original as IEquatable<EquatableGuidId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableGuidId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothGuidId>>(BothGuidId.Empty);
        Assert.IsAssignableFrom<IComparable<BothGuidId>>(BothGuidId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableGuidId>>(EquatableGuidId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableGuidId>>(ComparableGuidId.Empty);

#pragma warning disable 184
        Assert.False(GuidId1.Empty is IComparable<GuidId1>);
        Assert.False(GuidId1.Empty is IEquatable<GuidId1>);
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
                new TestEntity {Id = EfCoreGuidId.New()});
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
                .Properties<EfCoreGuidId>()
                .HaveConversion<EfCoreGuidId.EfValueConverter>();
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

        var idType = typeof(SwaggerGuidId);
        var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
        schemaFilter.Apply(schema,
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator,
                schemaRepository));

        Assert.Equal("string", schema.Type);
        Assert.Equal("uuid", schema.Format);
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
                        .HasConversion(new EfCoreGuidId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreGuidId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonGuidId? Id { get; set; }
    }

    [Fact]
    public void RecordSameValuesAreEqual()
    {
        var id = Guid.NewGuid();
        var foo1 = new RecordGuidId1(id);
        var foo2 = new RecordGuidId1(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void RecordHaveNewAndEmpty()
    {
        _ = RecordGuidId1.New();
        _ = RecordGuidId1.Empty;
    }

    [Fact]
    public void ImplicitCasts()
    {
        var value = Guid.NewGuid();
        ImplicitGuid typedValue = value;
        Guid valueBack = typedValue;

        Assert.Equal(value, typedValue.Value);
        Assert.Equal(value, valueBack);
    }

    [Fact]
    public void ExplicitCasts()
    {
        var value = Guid.NewGuid();
        var typedValue = (ExplicitGuid) value;
        var valueBack = (Guid) typedValue;

        Assert.Equal(value, typedValue.Value);
        Assert.Equal(value, valueBack);
    }

    [Fact]
    public void ShouldBeFormattable()
    {
        var id = FormattableGuidId.New();
        IFormattable fmtId = id;
        IFormattable fmtValue = id.Value;

        Assert.Equal(fmtId.ToString(), fmtValue.ToString());
    }

    [Fact]
    public void UseCustomConstructorValidation() =>
        Assert.Throws<ArgumentException>(() => new CtorGuidId(Guid.Empty));

    [Fact]
    public void UseCustomConstructor()
    {
        var guid = Guid.NewGuid();
        var id = new CtorGuidId(guid);
        Assert.Equal(guid, id.Value);
    }
}