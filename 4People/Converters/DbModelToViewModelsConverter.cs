using System;
using System.Globalization;
using System.Windows.Data;
using _4People.Database.Models;
using _4People.ViewModels;

namespace _4People.Converters
{
    public class DbModelToViewModelsConverter : IValueConverter
    {
        public object? Convert(object value,
            Type targetType,
            object parameter,
            CultureInfo culture) =>
            value switch
            {
                Company company => new CompanyViewModel(company),
                Subdivision subdivision => new SubdivisionViewModel(subdivision),
                Employee employee => new EmployeeViewModel(employee),
                _ => null
            };

        public object ConvertBack(object value,
            Type targetType,
            object parameter,
            CultureInfo culture) =>
            throw new NotImplementedException();
    }
}