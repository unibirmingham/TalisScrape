using System.Windows.Controls;


namespace TalisScrapeWPF.Pages.Settings
{
    /// <summary>
    /// Interaction logic for Appearance.xaml
    /// </summary>
    public partial class Appearance : UserControl
    {
        public Appearance()
        {
            InitializeComponent();

            // create and assign the appearance view model
            DataContext = new AppearanceViewModel();
        }
    }
}
