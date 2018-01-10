using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Tools.Attribute
{
    /// <summary>
    /// Attribute which contains the name to display.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DisplayNameAttribute : System.Attribute
    {
        /// <summary>
        /// The name to display.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="name"></param>
        public DisplayNameAttribute(string name)
        {
            Name = name;
        }
    }
}
