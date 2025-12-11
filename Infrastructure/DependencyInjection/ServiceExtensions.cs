using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Orion.Domain.Interfaces;
using Orion.Infrastructure.Data;
using Orion.Infrastructure.Repositories;
using Orion.Infrastructure.Security;
using Orion.Application.UseCases;

namespace Orion.Infrastructure.DependencyInjection
{
    /// <summary>
    /// Configuração de injeção de dependência
    /// Registra todos os serviços, repositórios e use cases
    /// </summary>
    public static class ServiceExtensions
    {
        /// <summary>
        /// Adiciona os serviços de infraestrutura ao contêiner de DI
        /// </summary>
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            string connectionString)
        {
            // Configurar DbContext
            services.AddDbContext<OrionDbContext>(options =>
                options.UseSqlServer(connectionString)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            );

            // Registrar Repositórios
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IPermissionRepository, PermissionRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            // Registrar Serviços de Segurança
            services.AddScoped<IPermissionService, PermissionService>();

            // Registrar Use Cases
            services.AddScoped<UserUseCase>();

            return services;
        }

        /// <summary>
        /// Inicializa o banco de dados com dados padrão
        /// </summary>
        public static async Task InitializeDatabase(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<OrionDbContext>();
                
                // Criar banco de dados se não existir
                await context.Database.MigrateAsync();

                // Seed initial data
                await SeedDataAsync(context);
            }
        }

        /// <summary>
        /// Seed de dados iniciais
        /// </summary>
        private static async Task SeedDataAsync(OrionDbContext context)
        {
            // Verificar se já existe dados
            if (await context.Roles.AnyAsync())
                return;

            // Criar permissões padrão
            var permissions = new List<Permission>
            {
                new Permission { Code = "USER_CREATE", Description = "Criar usuário", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Permission { Code = "USER_READ", Description = "Visualizar usuário", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Permission { Code = "USER_UPDATE", Description = "Atualizar usuário", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Permission { Code = "USER_DELETE", Description = "Deletar usuário", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Permission { Code = "ROLE_MANAGE", Description = "Gerenciar papéis", IsActive = true, CreatedAt = DateTime.UtcNow },
                new Permission { Code = "PERMISSION_MANAGE", Description = "Gerenciar permissões", IsActive = true, CreatedAt = DateTime.UtcNow },
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();

            // Criar papéis padrão
            var adminRole = new Role
            {
                Name = "Admin",
                Description = "Administrador do sistema",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var userRole = new Role
            {
                Name = "User",
                Description = "Usuário padrão",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await context.Roles.AddRangeAsync(adminRole, userRole);
            await context.SaveChangesAsync();

            // Atribuir permissões ao papel Admin
            var adminPermissions = permissions.Select(p => new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = p.Id
            });

            await context.RolePermissions.AddRangeAsync(adminPermissions);

            // Atribuir permissões ao papel User (apenas leitura)
            var userPermissions = new List<RolePermission>
            {
                new RolePermission { RoleId = userRole.Id, PermissionId = permissions.First(p => p.Code == "USER_READ").Id }
            };

            await context.RolePermissions.AddRangeAsync(userPermissions);
            await context.SaveChangesAsync();
        }
    }
}
