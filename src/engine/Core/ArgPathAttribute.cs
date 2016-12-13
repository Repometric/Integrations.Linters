﻿namespace Linterhub.Engine
{
    using System.IO;

    /// <summary>
    /// Represents configuration for linter argument with path to file/folder.
    /// </summary>
    public class ArgPathAttribute : ArgAttribute
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ArgAttribute"/> class.
        /// </summary>
        /// <param name="name">The argument name.</param>
        /// <param name="add">Whether to add a prefix and the value.</param>
        /// <param name="separator">The separator.</param>
        /// <param name="order">The order.</param>
        public ArgPathAttribute(string name = null, bool add = true, string separator = " ", int order = int.MaxValue)
            : base(name, add, separator, order)
        {
        }

        /// <summary>
        /// Get path/mask for analysis.
        /// </summary>
        /// <param name="fullPath">The full path.</param>
        /// <param name="mode">The mode.</param>
        /// <returns>The mask.</returns>
        public virtual string GetPath(string workDir, string fullPath, ArgMode mode)
        {
            var relative = workDir.Replace(fullPath, "./");
            if (mode == ArgMode.Folder)
            {
                return relative;
            }

            return fullPath;
        }
    }
}
