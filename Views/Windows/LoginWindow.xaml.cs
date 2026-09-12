using System.Windows;
using System.Windows.Controls.Primitives;
using WeighBridge.Models;
using WeighBridge.ViewModels;

namespace WeighBridge.Views.Windows
{
    public partial class LoginWindow : Window
    {
        private static readonly Dictionary<string,(string Id,string Pass,string Hint)>
            _creds = new()
            {
                ["Agent"]          = ("operator",   "operator123",   "operator / operator123"),
                ["Commercial"]     = ("commercial", "commercial123", "commercial / commercial123"),
                ["Administrator"] = ("admin",      "admin123",      "admin / admin123"),
            };

        private string _selectedRole = "Agent";

        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
            // ← wire PasswordBox manually (can't use binding)
            PbPassword.PasswordChanged += (_, _) =>
            {
                if (DataContext is LoginViewModel vm)
                    vm.Password = PbPassword.Password;
            };

            // ← wire TextBox manually if binding is broken
            TbOperatorId.TextChanged += (_, _) =>
            {
                if (DataContext is LoginViewModel vm)
                    vm.OperatorId = TbOperatorId.Text;
            };
            var timer = new System.Windows.Threading.DispatcherTimer
                { Interval = TimeSpan.FromSeconds(1) };
            timer.Tick += (_, _) => UpdateClock();
            UpdateClock();
            timer.Start();
        }

        private void UpdateClock()
        {
            TbTime.Text = DateTime.Now.ToString("HH:mm:ss");
            TbDate.Text = DateTime.Now.ToString("dd MMM yyyy").ToUpper();
        }

        private void RoleButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton clicked) return;
            BtnAgent.IsChecked = false;
            BtnCommercial.IsChecked = false;
            BtnAdmin.IsChecked = false;
            clicked.IsChecked = true;
            _selectedRole = clicked.Tag?.ToString() ?? "Agent";
            // Sync role to ViewModel
            if (DataContext is LoginViewModel vm)
            {
                vm.SelectedRole = _selectedRole switch
                {
                    "Commercial" => UserRole.Commercial,
                    "Administrator" => UserRole.Administrator,
                    _ => UserRole.Agent
                };

                if (_creds.TryGetValue(_selectedRole, out var c))
                    vm.ErrorMessage = null;
            }
            ;
        }



        

        private Shift GetShift() => CbShift.SelectedIndex switch
        {
            1 => Shift.Afternoon,
            2 => Shift.Night,
            _ => Shift.Morning
        };

        private static string GetFullName(string id) => id switch
        {
            "admin"      => "Karim Oussad",
            "commercial" => "Sara Benali",
            _            => "Ahmed Kaci"
        };

        private static string GetInitials(string id) => id switch
        {
            "admin" => "KO", "commercial" => "SB", _ => "AK"
        };

        private static UserRole ParseRole(string role) => role switch
        {
            "Commercial"     => UserRole.Commercial,
            "Administrator" => UserRole.Administrator,
            _                => UserRole.Agent
        };
    }
}
