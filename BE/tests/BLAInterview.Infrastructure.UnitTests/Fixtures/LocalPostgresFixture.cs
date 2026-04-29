using Npgsql;
using Xunit;

namespace BLAInterview.Infrastructure.UnitTests.Fixtures;

/// <summary>
/// Provides an isolated schema in the local PostgreSQL database for infrastructure persistence specs.
/// </summary>
public sealed class LocalPostgresFixture : IAsyncLifetime
{
    private const string DefaultConnectionString =
        "Host=127.0.0.1;Port=5432;Database=BLATests;Username=postgres;Password=C0ntr4s3n4@@;Timeout=10";

    private readonly string _schemaName = $"utests_{Guid.NewGuid():N}";
    private NpgsqlDataSource? _adminDataSource;

    /// <summary>
    /// Gets the Npgsql data source connected to the isolated local PostgreSQL schema.
    /// </summary>
    public NpgsqlDataSource DataSource { get; private set; } = default!;

    /// <summary>
    /// Drops the isolated schema and releases the database connection pools.
    /// </summary>
    public async Task DisposeAsync()
    {
        if (DataSource is not null)
        {
            await DataSource.DisposeAsync();
        }

        if (_adminDataSource is not null)
        {
            await using var command = _adminDataSource.CreateCommand(
                $"""drop schema if exists "{_schemaName}" cascade;""");

            await command.ExecuteNonQueryAsync();
            await _adminDataSource.DisposeAsync();
        }
    }

    /// <summary>
    /// Creates an isolated schema in the local database and applies the init-db schema.
    /// </summary>
    public async Task InitializeAsync()
    {
        var connectionString =
            Environment.GetEnvironmentVariable("BLAINTERVIEW_TEST_DB")
            ?? Environment.GetEnvironmentVariable("ConnectionStrings__MainDb")
            ?? DefaultConnectionString;

        _adminDataSource = NpgsqlDataSource.Create(connectionString);

        await using (var command = _adminDataSource.CreateCommand(
            $"""create schema if not exists "{_schemaName}";"""))
        {
            await command.ExecuteNonQueryAsync();
        }

        var testConnectionStringBuilder = new NpgsqlConnectionStringBuilder(connectionString)
        {
            SearchPath = _schemaName
        };

        DataSource = NpgsqlDataSource.Create(testConnectionStringBuilder.ConnectionString);

        await using var schemaCommand = DataSource.CreateCommand(ReadInitSql());
        await schemaCommand.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Removes task rows from the isolated schema before a spec runs.
    /// </summary>
    public async Task ResetAsync()
    {
        await using var command = DataSource.CreateCommand("truncate table tasks restart identity;");
        await command.ExecuteNonQueryAsync();
    }

    private static string ReadInitSql()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);

        while (directory is not null)
        {
            var initSqlPath = Path.Combine(directory.FullName, "init-db", "001-init.sql");
            if (File.Exists(initSqlPath))
            {
                return File.ReadAllText(initSqlPath);
            }

            directory = directory.Parent;
        }

        throw new FileNotFoundException("Could not find init-db/001-init.sql from the test output directory.");
    }
}
