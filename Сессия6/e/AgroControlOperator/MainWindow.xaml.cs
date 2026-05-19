using AgroControlOperator.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AgroControlOperator.Views
{
    public partial class MainWindow : Window
    {
        private Button _activeButton;
        private ObservableCollection<ActiveBatch> _activeBatches;
        private BatchProgram _currentProgram;
        private int _currentStepIndex = 0;
        private ProgramStep _currentStep;
        private bool _stepInProgress = false;
        private Random _random = new Random();
        private User _currentUser;

        // ==================== МОДЕЛИ ДАННЫХ ====================
        public class ActiveBatch
        {
            public int Id { get; set; }
            public string BatchNumber { get; set; }
            public string ProductName { get; set; }
            public string Line { get; set; }
            public string CurrentStep { get; set; }
            public string Status { get; set; }
            public int Progress { get; set; }
            public bool HasWarning { get; set; }
            public bool HasCritical { get; set; }
            public DateTime StartTime { get; set; }
            public string Operator { get; set; }
        }

        public class ProgramStep
        {
            public int Id { get; set; }
            public int StepOrder { get; set; }
            public string StepName { get; set; }
            public string StepType { get; set; }
            public string Status { get; set; }
            public string Instruction { get; set; }
            public int PlannedDurationMin { get; set; }
            public ObservableCollection<StepParameter> PlannedParameters { get; set; }
            public ObservableCollection<ActualParameter> ActualParameters { get; set; }
            public DateTime? StartTime { get; set; }
            public DateTime? EndTime { get; set; }
            public string OperatorComment { get; set; }
        }

        public class StepParameter
        {
            public string ParameterName { get; set; }
            public decimal PlannedValue { get; set; }
            public string Unit { get; set; }
            public decimal? ToleranceMin { get; set; }
            public decimal? ToleranceMax { get; set; }
            public bool IsCritical { get; set; }
        }

        public class ActualParameter
        {
            public string ParameterName { get; set; }
            public decimal? ActualValue { get; set; }
            public string Unit { get; set; }
            public bool IsDeviated { get; set; }
            public string DeviationReason { get; set; }
        }

        public class BatchProgram
        {
            public int Id { get; set; }
            public string BatchNumber { get; set; }
            public string ProductName { get; set; }
            public string Line { get; set; }
            public string Status { get; set; }
            public ObservableCollection<ProgramStep> Steps { get; set; }
        }

        public class JournalEvent
        {
            public DateTime Time { get; set; }
            public string TimeStr => Time.ToString("HH:mm:ss");
            public string Event { get; set; }
            public string User { get; set; }
            public string Details { get; set; }
        }

        public MainWindow(User user)
        {
            InitializeComponent();
            _currentUser = user;

            // Загрузка картинки
            LoadLogoImage();

            txtUserName.Text = user.FullName;
            txtUserShift.Text = "Смена: " + user.Shift;

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) =>
            {
                if (lblDateTime != null)
                    lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

                if (_stepInProgress && _currentStep != null)
                    UpdateTelemetry();
            };
            timer.Start();

            btnActiveBatches.Click += (s, e) => ShowActiveBatches();
            btnProgram.Click += (s, e) => ShowProgram();
            btnExtruder.Click += (s, e) => ShowExtruder();
            btnJournal.Click += (s, e) => ShowJournal();
            btnProblem.Click += (s, e) => ShowProblem();
            btnLogout.Click += (s, e) => Logout();

            InitData();
            ShowActiveBatches();
        }

        // ==================== ЗАГРУЗКА КАРТИНКИ ====================
        private void LoadLogoImage()
        {
            
        }

        // ==================== ИНИЦИАЛИЗАЦИЯ ДАННЫХ ====================
        private void InitData()
        {
            _activeBatches = new ObservableCollection<ActiveBatch>();

            string[] products = { "Гербицид А", "Инсектицид Б", "Фунгицид В", "Регулятор роста Д", "Инсектицид Е",
                                  "Гербицид F", "Фунгицид G", "Инсектицид H", "Регулятор I", "Гербицид J" };
            string[] steps = { "Смешивание", "Выдержка", "Экструзия", "Охлаждение", "Фасовка", "Контроль", "Упаковка" };

            for (int i = 1; i <= 12; i++)
            {
                int progress = _random.Next(10, 95);
                bool hasWarning = _random.Next(5) == 0;
                bool hasCritical = _random.Next(10) == 0;
                int stepIndex = progress / 25;
                stepIndex = Math.Min(stepIndex, steps.Length - 1);

                _activeBatches.Add(new ActiveBatch
                {
                    Id = i,
                    BatchNumber = $"B-2403-{i:D3}",
                    ProductName = products[(i - 1) % products.Length],
                    Line = $"Линия №{(i % 3) + 1}",
                    CurrentStep = steps[stepIndex],
                    Status = "running",
                    Progress = progress,
                    HasWarning = hasWarning,
                    HasCritical = hasCritical,
                    StartTime = DateTime.Now.AddHours(-_random.Next(1, 12)),
                    Operator = "Заводов С.Н."
                });
            }

            // Программа партии
            _currentProgram = new BatchProgram
            {
                Id = 1,
                BatchNumber = "B-2403-001",
                ProductName = "Гербицид А",
                Line = "Линия №1",
                Status = "running",
                Steps = new ObservableCollection<ProgramStep>()
            };

            // Шаг 1
            var step1 = new ProgramStep
            {
                Id = 1,
                StepOrder = 1,
                StepName = "Смешивание компонентов",
                StepType = "mixing",
                Status = "completed",
                Instruction = "Загрузить компоненты в порядке рецептуры. Перемешивать 30 минут.",
                PlannedDurationMin = 30,
                PlannedParameters = new ObservableCollection<StepParameter>(),
                ActualParameters = new ObservableCollection<ActualParameter>(),
                StartTime = DateTime.Now.AddHours(-3),
                EndTime = DateTime.Now.AddHours(-2.5)
            };
            step1.PlannedParameters.Add(new StepParameter { ParameterName = "Время", PlannedValue = 30, Unit = "мин", ToleranceMin = 25, ToleranceMax = 35 });
            step1.ActualParameters.Add(new ActualParameter { ParameterName = "Время", ActualValue = 32, Unit = "мин", IsDeviated = false });
            _currentProgram.Steps.Add(step1);

            // Шаг 2
            var step2 = new ProgramStep
            {
                Id = 2,
                StepOrder = 2,
                StepName = "Выдержка",
                StepType = "holding",
                Status = "completed",
                Instruction = "Выдержать при температуре 60°C в течение 2 часов.",
                PlannedDurationMin = 120,
                PlannedParameters = new ObservableCollection<StepParameter>(),
                ActualParameters = new ObservableCollection<ActualParameter>(),
                StartTime = DateTime.Now.AddHours(-2.5),
                EndTime = DateTime.Now.AddHours(-0.5)
            };
            step2.PlannedParameters.Add(new StepParameter { ParameterName = "Температура", PlannedValue = 60, Unit = "°C", ToleranceMin = 55, ToleranceMax = 65, IsCritical = true });
            step2.PlannedParameters.Add(new StepParameter { ParameterName = "Время", PlannedValue = 120, Unit = "мин", ToleranceMin = 110, ToleranceMax = 130 });
            step2.ActualParameters.Add(new ActualParameter { ParameterName = "Температура", ActualValue = 59.5m, Unit = "°C", IsDeviated = false });
            step2.ActualParameters.Add(new ActualParameter { ParameterName = "Время", ActualValue = 118, Unit = "мин", IsDeviated = false });
            _currentProgram.Steps.Add(step2);

            // Шаг 3 (текущий)
            var step3 = new ProgramStep
            {
                Id = 3,
                StepOrder = 3,
                StepName = "Экструзия",
                StepType = "extrusion",
                Status = "in_progress",
                Instruction = "Подать смесь на экструдер. Контролировать температуру (80°C) и давление (3.0 bar).",
                PlannedDurationMin = 45,
                PlannedParameters = new ObservableCollection<StepParameter>(),
                ActualParameters = new ObservableCollection<ActualParameter>(),
                StartTime = DateTime.Now.AddMinutes(-15)
            };
            step3.PlannedParameters.Add(new StepParameter { ParameterName = "Температура", PlannedValue = 80, Unit = "°C", ToleranceMin = 75, ToleranceMax = 85, IsCritical = true });
            step3.PlannedParameters.Add(new StepParameter { ParameterName = "Давление", PlannedValue = 3.0m, Unit = "bar", ToleranceMin = 2.5m, ToleranceMax = 3.5m, IsCritical = true });
            step3.PlannedParameters.Add(new StepParameter { ParameterName = "Скорость шнека", PlannedValue = 150, Unit = "rpm", ToleranceMin = 140, ToleranceMax = 160 });
            _currentProgram.Steps.Add(step3);
            _currentStep = step3;
            _currentStepIndex = 2;
            _stepInProgress = true;

            // Шаг 4
            var step4 = new ProgramStep
            {
                Id = 4,
                StepOrder = 4,
                StepName = "Охлаждение",
                StepType = "cooling",
                Status = "pending",
                Instruction = "Охладить продукт до 25°C.",
                PlannedDurationMin = 60,
                PlannedParameters = new ObservableCollection<StepParameter>(),
                ActualParameters = new ObservableCollection<ActualParameter>()
            };
            step4.PlannedParameters.Add(new StepParameter { ParameterName = "Температура", PlannedValue = 25, Unit = "°C", ToleranceMin = 20, ToleranceMax = 30 });
            _currentProgram.Steps.Add(step4);
        }

        private void SetActiveButton(Button button, string title)
        {
            if (_activeButton != null) _activeButton.Background = Brushes.Transparent;
            _activeButton = button;
            if (button != null) button.Background = new SolidColorBrush(Color.FromRgb(40, 53, 147));
            lblTitle.Text = title;
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

        // ==================== ЭКРАН АКТИВНЫХ ПАРТИЙ ====================
        private void ShowActiveBatches()
        {
            SetActiveButton(btnActiveBatches, "Активные партии");
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            panel.Children.Add(new TextBlock { Text = "Активные партии в цехе", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            Grid statsGrid = new Grid();
            statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            statsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            statsGrid.Margin = new Thickness(0, 0, 0, 15);

            statsGrid.Children.Add(CreateStatCard("Всего партий", _activeBatches.Count.ToString(), "📊", 0));
            statsGrid.Children.Add(CreateStatCard("С отклонениями", _activeBatches.Count(b => b.HasWarning || b.HasCritical).ToString(), "⚠️", 1));
            statsGrid.Children.Add(CreateStatCard("Критических", _activeBatches.Count(b => b.HasCritical).ToString(), "🔴", 2));
            panel.Children.Add(statsGrid);

            DataGrid dg = CreateDataGrid();
            dg.Height = 350;
            dg.Columns.Add(new DataGridTextColumn { Header = "Партия", Binding = new System.Windows.Data.Binding("BatchNumber"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("ProductName"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Линия", Binding = new System.Windows.Data.Binding("Line"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Текущий шаг", Binding = new System.Windows.Data.Binding("CurrentStep"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Прогресс", Binding = new System.Windows.Data.Binding("Progress"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(80) });

            dg.LoadingRow += (s, e) =>
            {
                var batch = e.Row.DataContext as ActiveBatch;
                if (batch != null)
                {
                    if (batch.HasCritical)
                        e.Row.Background = new SolidColorBrush(Colors.LightCoral);
                    else if (batch.HasWarning)
                        e.Row.Background = new SolidColorBrush(Colors.LightGoldenrodYellow);
                }
            };

            dg.ItemsSource = _activeBatches;
            panel.Children.Add(dg);

            StackPanel btnPanel = new StackPanel();
            btnPanel.Orientation = Orientation.Horizontal;
            btnPanel.Margin = new Thickness(0, 15, 0, 0);
            btnPanel.HorizontalAlignment = HorizontalAlignment.Center;

            Button selectBtn = new Button();
            selectBtn.Content = "Выбрать партию";
            selectBtn.Style = (Style)FindResource("PrimaryButton");
            selectBtn.Margin = new Thickness(0, 0, 15, 0);
            selectBtn.Click += (s, e) =>
            {
                if (dg.SelectedItem != null)
                {
                    var selected = dg.SelectedItem as ActiveBatch;
                    ShowProgramForBatch(selected);
                }
                else
                    MessageBox.Show("Выберите партию из списка", "Внимание", MessageBoxButton.OK, MessageBoxImage.Warning);
            };
            btnPanel.Children.Add(selectBtn);

            Button refreshBtn = new Button();
            refreshBtn.Content = "Обновить";
            refreshBtn.Style = (Style)FindResource("PrimaryButton");
            refreshBtn.Click += (s, e) => ShowActiveBatches();
            btnPanel.Children.Add(refreshBtn);

            panel.Children.Add(btnPanel);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private Border CreateStatCard(string title, string value, string icon, int column)
        {
            Border border = new Border();
            border.Style = (Style)FindResource("CardStyle");
            border.Margin = new Thickness(5);
            border.Padding = new Thickness(15);

            StackPanel stack = new StackPanel();
            stack.Children.Add(new TextBlock { Text = icon + " " + title, FontSize = 12, Foreground = Brushes.Gray });
            stack.Children.Add(new TextBlock { Text = value, FontSize = 24, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(21, 101, 192)) });
            border.Child = stack;

            Grid.SetColumn(border, column);
            return border;
        }

        private void ShowProgramForBatch(ActiveBatch batch)
        {
            if (batch.BatchNumber == "B-2403-001")
                ShowProgram();
            else
                MessageBox.Show($"Программа для партии {batch.BatchNumber} загружается...\n\nПродукт: {batch.ProductName}\nТекущий шаг: {batch.CurrentStep}\nПрогресс: {batch.Progress}%",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // ==================== ЭКРАН ПРОГРАММЫ ПАРТИИ ====================
        private void ShowProgram()
        {
            SetActiveButton(btnProgram, "Программа партии");
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            panel.Children.Add(new TextBlock { Text = "Программа партии: " + _currentProgram.BatchNumber, FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });
            panel.Children.Add(new TextBlock { Text = $"Продукт: {_currentProgram.ProductName} | Линия: {_currentProgram.Line} | Статус: В работе", Margin = new Thickness(0, 0, 0, 15) });

            int completedSteps = _currentProgram.Steps.Count(s => s.Status == "completed");
            int progressValue = completedSteps * 100 / _currentProgram.Steps.Count;

            panel.Children.Add(new TextBlock { Text = "Прогресс выполнения", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) });
            ProgressBar progressBar = new ProgressBar();
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = progressValue;
            progressBar.Height = 20;
            progressBar.Margin = new Thickness(0, 0, 0, 20);
            panel.Children.Add(progressBar);
            panel.Children.Add(new TextBlock { Text = $"Выполнено шагов: {completedSteps} из {_currentProgram.Steps.Count} ({progressValue}%)", FontSize = 12, Foreground = Brushes.Gray, Margin = new Thickness(0, -15, 0, 20) });

            panel.Children.Add(new TextBlock { Text = "Шаги технологического процесса", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) });

            Grid stepsGrid = new Grid();
            stepsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(60) });
            stepsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            stepsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            stepsGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            int rowIndex = 0;
            foreach (var step in _currentProgram.Steps)
            {
                stepsGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                string statusIcon = step.Status == "completed" ? "✅" : (step.Status == "in_progress" ? "🔄" : "⏳");

                TextBlock orderBlock = new TextBlock();
                orderBlock.Text = step.StepOrder.ToString();
                orderBlock.Margin = new Thickness(5);
                stepsGrid.Children.Add(orderBlock);
                Grid.SetRow(orderBlock, rowIndex);
                Grid.SetColumn(orderBlock, 0);

                TextBlock nameBlock = new TextBlock();
                nameBlock.Text = step.StepName;
                nameBlock.Margin = new Thickness(5);
                nameBlock.FontWeight = step.Status == "in_progress" ? FontWeights.Bold : FontWeights.Normal;
                stepsGrid.Children.Add(nameBlock);
                Grid.SetRow(nameBlock, rowIndex);
                Grid.SetColumn(nameBlock, 1);

                TextBlock statusBlock = new TextBlock();
                statusBlock.Text = statusIcon;
                statusBlock.Margin = new Thickness(5);
                statusBlock.FontSize = 16;
                stepsGrid.Children.Add(statusBlock);
                Grid.SetRow(statusBlock, rowIndex);
                Grid.SetColumn(statusBlock, 2);

                TextBlock detailsBlock = new TextBlock();
                detailsBlock.Text = step.Status == "completed" ? $"Выполнен: {step.EndTime?.ToString("HH:mm")}" :
                                   (step.Status == "in_progress" ? "В процессе..." : "Ожидает");
                detailsBlock.Margin = new Thickness(5);
                detailsBlock.Foreground = Brushes.Gray;
                detailsBlock.FontSize = 11;
                stepsGrid.Children.Add(detailsBlock);
                Grid.SetRow(detailsBlock, rowIndex);
                Grid.SetColumn(detailsBlock, 3);

                rowIndex++;
            }
            panel.Children.Add(stepsGrid);

            panel.Children.Add(new TextBlock { Text = "Текущий шаг: " + _currentStep.StepName, FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 20, 0, 10) });

            Border instructionCard = new Border();
            instructionCard.Style = (Style)FindResource("CardStyle");
            instructionCard.Padding = new Thickness(15);
            instructionCard.Margin = new Thickness(0, 0, 0, 15);
            instructionCard.Background = new SolidColorBrush(Colors.LightYellow);
            instructionCard.Child = new TextBlock { Text = "📋 Инструкция: " + _currentStep.Instruction, TextWrapping = TextWrapping.Wrap };
            panel.Children.Add(instructionCard);

            Border paramCard = new Border();
            paramCard.Style = (Style)FindResource("CardStyle");
            paramCard.Padding = new Thickness(15);
            paramCard.Margin = new Thickness(0, 0, 0, 15);

            StackPanel paramStack = new StackPanel();
            paramStack.Children.Add(new TextBlock { Text = "Плановые параметры:", FontWeight = FontWeights.Bold });

            foreach (var param in _currentStep.PlannedParameters)
            {
                string tolerance = "";
                if (param.ToleranceMin.HasValue && param.ToleranceMax.HasValue)
                    tolerance = $" (допуск: {param.ToleranceMin}-{param.ToleranceMax} {param.Unit})";
                else if (param.ToleranceMin.HasValue)
                    tolerance = $" (мин: {param.ToleranceMin} {param.Unit})";
                else if (param.ToleranceMax.HasValue)
                    tolerance = $" (макс: {param.ToleranceMax} {param.Unit})";

                paramStack.Children.Add(new TextBlock { Text = $"{param.ParameterName}: {param.PlannedValue} {param.Unit}{tolerance}", Margin = new Thickness(0, 5, 0, 0) });
            }

            paramStack.Children.Add(new Separator { Margin = new Thickness(0, 10, 0, 10) });
            paramStack.Children.Add(new TextBlock { Text = "Фактические значения:", FontWeight = FontWeights.Bold });

            Grid inputGrid = new Grid();
            inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            inputGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

            int paramRow = 0;
            foreach (var param in _currentStep.PlannedParameters)
            {
                inputGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var existingActual = _currentStep.ActualParameters.FirstOrDefault(a => a.ParameterName == param.ParameterName);

                inputGrid.Children.Add(new TextBlock { Text = param.ParameterName + ":", Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center });
                Grid.SetRow(inputGrid.Children[inputGrid.Children.Count - 1], paramRow);
                Grid.SetColumn(inputGrid.Children[inputGrid.Children.Count - 1], 0);

                TextBox valueBox = new TextBox();
                valueBox.Name = "txt" + param.ParameterName.Replace(" ", "");
                valueBox.Text = existingActual?.ActualValue?.ToString() ?? "";
                valueBox.Width = 100;
                valueBox.Margin = new Thickness(5);
                valueBox.Tag = param;
                inputGrid.Children.Add(valueBox);
                Grid.SetRow(valueBox, paramRow);
                Grid.SetColumn(valueBox, 1);

                inputGrid.Children.Add(new TextBlock { Text = param.Unit, Margin = new Thickness(5), VerticalAlignment = VerticalAlignment.Center });
                Grid.SetRow(inputGrid.Children[inputGrid.Children.Count - 1], paramRow);
                Grid.SetColumn(inputGrid.Children[inputGrid.Children.Count - 1], 2);

                paramRow++;
            }
            paramStack.Children.Add(inputGrid);

            TextBox commentBox = new TextBox();
            commentBox.Text = _currentStep.OperatorComment ?? "";
            commentBox.Height = 60;
            commentBox.TextWrapping = TextWrapping.Wrap;
            commentBox.Margin = new Thickness(0, 10, 0, 0);
            paramStack.Children.Add(commentBox);

            paramCard.Child = paramStack;
            panel.Children.Add(paramCard);

            StackPanel btnPanel = new StackPanel();
            btnPanel.Orientation = Orientation.Horizontal;
            btnPanel.HorizontalAlignment = HorizontalAlignment.Center;

            Button startStepBtn = new Button();
            startStepBtn.Content = "▶ Начать шаг";
            startStepBtn.Style = (Style)FindResource("PrimaryButton");
            startStepBtn.Margin = new Thickness(0, 0, 15, 0);
            startStepBtn.IsEnabled = _currentStep.Status != "in_progress";
            startStepBtn.Click += (s, e) =>
            {
                _currentStep.Status = "in_progress";
                _currentStep.StartTime = DateTime.Now;
                _stepInProgress = true;
                ShowProgram();
            };
            btnPanel.Children.Add(startStepBtn);

            Button saveResultsBtn = new Button();
            saveResultsBtn.Content = "💾 Сохранить результаты";
            saveResultsBtn.Style = (Style)FindResource("SuccessButton");
            saveResultsBtn.Margin = new Thickness(0, 0, 15, 0);
            saveResultsBtn.Click += (s, e) => SaveStepResults(inputGrid, commentBox);
            btnPanel.Children.Add(saveResultsBtn);

            Button completeStepBtn = new Button();
            completeStepBtn.Content = "✅ Завершить шаг";
            completeStepBtn.Style = (Style)FindResource("WarningButton");
            completeStepBtn.Click += (s, e) => CompleteStep();
            btnPanel.Children.Add(completeStepBtn);

            panel.Children.Add(btnPanel);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void SaveStepResults(Grid inputGrid, TextBox commentBox)
        {
            _currentStep.ActualParameters.Clear();
            bool hasDeviation = false;
            string deviationMessage = "";

            foreach (var child in inputGrid.Children)
            {
                if (child is TextBox tb && tb.Tag is StepParameter param && !string.IsNullOrEmpty(tb.Text))
                {
                    decimal value;
                    if (decimal.TryParse(tb.Text, out value))
                    {
                        bool isDeviated = false;
                        string deviationReason = "";

                        if (param.ToleranceMin.HasValue && value < param.ToleranceMin.Value)
                        {
                            isDeviated = true;
                            deviationReason = $"{param.ParameterName} ниже нормы: {value} {param.Unit} (мин: {param.ToleranceMin} {param.Unit})";
                            hasDeviation = true;
                            deviationMessage += deviationReason + "\n";
                        }
                        else if (param.ToleranceMax.HasValue && value > param.ToleranceMax.Value)
                        {
                            isDeviated = true;
                            deviationReason = $"{param.ParameterName} выше нормы: {value} {param.Unit} (макс: {param.ToleranceMax} {param.Unit})";
                            hasDeviation = true;
                            deviationMessage += deviationReason + "\n";
                        }

                        _currentStep.ActualParameters.Add(new ActualParameter
                        {
                            ParameterName = param.ParameterName,
                            ActualValue = value,
                            Unit = param.Unit,
                            IsDeviated = isDeviated,
                            DeviationReason = deviationReason
                        });
                    }
                }
            }

            _currentStep.OperatorComment = commentBox.Text;

            if (hasDeviation)
            {
                MessageBox.Show($"Обнаружены отклонения!\n\n{deviationMessage}\nПроверьте значения.",
                    "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                MessageBox.Show("Результаты сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            ShowProgram();
        }

        private void CompleteStep()
        {
            if (_currentStep.ActualParameters.Count == 0 && _currentStep.PlannedParameters.Count > 0)
            {
                MessageBox.Show("Сначала сохраните результаты измерений!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentStep.Status = "completed";
            _currentStep.EndTime = DateTime.Now;
            _stepInProgress = false;

            int completedSteps = _currentProgram.Steps.Count(s => s.Status == "completed");
            int totalSteps = _currentProgram.Steps.Count;
            int newProgress = completedSteps * 100 / totalSteps;

            var batch = _activeBatches.FirstOrDefault(b => b.BatchNumber == _currentProgram.BatchNumber);
            if (batch != null)
            {
                batch.Progress = newProgress;
            }

            int currentIndex = _currentProgram.Steps.IndexOf(_currentStep);
            if (currentIndex + 1 < _currentProgram.Steps.Count)
            {
                _currentStep = _currentProgram.Steps[currentIndex + 1];
                _currentStepIndex = currentIndex + 1;
                _stepInProgress = false;
            }

            MessageBox.Show($"Шаг '{_currentStep.StepName}' завершен!", "Успех",
                MessageBoxButton.OK, MessageBoxImage.Information);

            ShowProgram();
        }

        private void UpdateTelemetry()
        {
            if (_currentStep != null && _currentStep.StepType == "extrusion" && _stepInProgress)
            {
                double currentTemp = 78.5 + (_random.NextDouble() - 0.5) * 3;
                double currentPress = 2.9 + (_random.NextDouble() - 0.5) * 0.4;
            }
        }

        // ==================== ЭКРАН ЭКСТРУДЕРА ====================
        private void ShowExtruder()
        {
            SetActiveButton(btnExtruder, "Экструдер LIVE");
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            panel.Children.Add(new TextBlock { Text = "Мониторинг экструдера", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            string[] data = {
                "Зона 1:", "85°C / 85°C", "Зона 2:", "90°C / 90°C",
                "Зона 3:", "95°C / 95°C", "Зона 4:", "100°C / 100°C",
                "Давление:", "3.2 bar / 3.5 bar", "Скорость:", "148 rpm / 150 rpm"
            };

            for (int i = 0; i < data.Length / 2; i++)
            {
                TextBlock label = new TextBlock();
                label.Text = data[i * 2];
                label.FontWeight = FontWeights.Bold;
                label.Margin = new Thickness(5);
                grid.Children.Add(label);
                Grid.SetRow(label, i);
                Grid.SetColumn(label, 0);

                TextBlock value = new TextBlock();
                value.Text = data[i * 2 + 1];
                value.Margin = new Thickness(5);
                grid.Children.Add(value);
                Grid.SetRow(value, i);
                Grid.SetColumn(value, 1);
            }
            panel.Children.Add(grid);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        // ==================== ЖУРНАЛ ПАРТИИ ====================
        private void ShowJournal()
        {
            SetActiveButton(btnJournal, "Журнал партии");
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            panel.Children.Add(new TextBlock { Text = "Журнал событий партии B-2403-001", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            DataGrid dg = CreateDataGrid();
            dg.Height = 400;
            dg.Columns.Add(new DataGridTextColumn { Header = "Время", Binding = new System.Windows.Data.Binding("TimeStr"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Событие", Binding = new System.Windows.Data.Binding("Event"), Width = new DataGridLength(180) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Пользователь", Binding = new System.Windows.Data.Binding("User"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Подробности", Binding = new System.Windows.Data.Binding("Details"), Width = new DataGridLength(400) });

            var events = new ObservableCollection<JournalEvent>();
            events.Add(new JournalEvent { Time = DateTime.Now.AddHours(-3), Event = "Запуск партии", User = "Заводов С.Н.", Details = "Партия B-2403-001 запущена" });
            events.Add(new JournalEvent { Time = DateTime.Now.AddHours(-2.9), Event = "Начало шага", User = "Заводов С.Н.", Details = "Смешивание компонентов" });
            events.Add(new JournalEvent { Time = DateTime.Now.AddHours(-2.5), Event = "Завершение шага", User = "Заводов С.Н.", Details = "Смешивание компонентов" });
            events.Add(new JournalEvent { Time = DateTime.Now.AddHours(-2.4), Event = "Начало шага", User = "Заводов С.Н.", Details = "Выдержка" });
            events.Add(new JournalEvent { Time = DateTime.Now.AddHours(-0.5), Event = "Завершение шага", User = "Заводов С.Н.", Details = "Выдержка" });
            events.Add(new JournalEvent { Time = DateTime.Now.AddMinutes(-15), Event = "Начало шага", User = "Заводов С.Н.", Details = "Экструзия" });
            events.Add(new JournalEvent { Time = DateTime.Now.AddMinutes(-10), Event = "Отклонение", User = "Система", Details = "Температура ниже нормы: 78.5°C (план: 80°C)" });

            dg.ItemsSource = events;
            panel.Children.Add(dg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        // ==================== СООБЩИТЬ О ПРОБЛЕМЕ ====================
        private void ShowProblem()
        {
            SetActiveButton(btnProblem, "Сообщить о проблеме");
            StackPanel panel = new StackPanel();
            panel.Margin = new Thickness(20);

            panel.Children.Add(new TextBlock { Text = "Сообщить о проблеме", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            Border card = new Border();
            card.Style = (Style)FindResource("CardStyle");
            card.Padding = new Thickness(20);
            card.Margin = new Thickness(0, 0, 0, 20);

            StackPanel stack = new StackPanel();
            stack.Children.Add(new TextBlock { Text = "Выберите тип проблемы:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 10) });

            RadioButton rb1 = new RadioButton { Content = "Отклонение параметра", Margin = new Thickness(0, 5, 0, 5), IsChecked = true };
            RadioButton rb2 = new RadioButton { Content = "Остановка оборудования", Margin = new Thickness(0, 5, 0, 5) };
            RadioButton rb3 = new RadioButton { Content = "Ошибка оператора", Margin = new Thickness(0, 5, 0, 5) };
            RadioButton rb4 = new RadioButton { Content = "Прочее", Margin = new Thickness(0, 5, 0, 5) };
            stack.Children.Add(rb1);
            stack.Children.Add(rb2);
            stack.Children.Add(rb3);
            stack.Children.Add(rb4);

            stack.Children.Add(new TextBlock { Text = "Описание проблемы:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) });
            TextBox descBox = new TextBox();
            descBox.Height = 80;
            descBox.TextWrapping = TextWrapping.Wrap;
            stack.Children.Add(descBox);

            stack.Children.Add(new TextBlock { Text = "Выберите партию:", FontWeight = FontWeights.Bold, Margin = new Thickness(0, 10, 0, 5) });
            ComboBox batchCombo = new ComboBox();
            batchCombo.Height = 30;
            foreach (var batch in _activeBatches)
                batchCombo.Items.Add($"{batch.BatchNumber} ({batch.ProductName})");
            batchCombo.SelectedIndex = 0;
            stack.Children.Add(batchCombo);

            card.Child = stack;
            panel.Children.Add(card);

            Button sendBtn = new Button();
            sendBtn.Content = "Отправить сообщение";
            sendBtn.Style = (Style)FindResource("PrimaryButton");
            sendBtn.Width = 180;
            sendBtn.Click += (s, e) =>
            {
                string problemType = "";
                foreach (var child in stack.Children)
                {
                    if (child is RadioButton rb && rb.IsChecked == true)
                        problemType = rb.Content.ToString();
                }

                MessageBox.Show($"Сообщение отправлено технологу!\n\nТип: {problemType}\nПартия: {batchCombo.SelectedItem}\nОписание: {descBox.Text}",
                    "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                ShowActiveBatches();
            };

            StackPanel btnPanel = new StackPanel();
            btnPanel.Orientation = Orientation.Horizontal;
            btnPanel.HorizontalAlignment = HorizontalAlignment.Center;
            btnPanel.Children.Add(sendBtn);
            panel.Children.Add(btnPanel);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void Logout()
        {
            if (MessageBox.Show("Вы уверены, что хотите выйти?", "Выход", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                LoginWindow login = new LoginWindow();
                login.Show();
                this.Close();
            }
        }
    }
}