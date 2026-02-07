using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace CoreFlow.Core.Application.Common.Builders
{
    /// <summary>    
    /// Permite a composição eficiente e dinâmica de predicados de consulta. 
    /// </summary>    
    public static class PredicateBuilder
    {
        /// <summary>    
        /// Cria um predicado que é avaliado como verdadeiro.
        /// </summary>    
        public static Expression<Func<T, bool>> True<T>() { return param => true; }

        /// <summary>    
        /// Cria um predicado que é avaliado como false.
        /// </summary>    
        public static Expression<Func<T, bool>> False<T>() { return param => false; }

        /// <summary>    
        /// Cria uma expressão de predicado da expressão lambda especificada.   
        /// </summary>    
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) { return predicate; }

        /// <summary>    
        /// Combina o primeiro predicado com o segundo usando o "e" lógico.
        /// </summary>    
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>    
        /// Combina o primeiro predicado com o segundo usando o "ou" lógico.    
        /// </summary>    
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>    
        /// Nega o predicado.
        /// </summary>    
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>    
        /// Combina a primeira expressão com a segunda usando a função de mesclagem especificada.    
        /// </summary>    
        static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // mapa dos parâmetros do segundo para os parâmetros do primeiro 
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // substitua os parâmetros na segunda expressão lambda pelos parâmetros na primeira    
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // crie uma expressão lambda mesclada com parâmetros da primeira expressão   
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        class ParameterRebinder : ExpressionVisitor
        {
            readonly Dictionary<ParameterExpression, ParameterExpression> map;

            ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;

                if (map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }

                return base.VisitParameter(p);
            }
        }
    }
}
