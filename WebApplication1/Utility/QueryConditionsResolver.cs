using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Linq.Expressions;

namespace WebApplication1.Utility
{
    public abstract class QueryConditionsResolver<TEntry, TConditions>
    {
        private Expression predicate;
        private ParameterExpression parameter;

        public QueryConditionsResolver(TConditions queryConditions)
        {
            this.parameter = Expression.Parameter(typeof(TEntry));
            this.predicate = Expression.Constant(true);
            this.QueryConditions = queryConditions;
        }

        protected TConditions QueryConditions { get; set; }

        public abstract Expression<Func<TEntry, bool>> Resolve();

        protected Expression<Func<TEntry, bool>> GenerateLambdaExpression()
        {
            return Expression.Lambda<Func<TEntry, bool>>(this.predicate, this.parameter);
        }

        protected void And<TValue>(QueryCondition<TValue> queryCondition, string entryFieldName)
        {
            if (queryCondition != null)
            {
                Expression expression = null;
                Expression property = Expression.Property(this.parameter, entryFieldName);
                Expression constant = Expression.Constant(queryCondition.Value, typeof(TValue));

                switch (queryCondition.Comparsion)
                {
                    case QueryComparsion.GreaterThan:
                        expression = Expression.GreaterThan(property, constant);
                        break;

                    case QueryComparsion.LessThan:
                        expression = Expression.LessThan(property, constant);
                        break;

                    case QueryComparsion.Equal:
                        expression = Expression.Equal(property, constant);
                        break;

                    case QueryComparsion.NotEqual:
                        expression = Expression.NotEqual(property, constant);
                        break;

                    case QueryComparsion.LessThanOrEqual:
                        expression = Expression.LessThanOrEqual(property, constant);
                        break;

                    case QueryComparsion.GreaterThanOrEqual:
                        expression = Expression.GreaterThanOrEqual(property, constant);
                        break;

                    case QueryComparsion.StartsWith:
                        expression = Expression.Call(property, typeof(string).GetMethod("StartsWith", new Type[] { typeof(String) }), constant);
                        break;

                    default:
                        throw new NotSupportedException("不支援此類型");
                }
                
                this.predicate = Expression.And(this.predicate, expression);
            }
        }

        protected void Or<TValue>(QueryCondition<TValue> queryCondition, string entryFieldName)
        {

        }
    }

}

