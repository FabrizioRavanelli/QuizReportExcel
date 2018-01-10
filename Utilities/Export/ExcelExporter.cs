using System;
using System.Collections.Generic;
using System.Linq;
using OfficeOpenXml;
using Tools;
using TableStyles = OfficeOpenXml.Table.TableStyles;

namespace Utilities.Export
{
    [Serializable]
    public class ExcelExporter
    {

        /// <summary>
        /// Loads the values to export and creates a <see cref="CallResult{TResult}"/> of byte[] of the exported data.
        /// </summary>
        /// <typeparam name="TType">The type of data.</typeparam>
        /// <param name="objectToExport">The object to export.</param>
        /// <param name="autoFitColumns">Shall the columnwidth be automatically be assigned?</param>
        /// <returns>The <see cref="CallResult{TResult}"/> of byte[] of the export.</returns>
        public byte[] Export<TType>(TType objectToExport, bool autoFitColumns)
        {
            return Equals(objectToExport, null)
                       ? null
                       : ExportList(new[] { objectToExport }, autoFitColumns);
        }

        /// <summary>
        /// Loads the values to export and creates a <see cref="CallResult{TResult}"/> of byte[] of the exported data.
        /// </summary>
        /// <typeparam name="TType">The type of data.</typeparam>
        /// <param name="objectsToExport">The objects to export.</param>
        /// <param name="autoFitColumns">Shall the columnwidth be automatically be assigned?</param>
        /// <returns>The <see cref="CallResult{TResult}"/> of byte[] of the export.</returns>
        public byte[] ExportList<TType>(IEnumerable<TType> objectsToExport, bool autoFitColumns)
        {
            var objects = objectsToExport.ToList();
            if (!objects.Any())
                return null;
            using (var pck = new ExcelPackage())
            {
                var workSheet = pck.Workbook.Worksheets.Add("Datenexport");
                var dataRange = workSheet.Cells["A1"].LoadFromDataTable(objects.ToDataTable(), true, TableStyles.Medium2);
                if (autoFitColumns)
                    dataRange.AutoFitColumns();
                DisposeCollection(objects);
                return pck.GetAsByteArray();
            }
        }

        private void DisposeCollection<TType>(IEnumerable<TType> collection)
        {
            collection.ForEach(item => ((IDisposable)item).Dispose());
        }
    }
}
