using System.Windows;
using System.Windows.Controls;
using _4People.ViewModels.Main;

namespace _4People.View.Controls
{
    /// <summary>
    ///     Логика взаимодействия для SubdivisionControl.xaml
    /// </summary>
    public partial class SubdivisionControl : UserControl
    {
        public SubdivisionControl()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is SubdivisionViewModel model)
            {
                model.RemoveLeader();
            }
        }
    }
}