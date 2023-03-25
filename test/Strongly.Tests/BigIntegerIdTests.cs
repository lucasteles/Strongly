using System;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Strongly.IntegrationTests.Types;
using Xunit;
using NewtonsoftJsonSerializer = Newtonsoft.Json.JsonConvert;
using SystemTextJsonSerializer = System.Text.Json.JsonSerializer;

namespace Strongly.IntegrationTests
{
    public class BigIntegerIdTests
    {
        const string BigNString = "307056936428751749926718693741896073217";
        static readonly BigInteger BigN = BigInteger.Parse(BigNString);

        [Fact]
        public void SameValuesAreEqual()
        {
            var id = new BigInteger(123L);
            var foo1 = new BigIntegerId(id);
            var foo2 = new BigIntegerId(id);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void SameBigValuesAreEqual()
        {
            var foo1 = new BigIntegerId(BigN);
            var foo2 = new BigIntegerId(BigN);

            Assert.Equal(foo1, foo2);
        }

        [Fact]
        public void EmptyValueIsEmpty()
        {
            Assert.Equal(BigInteger.Zero, BigIntegerId.Empty.Value);
        }

        [Fact]
        public void DifferentValuesAreUnequal()
        {
            var foo1 = new BigIntegerId(1L);
            var foo2 = new BigIntegerId(2L);

            Assert.NotEqual(foo1, foo2);
        }

        [Fact]
        public void OverloadsWorkCorrectly()
        {
            var id = new BigInteger(12L);
            var same1 = new BigIntegerId(id);
            var same2 = new BigIntegerId(id);
            var different = new BigIntegerId(3L);

            Assert.True(same1 == same2);
            Assert.False(same1 == different);
            Assert.False(same1 != same2);
            Assert.True(same1 != different);
        }

        [Fact]
        public void DifferentTypesAreUnequal()
        {
            var bar = GuidId2.New();
            var foo = new BigIntegerId(23L);

            //Assert.NotEqual(bar, foo); // does not compile
            Assert.NotEqual((object) bar, (object) foo);
        }

        [Fact]
        public void CanParseString()
        {
            var value = new BigInteger(1L);
            var foo = BigIntegerId.Parse(value.ToString());
            var bar = new BigIntegerId(value);

            Assert.Equal(bar, foo);
        }

        [Fact]
        public void ThrowWhenInvalidParseString()
        {
            Assert.Throws<FormatException>(() => BigIntegerId.Parse(""));
        }

        [Fact]
        public void CanFailTryParse()
        {
            var result = BigIntegerId.TryParse("", out _);
            Assert.False(result);
        }


        [Fact]
        public void CanTryParseSuccessfully()
        {
            var value = new BigInteger(2L);
            var result = BigIntegerId.TryParse(value.ToString(), out BigIntegerId foo);
            var bar = new BigIntegerId(value);

            Assert.True(result);
            Assert.Equal(bar, foo);
        }


        [Fact]
        public void CanSerializeToBigInteger_WithNewtonsoftJsonProvider()
        {
            var foo = new NewtonsoftJsonBigIntegerId(BigN);

            var serializedFoo = NewtonsoftJsonSerializer.SerializeObject(foo);

            Assert.Equal(serializedFoo, BigNString);
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
        public void CanSerializeToBigInteger_WithSystemTextJsonProvider()
        {
            var foo = new SystemTextJsonBigIntegerId(BigN);

            var serializedFoo = SystemTextJsonSerializer.Serialize(foo);

            Assert.Equal(serializedFoo, BigNString);
        }

        [Fact]
        public void CanDeserializeFromBigInteger_WithNewtonsoftJsonProvider()
        {
            var value = BigN;
            var foo = new NewtonsoftJsonBigIntegerId(value);
            var serializedBigInteger = NewtonsoftJsonSerializer.SerializeObject(value);

            var deserializedFoo = NewtonsoftJsonSerializer
                .DeserializeObject<NewtonsoftJsonBigIntegerId>(
                    serializedBigInteger);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanDeserializeFromBigInteger_WithSystemTextJsonProvider()
        {
            var value = BigN;
            var foo = new SystemTextJsonBigIntegerId(value);
            var serializedBigInteger = BigNString;

            var deserializedFoo =
                SystemTextJsonSerializer.Deserialize<SystemTextJsonBigIntegerId>(
                    serializedBigInteger);

            Assert.Equal(foo, deserializedFoo);
        }

        [Fact]
        public void CanSerializeToBigInteger_WithBothJsonConverters()
        {
            var foo = new BothJsonBigIntegerId(BigN);

            var serializedFoo1 = NewtonsoftJsonSerializer.SerializeObject(foo);
            var serializedFoo2 = SystemTextJsonSerializer.Serialize(foo);

            Assert.Equal(serializedFoo1, BigNString);
            Assert.Equal(serializedFoo2, BigNString);
        }

        [Fact]
        public void WhenNoJsonConverter_NewtonsoftSerializesWithoutValueProperty()
        {
            var foo = new NoJsonBigIntegerId(BigN);

            var serialized = NewtonsoftJsonSerializer.SerializeObject(foo);
            var expected = NewtonsoftJsonSerializer.SerializeObject(BigNString);

            Assert.Equal(expected, serialized);
        }

        [Fact]
        public void WhenEfCoreValueConverterUsesValueConverter()
        {
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(connection)
                .Options;

            var original = new TestEntity {Id = new EfCoreBigIntegerId(BigN)};
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

            var results =
                await connection.QueryAsync<DapperBigIntegerId>("SELECT 9999");

            var value = Assert.Single(results);
            Assert.Equal(value, new DapperBigIntegerId(9999));
        }

        [Fact]
        public async Task WhenDapperValueConverterUsesValueConverterWithString()
        {
            await using var connection = new SqliteConnection("DataSource=:memory:");
            await connection.OpenAsync();

            var results =
                await connection.QueryAsync<DapperBigIntegerId>($"SELECT \"{BigNString}\"");

            var value = Assert.Single(results);
            Assert.Equal(value, new DapperBigIntegerId(BigN));
        }

        [Theory]
        [InlineData(123L)]
        [InlineData(123)]
        [InlineData(123U)]
        [InlineData("123")]
        public void TypeConverter_CanConvertFrom(object value)
        {
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonBigIntegerId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonBigIntegerId>(id);
            Assert.Equal(new NoJsonBigIntegerId(123), id);
        }

        [Fact]
        public void TypeConverter_CanConvertToAndFromBigNumber()
        {
            var value = (object) BigN;
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonBigIntegerId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonBigIntegerId>(id);
            Assert.Equal(new NoJsonBigIntegerId(BigN), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }

        [Fact]
        public void TypeConverter_CanConvertToAndFromBigString()
        {
            var value = (object) BigNString;
            var converter = TypeDescriptor.GetConverter(typeof(NoJsonBigIntegerId));
            var id = converter.ConvertFrom(value);
            Assert.IsType<NoJsonBigIntegerId>(id);
            Assert.Equal(new NoJsonBigIntegerId(BigN), id);

            var reconverted = converter.ConvertTo(id, value.GetType());
            Assert.Equal(value, reconverted);
        }


        [Fact]
        public void CanCompareDefaults()
        {
            ComparableBigIntegerId original = default;
            var other = ComparableBigIntegerId.Empty;

            var compare1 = original.CompareTo(other);
            var compare2 = other.CompareTo(original);
            Assert.Equal(compare1, -compare2);
        }

        [Fact]
        public void CanEquateDefaults()
        {
            EquatableBigIntegerId original = default;
            var other = EquatableBigIntegerId.Empty;

            var equals1 = (original as IEquatable<EquatableBigIntegerId>).Equals(other);
            var equals2 = (other as IEquatable<EquatableBigIntegerId>).Equals(original);

            Assert.Equal(equals1, equals2);
        }

        [Fact]
        public void ImplementsInterfaces()
        {
            Assert.IsAssignableFrom<IEquatable<BothBigIntegerId>>(BothBigIntegerId.Empty);
            Assert.IsAssignableFrom<IComparable<BothBigIntegerId>>(BothBigIntegerId.Empty);

            Assert.IsAssignableFrom<IEquatable<EquatableBigIntegerId>>(EquatableBigIntegerId.Empty);
            Assert.IsAssignableFrom<IComparable<ComparableBigIntegerId>>(ComparableBigIntegerId
                .Empty);

#pragma warning disable 184
            Assert.False(BigIntegerId.Empty is IComparable<BigIntegerId>);
            Assert.False(BigIntegerId.Empty is IEquatable<BigIntegerId>);
#pragma warning restore 184
        }


#if NET6_0_OR_GREATER
        [Fact]
        public void WhenConventionBasedEfCoreValueConverterUsesValueConverter()
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
                    new TestEntity {Id = new EfCoreBigIntegerId(BigN)});
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
                    .Properties<EfCoreBigIntegerId>()
                    .HaveConversion<EfCoreBigIntegerId.EfStringValueConverter>();
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

            var idType = typeof(SwaggerBigIntegerId);
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
                            .HasConversion(new EfCoreBigIntegerId.EfStringValueConverter())
                            .ValueGeneratedNever();
                    });
            }
        }

        public class TestEntity
        {
            public EfCoreBigIntegerId Id { get; set; }
        }

        public class EntityWithNullableId
        {
            public NewtonsoftJsonBigIntegerId? Id { get; set; }
        }
    }
}