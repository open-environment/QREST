using System.Linq.Expressions;

namespace System.Linq
{
    public static class LinqExtensions
    {
        public static IOrderedQueryable<TSource> OrderBy<TSource>(this IQueryable<TSource> source, string field, string dir = "asc")
        {
            try
            {
                var parameter = Expression.Parameter(typeof(TSource), "r");
                var expression = Expression.Property(parameter, field);
                var lambda = Expression.Lambda(expression, parameter); 
                var tipo = typeof(TSource).GetProperty(field).PropertyType;
                var nome = (dir == "desc" ? "OrderByDescending" : "OrderBy");

                var metodo = typeof(Queryable).GetMethods().First(m => m.Name == nome && m.GetParameters().Length == 2);
                var genericMethod = metodo.MakeGenericMethod(new[] { typeof(TSource), tipo });
                return genericMethod.Invoke(source, new object[] { source, lambda }) as IOrderedQueryable<TSource>;
            }
            catch
            {
                return source.OrderBy(p => 0);
            }
        }

        public static IOrderedQueryable<TSource> ThenBy<TSource>(this IOrderedQueryable<TSource> source, string field, string dir = "asc")
        {
            var parametro = Expression.Parameter(typeof(TSource), "r");
            var expressao = Expression.Property(parametro, field);
            var lambda = Expression.Lambda<Func<TSource, string>>(expressao, parametro); // r => r.AlgumaCoisa
            var tipo = typeof(TSource).GetProperty(field).PropertyType;
            var nome = (dir == "desc" ? "ThenByDescending" : "ThenBy"); 

            var metodo = typeof(Queryable).GetMethods().First(m => m.Name == nome && m.GetParameters().Length == 2);
            var metodoGenerico = metodo.MakeGenericMethod(new[] { typeof(TSource), tipo });
            return metodoGenerico.Invoke(source, new object[] { source, lambda }) as IOrderedQueryable<TSource>;
        }
    }
}
