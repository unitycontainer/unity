using System;
using System.Collections.Generic;
using System.Text;

namespace ObjectBuilder2
{
    /// <summary>
    /// A <see cref="IBuilderPolicy"/> that controls how instances are
    /// persisted and recovered with hierarchical containers. 
    /// </summary>    
    public interface IScopeLifetimePolicy : IBuilderPolicy
    {
        /// <summary>
        /// Creates controller for current scope
        /// </summary>
        /// <returns>IScopeLifetimePolicy</returns>
        IBuilderPolicy CreateScope();
    }
}
