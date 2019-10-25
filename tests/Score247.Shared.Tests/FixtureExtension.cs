using System;
using System.Linq.Expressions;

namespace Score247.Shared.Tests
{
    public static class PrivateSetterCaller
    {
        public static T With<T, TValue>(this T instance, Expression<Func<T, TValue>> propertyExpression, TValue value)
        {
            var type = typeof(T);
            var property = type.GetProperty(GetName(propertyExpression));
            property.SetValue(instance, value);

            return instance;
        }

        private static string GetName<T, TValue>(Expression<Func<T, TValue>> exp)
        {
            MemberExpression body = exp.Body as MemberExpression;

            if (body == null)
            {
                UnaryExpression ubody = (UnaryExpression)exp.Body;
                body = ubody.Operand as MemberExpression;
            }

            return body.Member.Name;
        }
    }
}