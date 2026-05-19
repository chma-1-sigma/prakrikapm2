using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AgroControlClient
{
    public partial class LoginWindow : Window
    {
        private string _currentCaptcha = string.Empty;
        private Random _random = new Random();
        private const string Characters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz0123456789";

        private int _refreshAttempts = 0;
        private const int MaxRefreshAttempts = 3;
        private DispatcherTimer _timer;
        private int _remainingSeconds = 30;

        public LoginWindow()
        {
            InitializeComponent();
            GenerateCaptcha();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;

            txtLogin.Text = "admin";
            txtPassword.Password = "123";

            btnRefresh.Click += BtnRefresh_Click;
            btnLogin.Click += BtnLogin_Click;
            btnRegister.Click += (s, e) =>
            {
                RegisterWindow registerWindow = new RegisterWindow();
                registerWindow.ShowDialog();
            };
            btnCancel.Click += (s, e) => Application.Current.Shutdown();
        }

        private void BtnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (_refreshAttempts >= MaxRefreshAttempts)
            {
                lblStatus.Text = "Лимит замены картинки исчерпан!";
                btnRefresh.IsEnabled = false;
                return;
            }

            if (_timer.IsEnabled)
            {
                lblStatus.Text = "Подождите " + _remainingSeconds + " секунд!";
                return;
            }

            _refreshAttempts++;
            GenerateCaptcha();
            _remainingSeconds = 30;
            _timer.Start();
            UpdateRefreshButtonState();
            lblStatus.Text = "Замена выполнена. Осталось попыток: " + (MaxRefreshAttempts - _refreshAttempts);
            txtCaptcha.Clear();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _remainingSeconds--;
            UpdateRefreshButtonState();

            if (_remainingSeconds <= 0)
            {
                _timer.Stop();
                UpdateRefreshButtonState();
                lblStatus.Text = "Можно заменить картинку. Осталось попыток: " + (MaxRefreshAttempts - _refreshAttempts);
            }
        }

        private void UpdateRefreshButtonState()
        {
            if (_timer.IsEnabled)
            {
                btnRefresh.Content = "⟳ (" + _remainingSeconds + "с)";
                btnRefresh.IsEnabled = false;
            }
            else
            {
                btnRefresh.Content = "⟳";
                btnRefresh.IsEnabled = _refreshAttempts < MaxRefreshAttempts;
            }
        }

        private void GenerateCaptcha()
        {
            _currentCaptcha = GenerateRandomText(6);
            imgCaptcha.Source = CreateCaptchaImage(_currentCaptcha, 280, 80);
        }

        private string GenerateRandomText(int length)
        {
            char[] result = new char[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = Characters[_random.Next(Characters.Length)];
            }
            return new string(result);
        }

        private BitmapSource CreateCaptchaImage(string text, int width, int height)
        {
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawRectangle(Brushes.White, null, new Rect(0, 0, width, height));

                for (int i = 0; i < 15; i++)
                {
                    var pen = new Pen(GetRandomBrush(), 1);
                    drawingContext.DrawLine(pen,
                        new Point(_random.Next(0, width), _random.Next(0, height)),
                        new Point(_random.Next(0, width), _random.Next(0, height)));
                }

                for (int i = 0; i < text.Length; i++)
                {
                    var formattedText = new FormattedText(
                        text[i].ToString(),
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial"),
                        24,
                        GetRandomDarkBrush(),
                        1);
                    drawingContext.DrawText(formattedText, new Point(12 + i * 38, 18));
                }
            }
            var renderTarget = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            renderTarget.Render(drawingVisual);
            return renderTarget;
        }

        private Brush GetRandomBrush()
        {
            return new SolidColorBrush(Color.FromRgb(
                (byte)_random.Next(100, 255),
                (byte)_random.Next(100, 255),
                (byte)_random.Next(100, 255)));
        }

        private Brush GetRandomDarkBrush()
        {
            return new SolidColorBrush(Color.FromRgb(
                (byte)_random.Next(0, 100),
                (byte)_random.Next(0, 100),
                (byte)_random.Next(0, 100)));
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (txtCaptcha.Text != _currentCaptcha)
            {
                lblStatus.Text = "Неверный код с картинки!";
                GenerateCaptcha();
                txtCaptcha.Clear();
                return;
            }

            if (string.IsNullOrEmpty(txtLogin.Text) || string.IsNullOrEmpty(txtPassword.Password))
            {
                lblStatus.Text = "Введите логин и пароль!";
                return;
            }

            if (txtLogin.Text == "admin" && txtPassword.Password == "123")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                lblStatus.Text = "Неверный логин или пароль!";
            }
        }
    }
}