using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MassTransit;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Strongly.IntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Strongly.IntegrationTests
{
    public class GuidCombIdTests
    {
        [Fact]
        public void SameValuesAreEqual()
        {
            var id = NewId.NextGuid();
            var foo1 = new GuidCombId1(id);
            var foo2 = new GuidCombId1(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(GuidCombId1.Empty.Value, Guid.Empty);
        }


        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = GuidCombId1.New();
            var foo2 = GuidCombId1.New();

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = NewId.NextGuid();
            var same1 = new GuidCombId1(id);
            var same2 = new GuidCombId1(id);
            var different = GuidCombId1.New();

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = GuidCombId2.New();
            var foo = GuidCombId1.New();

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CantCreateEmptyGeneratedId1()
        {
            var foo = new GuidCombId1();
            var bar = new GuidCombId2();

            Assert.NotEqual((object)bar, (object)foo);
        }

        [Fact]
        public void CanParseString()
        {
            var value = NewId.NextGuid();
            var foo = GuidCombId1.Parse(value.ToString());
            var bar = new GuidCombId1(value);

            Assert.Equal(bar, foo);
        }

        [Fact]
        public void ThrowWhenInvalidParseString()
        {
            Assert.Throws<FormatException>(() => GuidCombId1.Parse(""));
        }

        [Fact]
        public void CanFailTryParse()
        {
            var result = GuidCombId1.TryParse("", out _);
            Assert.False(result);
        }


        [Fact]
        public void CanTryParseSuccessfully()
        {
            var value = NewId.NextGuid();
            var result = GuidCombId1.TryParse(value.ToString(), out GuidCombId1 foo);
            var bar = new GuidCombId1(value);

            Assert.True(result);
            Assert.Equal(bar, foo);
        }


        [Fact]
        public void CanSerializeToGuidComb_WithTypeConverter()
        {
            var foo = NewtonsoftJsonGuidCombId.New();

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedGuidComb = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            Assert.Equal(serializedFoo, serializedGuidComb);
        }

        [Fact]
        public void CanSerializeToGuidComb_WithSystemTextJsonProvider()
        {
            var foo = SystemTextJsonGuidCombId.New();

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);
            var serializedGuidComb = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo, serializedGuidComb);
        }

        [Fact]
        public void CanDeserializeFromGuidComb_WithNewtonsoftJsonProvider()
        {
            var value = NewId.NextGuid();
            var foo = new NewtonsoftJsonGuidCombId(value);
            var serializedGuidComb = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo =
                NewtonsoftJsonSerializer.DeserializeObject<NewtonsoftJsonGuidCombId>(
                    serializedGuidComb);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToNullableInt_WithNewtonsoftJsonProvider()
        {
            var entity = new EntityWithNullableId { Id = null };

            var json = NewtonsoftJsonSerializer.SerializeObject(entity);
            var deserialize =
                NewtonsoftJsonSerializer.DeserializeObject<EntityWithNullableId>(json);

            Assert.NotNull(deserialize);
            Assert.Null(deserialize.Id);
        }

        [Fact]
        public void CanDeserializeFromGuidComb_WithSystemTextJsonProvider()
        {
            var value = NewId.NextGuid();
            var foo = new SystemTextJsonGuidCombId(value);
            var serializedGuidComb = SystemTextJsonSerializer.Serialize(value);

            var deserializedFoo =
                SystemTextJsonSerializer.Deserialize<SystemTextJsonGuidCombId>(
                    serializedGuidComb);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToGuidComb_WithBothJsonConverters()
        {
            var foo = BothJsonGuidCombId.New();

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedGuidComb1 = NewtonsoftJsonSerializer.SerializeObject(foo.Value);

            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);
            var serializedGuidComb2 = SystemTextJsonSerializer.Serialize(foo.Value);

            Assert.Equal(serializedFoo1, serializedGuidComb1);
            Assert.Equal(serializedFoo2, serializedGuidComb2);
        }

        [Fact]
        public void WhenNoJsonConverter_SystemTextJsonSerializesWithValueProperty()
        {
            var foo = NoJsonGuidCombId.New();

            var serialized = SystemTextJsonSerializer.Serialize(foo);

            var expected = "{\"Value\":\"" + foo.Value + "\"}";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = NoJsonGuidCombId.New();

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);

            var expected = $"\"{foo.Value}\"";

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenNoTypeConverter_SerializesWithValueProperty()
        {
            var foo = NoConverterGuidCombId.New();

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
                    new TestEntity { Id = EfCoreGuidCombId.New() });
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
                await connection.QueryAsync<DapperGuidCombId>(
                    "SELECT '5640dad4-862a-4738-9e3c-c76dc227eb66'");

            var value = Assert.Single(results);
            Assert.Equal(value,
                new DapperGuidCombId(Guid.Parse("5640dad4-862a-4738-9e3c-c76dc227eb66")));
        }

        [Theory]
        [InlineData("78104553-f1cd-41ec-bcb6-d3a8ff8d994d")]
        public void TypeConverter_CanConvertToAndFrom(string value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonGuidCombId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonGuidCombId>(id);
            Assert.Equal(new NoJsonGuidCombId(Guid.Parse(value)), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void CanCompareDefaults()
        {
            ComparableGuidCombId original = default;
            var other = ComparableGuidCombId.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            EquatableGuidCombId original = default;
            var other = EquatableGuidCombId.Empty;

            var equals1 = (original as IEquatable<EquatableGuidCombId>).Equals(other);
            var equals2 = (other as IEquatable<EquatableGuidCombId>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

        [Fact]
        public void ImplementsInterfaces()
        {
            Assert.IsAssignableFrom<IEquatable<BothGuidCombId>>(BothGuidCombId.Empty);
            Assert.IsAssignableFrom<IComparable<BothGuidCombId>>(BothGuidCombId.Empty);

            Assert.IsAssignableFrom<IEquatable<EquatableGuidCombId>>(EquatableGuidCombId
                .Empty);
            Assert.IsAssignableFrom<IComparable<ComparableGuidCombId>>(
                ComparableGuidCombId.Empty);

#pragma warning disable 184
            Assert.False(GuidCombId1.Empty is IComparable<GuidCombId1>);
            Assert.False(GuidCombId1.Empty is IEquatable<GuidCombId1>);
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
                    new TestEntity { Id = EfCoreGuidCombId.New() });
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
                    .Properties<EfCoreGuidCombId>()
                    .HaveConversion<EfCoreGuidCombId.EfValueConverter>();
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

            var idType = typeof(SwaggerGuidCombId);
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
                            .HasConversion(new EfCoreGuidCombId.EfValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public EfCoreGuidCombId Id { get; set; }
        }

        public class EntityWithNullableId
        {
            public NewtonsoftJsonGuidCombId? Id { get; set; }
        }
    }
}
