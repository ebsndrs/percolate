﻿using Percolate.Models;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Percolate.Builders
{
    public class PercolateEntityBuilder<TEntity> where TEntity : class
    {
        public PercolateEntityBuilder()
        {
            Model = new PercolateType<TEntity>();
        }

        public PercolateEntityBuilder(IPercolateEntity model)
        {
            Model = model;
        }

        public IPercolateEntity Model { get; set; }

        public PercolateEntityBuilder<TEntity> CanPage(bool canPage = true)
        {
            Model.IsPagingEnabled = canPage;
            return this;
        }

        public PercolateEntityBuilder<TEntity> CanSort(bool canSort = true)
        {
            Model.IsSortingEnabled = canSort;
            return this;
        }

        public PercolateEntityBuilder<TEntity> CanFilter(bool canFilter = true)
        {
            Model.IsFilteringEnabled = canFilter;
            return this;
        }

        public PercolateEntityBuilder<TEntity> HasDefaultPageSize(int defaultPageSize)
        {
            if (defaultPageSize < 1)
            {
                throw new ArgumentException();
            }

            Model.DefaultPageSize = defaultPageSize;
            return this;
        }

        public PercolateEntityBuilder<TEntity> HasMaxPageSize(int maximumPageSize)
        {
            if (maximumPageSize < 1)
            {
                throw new ArgumentException($"{nameof(maximumPageSize)} cannot be less than 1.");
            }

            Model.MaximumPageSize = maximumPageSize;
            return this;
        }

        public PercolatePropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TEntity, TProperty>> propertyExpression)
        {
            PercolatePropertyBuilder<TProperty> propertyBuilder;

            if (!(propertyExpression.Body is MemberExpression memberExpression))
            {
                throw new NotSupportedException();
            }

            if (!(memberExpression.Member is PropertyInfo propertyInfo))
            {
                throw new NotSupportedException();
            }

            var existingPropertyModel = Model.Properties
                .FirstOrDefault(p => p.Name == propertyInfo.Name);

            /*
             * TODO:
             * Right now, this method and its related methods and constructors only traverse down one level of the 'type graph.'
             * Eventually, we'd like to support any level so that one can pass in (x => x.Foo.Bar).Whatever() and it will resolve properly.
             * For now, we need to ensure that the expression that was passed in can resolve for TType. It will be a runtime error.
             * Maybe there is a way to resolve this in the propertyExpression?
             *
             * The consequence of this is that any property of a type that does not implement ICompareable cannot be sorted and any property that isn't a value type
             * cannot be filtered on. So this is a really important thing to implement. One can imagine when they might want to be able to sort on x.Foo.Bar.
             * Reflection can be used to find that property and call SetValue on it, but configuring the model here for it is harder.
             *
             * GetProperties with the correct binding flags can get us some of the way there, but there are certain classes (DateTime is one) that will cause a stack overflow.
             * So I'm not sure if there's a way to control for those certain cases that might blow it up. Maybe: get the properties, check if a stack overflow might occur by
             * seeing if any of the retreived properties share a type with the type you retrieved the properties for, and only add subproperties if that check returns false?
             * That will add any property that wouldn't cause a recursive stack overflow.
             */

            if (!typeof(TEntity).GetProperties().Any(p => p.Name == propertyInfo.Name))
            {
                throw new NotSupportedException();
            }

            if (existingPropertyModel == default)
            {
                propertyBuilder = new PercolatePropertyBuilder<TProperty>(propertyInfo);
                Model.Properties.Add(propertyBuilder.Model);
            }
            else
            {
                propertyBuilder = new PercolatePropertyBuilder<TProperty>(existingPropertyModel);
            }

            return propertyBuilder;
        }
    }
}
