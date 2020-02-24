using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Smart_Hold.Xml
{
    public interface IXmlConfig
    {
        /// <summary>
        /// Gets the root.
        /// </summary>
        /// <value>The root.</value>
        XmlNode Root { get; }

        /// <summary>
        /// Erase config
        /// </summary>
        void Clear();

        /// <summary>
        /// Check if property exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        bool Exists(string name);

        /// <summary>
        /// Gets the property as string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        string GetString(string name);

        /// <summary>
        /// Gets the property as decimal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        decimal GetDecimal(string name);

        /// <summary>
        /// Sets the property as string.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void SetString(string name, string value);

        /// <summary>
        /// Sets the property as decimal.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void SetDecimal(string name, decimal value);
    }
}
