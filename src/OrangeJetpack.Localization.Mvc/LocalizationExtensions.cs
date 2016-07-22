using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace OrangeJetpack.Localization.Mvc
{
    public static class LocalizationExtensions
    {
        public static T Update<T>(this T item, ModelStateDictionary modelStateDictionary, Expression<Func<T, string>> property, LocalizedContent[] contents) where T : class, ILocalizable
        {
            item.Set(property, contents);

            var isValid = true;

            foreach (var localizedContent in contents)
            {
                if (LocalizedContent.RequiredLanguages.Contains(localizedContent.Key))
                {
                    if (string.IsNullOrWhiteSpace(localizedContent.Value))
                    {
                        isValid = false;
                    }
                }
            }

            if (isValid)
            {
                var memberExpression = (MemberExpression)property.Body;
                var propertyInfo = (PropertyInfo)memberExpression.Member;
                var propertyName = propertyInfo.Name;

                var modelState = modelStateDictionary
                    .Where(i => i.Key.EndsWith(propertyName))
                    .Where(i => i.Value.Errors.Any())
                    .Select(i => i.Value)
                    .FirstOrDefault();

                modelState?.Errors.Clear();
            }

            return item;
        }
    }
}