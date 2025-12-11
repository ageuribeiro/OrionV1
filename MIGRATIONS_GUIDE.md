# 🔄 MIGRATIONS - ENTITY FRAMEWORK CORE

## 📋 Comandos Principais

```bash
# 1️⃣ Criar Migration Inicial
dotnet ef migrations add InitialCreate

# 2️⃣ Aplicar Migration ao banco
dotnet ef database update

# 3️⃣ Criar Migration após adicionar nova entidade
dotnet ef migrations add AddNewFeature

# 4️⃣ Remover última migration (antes de aplicar)
dotnet ef migrations remove

# 5️⃣ Reverter banco para migration anterior
dotnet ef database update PreviousMigrationName

# 6️⃣ Listar todas as migrations
dotnet ef migrations list

# 7️⃣ Gerar SQL script
dotnet ef migrations script > migration.sql
```

---

## 📁 Estrutura de Migrations

Após executar `dotnet ef migrations add InitialCreate`, será criada a pasta:

```
Migrations/
├── 20231215120000_InitialCreate.cs        ← Migration Up/Down
├── 20231215120100_AddNewFeature.cs        ← Próxima migration
└── OrionDbContextModelSnapshot.cs         ← Estado atual do modelo
```

---

## 🗄️ SQL Gerado Exemplo

As seguintes tabelas serão criadas:

```sql
-- Tabela de Permissões
CREATE TABLE [Permissions] (
    [Id] int PRIMARY KEY IDENTITY(1,1),
    [Code] nvarchar(100) NOT NULL UNIQUE,
    [Description] nvarchar(max),
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL
);

-- Tabela de Papéis
CREATE TABLE [Roles] (
    [Id] int PRIMARY KEY IDENTITY(1,1),
    [Name] nvarchar(100) NOT NULL UNIQUE,
    [Description] nvarchar(max),
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL
);

-- Tabela de Usuários
CREATE TABLE [Users] (
    [Id] int PRIMARY KEY IDENTITY(1,1),
    [Username] nvarchar(100) NOT NULL UNIQUE,
    [Email] nvarchar(256) NOT NULL UNIQUE,
    [PasswordHash] nvarchar(max) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2
);

-- Tabela de Relacionamento Usuário-Papel
CREATE TABLE [UserRoles] (
    [Id] int PRIMARY KEY IDENTITY(1,1),
    [UserId] int NOT NULL,
    [RoleId] int NOT NULL,
    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]) ON DELETE CASCADE,
    UNIQUE([UserId], [RoleId])
);

-- Tabela de Relacionamento Papel-Permissão
CREATE TABLE [RolePermissions] (
    [Id] int PRIMARY KEY IDENTITY(1,1),
    [RoleId] int NOT NULL,
    [PermissionId] int NOT NULL,
    FOREIGN KEY ([RoleId]) REFERENCES [Roles]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([PermissionId]) REFERENCES [Permissions]([Id]) ON DELETE CASCADE,
    UNIQUE([RoleId], [PermissionId])
);

-- Tabela de Relacionamento Usuário-Permissão (diretas)
CREATE TABLE [UserPermissions] (
    [Id] int PRIMARY KEY IDENTITY(1,1),
    [UserId] int NOT NULL,
    [PermissionId] int NOT NULL,
    FOREIGN KEY ([UserId]) REFERENCES [Users]([Id]) ON DELETE CASCADE,
    FOREIGN KEY ([PermissionId]) REFERENCES [Permissions]([Id]) ON DELETE CASCADE,
    UNIQUE([UserId], [PermissionId])
);

-- Tabela de controle de migrations
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) PRIMARY KEY,
    [ProductVersion] nvarchar(32) NOT NULL
);
```

---

## ✅ Workflow Completo

### 1. Criar Nova Entidade

```csharp
// Domain/Entities/NewEntity.cs
public class NewEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

### 2. Adicionar DbSet ao Contexto

```csharp
// Infrastructure/Data/OrionDbContext.cs
public DbSet<NewEntity> NewEntities { get; set; }

// Configurar no OnModelCreating
modelBuilder.Entity<NewEntity>(entity =>
{
    entity.HasKey(e => e.Id);
    entity.Property(e => e.Name).IsRequired();
});
```

### 3. Criar Migration

```bash
dotnet ef migrations add AddNewEntity
```

### 4. Verificar Arquivo Gerado

Abrir `Migrations/20231215_AddNewEntity.cs` e revisar.

### 5. Aplicar ao Banco

```bash
dotnet ef database update
```

---

## 🐛 Troubleshooting

### ❌ Erro: "Unable to create an object of type 'OrionDbContext'"

**Causa**: DbContext não pode ser instanciado em tempo de design.

**Solução**: Implementar `IDesignTimeDbContextFactory`

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Orion.Infrastructure.Data;

public class OrionDbContextFactory : IDesignTimeDbContextFactory<OrionDbContext>
{
    public OrionDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OrionDbContext>();
        
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=OrionDB;Trusted_Connection=true;Encrypt=false;"
        );

        return new OrionDbContext(optionsBuilder.Options);
    }
}
```

### ❌ Erro: "The database already exists"

**Solução**:
```bash
dotnet ef database drop
dotnet ef database update
```

### ❌ Erro: "Migration 'X' has already been applied"

**Verificar**: Tabela `__EFMigrationsHistory` no banco

```sql
SELECT * FROM [__EFMigrationsHistory];
```

**Remover entrada**:
```sql
DELETE FROM [__EFMigrationsHistory] WHERE MigrationId = 'MigrationNameToRemove';
```

### ❌ Erro: "Unable to resolve service"

**Causa**: Dependências não configuradas no DI.

**Solução**: Verificar `Program.cs` ou `Startup.cs`

```csharp
services.AddInfrastructureServices(connectionString);
```

---

## 📝 appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=OrionDB;Integrated Security=true;Encrypt=false;"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

### SQL Server localmente:

```json
"DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=OrionDB;Trusted_Connection=true;"
```

### SQL Server remoto:

```json
"DefaultConnection": "Server=seu-servidor.database.windows.net;Database=OrionDB;User Id=usuario;Password=senha;"
```

---

## 🔄 Boas Práticas

✅ **Sempre fazer backup antes de migrations em produção**

✅ **Testar migrations em ambiente de desenvolvimento primeiro**

✅ **Nomear migrations descritivamente**: `AddUserEmailColumn` não `UpdateDb`

✅ **Revisar migration gerada antes de aplicar**

✅ **Usar `Update-Database -Script` para gerar script SQL**

✅ **Manter migrations em controle de versão (Git)**

❌ **Nunca modificar migration já aplicada em produção**

❌ **Não deletar migrations sem motivo**

---

## 📊 Visualizar Banco com SQL Server Management Studio

```bash
# Conectar a:
# Server: (localdb)\mssqllocaldb
# Database: OrionDB

# Ou remoto:
# Server: seu-servidor.database.windows.net
# Authentication: SQL Server Authentication
```

---

## ✨ Próximos Passos

1. ✅ Executar: `dotnet ef migrations add InitialCreate`
2. ✅ Executar: `dotnet ef database update`
3. ✅ Testar conexão ao banco
4. ✅ Executar testes com dados
5. ✅ Fazer seed de dados iniciais
