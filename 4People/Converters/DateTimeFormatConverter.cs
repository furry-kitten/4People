﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace _4People.Converters
{
    public class DateTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is DateTime dateTime ? dateTime.ToShortDateString() : value;

        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            CultureInfo culture) =>
            DependencyProperty.UnsetValue;
    }
}