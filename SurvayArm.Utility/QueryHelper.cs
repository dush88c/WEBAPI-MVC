using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SurvayArm.Utility
{
    public static class QueryHelper
    {
        public static IQueryable<TEntity> CallOrderBy<TEntity>(this IQueryable<TEntity> source, string orderByValues)
        {
            var sortingOption = orderByValues.Trim().Split(',');
            var orderPair = sortingOption[0];
            var command = orderPair.ToUpper().Contains("DESC") ? "OrderByDescending" : "OrderBy";

            var type = typeof(TEntity);
            var parameter = Expression.Parameter(type, "p");

            var propertyName = sortingOption[1].Trim();


            var property = type.GetProperty(propertyName);
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);


            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var resultExpression = Expression.Call(typeof(Queryable), command, new[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExpression));

            var returnIQuery = source.Provider.CreateQuery<TEntity>(resultExpression);

            return returnIQuery;
        }

        public static IQueryable<TEntity> CallContainOf<TEntity>(this IQueryable<TEntity> source, string containedValues, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            var containOptions = containedValues.Trim().Split('|').ToList();

            foreach (var option in containOptions)
            {
                var containOption = option.Trim().Split(',');
                var type = typeof(TEntity);
                Type propertyType; 

                var propertyName = containOption[0].Trim();
                var value = containOption[1].Trim();
                var property = type.GetProperty(propertyName);

                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                   propertyType  = property.PropertyType.GetGenericArguments()[0];
                }
                else
                {
                    propertyType = property.PropertyType;
                }

                var predicate = PredicateBuilder.New<TEntity>(true);
                if (propertyType == typeof(string))
                {
                    predicate = predicate.And(CreateLike<TEntity>(property, value));
                }
                else if (propertyType == typeof(int))
                {
                    var castValue = Convert.ChangeType(value, typeof(int));
                    predicate = predicate.And(CreateEqual<TEntity>(property, castValue));
                }
                else if(propertyType == typeof(bool))
                {
                    var castValue = Convert.ChangeType(value, typeof(bool));
                    predicate = predicate.And(CreateEqual<TEntity>(property, castValue));
                }

                source = source.AsExpandable().Where(predicate);
            }
            return source;
        }

        private static Expression<Func<T, bool>> CreateLike<T>(PropertyInfo prop, string value, StringComparison stringComparison = StringComparison.InvariantCultureIgnoreCase)
        {
            var parameter = Expression.Parameter(typeof(T), "f");
            var propertyAccess = Expression.MakeMemberAccess(parameter, prop);
            MethodInfo equalsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            var like = Expression.Call(propertyAccess, equalsMethod, Expression.Constant(value, prop.PropertyType));

            return Expression.Lambda<Func<T, bool>>(like, parameter);
        }

        private static Expression<Func<T, bool>> CreateEqual<T>(PropertyInfo prop, object value)
        {
            var arg = Expression.Parameter(typeof(T), "i");
            return Expression.Lambda<Func<T, bool>>(Expression.Equal(Expression.Property(arg, prop.Name), Expression.Constant(value)), arg);

        }

        public static IQueryable<TEntity> GetMaxValue<TEntity>(this IQueryable<TEntity> source, string maxBy)  
        {
            var type = typeof(TEntity);
            var parameter = Expression.Parameter(type, "p");

            var command = "MAX";
            var propertyName = maxBy;

            var property = type.GetProperty(propertyName);
            var propertyAccess = Expression.MakeMemberAccess(parameter, property);


            var orderByExpression = Expression.Lambda(propertyAccess, parameter);

            var resultExpression = Expression.Call(typeof(Queryable), command, new[] { type, property.PropertyType },
                source.Expression, Expression.Quote(orderByExpression));

            var returnIQuery = source.Provider.CreateQuery<TEntity>(resultExpression);

            return returnIQuery;
        }
    }
}
