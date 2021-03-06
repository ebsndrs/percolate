﻿//using Microsoft.AspNetCore.Http.Internal;
//using Microsoft.Extensions.Primitives;
//using Percolate.Exceptions;
//using Percolate.Filtering;
//using System.Collections.Generic;
//using System.Linq;
//using Xunit;

//namespace PercolateTests.UnitTests.ParserTests
//{
//    public class FilterParserTests
//    {
//        [Fact]
//        public void ParseFilterParameters_WhenCalledWithNoQueryParameter_ReturnsDefaultValues()
//        {
//            var queryCollection = new QueryCollection();

//            var result = FilterParser.ParseFilterQuery(queryCollection);

//            Assert.Empty(result.Nodes);
//        }

//        /*
//         * We might consider breaking out these test methods; perhaps one with "normal" query strings,
//         * one  with crazy wonky strings with multiple operators, and one with pipe delimiters?
//         */
//        [Fact]
//        public void ParseFilterParameters_WhenCalledWithValidQueryParameter_ReturnsParsedValues()
//        {
//            var expectedResults = new List<FilterQueryNode>
//            {
//                new FilterQueryNode
//                {
//                    RawNode = "name=James",
//                    Properties = new string[] { "name" },
//                    Values = new string[] { "James" },
//                    RawOperator = "=",
//                    Operator = FilterQueryOperator.Equals,
//                    IsOperatorNegated = false
//                },
//                new FilterQueryNode
//                {
//                    RawNode = "country!=USA",
//                    Properties = new string[] { "country" },
//                    Values = new string[] { "USA" },
//                    RawOperator = "!=",
//                    Operator = FilterQueryOperator.DoesNotEqual,
//                    IsOperatorNegated = true
//                },
//                new FilterQueryNode
//                {
//                    RawNode = "age>20",
//                    Properties = new string[] { "age" },
//                    Values = new string[] { "20" },
//                    RawOperator = ">",
//                    Operator = FilterQueryOperator.GreaterThan,
//                    IsOperatorNegated = false
//                },
//                new FilterQueryNode
//                {
//                    RawNode = $"birthday<12/2/1994",
//                    Properties = new string[] { "birthday" },
//                    Values = new string[] { "12/2/1994" },
//                    RawOperator = "<",
//                    Operator = FilterQueryOperator.LessThan,
//                    IsOperatorNegated = false
//                },
//                new FilterQueryNode
//                {
//                    RawNode = "likes>=20",
//                    Properties = new string[] { "likes" },
//                    Values = new string[] { "20" },
//                    RawOperator = ">=",
//                    Operator = FilterQueryOperator.GreaterThanOrEqual,
//                    IsOperatorNegated = false
//                },
//                new FilterQueryNode
//                {
//                    RawNode = "posts<=10",
//                    Properties = new string[] { "posts" },
//                    Values = new string[] { "10" },
//                    RawOperator = "<=",
//                    Operator = FilterQueryOperator.LessThanOrEqual,
//                    IsOperatorNegated = false
//                },

//                //some wonky ones with multiple operators
//                new FilterQueryNode
//                {
//                    RawNode = "x=!>=!==y",
//                    Properties = new string[] { "x" },
//                    Values = new string[] { "!>=!==y" },
//                    RawOperator = "=",
//                    Operator = FilterQueryOperator.Equals,
//                    IsOperatorNegated = false
//                },
//                new FilterQueryNode
//                {
//                    RawNode = "i=<=>=!==<>j",
//                    Properties = new string[] { "i" },
//                    Values = new string[] { "<=>=!==<>j" },
//                    RawOperator = "=",
//                    Operator = FilterQueryOperator.Equals,
//                    IsOperatorNegated = false
//                },

//                //some ones with pipe delimited properties and values
//                new FilterQueryNode
//                {
//                    RawNode = "age|posts>20",
//                    Properties = new string[] { "age", "posts"},
//                    Values = new string[] { "20"},
//                    RawOperator = ">",
//                    Operator = FilterQueryOperator.GreaterThan,
//                    IsOperatorNegated = false
//                },
//                new FilterQueryNode
//                {
//                    RawNode = "name=Amy|Joe",
//                    Properties = new string[] { "name" },
//                    Values = new string[] { "Amy", "Joe"},
//                    RawOperator = "=",
//                    Operator = FilterQueryOperator.Equals,
//                    IsOperatorNegated = false
//                },
//                new FilterQueryNode
//                {
//                    RawNode = "age|posts!=13|45",
//                    Properties = new string[] { "age", "posts" },
//                    Values = new string[] { "13", "45" },
//                    RawOperator = "!=",
//                    Operator = FilterQueryOperator.DoesNotEqual,
//                    IsOperatorNegated = true
//                },

//                //and some with escaped pipes and commas in the value
//                new FilterQueryNode
//                {
//                    RawNode = @"text=hello\, world!",
//                    Properties = new string[] { "text" },
//                    Values = new string[] { "hello, world!" },
//                    RawOperator = "=",
//                    Operator = FilterQueryOperator.Equals,
//                    IsOperatorNegated = false
//                },
//                new FilterQueryNode
//                {
//                    RawNode = @"text=i\|hate\|regex",
//                    Properties = new string[] { "text" },
//                    Values = new string[] { "i|hate|regex" },
//                    RawOperator = "=",
//                    Operator = FilterQueryOperator.Equals,
//                    IsOperatorNegated = false
//                },
//                new FilterQueryNode
//                {
//                    RawNode = "name|city!=Jane Doe|Pawnee",
//                    Properties = new string[] { "name", "city" },
//                    Values = new string[] { "Jane Doe", "Pawnee" },
//                    RawOperator = "!=",
//                    Operator = FilterQueryOperator.DoesNotEqual,
//                    IsOperatorNegated = true
//                }
//            };

//            var queryCollection = new QueryCollection(new Dictionary<string, StringValues>()
//            {
//                { "filter", string.Join(',', expectedResults.Select(node => node.RawNode)) }
//            });

//            var result = FilterParser.ParseFilterQuery(queryCollection);

//            var resultNodes = result.Nodes.ToList();

//            Assert.All(resultNodes, (node) =>
//            {
//                //for any given node, the index of expected results and the parsed results should match
//                var expectedResult = expectedResults.ElementAt(resultNodes.IndexOf(node));

//                Assert.Equal(expectedResult.RawNode, node.RawNode);
//                Assert.Equal(expectedResult.Properties, node.Properties);
//                Assert.Equal(expectedResult.Values, node.Values);
//                Assert.Equal(expectedResult.RawOperator, node.RawOperator);
//                Assert.Equal(expectedResult.Operator, node.Operator);
//                Assert.Equal(expectedResult.IsOperatorNegated, node.IsOperatorNegated);
//            });
//        }

//        //TODO: rewrite this entire test to account for more invalid parameters
//        [Fact]
//        public void ParseFilterParameters_WhenCalledWithInvalidParameter_ThrowsException()
//        {
//            var filterString = "foo=";

//            var store = new Dictionary<string, StringValues>()
//            {
//                { "filter", filterString }
//            };

//            var queryCollection = new QueryCollection(store);

//            /*
//             * At this point, you might think, "Why not just call Assert.Throws with () => FilterParser.ParseFilterQuery(queryCollection)?
//             * We have to write the test this way because ParseFilterQuery() does not explicitly enumerate the list of nodes;
//             * it projects them using a select statement. Thus, the expected exception is never *actually* thrown.
//             * For normal operations, this is fine, because it defers the enumeration until we actually need it.
//             * But we didn't want to rewrite the business logic for a test, so we wrote the test to enumerate the list of nodes.
//             */

//            var result = FilterParser.ParseFilterQuery(queryCollection);

//            Assert.Throws<ParameterParsingException>(() => result.Nodes.ToList());
//        }
//    }
//}
