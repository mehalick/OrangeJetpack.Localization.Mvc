using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Mvc;

namespace OrangeJetpack.Localization.Mvc
{
    public static class LocalizationExtensions
    {
        /// <summary>
        /// Sets an item's localized content property from a collection and updates ModelState.
        /// </summary>
        /// <param name="item">An ILocalizable item.</param>
        /// <param name="modelStateDictionary"></param>
        /// <param name="property">The property to set.</param>
        /// <param name="contents">The collection of localized content.</param>
        /// <returns>The original item with localized property set.</returns>
        public static T Set<T>(this T item, ModelStateDictionary modelStateDictionary, Expression<Func<T, string>> property, LocalizedContent[] contents) where T : class, ILocalizable
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

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
                ClearModelStateErrors(modelStateDictionary, property);
            }

            return item;
        }

        private static void ClearModelStateErrors<T>(ModelStateDictionary modelStateDictionary, Expression<Func<T, string>> property) where T : class, ILocalizable
        {
            var memberExpression = (MemberExpression) property.Body;
            var propertyInfo = (PropertyInfo) memberExpression.Member;
            var propertyName = propertyInfo.Name;

            var modelState = modelStateDictionary
                .Where(i => i.Key.EndsWith(propertyName))
                .Where(i => i.Value.Errors.Any())
                .Select(i => i.Value)
                .FirstOrDefault();

            modelState?.Errors.Clear();
        }
    }
}