﻿using NHateoas.Configuration;
using System;


namespace NHateoas.Dynamic.Interfaces
{
    internal interface ITypeBuilderStrategy
    {
        /// <summary>
        /// Returns class key string according to specified strategy and type
        /// Different source types and strategies creates different proxy types. Class key used to distnguish namespaces 
        /// and works as a key for internal type cache 
        /// </summary>
        /// <returns></returns>
        string ClassKey(Type originalType);

        /// <summary>
        /// Configure specified container according to the specified strategy
        /// </summary>
        /// <param name="container"></param>
        void Configure(ITypeBuilderContainer container);

        /// <summary>
        /// Activate concrete instance
        /// </summary>
        /// <param name="proxyInstance"></param>
        /// <param name="originalInstance"></param>
        /// <param name="metadataProvider"></param>
        void ActivateInstance(object proxyInstance, object originalInstance, IActionConfiguration metadataProvider);
    }
}
