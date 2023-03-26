using System.Windows;
using PriceSupplier;
using Blotter.ViewModels;
namespace Blotter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            ConsoleWrapper.ShowConsoleWindow();

            InitializeComponent();
            // in a real application use  dependency injection framework and bootstrap the application to register the interfaces and the life cycle of the object
            // this code won't vew in the view when a dependency injection framework is used. 
            var viewModel = new BlotterViewModel(new PriceSourceCache((ccyPair,initialPrice)=> new PriceSource(ccyPair, initialPrice)));
            DataContext = viewModel;
          
        }
    }
}
