﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace _4People.Converters
{
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is false;

        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            CultureInfo culture) =>
            DependencyProperty.UnsetValue;
    }
}