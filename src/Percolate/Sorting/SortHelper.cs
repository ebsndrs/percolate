﻿using Microsoft.Extensions.Primitives;
using Percolate.Models;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace Percolate.Sorting
{
    public static class SortHelper
    {
        public static bool IsSortingEnabled(EnablePercolateAttribute attribute, IPercolateEntity type, PercolateOptions options)
        {
            if (attribute != default && attribute.SortSetting != PercolateAttributeSetting.Unset)
            {
                return attribute.SortSetting == PercolateAttributeSetting.Enabled;
            }
            else if (type.IsSortingEnabled.HasValue)
            {
                return type.IsSortingEnabled.Value;
            }
            else
            {
                return options.IsSortingEnabled;
            }
        }

        public static SortQuery ParseSortQuery(Dictionary<string, StringValues> queryCollection)
        {
            return SortParser.ParseSortQuery(queryCollection);
        }

        public static void ValidateSortQuery(SortQuery query, IPercolateEntity type)
        {
            SortValidator.ValidateSortQuery(query, type, SortValidator.GetSortQueryValidationRules(type));
        }

        public static IQueryable<T> ApplySortQuery<T>(IQueryable<T> queryable, SortQuery query)
        {
            if (query != null && query.Nodes.Any())
            {
                return queryable
                    .OrderBy(string.Join(',', query.Nodes.Select(node => $"{node.Name} {(node.Direction == SortQueryDirection.Ascending ? "asc" : "desc")}")));
            }
            else
            {
                return queryable;
            }
        }
    }
}
