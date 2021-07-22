using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace StaffRandomSelect.Domain
{
    public class ListContext : Freezable
    {
        protected override Freezable CreateInstanceCore()
        {
            return new ListContext();
        }

        public object Context
        {
            get => GetValue(dependencyProperty);
            set => SetValue(dependencyProperty, value);
        }

        public static readonly DependencyProperty dependencyProperty =
            DependencyProperty.Register("Context", typeof(object), typeof(ListContext), new UIPropertyMetadata(null));
    }
}
