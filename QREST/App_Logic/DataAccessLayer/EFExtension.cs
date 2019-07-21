using System.Linq.Expressions;

namespace System.Linq
{
    public static class LinqExtensions
    {

        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string field, string dir = "asc")
        {
            // parametro => expressão
            var parametro = Expression.Parameter(typeof(TSource), "r");
            var expressao = Expression.Property(parametro, field);
            var lambda = Expression.Lambda(expressao, parametro); // r => r.AlgumaCoisa
            var tipo = typeof(TSource).GetProperty(field).PropertyType;

            var nome = "OrderBy";
            if (string.Equals(dir, "desc", StringComparison.InvariantCultureIgnoreCase))
            {
                nome = "OrderByDescending";
            }
            var metodo = typeof(Queryable).GetMethods().First(m => m.Name == nome && m.GetParameters().Length == 2);
            var metodoGenerico = metodo.MakeGenericMethod(new[] { typeof(TSource), tipo });
            return metodoGenerico.Invoke(source, new object[] { source, lambda }) as IOrderedQueryable<TSource>;
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource>(this IOrderedQueryable<TSource> source, string field, string dir = "asc")
        {
            var parametro = Expression.Parameter(typeof(TSource), "r");
            var expressao = Expression.Property(parametro, field);
            var lambda = Expression.Lambda<Func<TSource, string>>(expressao, parametro); // r => r.AlgumaCoisa
            var tipo = typeof(TSource).GetProperty(field).PropertyType;

            var nome = "ThenBy";
            if (string.Equals(dir, "desc", StringComparison.InvariantCultureIgnoreCase))
            {
                nome = "ThenByDescending";
            }

            var metodo = typeof(Queryable).GetMethods().First(m => m.Name == nome && m.GetParameters().Length == 2);
            var metodoGenerico = metodo.MakeGenericMethod(new[] { typeof(TSource), tipo });
            return metodoGenerico.Invoke(source, new object[] { source, lambda }) as IOrderedQueryable<TSource>;
        }

    }
}