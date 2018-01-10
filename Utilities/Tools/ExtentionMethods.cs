using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Transactions;
using IsolationLevel = System.Transactions.IsolationLevel;
using CC = System.Diagnostics.Contracts.Contract;
using Utilities.Tools.Attribute;

namespace Tools
{
    /// <summary>
    /// Class, which provides extentions to certain types.
    /// </summary>
    public static class ExtentionMethods
    {
        /// <summary>
        /// Loops through the <see cref="IEnumerable{T}"/> and executes the given action.
        /// </summary>
        /// <typeparam name="TType">The elementtype.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
        /// <param name="doWithElement">The action.</param>
        public static void ForEach<TType>(this IEnumerable<TType> enumerable, Action<TType> doWithElement)
        {
            CC.Requires(enumerable != null);
            foreach (var type in enumerable)
                doWithElement(type);
        }

        /// <summary>
        /// Loops through the <see cref="Array"/> and executes the given action.
        /// </summary>
        /// <typeparam name="TType">The elementtype.</typeparam>
        /// <param name="array">The <see cref="IEnumerable{T}"/>.</param>
        /// <param name="doWithElement">The action.</param>
        public static void ForEach<TType>(this Array array, Action<TType> doWithElement)
        {
            CC.Requires(array != null);
            foreach (var type in array)
                doWithElement((TType)type);
        }

        /// <summary>
        /// Loops through the <see cref="IEnumerable{T}"/> and executes the given action.<br></br>
        /// As second parameter the index of the element in the <see cref="IEnumerable{T}"/> goes in.
        /// </summary>
        /// <typeparam name="TType">The elementtype.</typeparam>
        /// <param name="enumerable">The <see cref="IEnumerable{T}"/>.</param>
        /// <param name="doWithElement">The action.</param>
        public static void ForEach<TType>(this IEnumerable<TType> enumerable, Action<TType, int> doWithElement)
        {
            CC.Requires(enumerable != null);
            var list = enumerable.ToList();
            for (int index = 0; index < list.Count; index++)
                doWithElement(list[index], index);
        }

        #region DataTable

        /// <summary>
        /// Transforms the given <see cref="DataTable"/> into a "CSV" - text represention.
        /// </summary>
        /// <param name="table">The <see cref="DataTable"/>.</param>
        /// <returns>The csv - text.</returns>
        public static string ToCsv(this DataTable table)
        {
            var result = new StringBuilder();
            for (int i = 0; i < table.Columns.Count; i++)
            {
                result.Append(table.Columns[i].Caption);
                result.Append(i == table.Columns.Count - 1 ? "\r\n" : ";");
            }

            foreach (DataRow row in table.Rows)
            {
                for (int i = 0; i < table.Columns.Count; i++)
                {
                    result.Append(row[i]);
                    result.Append(i == table.Columns.Count - 1 ? "\r\n" : ";");
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Creates a <see cref="DataTable"/> from a <see cref="IEnumerable{TType}"/>. 
        /// </summary>
        /// <typeparam name="TType">The type of the list.</typeparam>
        /// <param name="items">The itemlist.</param>
        /// <returns>The <see cref="DataTable"/>.</returns>
        public static DataTable ToDataTable<TType>(this IEnumerable<TType> items)
        {
            CC.Requires(items != null);
            CC.Requires(items.Any());
            var itemList = items.ToArray();
            var type = typeof(TType);
            if (type == typeof(object))
            {
                type = itemList.First().GetType();
            }

            var dataTable = new DataTable(type.Name);

            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(prop => TryGetDoNotExportAttribute(prop) == null)
                            .ToArray();

            dataTable.Columns.AddRange(props.Select(prop =>
            {
                var foundDisplayAttribute = TryGetDisplayNameAttribute(prop);
                return new DataColumn(foundDisplayAttribute != null
                                          ? GetValueOfAttributeData<DisplayNameAttribute>(foundDisplayAttribute).Name
                                          : prop.Name.Replace('_', ' '));
            }).ToArray());

            var resultCollection = new ConcurrentBag<object[]>();
            Parallel.ForEach(itemList, item =>
            {
                var values = new object[props.Length];
                for (int index = 0; index < props.Length; index++)
                    values[index] = GetRepresentationOfValue(props[index].GetValue(item, null));
                resultCollection.Add(values);
            });
            resultCollection.ForEach(objects => dataTable.Rows.Add(objects));
            return dataTable;
        }

        /// <summary>
        /// Cast the object to the attribute of choice if possible, otherwise throws exception.
        /// </summary>
        /// <typeparam name="TAttribute">The type of the attribute.</typeparam>
        /// <param name="attribute">The attribute itself.</param>
        /// <returns>The casted attribute.</returns>
        public static TAttribute GetValueOfAttributeData<TAttribute>(this object attribute)
            where TAttribute : System.Attribute
        {
            return (TAttribute)attribute;
        }

        private static object GetRepresentationOfValue(object value)
        {
            if (value == null)
                return String.Empty;
            if (value is DateTime)
                return String.Format("{0:dd.MM.yyyy HH:mm}", (DateTime)value);
            if (value is bool)
                return GetBoolDisplayValue((bool)value);
            if (value is string || value.GetType().IsPrimitive || value is Decimal)
                return value;
            return value.ToString();
        }

        private static object TryGetDoNotExportAttribute(MemberInfo member)
        {
            return member.GetCustomAttributes(true).FirstOrDefault(attribute => attribute is DoNotExportAttribute);
        }

        /// <summary>
        /// Retrieves the <see cref="DisplayNameAttribute"/> from the <see cref="MemberInfo"/> if applied.
        /// </summary>
        /// <param name="member">The <see cref="MemberInfo"/>.</param>
        /// <returns>The Attribute as object.</returns>
        [Pure]
        public static object TryGetDisplayNameAttribute(this MemberInfo member)
        {
            CC.Requires(member != null);
            return member.GetCustomAttributes(true).FirstOrDefault(attribute => attribute is DisplayNameAttribute);
        }

        private static string GetBoolDisplayValue(bool value)
        {
            return value
                       ? "Ja"
                       : "Nein";
        }

        #endregion



    }
}
