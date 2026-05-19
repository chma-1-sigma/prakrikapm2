using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AgroControlLaboratory.Views
{
    public partial class LoginWindow : Window
    {
        private string _currentCaptcha = string.Empty;
        private Random _random = new Random();
        private const string Characters = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz0123456789";

        public LoginWindow()
        {
            InitializeComponent();
            GenerateCaptcha();

            btnRefresh.Click += (s, e) => GenerateCaptcha();
            btnLogin.Click += BtnLogin_Click;
            btnCancel.Click += (s, e) => Application.Current.Shutdown();
        }

        private void GenerateCaptcha()
        {
            _currentCaptcha = GenerateRandomText(6);
            imgCaptcha.Source = CreateCaptchaImage(_currentCaptcha, 260, 70);
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

            if (txtLogin.Text == "lab.vasilieva" && txtPassword.Password == "123")
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