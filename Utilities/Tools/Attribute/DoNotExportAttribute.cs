using System;

namespace Utilities.Tools.Attribute
{
    /// <summary>
    /// When attributed, the target property shall not be respected by an exporter.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DoNotExportAttribute : System.Attribute
    {
    }
}
