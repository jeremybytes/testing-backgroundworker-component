using System.Windows;

namespace DataProcessor
{
    public partial class MainView : Window
    {
        MainViewModel viewModel;

        public MainView()
        {
            InitializeComponent();
            viewModel = new MainViewModel();
            this.DataContext = viewModel;
        }

        private void StartButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.StartProcess();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CancelProcess();
        }

    }
}
