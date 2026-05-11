using System;

namespace AutoDI
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class RegisterScopedAttribute : Attribute
    {
    }
}
