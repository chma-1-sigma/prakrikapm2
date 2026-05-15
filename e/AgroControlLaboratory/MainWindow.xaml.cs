using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Win32;
using AgroControlLaboratory.Models;

namespace AgroControlLaboratory.Views
{
    public partial class MainWindow : Window
    {
        private Button _activeButton;
        private ObservableCollection<BatchForQuality> _rawMaterialBatches;
        private ObservableCollection<BatchForQuality> _finishedGoodsBatches;
        private ObservableCollection<QualityTestHistory> _testHistory;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => { if (lblDateTime != null) lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"); };
            timer.Start();

            btnDashboard.Click += (s, e) => ShowDashboard();
            btnRawMaterials.Click += (s, e) => ShowRawMaterials();
            btnFinishedGoods.Click += (s, e) => ShowFinishedGoods();
            btnHistory.Click += (s, e) => ShowHistory();
            btnReports.Click += (s, e) => ShowReports();
            btnLogout.Click += (s, e) => Logout();

            ShowDashboard();
        }

        private void InitializeData()
        {
            _rawMaterialBatches = new ObservableCollection<BatchForQuality>();
            _rawMaterialBatches.Add(new BatchForQuality
            {
                Id = 1,
                BatchNumber = "RM-2025-001",
                ProductName = "Активное вещество А",
                Supplier = "BASF",
                ArrivalDate = new DateTime(2025, 3, 10),
                ActualQuantityKg = 1000,
                LabStatus = "Ожидает",
                SampleType = "raw_material"
            });
            _rawMaterialBatches.Add(new BatchForQuality
            {
                Id = 2,
                BatchNumber = "RM-2025-002",
                ProductName = "Растворитель Б",
                Supplier = "Dow",
                ArrivalDate = new DateTime(2025, 3, 12),
                ActualQuantityKg = 500,
                LabStatus = "Ожидает",
                SampleType = "raw_material"
            });

            _finishedGoodsBatches = new ObservableCollection<BatchForQuality>();
            _finishedGoodsBatches.Add(new BatchForQuality
            {
                Id = 1,
                BatchNumber = "B-2401-01",
                ProductName = "Гербицид А",
                StartTime = new DateTime(2025, 3, 1, 8, 0, 0),
                EndTime = new DateTime(2025, 3, 1, 14, 30, 0),
                ActualQuantityKg = 998,
                LabStatus = "Ожидает",
                SampleType = "finished_good"
            });
            _finishedGoodsBatches.Add(new BatchForQuality
            {
                Id = 2,
                BatchNumber = "B-2402-01",
                ProductName = "Инсектицид Б",
                StartTime = new DateTime(2025, 3, 3, 9, 0, 0),
                ActualQuantityKg = 250,
                LabStatus = "Ожидает",
                SampleType = "finished_good"
            });

            _testHistory = new ObservableCollection<QualityTestHistory>();
            _testHistory.Add(new QualityTestHistory
            {
                Id = 1,
                BatchNumber = "B-2401-01",
                SampleType = "Готовая продукция",
                TestDate = new DateTime(2025, 3, 1, 15, 0, 0),
                AnalystName = "Васильева Е.А.",
                Result = "Пройдено",
                Decision = "Одобрено"
            });
        }

        private void SetActiveButton(Button button, string title)
        {
            if (_activeButton != null) _activeButton.Background = Brushes.Transparent;
            _activeButton = button;
            if (button != null) button.Background = new SolidColorBrush(Color.FromRgb(61, 86, 110));
            if (lblTitle != null) lblTitle.Text = title;
        }

        private DataGrid CreateDataGrid()
        {
            DataGrid dg = new DataGrid();
            dg.Background = Brushes.White;
            dg.BorderBrush = Brushes.LightGray;
            dg.BorderThickness = new Thickness(1);
            dg.RowHeight = 35;
            dg.HeadersVisibility = DataGridHeadersVisibility.Column;
            dg.GridLinesVisibility = DataGridGridLinesVisibility.Horizontal;
            dg.HorizontalGridLinesBrush = Brushes.LightGray;
            dg.AutoGenerateColumns = false;
            dg.CanUserAddRows = false;
            dg.SelectionMode = DataGridSelectionMode.Single;
            dg.SelectionUnit = DataGridSelectionUnit.FullRow;
            return dg;
        }

        private Border CreateStatCard(string title, string value, string icon, int column)
        {
            Border border = new Border();
            border.Style = (Style)FindResource("CardStyle");
            border.Margin = new Thickness(5);
            border.Padding = new Thickness(15);

            StackPanel stack = new StackPanel();
            stack.Children.Add(new TextBlock { Text = icon + " " + title, FontSize = 12, Foreground = Brushes.Gray });
            stack.Children.Add(new TextBlock { Text = value, FontSize = 28, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(123, 31, 162)) });
            border.Child = stack;

            Grid.SetColumn(border, column);
            return border;
        }

        private void ShowDashboard()
        {
            SetActiveButton(btnDashboard, "Главная");
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            panel.Children.Add(new TextBlock { Text = "🔬 Панель управления лаборатории", FontSize = 24, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            grid.Children.Add(CreateStatCard("Сырье на контроле", _rawMaterialBatches.Count.ToString(), "📦", 0));
            grid.Children.Add(CreateStatCard("Готовая продукция", _finishedGoodsBatches.Count.ToString(), "🏭", 1));
            grid.Children.Add(CreateStatCard("Одобрено", "12", "✅", 2));
            grid.Children.Add(CreateStatCard("Заблокировано", "2", "❌", 3));
            panel.Children.Add(grid);

            panel.Children.Add(new TextBlock { Text = "Ожидают контроля", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 20, 0, 10) });

            DataGrid dg = CreateDataGrid();
            dg.Height = 300;
            dg.Columns.Add(new DataGridTextColumn { Header = "Партия", Binding = new System.Windows.Data.Binding("BatchNumber"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("ProductName"), Width = new DataGridLength(200) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Тип", Binding = new System.Windows.Data.Binding("SampleType"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("LabStatus"), Width = new DataGridLength(100) });

            ObservableCollection<BatchForQuality> allPending = new ObservableCollection<BatchForQuality>();
            foreach (var b in _rawMaterialBatches) allPending.Add(b);
            foreach (var b in _finishedGoodsBatches) allPending.Add(b);
            dg.ItemsSource = allPending;
            panel.Children.Add(dg);

            ScrollViewer scroll = new ScrollViewer();
            scroll.Content = panel;
            ContentContainer.Content = scroll;
        }

        private void ShowRawMaterials()
        {
            SetActiveButton(btnRawMaterials, "Сырье");
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);
            panel.Children.Add(new TextBlock { Text = "📦 Контроль качества сырья", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            DataGrid dg = CreateDataGrid();
            dg.Height = 350;
            dg.Columns.Add(new DataGridTextColumn { Header = "Номер партии", Binding = new System.Windows.Data.Binding("BatchNumber"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Материал", Binding = new System.Windows.Data.Binding("ProductName"), Width = new DataGridLength(200) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Поставщик", Binding = new System.Windows.Data.Binding("Supplier"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Дата поступления", Binding = new System.Windows.Data.Binding("ArrivalDateStr"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new System.Windows.Data.Binding("QuantityStr"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("LabStatus"), Width = new DataGridLength(100) });
            dg.ItemsSource = _rawMaterialBatches;
            panel.Children.Add(dg);

            StackPanel btnPanel = new StackPanel();
            btnPanel.Orientation = Orientation.Horizontal;
            btnPanel.Margin = new Thickness(0, 10, 0, 0);

            Button testBtn = new Button();
            testBtn.Content = "🔬 Провести испытание";
            testBtn.Style = (Style)FindResource("PrimaryButton");
            int selectedId = 0;
            testBtn.Click += (s, e) =>
            {
                BatchForQuality selected = dg.SelectedItem as BatchForQuality;
                if (selected != null)
                {
                    QualityTestWindow testWindow = new QualityTestWindow(selected);
                    testWindow.Owner = this;
                    testWindow.ShowDialog();
                    ShowRawMaterials();
                }
                else
                {
                    MessageBox.Show("Выберите партию для испытания", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };
            btnPanel.Children.Add(testBtn);
            panel.Children.Add(btnPanel);

            ScrollViewer scroll = new ScrollViewer();
            scroll.Content = panel;
            ContentContainer.Content = scroll;
        }

        private void ShowFinishedGoods()
        {
            SetActiveButton(btnFinishedGoods, "Готовая продукция");
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);
            panel.Children.Add(new TextBlock { Text = "🏭 Контроль качества готовой продукции", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            DataGrid dg = CreateDataGrid();
            dg.Height = 350;
            dg.Columns.Add(new DataGridTextColumn { Header = "Номер партии", Binding = new System.Windows.Data.Binding("BatchNumber"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("ProductName"), Width = new DataGridLength(200) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Дата производства", Binding = new System.Windows.Data.Binding("StartTimeStr"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new System.Windows.Data.Binding("QuantityStr"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("LabStatus"), Width = new DataGridLength(100) });
            dg.ItemsSource = _finishedGoodsBatches;
            panel.Children.Add(dg);

            StackPanel btnPanel = new StackPanel();
            btnPanel.Orientation = Orientation.Horizontal;
            btnPanel.Margin = new Thickness(0, 10, 0, 0);

            Button testBtn = new Button();
            testBtn.Content = "🔬 Провести испытание";
            testBtn.Style = (Style)FindResource("PrimaryButton");
            testBtn.Click += (s, e) =>
            {
                BatchForQuality selected = dg.SelectedItem as BatchForQuality;
                if (selected != null)
                {
                    QualityTestWindow testWindow = new QualityTestWindow(selected);
                    testWindow.Owner = this;
                    testWindow.ShowDialog();
                    ShowFinishedGoods();
                }
                else
                {
                    MessageBox.Show("Выберите партию для испытания", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };
            btnPanel.Children.Add(testBtn);
            panel.Children.Add(btnPanel);

            ScrollViewer scroll = new ScrollViewer();
            scroll.Content = panel;
            ContentContainer.Content = scroll;
        }

        private void ShowHistory()
        {
            SetActiveButton(btnHistory, "История");
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);
            panel.Children.Add(new TextBlock { Text = "📋 История лабораторных испытаний", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            DataGrid dg = CreateDataGrid();
            dg.Height = 400;
            dg.Columns.Add(new DataGridTextColumn { Header = "Партия", Binding = new System.Windows.Data.Binding("BatchNumber"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Тип", Binding = new System.Windows.Data.Binding("SampleType"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Дата", Binding = new System.Windows.Data.Binding("TestDateStr"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Лаборант", Binding = new System.Windows.Data.Binding("AnalystName"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Результат", Binding = new System.Windows.Data.Binding("Result"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Решение", Binding = new System.Windows.Data.Binding("Decision"), Width = new DataGridLength(100) });
            dg.ItemsSource = _testHistory;
            panel.Children.Add(dg);

            StackPanel btnPanel = new StackPanel();
            btnPanel.Orientation = Orientation.Horizontal;
            btnPanel.Margin = new Thickness(0, 10, 0, 0);

            Button protocolBtn = new Button();
            protocolBtn.Content = "📄 Сформировать протокол";
            protocolBtn.Style = (Style)FindResource("PrimaryButton");
            protocolBtn.Click += (s, e) =>
            {
                QualityTestHistory selected = dg.SelectedItem as QualityTestHistory;
                if (selected != null)
                {
                    GenerateProtocol(selected);
                }
                else
                {
                    MessageBox.Show("Выберите испытание", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            };
            btnPanel.Children.Add(protocolBtn);
            panel.Children.Add(btnPanel);

            ScrollViewer scroll = new ScrollViewer();
            scroll.Content = panel;
            ContentContainer.Content = scroll;
        }

        private void ShowReports()
        {
            SetActiveButton(btnReports, "Отчеты");
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);
            panel.Children.Add(new TextBlock { Text = "📊 Формирование отчетов", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            string[] reportTypes = { "Отчет по качеству сырья", "Отчет по качеству ГП", "Сводный отчет" };

            foreach (string report in reportTypes)
            {
                Border card = new Border();
                card.Style = (Style)FindResource("CardStyle");
                card.Padding = new Thickness(15);
                card.Margin = new Thickness(0, 0, 0, 10);

                StackPanel stack = new StackPanel();
                stack.Children.Add(new TextBlock { Text = "📊 " + report, FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 5) });
                stack.Children.Add(new TextBlock { Text = "Сформировать отчет за период", FontSize = 12, Foreground = Brushes.Gray, Margin = new Thickness(0, 0, 0, 10) });

                StackPanel datePanel = new StackPanel();
                datePanel.Orientation = Orientation.Horizontal;
                datePanel.Margin = new Thickness(0, 0, 0, 10);

                DatePicker dpFrom = new DatePicker();
                dpFrom.Width = 120;
                dpFrom.SelectedDate = new DateTime(2025, 3, 1);
                datePanel.Children.Add(dpFrom);

                datePanel.Children.Add(new TextBlock { Text = " - ", Margin = new Thickness(10, 0, 10, 0), VerticalAlignment = VerticalAlignment.Center });

                DatePicker dpTo = new DatePicker();
                dpTo.Width = 120;
                dpTo.SelectedDate = new DateTime(2025, 3, 31);
                datePanel.Children.Add(dpTo);
                stack.Children.Add(datePanel);

                Button btn = new Button();
                btn.Content = "📄 Сформировать";
                btn.Width = 150;
                btn.Height = 35;
                btn.Style = (Style)FindResource("PrimaryButton");
                stack.Children.Add(btn);

                card.Child = stack;
                panel.Children.Add(card);
            }

            ScrollViewer scroll = new ScrollViewer();
            scroll.Content = panel;
            ContentContainer.Content = scroll;
        }

        private void GenerateProtocol(QualityTestHistory test)
        {
            string content = "ПРОТОКОЛ ЛАБОРАТОРНЫХ ИСПЫТАНИЙ\n";
            content += new string('=', 60) + "\n\n";
            content += "Партия: " + test.BatchNumber + "\n";
            content += "Тип образца: " + test.SampleType + "\n";
            content += "Дата испытания: " + test.TestDateStr + "\n";
            content += "Лаборант: " + test.AnalystName + "\n\n";
            content += "РЕЗУЛЬТАТЫ:\n";
            content += new string('-', 60) + "\n";
            content += "Решение: " + test.Decision + "\n";
            content += "Комментарий: " + test.Comment + "\n";

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FileName = "Протокол_" + test.BatchNumber + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".txt";

            if (dialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(dialog.FileName, content);
                MessageBox.Show("Протокол сохранен: " + dialog.FileName, "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void Logout()
        {
            MessageBoxResult result = MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }
    }
}