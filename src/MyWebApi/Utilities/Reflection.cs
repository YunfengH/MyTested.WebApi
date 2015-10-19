﻿// MyWebApi - ASP.NET Web API Fluent Testing Framework
// Copyright (C) 2015 Ivaylo Kenov.
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see http://www.gnu.org/licenses/.

namespace MyWebApi.Utilities
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    /// <summary>
    /// Class for validating reflection checks.
    /// </summary>
    public static class Reflection
    {
        /// <summary>
        /// Checks whether two objects have the same types.
        /// </summary>
        /// <param name="firstObject">First object to be checked.</param>
        /// <param name="secondObject">Second object to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool AreSameTypes(object firstObject, object secondObject)
        {
            if (firstObject == null || secondObject == null)
            {
                return firstObject == secondObject;
            }

            return AreSameTypes(firstObject.GetType(), secondObject.GetType());
        }

        /// <summary>
        /// Checks whether two types are different.
        /// </summary>
        /// <param name="firstType">First type to be checked.</param>
        /// <param name="secondType">Second type to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool AreSameTypes(Type firstType, Type secondType)
        {
            return firstType == secondType;
        }

        /// <summary>
        /// Checks whether two objects have different types.
        /// </summary>
        /// <param name="firstObject">First object to be checked.</param>
        /// <param name="secondObject">Second object to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool AreDifferentTypes(object firstObject, object secondObject)
        {
            return !AreSameTypes(firstObject, secondObject);
        }

        /// <summary>
        /// Checks whether two types are different.
        /// </summary>
        /// <param name="firstType">First type to be checked.</param>
        /// <param name="secondType">Second type to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool AreDifferentTypes(Type firstType, Type secondType)
        {
            return !AreSameTypes(firstType, secondType);
        }

        /// <summary>
        /// Checks whether two types are assignable.
        /// </summary>
        /// <param name="baseType">Base type to be checked.</param>
        /// <param name="inheritedType">Inherited type to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool AreAssignable(Type baseType, Type inheritedType)
        {
            return baseType.IsAssignableFrom(inheritedType);
        }

        /// <summary>
        /// Checks whether two types are not assignable.
        /// </summary>
        /// <param name="baseType">Base type to be checked.</param>
        /// <param name="inheritedType">Inherited type to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool AreNotAssignable(Type baseType, Type inheritedType)
        {
            return !AreAssignable(baseType, inheritedType);
        }

        /// <summary>
        /// Checks whether a type is generic.
        /// </summary>
        /// <param name="type">Type to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool IsGeneric(Type type)
        {
            return type.IsGenericType;
        }

        /// <summary>
        /// Checks whether a type is not generic.
        /// </summary>
        /// <param name="type">Type to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool IsNotGeneric(Type type)
        {
            return !IsGeneric(type);
        }

        /// <summary>
        /// Checks whether a type is generic definition.
        /// </summary>
        /// <param name="type">Type to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool IsGenericTypeDefinition(Type type)
        {
            return type.IsGenericTypeDefinition;
        }

        /// <summary>
        /// Checks whether two types are assignable by generic definition.
        /// </summary>
        /// <param name="baseType">Base type to be checked.</param>
        /// <param name="inheritedType">Inherited type to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool AreAssignableByGeneric(Type baseType, Type inheritedType)
        {
            return IsGeneric(inheritedType) && IsGeneric(baseType) &&
                   baseType.IsAssignableFrom(inheritedType.GetGenericTypeDefinition());
        }

        /// <summary>
        /// Checks whether two generic types have different generic arguments.
        /// </summary>
        /// <param name="baseType">Base type to be checked.</param>
        /// <param name="inheritedType">Inherited type to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool HaveDifferentGenericArguments(Type baseType, Type inheritedType)
        {
            if (IsNotGeneric(baseType) || IsNotGeneric(inheritedType))
            {
                return true;
            }

            var baseTypeGenericArguments = baseType.GetGenericArguments();
            var inheritedTypeGenericArguments = inheritedType.GetGenericArguments();

            if (baseTypeGenericArguments.Length != inheritedTypeGenericArguments.Length)
            {
                return true;
            }

            return baseTypeGenericArguments.Where((t, i) => t != inheritedTypeGenericArguments[i]).Any();
        }

        /// <summary>
        /// Checks whether generic definition contains an base interface generic definition.
        /// </summary>
        /// <param name="baseType">Base type to be checked.</param>
        /// <param name="inheritedType">Inherited type to be checked.</param>
        /// <returns>True or false.</returns>
        public static bool ContainsGenericTypeDefinitionInterface(Type baseType, Type inheritedType)
        {
            return inheritedType
                .GetInterfaces()
                .Where(t => t.IsGenericType)
                .Select(t => t.GetGenericTypeDefinition())
                .Any(t => t == baseType);
        }

        /// <summary>
        /// Performs dynamic casting from type to generic result.
        /// </summary>
        /// <typeparam name="TResult">Result type from casting.</typeparam>
        /// <param name="type">Type from which the casting should be done.</param>
        /// <param name="data">Object from which the casting should be done.</param>
        /// <returns>Casted object of type TResult.</returns>
        public static TResult CastTo<TResult>(this Type type, object data)
        {
            var dataParam = Expression.Parameter(typeof(object), "data");
            var firstConvert = Expression.Convert(dataParam, data.GetType());
            var secondConvert = Expression.Convert(firstConvert, type);
            var body = Expression.Block(new Expression[] { secondConvert });

            var run = Expression.Lambda(body, dataParam).Compile();
            var ret = run.DynamicInvoke(data);
            return (TResult)ret;
        }

        /// <summary>
        /// Transforms generic type name to friendly one, showing generic type arguments.
        /// </summary>
        /// <param name="type">Type which name will be transformed.</param>
        /// <returns>Transformed name as string.</returns>
        public static string ToFriendlyTypeName(this Type type)
        {
            if (!type.IsGenericType)
            {
                return type.Name;
            }

            var genericArgumentNames = type.GetGenericArguments().Select(ga => ga.ToFriendlyTypeName());
            var friendlyGenericName = type.Name.Split('`')[0];
            var joinedGenericArgumentNames = string.Join(", ", genericArgumentNames);

            return string.Format("{0}<{1}>", friendlyGenericName, joinedGenericArgumentNames);
        }

        /// <summary>
        /// Tries to create instance of type T by using the provided unordered constructor parameters.
        /// </summary>
        /// <typeparam name="T">Type of created instance.</typeparam>
        /// <param name="constructorParameters">Unordered constructor parameters.</param>
        /// <returns>Created instance or null, if no suitable constructor found.</returns>
        public static T TryCreateInstance<T>(params object[] constructorParameters)
            where T : class
        {
            var type = typeof(T);
            T instance = null;
            try
            {
                instance = Activator.CreateInstance(type, constructorParameters) as T;
            }
            catch (Exception)
            {
                var constructorParameterTypes = constructorParameters
                    .Select(cp => cp.GetType())
                    .ToList();

                var constructor = type.GetConstructorByUnorderedParameters(constructorParameterTypes);
                if (constructor == null)
                {
                    return instance;
                }

                var selectedConstructorParameters = constructor
                    .GetParameters()
                    .Select(cp => cp.ParameterType)
                    .ToList();

                var typeObjectDictionary = constructorParameters.ToDictionary(k => k.GetType());
                var resultParameters = new List<object>();
                foreach (var selectedConstructorParameterType in selectedConstructorParameters)
                {
                    foreach (var constructorParameterType in constructorParameterTypes)
                    {
                        if (selectedConstructorParameterType.IsAssignableFrom(constructorParameterType))
                        {
                            resultParameters.Add(typeObjectDictionary[constructorParameterType]);
                            break;
                        }
                    }
                }

                instance = Activator.CreateInstance(type, resultParameters.ToArray()) as T;
            }

            return instance;
        }

        /// <summary>
        /// Gets custom attributes on the provided object.
        /// </summary>
        /// <param name="obj">Object decorated with custom attribute.</param>
        /// <returns>IEnumerable of objects representing the custom attributes.</returns>
        public static IEnumerable<object> GetCustomAttributes(object obj)
        {
            return obj.GetType().GetCustomAttributes(true);
        }

        public static bool AreDeeplyEqual(object expected, object actual)
        {
            if (expected == null && actual == null)
            {
                return true;
            }

            if (expected == null || actual == null)
            {
                return false;
            }

            var expectedType = expected.GetType();
            var actualType = actual.GetType();
            var objectType = typeof(object);

            if ((expectedType == objectType && actualType != objectType)
                || (actualType == objectType && expectedType != objectType))
            {
                return false;
            }

            if (expected is IEnumerable)
            {
                if (CollectionsAreDeeplyEqual(expected, actual))
                {
                    return true;
                }

                return false;
            }

            if (expectedType != actualType
                && !expectedType.IsAssignableFrom(actualType)
                && !actualType.IsAssignableFrom(expectedType))
            {
                return false;
            }

            if (expectedType.IsPrimitive && actualType.IsPrimitive)
            {
                return expected.ToString() == actual.ToString();
            }

            if (expectedType != objectType)
            {
                var equalsMethod = expectedType.GetMethods().FirstOrDefault(m => m.Name == "Equals" && m.DeclaringType == expectedType);
                if (equalsMethod != null)
                {
                    return (bool)equalsMethod.Invoke(expected, new[] { actual });
                }
            }

            var equalsOperator = expectedType.GetMethods().FirstOrDefault(m => m.Name == "op_Equality");
            if (equalsOperator != null)
            {
                return (bool)equalsOperator.Invoke(null, new[] { expected, actual });
            }

            if (ComparablesAreDeeplyEqual(expected, actual))
            {
                return true;
            }

            if (!ObjectPropertiesAreDeeplyEqual(expected, actual))
            {
                return false;
            }

            return true;
        }

        public static bool AreNotDeeplyEqual(object expected, object actual)
        {
            return !AreDeeplyEqual(expected, actual);
        }

        private static ConstructorInfo GetConstructorByUnorderedParameters(this Type type, IEnumerable<Type> types)
        {
            var orderedTypes = types
                .OrderBy(t => t.FullName)
                .ToList();

            var constructor = type
                .GetConstructors()
                .Where(c =>
                {
                    var parameters = c.GetParameters()
                        .OrderBy(p => p.ParameterType.FullName)
                        .Select(p => p.ParameterType)
                        .ToList();

                    if (orderedTypes.Count != parameters.Count)
                    {
                        return false;
                    }

                    return !orderedTypes.Where((t, i) => !parameters[i].IsAssignableFrom(t)).Any();
                })
                .FirstOrDefault();

            return constructor;
        }

        private static bool CollectionsAreDeeplyEqual(object expected, object actual)
        {
            var expectedAsEnumerable = (IEnumerable)expected;
            var actualAsEnumerable = actual as IEnumerable;
            if (actualAsEnumerable == null)
            {
                return false;
            }

            var listOfExpectedValues = expectedAsEnumerable.Cast<object>().ToList();
            var listOfActualValues = actualAsEnumerable.Cast<object>().ToList();

            if (listOfExpectedValues.Count != listOfActualValues.Count)
            {
                return false;
            }

            var collectionIsNotEqual = listOfExpectedValues
                .Where((t, i) => AreNotDeeplyEqual(t, listOfActualValues[i]))
                .Any();

            if (collectionIsNotEqual)
            {
                return false;
            }

            return true;
        }

        private static bool ComparablesAreDeeplyEqual(object expected, object actual)
        {
            var expectedAsIComparable = expected as IComparable;
            if (expectedAsIComparable != null)
            {
                if (expectedAsIComparable.CompareTo(actual) == 0)
                {
                    return true;
                }
            }

            if (ObjectImplementsIComparable(expected) && ObjectImplementsIComparable(actual))
            {
                var method = expected.GetType().GetMethod("CompareTo");
                if (method != null)
                {
                    return (int)method.Invoke(expected, new[] { actual }) == 0;
                }
            }


            return false;
        }

        private static bool ObjectImplementsIComparable(object obj)
        {
            return obj.GetType()
                .GetInterfaces()
                .FirstOrDefault(i => i.Name.StartsWith("IComparable")) != null;
        }

        private static bool ObjectPropertiesAreDeeplyEqual(object expected, object actual)
        {
            var properties = expected.GetType().GetProperties();
            foreach (var property in properties)
            {
                if (property.GetIndexParameters().Length != 0)
                {
                    continue;
                }

                var expectedPropertyValue = property.GetValue(expected);
                var actualPropertyValue = property.GetValue(actual);

                if (expectedPropertyValue is IEnumerable)
                {
                    if (!CollectionsAreDeeplyEqual(expectedPropertyValue, actualPropertyValue))
                    {
                        return false;
                    }
                }

                var propertiesAreDifferent = AreNotDeeplyEqual(expectedPropertyValue, actualPropertyValue);
                if (propertiesAreDifferent)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
