using AgroControlOperator.Models;
using System.Windows;
using System.Windows.Controls;

namespace AgroControlOperator.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();

            btnLogin.Click += BtnLogin_Click;
            btnCancel.Click += (s, e) => Application.Current.Shutdown();
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtLogin.Text))
            {
                lblStatus.Text = "Введите табельный номер!";
                return;
            }

            if (txtLogin.Text == "operator.zavodov" && txtPassword.Password == "123")
            {
                var shift = (cmbShift.SelectedItem as ComboBoxItem)?.Content.ToString();
                var user = new User
                {
                    Id = 1,
                    Username = txtLogin.Text,
                    FullName = "Заводов Сергей Николаевич",
                    Role = "operator",
                    Shift = shift
                };
                var mainWindow = new MainWindow(user);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                lblStatus.Text = "Неверный табельный номер или пароль!";
            }
        }
    }
}