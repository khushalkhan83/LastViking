using System;

namespace Core
{
    [AttributeUsage(AttributeTargets.Property)]
    public class InjectAttribute : Attribute
    {
        public bool IsReinjectable { get; private set; }

        public InjectAttribute(bool reInjectable = false) => IsReinjectable = reInjectable;
    }
}
