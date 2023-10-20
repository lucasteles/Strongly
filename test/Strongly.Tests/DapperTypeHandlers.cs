using System.Runtime.CompilerServices;
using Dapper;
using Strongly.IntegrationTests.Types;

namespace Strongly.IntegrationTests;

public static class DapperTypeHandlers
{
    [ModuleInitializer]
    public static void AddHandlers()
    {
        SqlMapper.AddTypeHandler(new DapperGuidId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperIntId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperStringId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperLongId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperNullableStringId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperNewIdId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperBigIntegerId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperDecimalId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperSequentialGuidId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperGuidCombId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperDoubleId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperFloatId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperShortId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperByteId.DapperTypeHandler());
        SqlMapper.AddTypeHandler(new DapperNativeIntId.DapperTypeHandler());
    }
}