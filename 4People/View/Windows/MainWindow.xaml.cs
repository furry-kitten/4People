using System.Linq;
using System.Windows;
using System.Windows.Controls;
using _4People.View.Windows;
using _4People.ViewModels;

namespace _4People
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _model;

        public MainWindow()
        {
            InitializeComponent();
        }

        public MainWindow(MainViewModel model)
        {
            InitializeComponent();
            _model = model;
            DataContext = _model;
        }

        private void TreeView_OnSelectedItemChanged(object sender,
            RoutedPropertyChangedEventArgs<object> e)
        {
            _model.SelectedItem = e.NewValue as BaseDbModelViewModel;
        }

        private void AddCompanyBtn_OnClick(object sender, RoutedEventArgs e)
        {
            _model.AddCompany();
        }

        private void AddSubdivisionBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var dataContext = ((e.Source as Button)!.DataContext as CompanyViewModel)!;
            dataContext.AddSubdivision();
            _model.SelectedItem = dataContext.Subdivisions.Last();
        }

        private void AddEmployeesBtn_OnClick(object sender, RoutedEventArgs e)
        {
            var dataContext = ((e.Source as Button)!.DataContext as SubdivisionViewModel)!;
            dataContext.AddEmployee();
            _model.SelectedItem = dataContext.Employees.Last();
        }

        private void Save_OnClick(object sender, RoutedEventArgs e)
        {
            _model.Save();
        }

        private void ShowReports_OnClick(object sender, RoutedEventArgs e)
        {
            ReportWindow reportWindow = new();
            var reportWindowDataContext = new ReportViewModel();
            reportWindow.DataContext = reportWindowDataContext;
            reportWindow.Show();
        }
        private void Remove_OnClick(object sender, RoutedEventArgs e)
        {
            _model.Remove();
        }
    }
}