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

public class NativeIntIdTests
{
    [Fact]
    public void SameValuesAreEqual()
    {
        nint id = 123;
        var foo1 = new NativeIntId(id);
        var foo2 = new NativeIntId(id);

        Assert.Equal(foo1, foo2);
    }

    [Fact]
    public void EmptyValueIsEmpty()
    {
        Assert.Equal(0, NativeIntId.Empty.Value);
    }


    [Fact]
    public void DifferentValuesAreUnequal()
    {
        var foo1 = new NativeIntId(1);
        var foo2 = new NativeIntId(2);

        Assert.NotEqual(foo1, foo2);
    }

    [Fact]
    public void OverloadsWorkCorrectly()
    {
        var id = 12;
        var same1 = new NativeIntId(id);
        var same2 = new NativeIntId(id);
        var different = new NativeIntId(3);

        Assert.True(same1 == same2);
        Assert.False(same1 == different);
        Assert.False(same1 != same2);
        Assert.True(same1 != different);
    }

    [Fact]
    public void DifferentTypesAreUnequal()
    {
        var bar = GuidId2.New();
        var foo = new NativeIntId(23);

        //Assert.NotEqual(bar, foo); // does not compile
        Assert.NotEqual((object) bar, (object) foo);
    }

    [Fact]
    public void CanParseString()
    {
        var value = 1;
        var foo = NativeIntId.Parse(value.ToString());
        var bar = new NativeIntId(value);

        Assert.Equal(bar, foo);
    }

    [Fact]
    public void ThrowWhenInvalidParseString()
    {
        Assert.Throws<FormatException>(() => NativeIntId.Parse(""));
    }

    [Fact]
    public void CanFailTryParse()
    {
        var result = NativeIntId.TryParse("", out _);
        Assert.False(result);
    }


    [Fact]
    public void CanTryParseSuccessfully()
    {
        nint value = 2;
        var result = NativeIntId.TryParse(value.ToString(), out var foo);
        var bar = new NativeIntId(value);

        Assert.True(result);
        Assert.Equal(bar, foo);
    }

    [Fact]
    public void CanSerializeToNativeInt_WithNewtonsoftJsonProvider()
    {
        var foo = new NewtonsoftJsonNativeIntId(123);

        var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedNativeInt = NewtonsoftJsonSerializer.SerializeObject((int) foo.Value);

        Assert.Equal(serializedFoo, serializedNativeInt);
    }

    [Fact]
    public void CanSerializeToNullableNativeInt_WithNewtonsoftJsonProvider()
    {
        var entity = new EntityWithNullableId {Id = null};

        var json = NewtonsoftJsonSerializer.SerializeObject(entity);
        var deserialize =
            NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

        Assert.NotNull(deserialize);
        Assert.Null(deserialize.Id);
    }

    [Fact]
    public void CanSerializeToNativeInt_WithSystemTextJsonProvider()
    {
        var foo = new SystemTextJsonNativeIntId(123);

        var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
        var serializedNativeInt = SystemTextJsonSerializer.Serialize((int) foo.Value);

        Assert.Equal(serializedFoo, serializedNativeInt);
    }

    [Fact]
    public void CanDeserializeFromNativeInt_WithNewtonsoftJsonProvider()
    {
        nint value = 123;
        var foo = new NewtonsoftJsonNativeIntId(value);
        var serializedNativeInt = NewtonsoftJsonSerializer.SerializeObject((int) value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonNativeIntId>(
                serializedNativeInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromIntPtr_WithNewtonsoftJsonProvider()
    {
        var value = (IntPtr) 123;
        var foo = new NewtonsoftJsonNativeIntId(value);
        var serializedNativeInt = NewtonsoftJsonSerializer.SerializeObject((int) value);

        var deserializedFoo =
            NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonNativeIntId>(
                serializedNativeInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanDeserializeFromNativeInt_WithSystemTextJsonProvider()
    {
        nint value = 123;
        var foo = new SystemTextJsonNativeIntId(value);
        var serializedNativeInt = SystemTextJsonSerializer.Serialize((int) value);

        var deserializedFoo =
            SystemTextJsonSerializer.Deserialize<SystemTextJsonNativeIntId>(serializedNativeInt);

        Assert.Equal(foo, deserializedFoo);
    }

    [Fact]
    public void CanSerializeToNativeInt_WithBothJsonConverters()
    {
        var foo = new BothJsonNativeIntId(123);

        var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
        var serializedNativeInt1 = NewtonsoftJsonSerializer.SerializeObject((int) foo.Value);

        var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
        var serializedNativeInt2 = SystemTextJsonSerializer.Serialize((int) foo.Value);

        Assert.Equal(serializedFoo1, serializedNativeInt1);
        Assert.Equal(serializedFoo2, serializedNativeInt2);
    }

    [Fact]
    public void WhenEfValueConverterUsesValueConverter()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseSqlite(connection)
            .Options;

        var original = new TestEntity {Id = new EfCoreNativeIntId(123)};
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

        var results = await connection.QueryAsync<DapperNativeIntId>("SELECT 123");

        var value = Assert.Single(results);
        Assert.Equal(new DapperNativeIntId(123), value);
    }

    [Fact]
    public void TypeConverter_CanConvertToAndFromNInt()
    {
        nint value = 123;
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonNativeIntId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonNativeIntId>(id);
        Assert.Equal(new NoJsonNativeIntId(123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Theory]
    [InlineData(123)]
    [InlineData("123")]
    public void TypeConverter_CanConvertToAndFrom(object value)
    {
        var converter = TypeDescriptor.GetConverter(typeof(NoJsonNativeIntId));
        var id = converter.ConvertFrom(value);
        Assert.IsType<NoJsonNativeIntId>(id);
        Assert.Equal(new NoJsonNativeIntId(123), id);

        var reconverted = converter.ConvertTo(id, value.GetType());
        Assert.Equal(value, reconverted);
    }

    [Fact]
    public void CanCompareDefaults()
    {
        ComparableNativeIntId original = default;
        var other = ComparableNativeIntId.Empty;

        var compare1 = original.CompareTo(other);
        var compare2 = other.CompareTo(original);
        Assert.Equal(compare1, -compare2);
    }

    [Fact]
    public void CanEquateDefaults()
    {
        EquatableNativeIntId original = default;
        var other = EquatableNativeIntId.Empty;

        var equals1 = (original as IEquatable<EquatableNativeIntId>).Equals(other);
        var equals2 = (other as IEquatable<EquatableNativeIntId>).Equals(original);

        Assert.Equal(equals1, equals2);
    }

    [Fact]
    public void ImplementsNativeInterfaces()
    {
        Assert.IsAssignableFrom<IEquatable<BothNativeIntId>>(BothNativeIntId.Empty);
        Assert.IsAssignableFrom<IComparable<BothNativeIntId>>(BothNativeIntId.Empty);

        Assert.IsAssignableFrom<IEquatable<EquatableNativeIntId>>(EquatableNativeIntId.Empty);
        Assert.IsAssignableFrom<IComparable<ComparableNativeIntId>>(ComparableNativeIntId.Empty);

#pragma warning disable 184
        Assert.False(NativeIntId.Empty is IComparable<NativeIntId>);
        Assert.False(NativeIntId.Empty is IEquatable<NativeIntId>);
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
                new TestEntity {Id = new EfCoreNativeIntId(123)});
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
                .Properties<EfCoreNativeIntId>()
                .HaveConversion<EfCoreNativeIntId.EfValueConverter>();
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

        var idType = typeof(SwaggerNativeIntId);
        var schema = schemaGenerator.GenerateSchema(idType, schemaRepository);
        schemaFilter.Apply(schema,
            new Swashbuckle.AspNetCore.SwaggerGen.SchemaFilterContext(idType, schemaGenerator,
                schemaRepository));

        Assert.Equal("integer", schema.Type);
        Assert.Equal("", schema.Format);
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
                        .HasConversion(new EfCoreNativeIntId.EfValueConverter())
                        .ValueGeneratedNever();
                });
        }
    }

    public class TestEntity
    {
        public EfCoreNativeIntId Id { get; set; }
    }

    public class EntityWithNullableId
    {
        public NewtonsoftJsonNativeIntId? Id { get; set; }
    }
}