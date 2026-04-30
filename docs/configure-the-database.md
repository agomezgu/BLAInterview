# Configure the database (PostgreSQL)

The **Web API** ([`BLAInterview.WebApi`](../BE/src/BLAInterview.WebApi)) persists tasks in **PostgreSQL** using the connection string key `ConnectionStrings:MainDb`. The sample **IDP** does not use this database (Duende uses in-memory stores in this repo).

Follow the steps below once per environment (local machine, CI, or server).

---

## Prerequisites

- A running **PostgreSQL** instance (13+ is fine; the project is tested with recent versions).
- A user that can `CREATE DATABASE` (or an existing empty database you are allowed to use).
- The schema script: [`init-db/001-init.sql`](../init-db/001-init.sql).

---

## 1. Create a database

Pick a database name. The sample appsettings in the Web API use `BLAInt`; you may use any name as long as the connection string matches.

**Using `psql` as a superuser (or a role with `CREATEDB`):**

```sql
CREATE DATABASE "BLAInt"
  WITH OWNER = your_app_user
  ENCODING = 'UTF8';
```

Or from the shell:

```bash
createdb -U postgres -O your_app_user BLAInt
```

**Using Docker (example):**

```bash
docker run --name blaint-postgres -e POSTGRES_PASSWORD=devpassword -e POSTGRES_USER=postgres -e POSTGRES_DB=BLAInt -p 5432:5432 -d postgres:16
```

Adjust image tag and credentials to your standards.

---

## 2. Apply the schema

The file [`init-db/001-init.sql`](../init-db/001-init.sql) creates the `tasks` table (and related constraints) if it does not already exist.

**`psql` (run from the repository root, or use an absolute path to the script):**

```bash
psql "Host=127.0.0.1;Port=5432;Database=BLAInt;Username=YOUR_USER;Password=YOUR_PASSWORD" -f init-db/001-init.sql
```

**psql with separate args:**

```bash
psql -h 127.0.0.1 -p 5432 -U YOUR_USER -d BLAInt -f init-db/001-init.sql
```

**GUI clients:** Open `001-init.sql` in pgAdmin, DBeaver, or similar, connect to the target database, and execute the script.

**What the script creates:** a `tasks` table with `id`, `title`, `owner_id`, optional `description` / `priority` / `status` (with check constraints), `created`, `created_by`, `last_modified`, and `last_modified_by`. The Web API’s [`TaskRepository`](../BE/src/BLAInterview.Infrastructure/Tasks/TaskRepository.cs) targets this table.

---

## 3. Configure the Web API connection string

The application reads `ConnectionStrings:MainDb` (see [`Program.cs`](../BE/src/BLAInterview.WebApi/Program.cs)). Configure it in one of the following ways (pick one; **do not commit real secrets** to Git).

### Option A: User Secrets (recommended for local dev)

From the Web API project directory:

```bash
cd BE/src/BLAInterview.WebApi
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:MainDb" "Host=127.0.0.1;Port=5432;Database=BLAInt;Username=YOUR_USER;Password=YOUR_PASSWORD;Timeout=10"
```

### Option B: Environment variables

Set the standard .NET configuration key (double underscore for nested keys):

**Windows (PowerShell):**

```powershell
$env:ConnectionStrings__MainDb = "Host=127.0.0.1;Port=5432;Database=BLAInt;Username=YOUR_USER;Password=YOUR_PASSWORD;Timeout=10"
```

**Linux / macOS:**

```bash
export ConnectionStrings__MainDb="Host=127.0.0.1;Port=5432;Database=BLAInt;Username=YOUR_USER;Password=YOUR_PASSWORD;Timeout=10"
```

### Option C: `appsettings.Development.json`

You may add or edit `ConnectionStrings:MainDb` in [`appsettings.Development.json`](../BE/src/BLAInterview.WebApi/appsettings.Development.json) for your machine. Prefer User Secrets or environment variables if multiple developers share the repo, so personal credentials are not committed.

### Connection string shape (Npgsql)

Typical development format:

`Host=127.0.0.1;Port=5432;Database=BLAInt;Username=...;Password=...;Timeout=10`

For TLS to the server, you may add parameters such as `SSL Mode=Prefer` or `Require` depending on your PostgreSQL setup (see [Npgsql connection string docs](https://www.npgsql.org/doc/connection-string-parameters.html)).

---

## 4. Verify

1. Start PostgreSQL and confirm you can connect with the same user/password and database name you put in `MainDb`.
2. In `psql`, confirm the table exists:

   ```sql
   \dt tasks
   SELECT count(*) FROM tasks;
   ```

3. Run the Web API ([`README.md`](../README.md)) and open `https://localhost:7205/swagger`. If the connection string is wrong, the process may fail on first DB access or health-related paths depending on your setup—check logs for Npgsql errors.

---

## Troubleshooting

| Issue | What to check |
|--------|----------------|
| `relation "tasks" does not exist` | Run `001-init.sql` against the **same** database name as in `MainDb`. |
| Authentication failed | User/password in the connection string; PostgreSQL `pg_hba.conf` for host auth. |
| Connection refused | PostgreSQL not running, wrong `Host`/`Port`, or firewall. |
| SSL errors | Add or adjust `SSL Mode` in the connection string to match your server. |

---

## Tests and alternate databases

Some tests use PostgreSQL or Testcontainers with their own connection strings. For local unit/integration testing, see environment variables such as `BLAINTERVIEW_TEST_DB` or `ConnectionStrings__MainDb` in [`LocalPostgresFixture`](../BE/tests/BLAInterview.Infrastructure.UnitTests/Fixtures/LocalPostgresFixture.cs). CI may spin up a container—your **runtime** Web API still needs `MainDb` configured as above.

---

## Related documentation

- Root [**README.md**](../README.md) — Run order, ports, and full stack setup.
- [**init-db/001-init.sql**](../init-db/001-init.sql) — Schema source of truth.
