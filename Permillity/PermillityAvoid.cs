using System;

namespace Permillity
{
    /// <summary>
    /// Attribute that tells permillity to avoid tracking performance of this endpoint.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
    public class PermillityAvoid : Attribute
    {
    }
}
