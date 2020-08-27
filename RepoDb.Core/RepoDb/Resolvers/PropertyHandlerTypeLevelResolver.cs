﻿using RepoDb.Interfaces;
using System;

namespace RepoDb.Resolvers
{
    /// <summary>
    /// A class that is used to resolve the equivalent <see cref="IPropertyHandler{TInput, TResult}"/> object of the .NET CLR type.
    /// </summary>
    public class PropertyHandlerTypeLevelResolver : IResolver<Type, object>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PropertyHandlerTypeLevelResolver"/> object.
        /// </summary>
        public PropertyHandlerTypeLevelResolver() { }

        /// <summary>
        /// Resolves the equivalent <see cref="IPropertyHandler{TInput, TResult}"/> object of the .NET CLR type.
        /// </summary>
        /// <param name="type">The .NET CLR type</param>
        /// <returns>The equivalent <see cref="IPropertyHandler{TInput, TResult}"/> object of the .NET CLR type.</returns>
        public object Resolve(Type type)
        {
            return PropertyHandlerMapper.Get<object>(type);
        }
    }
}
