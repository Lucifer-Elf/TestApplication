using StringToExpression.GrammerDefinitions;
using StringToExpression.LanguageDefinitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Servize.Utility.QueryFilter.StringToExpression
{
    /// <summary>
    /// This is the StringToExpressionExtension class. This class provides RSQL format supported ODataFilterLanguage.
    /// </summary>
    public class StringToExpressionExtension : ODataFilterLanguage
    {
        /// <summary>
        /// Returns the definitions for logic operators used within the language.
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<GrammerDefinition> LogicalOperatorDefinitions()
        {
            List<GrammerDefinition> baseLogicalOperatorDefinitions = base.LogicalOperatorDefinitions().ToList();
            baseLogicalOperatorDefinitions.AddRange(GetExtendedLogicalOperatorDefinitions());

            return baseLogicalOperatorDefinitions;
        }

        private IEnumerable<GrammerDefinition> GetExtendedLogicalOperatorDefinitions()
        {
            return new GrammerDefinition[]
            {
                new BinaryOperatorDefinition(
                    name:"RSQLEQ",
                    regex: @"==",
                    orderOfPrecedence:11,
                    expressionBuilder: ConvertEnumsIfRequired((left,right) => Expression.Equal(left, right))),
                new BinaryOperatorDefinition(
                    name:"RSQLNE",
                    regex: @"!=",
                    orderOfPrecedence:12,
                    expressionBuilder: ConvertEnumsIfRequired((left,right) => Expression.NotEqual(left, right))),

                new BinaryOperatorDefinition(
                    name:"RSQLGT",
                    regex: @"=gt=",
                    orderOfPrecedence:13,
                    expressionBuilder: (left,right) => Expression.GreaterThan(left, right)),
                new BinaryOperatorDefinition(
                    name:"RSQLGTSYM",
                    regex: @">",
                    orderOfPrecedence:13,
                    expressionBuilder: (left,right) => Expression.GreaterThan(left, right)),

                new BinaryOperatorDefinition(
                    name:"RSQLGE",
                    regex: @"=ge=",
                    orderOfPrecedence:14,
                    expressionBuilder: (left,right) => Expression.GreaterThanOrEqual(left, right)),

                new BinaryOperatorDefinition(
                    name:"RSQLLT",
                    regex: @"=lt=",
                    orderOfPrecedence:15,
                    expressionBuilder: (left,right) => Expression.LessThan(left, right)),
                new BinaryOperatorDefinition(
                    name:"RSQLLTSYM",
                    regex: @"<",
                    orderOfPrecedence:15,
                    expressionBuilder: (left,right) => Expression.LessThan(left, right)),

                new BinaryOperatorDefinition(
                    name:"RSQLLE",
                    regex: @"=le=",
                    orderOfPrecedence:16,
                    expressionBuilder: (left,right) => Expression.LessThanOrEqual(left, right)),

                new BinaryOperatorDefinition(
                    name:"RSQLAND",
                    regex: @";",
                    orderOfPrecedence:17,
                    expressionBuilder: (left,right) => Expression.And(left, right)),
                new BinaryOperatorDefinition(
                    name:"RSQLOR",
                    regex: @",",
                    orderOfPrecedence:18,
                    expressionBuilder: (left,right) => Expression.Or(left, right)),

                new BinaryOperatorDefinition(
                    name:"RSQLLIKE",
                    regex: @"=like=",
                    orderOfPrecedence:19,
                    expressionBuilder: (left,right) => Expression.Call(left, "Contains", Type.EmptyTypes, right)),
                new BinaryOperatorDefinition(
                    name:"RSQLNOTLIKE",
                    regex: @"=notlike=",
                    orderOfPrecedence:20,
                    expressionBuilder: (left,right) => Expression.Not(Expression.Call(left, "Contains", Type.EmptyTypes, right)))
            };
        }
    }
}
