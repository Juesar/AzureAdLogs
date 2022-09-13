using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Graph;

namespace AzureAdLogsClient.Core
{
    public interface ICsvHelper
    {
        string ObjectToCvsText<T>(List<T> data);
    }
    
    public class CsvHelper: ICsvHelper
    {

        public string ObjectToCvsText<T>(List<T> data)
        {
            var stringBuilder = new StringBuilder();

            if (data.Count == 0)
                return String.Empty;
            
            var properties = GetCsvHeaders(stringBuilder, data[0]);
            foreach (var row in data)
            {
                GetCsvData(stringBuilder, row, properties);
            }
            
            return stringBuilder.ToString();
        }
        
        private static void GetCsvData<T>(StringBuilder stringBuilder, T data, List<PropertyData> listOfProperties)
        {
            stringBuilder.AppendLine();

            foreach (var propertyData in listOfProperties)
            {
                if (propertyData.IsChildObject)
                {
                    var internalObject = propertyData.ParentPropertyInfo.GetValue(data);
                    stringBuilder.Append(propertyData.PropertyInfo.GetValue(internalObject)?.ToString().Replace(",", "-"));
                    stringBuilder.Append(",");
                    continue;
                }

                var property = propertyData.PropertyInfo;

                if (property.PropertyType == typeof(string) ||
                    property.PropertyType == typeof(bool) ||
                    property.PropertyType == typeof(int) ||
                    property.PropertyType == typeof(DateTime))
                {
                    stringBuilder.Append(property.GetValue(data)?.ToString().Replace(",", "-"));
                    stringBuilder.Append(",");
                }
            }
        }

        private static List<PropertyData> GetCsvHeaders<T>(StringBuilder stringBuilder, T data)
        {
            var dataType = data.GetType();
            var properties = dataType.GetProperties();
            var listOfProperties = new List<PropertyData>();
            foreach (var property in properties)
            {
                if (property.Name.Contains("List"))
                {
                    continue;
                }

                if (property.PropertyType == typeof(string) ||
                    property.PropertyType == typeof(bool) ||
                    property.PropertyType == typeof(int) ||
                    property.PropertyType == typeof(DateTime))
                {
                    stringBuilder.Append(property.Name);
                    stringBuilder.Append(",");
                    listOfProperties.Add(new PropertyData() { PropertyInfo = property, IsChildObject = false });
                    continue;
                }

                var internalObject = property.GetValue(data);
                if (internalObject == null)
                    continue;
                var internalType = internalObject.GetType();

                if (internalType.Name.Contains("List"))
                    continue;

                var internalClassProperties = internalType.GetProperties().OrderBy(p => p.Name).ToList();
                foreach (var internalProperty in internalClassProperties)
                {
                    stringBuilder.Append(property.Name);
                    stringBuilder.Append("_");
                    stringBuilder.Append(internalProperty.Name);
                    stringBuilder.Append(",");
                    listOfProperties.Add(new PropertyData()
                        { PropertyInfo = internalProperty, ParentPropertyInfo = property, IsChildObject = true });
                }
            }

            return listOfProperties;
        }
    }
    
    public class PropertyData
    {
        public PropertyInfo PropertyInfo { get; set; }
        public PropertyInfo ParentPropertyInfo { get; set; }
        public bool IsChildObject { get; set; }
    }
}