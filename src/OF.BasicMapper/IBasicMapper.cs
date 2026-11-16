using System.Collections.Generic;

namespace OF.BasicMapper
{
    /// <summary>
    /// Copies values between objects by matching public property names.
    /// </summary>
    public interface IBasicMapper
    {
        /// <summary>
        /// Creates a new <typeparamref name="T"/> and copies every compatible property from <paramref name="from"/> into it.
        /// </summary>
        /// <typeparam name="T">Destination type. Must have a public parameterless constructor.</typeparam>
        /// <param name="from">Source object.</param>
        T MapFields<T>(object from);

        /// <summary>
        /// Creates a new <typeparamref name="T"/> and copies every compatible property from <paramref name="from"/>,
        /// except the property names listed in <paramref name="skip"/>.
        /// </summary>
        T MapFields<T>(object from, List<string> skip);

        /// <summary>
        /// Copies every compatible property from <paramref name="from"/> onto the existing object <paramref name="to"/>
        /// and returns it as <typeparamref name="T"/>.
        /// </summary>
        T MapFields<T>(object from, object to);

        /// <summary>
        /// Copies every compatible property from <paramref name="from"/> onto the existing object <paramref name="to"/>,
        /// except the property names listed in <paramref name="skip"/>, and returns it as <typeparamref name="T"/>.
        /// </summary>
        T MapFields<T>(object from, object to, List<string> skip);
    }
}
