using System.Collections.Generic;
using System.Linq;
using ServiceStack.Text;

namespace TalisScraper
{
    public static class Extensions
    {
        /// <summary>
        /// Serialises an object to Json using the ServiceStack Json serialiser.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this object obj)
        {
            return JsonSerializer.SerializeToString(obj);
        }

        /// <summary>
        /// Converts Json object to the specified 'T' class object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T FromJson<T>(this string obj)
        {
            return JsonSerializer.DeserializeFromString<T>(obj);
        }



        /// <summary>
        /// Checks if a list is not null and contains objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static bool HasContent<T>(this IList<T> collection)
        {
            return collection != null && collection.Any();
        }

        public static bool HasContent<T>(this IEnumerable<T> collection)
        {
            //TODO: Will this cause performance issues???
            return collection != null && collection.Skip(0).Any();
        }

        public static bool HasContent<T, T1>(this IDictionary<T, T1> collection)
        {
            return collection != null && collection.Any();
        }
    }
}
