// Copyright (c) All contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

#if !ENABLE_IL2CPP
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Utf8Json.Emit
{
    internal static class ExpressionUtility
    {
        // Method

        private static MethodInfo GetMethodInfoCore(LambdaExpression expression)
        {
            return ((MethodCallExpression)expression.Body).Method;
        }

        /// <summary>
        /// Get MethodInfo from Expression for Static(with result) method.
        /// </summary>
        public static MethodInfo GetMethodInfo<T>(Expression<Func<T>> expression)
        {
            return GetMethodInfoCore(expression);
        }

        /// <summary>
        /// Get MethodInfo from Expression for Static(void) method.
        /// </summary>
        public static MethodInfo GetMethodInfo(Expression<Action> expression)
        {
            return GetMethodInfoCore(expression);
        }

        /// <summary>
        /// Get MethodInfo from Expression for Instance(with result) method.
        /// </summary>
        public static MethodInfo GetMethodInfo<T, TR>(Expression<Func<T, TR>> expression)
        {
            return GetMethodInfoCore(expression);
        }

        /// <summary>
        /// Get MethodInfo from Expression for Instance(void) method.
        /// </summary>
        public static MethodInfo GetMethodInfo<T>(Expression<Action<T>> expression)
        {
            return GetMethodInfoCore(expression);
        }

        // WithArgument(for ref, out) helper

        /// <summary>
        /// Get MethodInfo from Expression for Instance(void) method.
        /// </summary>
        public static MethodInfo GetMethodInfo<TArg1, TArg2>(Expression<Action<TArg1, TArg2>> expression)
        {
            return GetMethodInfoCore(expression);
        }

        /// <summary>
        /// Get MethodInfo from Expression for Instance(with result) method.
        /// </summary>
        public static MethodInfo GetMethodInfo<T, TArg1, TR>(Expression<Func<T, TArg1, TR>> expression)
        {
            return GetMethodInfoCore(expression);
        }

        // Property

        private static MemberInfo GetMemberInfoCore<T>(Expression<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            var memberExpression = (MemberExpression)source.Body;
            return memberExpression.Member;
        }

        public static PropertyInfo GetPropertyInfo<T, TR>(Expression<Func<T, TR>> expression)
        {
            return (PropertyInfo)GetMemberInfoCore(expression);
        }

        // Field

        public static FieldInfo GetFieldInfo<T, TR>(Expression<Func<T, TR>> expression)
        {
            return (FieldInfo)GetMemberInfoCore(expression);
        }
    }
}
#endif
