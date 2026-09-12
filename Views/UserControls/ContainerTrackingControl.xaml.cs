using System.Windows.Controls;

namespace WeighBridge.Views.UserControls
{
    /// <summary>
    /// Interaction logic for ContainerTrackingControl.xaml
    /// </summary>
    public partial class ContainerTrackingControl : UserControl
    {
        public ContainerTrackingControl()
        {
            InitializeComponent();
        }
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // nothing needed here — SelectedItem binding handles it
        }
    }
}
