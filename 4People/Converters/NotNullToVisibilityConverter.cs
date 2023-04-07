﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace _4People.Converters
{
    public class NotNullToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
            value is null ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            CultureInfo culture) =>
            DependencyProperty.UnsetValue;
    }
}