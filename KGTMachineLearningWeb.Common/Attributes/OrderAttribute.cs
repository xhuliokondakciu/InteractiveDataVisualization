using System;
using System.Runtime.CompilerServices;

namespace KGTMachineLearningWeb.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class OrderAttribute : Attribute
    {
        public int Order { get; }

        public OrderAttribute([CallerLineNumber]int order = 0)
        {
            Order = order;
        }
    }
}
