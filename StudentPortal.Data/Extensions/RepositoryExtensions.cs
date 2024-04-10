using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using StudentPortal.Data.Extensions.Utilities;
using StudentPortal.Models.Dtos.Requests;
using StudentPortal.Models.Dtos.Responses;

namespace StudentPortal.Data.Extensions
{
    public static class RepositoryExtensions
    {
        public static async Task RunPendingMigrationsAsync<T>(this T db) where T : DbContext
        {
            var pendingMigrations = (await db.Database.GetPendingMigrationsAsync()).ToList();

            if (pendingMigrations.Any())
            {
                var migrator = db.Database.GetService<IMigrator>();

                foreach (var targetMigration in pendingMigrations)
                    await migrator.MigrateAsync(targetMigration);
            }

            await Task.CompletedTask;
        }

        public static IQueryable<T> Sort<T>(this IQueryable<T> query, string orderByQueryString)
        {
            if (string.IsNullOrWhiteSpace(orderByQueryString))
                return query;

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<T>(orderByQueryString);

            if (string.IsNullOrWhiteSpace(orderQuery))
                return query;

            return query.OrderBy(orderQuery);
        }

        public static async Task<PagedList<T>> GetPagedItems<T>(this IQueryable<T> query, RequestParameters parameters,
            Expression<Func<T, bool>> searchExpression = null)
        {
            var skip = (parameters.PageNumber - 1) * parameters.PageSize;
            if (searchExpression != null)
                query = query.Where(searchExpression);

            if (!string.IsNullOrWhiteSpace(parameters.OrderBy))
                query = query.Sort(parameters.OrderBy);

            var items = await query.Skip(skip).Take(parameters.PageSize).ToListAsync();
            return new PagedList<T>(items, await query.CountAsync(), parameters.PageNumber, parameters.PageSize);
        }

        public static PagedList<T> GetPagedItems<T>(this IEnumerable<T> query, RequestParameters parameters,
            Func<T, bool> searchExpression = null)
        {
            var skip = (parameters.PageNumber - 1) * parameters.PageSize;
            if (searchExpression != null)
                query = query.Where(searchExpression);
            var items = query.Skip(skip).Take(parameters.PageSize).ToList();
            return new PagedList<T>(items, query.Count(), parameters.PageNumber, parameters.PageSize);
        }
    }
}