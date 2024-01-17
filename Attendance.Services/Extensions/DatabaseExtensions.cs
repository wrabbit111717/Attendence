using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Attendance.Services.Extensions
{
    /// <summary>
    /// Extension functions for classes related Database
    /// </summary>
    public static class DatabaseExtensions
    {
        /// <summary>
        /// Convert DataTable class to List of model
        /// </summary>
        /// <typeparam name="T">Model type</typeparam>
        /// <returns>List of models</returns>
        public static List<T> ToObjects<T>(this DataTable dataTable) where T : new()
        {
            // Get properties of destination type
            // get all name of columns in Data table to compare and put this data column into model Property
            var result = new List<T>();
            var setProps = typeof(T).GetProperties().Where(p => p.CanWrite).ToList();
            var getCols = new List<string>();
            foreach (DataColumn col in dataTable.Columns)
            {
                getCols.Add(col.ColumnName);
            }

            // browsing each row and fill into model
            foreach (DataRow row in dataTable.Rows)
            {
                var oRow = new T();
                foreach (var setProp in setProps)
                {
                    if (getCols.Contains(setProp.Name))
                    {
                        var colVal = row[setProp.Name];
                        if (colVal != DBNull.Value)
                            setProp.SetValue(oRow, colVal);
                    }
                }
                result.Add(oRow);
            }

            return result;
        }
    }
}