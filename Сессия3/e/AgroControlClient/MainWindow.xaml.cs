using ClosedXML.Excel;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AgroControlClient
{
    // ==================== МОДЕЛИ ДАННЫХ ====================
    public class ProductItem
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Form { get; set; }
        public string Unit { get; set; }
        public string Status { get; set; }
    }

    public class RecipeItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ComponentItem
    {
        public int Id { get; set; }
        public int RecipeId { get; set; }
        public string Component { get; set; }
        public string Percent { get; set; }
        public string Order { get; set; }
        public string Critical { get; set; }
        public decimal ToleranceMin { get; set; }
        public decimal ToleranceMax { get; set; }
    }

    public class TechCardItem
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Version { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
    }

    public class StepItem
    {
        public int Id { get; set; }
        public int TechCardId { get; set; }
        public string Order { get; set; }
        public string Step { get; set; }
        public string Type { get; set; }
        public string Duration { get; set; }
        public string Instruction { get; set; }
        public int DurationMin { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int ProductId { get; set; }
        public string Product { get; set; }
        public int RecipeId { get; set; }
        public string RecipeName { get; set; }
        public int Quantity { get; set; }
        public string QuantityStr => $"{Quantity} кг";
        public int Priority { get; set; }
        public string Status { get; set; }
        public DateTime Date { get; set; }
        public string DateStr => Date.ToString("dd.MM.yyyy");
    }

    public class BatchItem
    {
        public int Id { get; set; }
        public string Number { get; set; }
        public int OrderId { get; set; }
        public string OrderNumber { get; set; }
        public int ProductId { get; set; }
        public string Product { get; set; }
        public string Status { get; set; }
        public int Quantity { get; set; }
        public string QuantityStr => $"{Quantity} кг";
        public string Lab { get; set; }
        public DateTime? StartTime { get; set; }
        public string StartTimeStr => StartTime?.ToString("dd.MM.yyyy HH:mm") ?? "";
    }

    public class DeviationItem
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string Batch { get; set; }
        public string Step { get; set; }
        public string Parameter { get; set; }
        public string Planned { get; set; }
        public string Actual { get; set; }
        public string Deviation { get; set; }
        public string Severity { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public bool IsResolved { get; set; }
    }

    public partial class MainWindow : Window
    {
        private Button _activeButton;

        // Коллекции данных
        private ObservableCollection<ProductItem> _products;
        private ObservableCollection<RecipeItem> _recipes;
        private ObservableCollection<ComponentItem> _components;
        private ObservableCollection<TechCardItem> _techCards;
        private ObservableCollection<StepItem> _steps;
        private ObservableCollection<OrderItem> _orders;
        private ObservableCollection<BatchItem> _batches;
        private ObservableCollection<DeviationItem> _deviations;

        // ID счетчики
        private int _nextProductId = 4;
        private int _nextRecipeId = 3;
        private int _nextComponentId = 4;
        private int _nextTechCardId = 3;
        private int _nextStepId = 5;
        private int _nextOrderId = 5;
        private int _nextBatchId = 4;
        private int _nextDeviationId = 3;

        public MainWindow()
        {
            InitializeComponent();
            InitializeData();

            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += (s, e) => { if (lblDateTime != null) lblDateTime.Text = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"); };
            timer.Start();

            btnDashboard.Click += (s, e) => ShowDashboard();
       
            btnRecipes.Click += (s, e) => ShowRecipes();
            btnTechCards.Click += (s, e) => ShowTechCards();
            btnOrders.Click += (s, e) => ShowOrders();
            btnBatches.Click += (s, e) => ShowBatches();
            btnExtruder.Click += (s, e) => ShowExtruder();
            btnMonitor.Click += (s, e) => ShowMonitor();
            btnDeviations.Click += (s, e) => ShowDeviations();
            btnReports.Click += (s, e) => ShowReports();
            btnLogout.Click += (s, e) => Logout();

            ShowDashboard();
        }

        private void InitializeData()

        {
            // Продукты
            _products = new ObservableCollection<ProductItem>
            {
                new ProductItem { Id = 1, Code = "HERB-A", Name = "Гербицид А", Type = "Гербицид", Form = "Жидкость", Unit = "л", Status = "Активен" },
                new ProductItem { Id = 2, Code = "INS-B", Name = "Инсектицид Б", Type = "Инсектицид", Form = "Жидкость", Unit = "л", Status = "Активен" },
                new ProductItem { Id = 3, Code = "FUN-C", Name = "Фунгицид В", Type = "Фунгицид", Form = "Гранулы", Unit = "кг", Status = "Черновик" }
            };

            // Рецептуры
            _recipes = new ObservableCollection<RecipeItem>
            {
                new RecipeItem { Id = 1, ProductId = 1, ProductName = "Гербицид А", Version = "2", Status = "Активна", Description = "Улучшенная формула", CreatedAt = DateTime.Now.AddDays(-30) },
                new RecipeItem { Id = 2, ProductId = 2, ProductName = "Инсектицид Б", Version = "2", Status = "Активна", Description = "Новая формула", CreatedAt = DateTime.Now.AddDays(-20) }
            };

            // Компоненты
            _components = new ObservableCollection<ComponentItem>
            {
                new ComponentItem { Id = 1, RecipeId = 1, Component = "Активное вещество А", Percent = "48", Order = "1", Critical = "Да", ToleranceMin = 46, ToleranceMax = 50 },
                new ComponentItem { Id = 2, RecipeId = 1, Component = "Растворитель Б", Percent = "28", Order = "2", Critical = "Нет", ToleranceMin = 26, ToleranceMax = 30 },
                new ComponentItem { Id = 3, RecipeId = 1, Component = "Эмульгатор В", Percent = "24", Order = "3", Critical = "Нет", ToleranceMin = 22, ToleranceMax = 26 }
            };

            // Технологические карты
            _techCards = new ObservableCollection<TechCardItem>
            {
                new TechCardItem { Id = 1, ProductId = 1, ProductName = "Гербицид А", Version = "1", Status = "Активна", IsActive = true },
                new TechCardItem { Id = 2, ProductId = 2, ProductName = "Инсектицид Б", Version = "1", Status = "Активна", IsActive = true }
            };

            // Шаги
            _steps = new ObservableCollection<StepItem>
            {
                new StepItem { Id = 1, TechCardId = 1, Order = "1", Step = "Смешивание компонентов", Type = "mixing", Duration = "30 мин", Instruction = "Загрузить компоненты по порядку", DurationMin = 30 },
                new StepItem { Id = 2, TechCardId = 1, Order = "2", Step = "Выдержка", Type = "holding", Duration = "120 мин", Instruction = "Выдержать при 60°C", DurationMin = 120 },
                new StepItem { Id = 3, TechCardId = 1, Order = "3", Step = "Экструзия", Type = "extrusion", Duration = "45 мин", Instruction = "Температура 80°C, давление 3.0 bar", DurationMin = 45 },
                new StepItem { Id = 4, TechCardId = 1, Order = "4", Step = "Охлаждение", Type = "cooling", Duration = "60 мин", Instruction = "Охладить до 25°C", DurationMin = 60 }
            };

            // Заказы
            _orders = new ObservableCollection<OrderItem>
            {
                new OrderItem { Id = 1, Number = "PO-2401", ProductId = 1, Product = "Гербицид А", RecipeId = 1, RecipeName = "HERB-A v2", Quantity = 1000, Priority = 1, Status = "Выполнен", Date = new DateTime(2025, 3, 1) },
                new OrderItem { Id = 2, Number = "PO-2402", ProductId = 2, Product = "Инсектицид Б", RecipeId = 2, RecipeName = "INS-B v2", Quantity = 500, Priority = 2, Status = "В работе", Date = new DateTime(2025, 3, 3) },
                new OrderItem { Id = 3, Number = "PO-2403", ProductId = 1, Product = "Гербицид А", RecipeId = 1, RecipeName = "HERB-A v2", Quantity = 800, Priority = 1, Status = "В работе", Date = new DateTime(2025, 3, 4) },
                new OrderItem { Id = 4, Number = "PO-2404", ProductId = 2, Product = "Инсектицид Б", RecipeId = 2, RecipeName = "INS-B v2", Quantity = 300, Priority = 3, Status = "План", Date = new DateTime(2025, 3, 10) }
            };

            // Партии
            _batches = new ObservableCollection<BatchItem>
            {
                new BatchItem { Id = 1, Number = "B-2401-01", OrderId = 1, OrderNumber = "PO-2401", ProductId = 1, Product = "Гербицид А", Status = "Завершена", Quantity = 998, Lab = "Одобрена", StartTime = new DateTime(2025, 3, 1, 8, 0, 0) },
                new BatchItem { Id = 2, Number = "B-2402-01", OrderId = 2, OrderNumber = "PO-2402", ProductId = 2, Product = "Инсектицид Б", Status = "В работе", Quantity = 250, Lab = "Ожидает", StartTime = new DateTime(2025, 3, 3, 9, 0, 0) },
                new BatchItem { Id = 3, Number = "B-2403-01", OrderId = 3, OrderNumber = "PO-2403", ProductId = 1, Product = "Гербицид А", Status = "В работе", Quantity = 400, Lab = "Ожидает", StartTime = new DateTime(2025, 3, 4, 10, 0, 0) }
            };

            // Отклонения
            _deviations = new ObservableCollection<DeviationItem>
            {
                new DeviationItem { Id = 1, BatchId = 1, Batch = "B-2401-01", Step = "Экструзия", Parameter = "Температура", Planned = "80°C", Actual = "78.2°C", Deviation = "-1.8°C", Severity = "warning", Description = "Температура ниже нормы", Date = new DateTime(2025, 3, 1, 10, 35, 0), IsResolved = true },
                new DeviationItem { Id = 2, BatchId = 2, Batch = "B-2402-01", Step = "Экструзия", Parameter = "Давление", Planned = "3.5 bar", Actual = "2.9 bar", Deviation = "-0.6 bar", Severity = "critical", Description = "Давление ниже нормы", Date = new DateTime(2025, 3, 3, 9, 40, 0), IsResolved = false }
            };
            
        }

        private void SetActiveButton(Button button, string title)
        {
            if (_activeButton != null) _activeButton.Background = Brushes.Transparent;
            _activeButton = button;
            if (button != null) button.Background = new SolidColorBrush(Color.FromRgb(61, 86, 110));
            if (lblTitle != null) lblTitle.Text = title;
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
        // ==================== ГЛАВНАЯ ====================
        private void ShowDashboard()
        {
            SetActiveButton(btnDashboard, "Главная");
            StackPanel mainPanel = new StackPanel { Margin = new Thickness(20) };

            // ========== ВЕРХНЯЯ ПАНЕЛЬ С ЛОГОТИПОМ ==========
            DockPanel topPanel = new DockPanel { Margin = new Thickness(0, 0, 0, 20) };

            // Логотип слева
            try
            {
                Uri imageUri = new Uri("pack://application:,,,/i/15.png", UriKind.Absolute);
                BitmapImage bitmap = new BitmapImage(imageUri);

                Image logoImage = new Image();
                logoImage.Source = bitmap;
                logoImage.Width = 60;
                logoImage.Height = 60;
                logoImage.Margin = new Thickness(0, 0, 15, 0);
                DockPanel.SetDock(logoImage, Dock.Left);

                topPanel.Children.Add(logoImage);
            }
            catch
            {
                TextBlock iconBlock = new TextBlock { Text = "🏭", FontSize = 45, Margin = new Thickness(0, 0, 15, 0) };
                DockPanel.SetDock(iconBlock, Dock.Left);
                topPanel.Children.Add(iconBlock);
            }

            // Текст справа
            StackPanel titlePanel = new StackPanel();
            titlePanel.Children.Add(new TextBlock
            {
                Text = "АГРОКОНТРОЛЬ",
                FontSize = 24,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(46, 125, 50))
            });
            titlePanel.Children.Add(new TextBlock
            {
                Text = "Модуль технолога",
                FontSize = 12,
                Foreground = Brushes.Gray
            });

            topPanel.Children.Add(titlePanel);
            mainPanel.Children.Add(topPanel);

            // ========== ОСТАЛЬНОЙ КОНТЕНТ ==========

            // Разделитель
            mainPanel.Children.Add(new Separator { Background = Brushes.LightGray, Margin = new Thickness(0, 0, 0, 15), Height = 1 });

            // Дата и время
            mainPanel.Children.Add(new TextBlock
            {
                Text = DateTime.Now.ToString("dddd, dd MMMM yyyy HH:mm:ss"),
                FontSize = 12,
                Foreground = Brushes.Gray,
                Margin = new Thickness(0, 0, 0, 20)
            });

            // Приветствие
            mainPanel.Children.Add(new TextBlock
            {
                Text = $"Добро пожаловать, {txtUserName.Text}!",
                FontSize = 18,
                FontWeight = FontWeights.SemiBold,
                Margin = new Thickness(0, 0, 0, 20)
            });

            // Карточки KPI
            Grid grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            grid.Children.Add(CreateKpiCard("Продуктов", _products.Count.ToString(), "📦", 0));
            grid.Children.Add(CreateKpiCard("Рецептур", _recipes.Count(r => r.Status == "Активна").ToString(), "📋", 1));
            grid.Children.Add(CreateKpiCard("Партий", _batches.Count(b => b.Status == "В работе").ToString(), "🏭", 2));
            grid.Children.Add(CreateKpiCard("Отклонений", _deviations.Count(d => !d.IsResolved).ToString(), "⚠️", 3));
            mainPanel.Children.Add(grid);

            // Таблица последних партий
            mainPanel.Children.Add(new TextBlock
            {
                Text = "Последние партии",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 20, 0, 10)
            });

            var recentDg = CreateDataGrid();
            recentDg.Height = 200;
            recentDg.Columns.Add(new DataGridTextColumn { Header = "Номер партии", Binding = new System.Windows.Data.Binding("BatchNumber"), Width = new DataGridLength(120) });
            recentDg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("ProductName"), Width = new DataGridLength(150) });
            recentDg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            recentDg.Columns.Add(new DataGridTextColumn { Header = "Дата запуска", Binding = new System.Windows.Data.Binding("StartTime"), Width = new DataGridLength(150) });
            recentDg.ItemsSource = _batches;
            mainPanel.Children.Add(recentDg);

            ContentContainer.Content = new ScrollViewer { Content = mainPanel };
        }

        // ==================== РЕЦЕПТУРЫ ====================
        private void ShowRecipes()
        {
            SetActiveButton(btnRecipes, "Рецептуры");
            StackPanel panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "📋 Управление рецептурами", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            StackPanel actionBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            Button addBtn = CreateButton("➕ Создать рецептуру", "#2E7D32", 0, 35, 170);
            Button approveBtn = CreateButton("✅ Утвердить", "#FF9800", 10, 35, 140);
            Button archiveBtn = CreateButton("📦 Архивировать", "#607D8B", 10, 35, 150);

            addBtn.Click += (s, e) => AddRecipe();
            approveBtn.Click += (s, e) => ApproveRecipe();
            archiveBtn.Click += (s, e) => ArchiveRecipe();

            actionBar.Children.Add(addBtn);
            actionBar.Children.Add(approveBtn);
            actionBar.Children.Add(archiveBtn);
            panel.Children.Add(actionBar);

            DataGrid dg = CreateDataGrid();
            dg.Height = 200;
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("ProductName"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Версия", Binding = new System.Windows.Data.Binding("Version"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Описание", Binding = new System.Windows.Data.Binding("Description"), Width = new DataGridLength(250) });
            dg.ItemsSource = _recipes;
            panel.Children.Add(dg);

            // Компоненты
            panel.Children.Add(new TextBlock { Text = "📝 Компоненты рецептуры", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 20, 0, 10) });

            StackPanel compActionBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
            Button addCompBtn = CreateButton("➕ Добавить компонент", "#2E7D32", 0, 30, 170);
            Button editCompBtn = CreateButton("✏️ Редактировать", "#2196F3", 10, 30, 140);
            Button deleteCompBtn = CreateButton("🗑️ Удалить", "#F44336", 10, 30, 120);

            addCompBtn.Click += (s, e) => AddComponent();
            editCompBtn.Click += (s, e) => EditComponent();
            deleteCompBtn.Click += (s, e) => DeleteComponent();

            compActionBar.Children.Add(addCompBtn);
            compActionBar.Children.Add(editCompBtn);
            compActionBar.Children.Add(deleteCompBtn);
            panel.Children.Add(compActionBar);

            DataGrid compDg = CreateDataGrid();
            compDg.Height = 150;
            compDg.Columns.Add(new DataGridTextColumn { Header = "Компонент", Binding = new System.Windows.Data.Binding("Component"), Width = new DataGridLength(200) });
            compDg.Columns.Add(new DataGridTextColumn { Header = "Доля, %", Binding = new System.Windows.Data.Binding("Percent"), Width = new DataGridLength(100) });
            compDg.Columns.Add(new DataGridTextColumn { Header = "Порядок", Binding = new System.Windows.Data.Binding("Order"), Width = new DataGridLength(100) });
            compDg.Columns.Add(new DataGridTextColumn { Header = "Критичный", Binding = new System.Windows.Data.Binding("Critical"), Width = new DataGridLength(80) });
            compDg.ItemsSource = _components;
            panel.Children.Add(compDg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void AddRecipe()
        {
            var products = _products.Where(p => p.Status == "Активен").ToList();
            if (products.Count == 0) return;

            var newRecipe = new RecipeItem
            {
                Id = _nextRecipeId++,
                ProductId = products.First().Id,
                ProductName = products.First().Name,
                Version = "1",
                Status = "Черновик",
                Description = "Новая рецептура",
                CreatedAt = DateTime.Now
            };
            _recipes.Add(newRecipe);
            ShowRecipes();
        }

        private void ApproveRecipe()
        {
            if (_recipes.Count > 0)
            {
                var lastRecipe = _recipes.Last();
                lastRecipe.Status = "Активна";
                ShowRecipes();
            }
        }

        private void ArchiveRecipe()
        {
            if (_recipes.Count > 0)
            {
                var lastRecipe = _recipes.Last();
                lastRecipe.Status = "Архив";
                ShowRecipes();
            }
        }

        private void AddComponent()
        {
            if (_recipes.Count == 0) return;

            var newComponent = new ComponentItem
            {
                Id = _nextComponentId++,
                RecipeId = _recipes.First().Id,
                Component = "Новый компонент",
                Percent = "0",
                Order = (_components.Count + 1).ToString(),
                Critical = "Нет"
            };
            _components.Add(newComponent);
            ShowRecipes();
        }

        private void EditComponent() => MessageBox.Show("Выберите компонент из таблицы для редактирования", "Редактирование", MessageBoxButton.OK, MessageBoxImage.Information);

        private void DeleteComponent()
        {
            if (_components.Count > 0)
            {
                _components.RemoveAt(_components.Count - 1);
                ShowRecipes();
            }
        }

        // ==================== ТЕХНОЛОГИЧЕСКИЕ КАРТЫ ====================
        private void ShowTechCards()
        {
            SetActiveButton(btnTechCards, "Технологические карты");
            StackPanel panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "🗺️ Технологические карты", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            StackPanel actionBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            Button addBtn = CreateButton("➕ Создать карту", "#2E7D32", 0, 35, 160);
            Button activateBtn = CreateButton("✅ Активировать", "#FF9800", 10, 35, 150);

            addBtn.Click += (s, e) => AddTechCard();
            activateBtn.Click += (s, e) => ActivateTechCard();

            actionBar.Children.Add(addBtn);
            actionBar.Children.Add(activateBtn);
            panel.Children.Add(actionBar);

            DataGrid dg = CreateDataGrid();
            dg.Height = 200;
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("ProductName"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Версия", Binding = new System.Windows.Data.Binding("Version"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            dg.ItemsSource = _techCards;
            panel.Children.Add(dg);

            panel.Children.Add(new TextBlock { Text = "📋 Шаги технологического процесса", FontSize = 16, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 20, 0, 10) });

            StackPanel stepActionBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 10) };
            Button addStepBtn = CreateButton("➕ Добавить шаг", "#2E7D32", 0, 30, 150);
            Button deleteStepBtn = CreateButton("🗑️ Удалить", "#F44336", 10, 30, 120);

            addStepBtn.Click += (s, e) => AddStep();
            deleteStepBtn.Click += (s, e) => DeleteStep();

            stepActionBar.Children.Add(addStepBtn);
            stepActionBar.Children.Add(deleteStepBtn);
            panel.Children.Add(stepActionBar);

            DataGrid stepsDg = CreateDataGrid();
            stepsDg.Height = 200;
            stepsDg.Columns.Add(new DataGridTextColumn { Header = "№", Binding = new System.Windows.Data.Binding("Order"), Width = new DataGridLength(50) });
            stepsDg.Columns.Add(new DataGridTextColumn { Header = "Шаг", Binding = new System.Windows.Data.Binding("Step"), Width = new DataGridLength(180) });
            stepsDg.Columns.Add(new DataGridTextColumn { Header = "Тип", Binding = new System.Windows.Data.Binding("Type"), Width = new DataGridLength(100) });
            stepsDg.Columns.Add(new DataGridTextColumn { Header = "Длительность", Binding = new System.Windows.Data.Binding("Duration"), Width = new DataGridLength(100) });
            stepsDg.ItemsSource = _steps;
            panel.Children.Add(stepsDg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void AddTechCard()
        {
            var products = _products.Where(p => p.Status == "Активен" && !_techCards.Any(t => t.ProductId == p.Id)).ToList();
            if (products.Count == 0) return;

            var newCard = new TechCardItem
            {
                Id = _nextTechCardId++,
                ProductId = products.First().Id,
                ProductName = products.First().Name,
                Version = "1",
                Status = "Черновик",
                IsActive = false
            };
            _techCards.Add(newCard);
            ShowTechCards();
        }

        private void ActivateTechCard()
        {
            if (_techCards.Count > 0)
            {
                foreach (var card in _techCards) card.IsActive = false;
                var lastCard = _techCards.Last();
                lastCard.IsActive = true;
                lastCard.Status = "Активна";
                ShowTechCards();
            }
        }

        private void AddStep()
        {
            if (_techCards.Count == 0) return;

            var newStep = new StepItem
            {
                Id = _nextStepId++,
                TechCardId = _techCards.First().Id,
                Order = (_steps.Count + 1).ToString(),
                Step = "Новый шаг",
                Type = "general",
                Duration = "0 мин",
                Instruction = "Описание шага",
                DurationMin = 0
            };
            _steps.Add(newStep);
            ShowTechCards();
        }

        private void DeleteStep()
        {
            if (_steps.Count > 0)
            {
                _steps.RemoveAt(_steps.Count - 1);
                ShowTechCards();
            }
        }

        // ==================== ЗАКАЗЫ ====================
        private void ShowOrders()
        {
            SetActiveButton(btnOrders, "Заказы");
            StackPanel panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "📄 Производственные заказы", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            StackPanel actionBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            Button addBtn = CreateButton("➕ Создать заказ", "#2E7D32", 0, 35, 160);
            Button startBtn = CreateButton("▶ Выполнить", "#4CAF50", 10, 35, 140);
            Button cancelBtn = CreateButton("✖ Отменить", "#F44336", 10, 35, 140);

            addBtn.Click += (s, e) => AddOrder();
            startBtn.Click += (s, e) => StartOrder();
            cancelBtn.Click += (s, e) => CancelOrder();

            actionBar.Children.Add(addBtn);
            actionBar.Children.Add(startBtn);
            actionBar.Children.Add(cancelBtn);
            panel.Children.Add(actionBar);

            DataGrid dg = CreateDataGrid();
            dg.Height = 350;
            dg.Columns.Add(new DataGridTextColumn { Header = "Номер заказа", Binding = new System.Windows.Data.Binding("Number"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("Product"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new System.Windows.Data.Binding("QuantityStr"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Приоритет", Binding = new System.Windows.Data.Binding("Priority"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Дата", Binding = new System.Windows.Data.Binding("DateStr"), Width = new DataGridLength(120) });
            dg.ItemsSource = _orders;
            panel.Children.Add(dg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void AddOrder()
        {
            var products = _products.Where(p => p.Status == "Активен").ToList();
            if (products.Count == 0) return;

            var newOrder = new OrderItem
            {
                Id = _nextOrderId++,
                Number = $"PO-{_nextOrderId:D3}",
                ProductId = products.First().Id,
                Product = products.First().Name,
                RecipeId = 1,
                RecipeName = "Стандартная",
                Quantity = 500,
                Priority = 2,
                Status = "План",
                Date = DateTime.Now
            };
            _orders.Add(newOrder);
            ShowOrders();
        }

        private void StartOrder()
        {
            var order = _orders.LastOrDefault();
            if (order != null && order.Status == "План")
            {
                order.Status = "В работе";
                ShowOrders();
            }
        }

        private void CancelOrder()
        {
            var order = _orders.LastOrDefault();
            if (order != null && order.Status != "Выполнен")
            {
                order.Status = "Отменен";
                ShowOrders();
            }
        }

        // ==================== ПАРТИИ ====================
        private void ShowBatches()
        {
            SetActiveButton(btnBatches, "Партии");
            StackPanel panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "🏭 Производственные партии", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            StackPanel actionBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            Button addBtn = CreateButton("➕ Сформировать партию", "#2E7D32", 0, 35, 180);
            Button startBtn = CreateButton("▶ Запустить", "#4CAF50", 10, 35, 130);
            Button completeBtn = CreateButton("✅ Завершить", "#FF9800", 10, 35, 130);

            addBtn.Click += (s, e) => AddBatch();
            startBtn.Click += (s, e) => StartBatch();
            completeBtn.Click += (s, e) => CompleteBatch();

            actionBar.Children.Add(addBtn);
            actionBar.Children.Add(startBtn);
            actionBar.Children.Add(completeBtn);
            panel.Children.Add(actionBar);

            DataGrid dg = CreateDataGrid();
            dg.Height = 350;
            dg.Columns.Add(new DataGridTextColumn { Header = "Номер партии", Binding = new System.Windows.Data.Binding("Number"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Продукт", Binding = new System.Windows.Data.Binding("Product"), Width = new DataGridLength(150) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("Status"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Количество", Binding = new System.Windows.Data.Binding("QuantityStr"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Лаборатория", Binding = new System.Windows.Data.Binding("Lab"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Дата запуска", Binding = new System.Windows.Data.Binding("StartTimeStr"), Width = new DataGridLength(150) });
            dg.ItemsSource = _batches;
            panel.Children.Add(dg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void AddBatch()
        {
            var orders = _orders.Where(o => o.Status == "План" || o.Status == "В работе").ToList();
            if (orders.Count == 0) return;

            var order = orders.First();
            var newBatch = new BatchItem
            {
                Id = _nextBatchId++,
                Number = $"B-{_nextBatchId:D3}",
                OrderId = order.Id,
                OrderNumber = order.Number,
                ProductId = order.ProductId,
                Product = order.Product,
                Status = "План",
                Quantity = 0,
                Lab = "Не назначен",
                StartTime = null
            };
            _batches.Add(newBatch);
            ShowBatches();
        }

        private void StartBatch()
        {
            var batch = _batches.LastOrDefault(b => b.Status == "План");
            if (batch != null)
            {
                batch.Status = "В работе";
                batch.StartTime = DateTime.Now;
                ShowBatches();
            }
        }

        private void CompleteBatch()
        {
            var batch = _batches.LastOrDefault(b => b.Status == "В работе");
            if (batch != null)
            {
                batch.Status = "Завершена";
                batch.Lab = "Ожидает проверки";
                ShowBatches();
            }
        }

        // ==================== ЭКСТРУДЕР ====================
        private void ShowExtruder()
        {
            SetActiveButton(btnExtruder, "Экструдер");
            StackPanel panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "⚙️ Настройка экструдера", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            StackPanel actionBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            Button saveBtn = CreateButton("💾 Сохранить настройки", "#2E7D32", 0, 35, 160);
            Button loadBtn = CreateButton("📂 Загрузить настройки", "#2196F3", 10, 35, 160);
            Button resetBtn = CreateButton("🔄 Сбросить", "#FF9800", 10, 35, 120);

            saveBtn.Click += (s, e) => SaveExtruderSettings();
            loadBtn.Click += (s, e) => LoadExtruderSettings();
            resetBtn.Click += (s, e) => ResetExtruderSettings();

            actionBar.Children.Add(saveBtn);
            actionBar.Children.Add(loadBtn);
            actionBar.Children.Add(resetBtn);
            panel.Children.Add(actionBar);

            Grid grid = new Grid();
            for (int i = 0; i < 7; i++) grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });

            AddParamRow(grid, 0, "Температура зоны 1:", "85", "°C");
            AddParamRow(grid, 1, "Температура зоны 2:", "90", "°C");
            AddParamRow(grid, 2, "Температура зоны 3:", "95", "°C");
            AddParamRow(grid, 3, "Температура зоны 4:", "100", "°C");
            AddParamRow(grid, 4, "Температура зоны 5:", "95", "°C");
            AddParamRow(grid, 5, "Давление:", "3.5", "bar");
            AddParamRow(grid, 6, "Скорость шнека:", "150", "rpm");

            panel.Children.Add(grid);
            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void AddParamRow(Grid grid, int row, string label, string defaultValue, string unit)
        {
            grid.Children.Add(new TextBlock { Text = label, FontSize = 14, VerticalAlignment = VerticalAlignment.Center });
            Grid.SetRow(grid.Children[grid.Children.Count - 1], row);
            Grid.SetColumn(grid.Children[grid.Children.Count - 1], 0);

            TextBox textBox = new TextBox { Text = defaultValue, Width = 100, Height = 35, Margin = new Thickness(5) };
            grid.Children.Add(textBox);
            Grid.SetRow(textBox, row);
            Grid.SetColumn(textBox, 1);

            grid.Children.Add(new TextBlock { Text = unit, FontSize = 14, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(5, 0, 0, 0) });
            Grid.SetRow(grid.Children[grid.Children.Count - 1], row);
            Grid.SetColumn(grid.Children[grid.Children.Count - 1], 2);
        }

        private void SaveExtruderSettings() => MessageBox.Show("Настройки экструдера сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        private void LoadExtruderSettings() => MessageBox.Show("Настройки экструдера загружены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        private void ResetExtruderSettings() => MessageBox.Show("Настройки экструдера сброшены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

        // ==================== МОНИТОРИНГ ====================
        private void ShowMonitor()
        {
            SetActiveButton(btnMonitor, "Мониторинг");
            StackPanel panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "📊 Мониторинг выполнения партий", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            StackPanel actionBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            Button refreshBtn = CreateButton("🔄 Обновить", "#2196F3", 0, 35, 120);
            Button exportBtn = CreateButton("📊 Экспорт", "#2E7D32", 10, 35, 120);

            refreshBtn.Click += (s, e) => ShowMonitor();
            exportBtn.Click += (s, e) => ExportMonitorData();

            actionBar.Children.Add(refreshBtn);
            actionBar.Children.Add(exportBtn);
            panel.Children.Add(actionBar);

            var activeBatch = _batches.FirstOrDefault(b => b.Status == "В работе");

            Border card = new Border { Background = Brushes.White, BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Padding = new Thickness(20), Margin = new Thickness(0, 0, 0, 20) };
            StackPanel cardStack = new StackPanel();
            cardStack.Children.Add(new TextBlock { Text = "🟢 АКТИВНАЯ ПАРТИЯ", FontSize = 16, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(76, 175, 80)) });
            cardStack.Children.Add(new TextBlock { Text = activeBatch != null ? $"{activeBatch.Number} | {activeBatch.Product}" : "Нет активных партий", FontSize = 14, Margin = new Thickness(0, 5, 0, 0) });

            var currentStep = _steps.FirstOrDefault(s => s.TechCardId == 1);
            if (currentStep != null)
            {
                cardStack.Children.Add(new TextBlock { Text = $"Текущий шаг: {currentStep.Step}", FontSize = 14, Margin = new Thickness(0, 5, 0, 0) });
            }

            ProgressBar progressBar = new ProgressBar { Minimum = 0, Maximum = 100, Value = 45, Height = 20, Margin = new Thickness(0, 15, 0, 0) };
            cardStack.Children.Add(progressBar);
            cardStack.Children.Add(new TextBlock { Text = "Прогресс выполнения: 45%", FontSize = 12, Foreground = Brushes.Gray, Margin = new Thickness(0, 5, 0, 0) });

            card.Child = cardStack;
            panel.Children.Add(card);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void ExportMonitorData()
        {
            string content = "МОНИТОРИНГ ПРОИЗВОДСТВА\n" + new string('=', 50) + "\n\n";
            content += $"Дата: {DateTime.Now:dd.MM.yyyy HH:mm:ss}\n\n";
            content += "Активные партии:\n";
            foreach (var batch in _batches.Where(b => b.Status == "В работе"))
            {
                content += $"- {batch.Number}: {batch.Product}, статус: {batch.Status}\n";
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FileName = $"Мониторинг_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            if (dialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(dialog.FileName, content, System.Text.Encoding.UTF8);
                MessageBox.Show($"Данные экспортированы: {dialog.FileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ==================== ОТКЛОНЕНИЯ ====================
        private void ShowDeviations()
        {
            SetActiveButton(btnDeviations, "Отклонения");
            StackPanel panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "⚠️ Анализ отклонений", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            StackPanel filterBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 15) };
            Button allBtn = CreateButton("Все", "#2E7D32", 0, 30, 80);
            Button criticalBtn = CreateButton("Критические", "#F44336", 10, 30, 120);
            Button resolveBtn = CreateButton("✅ Устранить", "#4CAF50", 10, 30, 120);
            Button exportBtn = CreateButton("📊 Экспорт", "#607D8B", 10, 30, 100);

            allBtn.Click += (s, e) => ShowAllDeviations();
            criticalBtn.Click += (s, e) => ShowCriticalDeviations();
            resolveBtn.Click += (s, e) => ResolveDeviation();
            exportBtn.Click += (s, e) => ExportDeviationsReport();

            filterBar.Children.Add(allBtn);
            filterBar.Children.Add(criticalBtn);
            filterBar.Children.Add(resolveBtn);
            filterBar.Children.Add(exportBtn);
            panel.Children.Add(filterBar);

            DataGrid dg = CreateDataGrid();
            dg.Height = 350;
            dg.Columns.Add(new DataGridTextColumn { Header = "Партия", Binding = new System.Windows.Data.Binding("Batch"), Width = new DataGridLength(100) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Параметр", Binding = new System.Windows.Data.Binding("Parameter"), Width = new DataGridLength(120) });
            dg.Columns.Add(new DataGridTextColumn { Header = "План", Binding = new System.Windows.Data.Binding("Planned"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Факт", Binding = new System.Windows.Data.Binding("Actual"), Width = new DataGridLength(80) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Описание", Binding = new System.Windows.Data.Binding("Description"), Width = new DataGridLength(250) });
            dg.Columns.Add(new DataGridTextColumn { Header = "Статус", Binding = new System.Windows.Data.Binding("IsResolved"), Width = new DataGridLength(80) });
            dg.ItemsSource = _deviations;
            panel.Children.Add(dg);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void ShowAllDeviations()
        {
            DataGrid dg = (DataGrid)((StackPanel)((ScrollViewer)ContentContainer.Content).Content).Children[2];
            dg.ItemsSource = _deviations;
        }

        private void ShowCriticalDeviations()
        {
            DataGrid dg = (DataGrid)((StackPanel)((ScrollViewer)ContentContainer.Content).Content).Children[2];
            dg.ItemsSource = _deviations.Where(d => d.Severity == "critical").ToList();
        }

        private void ResolveDeviation()
        {
            var unresolved = _deviations.FirstOrDefault(d => !d.IsResolved);
            if (unresolved != null)
            {
                unresolved.IsResolved = true;
                ShowDeviations();
            }
        }

        private void ExportDeviationsReport()
        {
            string content = "ОТЧЕТ ПО ОТКЛОНЕНИЯМ\n" + new string('=', 50) + "\n\n";
            content += "| Партия | Параметр | План | Факт | Описание | Статус |\n";
            foreach (var d in _deviations)
            {
                content += $"| {d.Batch} | {d.Parameter} | {d.Planned} | {d.Actual} | {d.Description} | {(d.IsResolved ? "Устранено" : "Активно")} |\n";
            }

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            dialog.FileName = $"Отчет_по_отклонениям_{DateTime.Now:yyyyMMdd_HHmmss}.txt";

            if (dialog.ShowDialog() == true)
            {
                System.IO.File.WriteAllText(dialog.FileName, content, System.Text.Encoding.UTF8);
                MessageBox.Show($"Отчет сохранен: {dialog.FileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        // ==================== ОТЧЕТЫ С ЭКСПОРТОМ В EXCEL ====================
        private void ShowReports()
        {
            SetActiveButton(btnReports, "Отчеты");
            StackPanel panel = new StackPanel { Margin = new Thickness(20) };
            panel.Children.Add(new TextBlock { Text = "📈 Формирование отчетов", FontSize = 22, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 0, 0, 20) });

            StackPanel reportBar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 20), HorizontalAlignment = HorizontalAlignment.Center };
            Button batchesBtn = CreateButton("📊 Отчет по партиям (Excel)", "#2E7D32", 0, 40, 200);
            Button deviationsBtn = CreateButton("⚠️ Отчет по отклонениям (Excel)", "#F44336", 10, 40, 200);
            Button recipesBtn = CreateButton("📋 Отчет по рецептурам (Excel)", "#2196F3", 10, 40, 200);
            Button summaryBtn = CreateButton("📈 Сводный отчет (Excel)", "#FF9800", 10, 40, 180);

            batchesBtn.Click += (s, e) => ExportToExcel("batches");
            deviationsBtn.Click += (s, e) => ExportToExcel("deviations");
            recipesBtn.Click += (s, e) => ExportToExcel("recipes");
            summaryBtn.Click += (s, e) => ExportToExcel("summary");

            reportBar.Children.Add(batchesBtn);
            reportBar.Children.Add(deviationsBtn);
            reportBar.Children.Add(recipesBtn);
            reportBar.Children.Add(summaryBtn);
            panel.Children.Add(reportBar);

            ContentContainer.Content = new ScrollViewer { Content = panel };
        }

        private void ExportToExcel(string reportType)
        {
            try
            {
                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Отчет");

                    switch (reportType)
                    {
                        case "batches":
                            ExportBatchesToExcel(worksheet);
                            break;
                        case "deviations":
                            ExportDeviationsToExcel(worksheet);
                            break;
                        case "recipes":
                            ExportRecipesToExcel(worksheet);
                            break;
                        case "summary":
                            ExportSummaryToExcel(worksheet);
                            break;
                    }

                    worksheet.Columns().AdjustToContents();

                    var dialog = new SaveFileDialog
                    {
                        Filter = "Excel файлы (*.xlsx)|*.xlsx",
                        FileName = $"{reportType}_report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
                    };

                    if (dialog.ShowDialog() == true)
                    {
                        workbook.SaveAs(dialog.FileName);
                        MessageBox.Show($"Отчет успешно сохранен!\n{dialog.FileName}", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка экспорта: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportBatchesToExcel(IXLWorksheet ws)
        {
            ws.Cell(1, 1).Value = "ОТЧЕТ ПО ПРОИЗВОДСТВЕННЫМ ПАРТИЯМ";
            ws.Range("A1:F1").Merge().Style.Font.Bold = true;

            string[] headers = { "№", "Номер партии", "Продукт", "Статус", "Количество, кг", "Лаборатория" };
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(3, i + 1).Value = headers[i];

            int row = 4, num = 1;
            foreach (var batch in _batches)
            {
                ws.Cell(row, 1).Value = num++;
                ws.Cell(row, 2).Value = batch.Number;
                ws.Cell(row, 3).Value = batch.Product;
                ws.Cell(row, 4).Value = batch.Status;
                ws.Cell(row, 5).Value = batch.Quantity;
                ws.Cell(row, 6).Value = batch.Lab;
                row++;
            }
        }

        private void ExportDeviationsToExcel(IXLWorksheet ws)
        {
            ws.Cell(1, 1).Value = "ОТЧЕТ ПО ОТКЛОНЕНИЯМ";
            ws.Range("A1:F1").Merge().Style.Font.Bold = true;

            string[] headers = { "№", "Партия", "Параметр", "План", "Факт", "Описание" };
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(3, i + 1).Value = headers[i];

            int row = 4, num = 1;
            foreach (var dev in _deviations)
            {
                ws.Cell(row, 1).Value = num++;
                ws.Cell(row, 2).Value = dev.Batch;
                ws.Cell(row, 3).Value = dev.Parameter;
                ws.Cell(row, 4).Value = dev.Planned;
                ws.Cell(row, 5).Value = dev.Actual;
                ws.Cell(row, 6).Value = dev.Description;
                row++;
            }
        }

        private void ExportRecipesToExcel(IXLWorksheet ws)
        {
            ws.Cell(1, 1).Value = "ОТЧЕТ ПО РЕЦЕПТУРАМ";
            ws.Range("A1:E1").Merge().Style.Font.Bold = true;

            string[] headers = { "№", "Продукт", "Версия", "Статус", "Описание" };
            for (int i = 0; i < headers.Length; i++)
                ws.Cell(3, i + 1).Value = headers[i];

            int row = 4, num = 1;
            foreach (var recipe in _recipes)
            {
                ws.Cell(row, 1).Value = num++;
                ws.Cell(row, 2).Value = recipe.ProductName;
                ws.Cell(row, 3).Value = recipe.Version;
                ws.Cell(row, 4).Value = recipe.Status;
                ws.Cell(row, 5).Value = recipe.Description;
                row++;
            }
        }

        private void ExportSummaryToExcel(IXLWorksheet ws)
        {
            ws.Cell(1, 1).Value = "СВОДНЫЙ ПРОИЗВОДСТВЕННЫЙ ОТЧЕТ";
            ws.Range("A1:D1").Merge().Style.Font.Bold = true;

            ws.Cell(3, 1).Value = "Всего продуктов:"; ws.Cell(3, 2).Value = _products.Count;
            ws.Cell(4, 1).Value = "Активных рецептур:"; ws.Cell(4, 2).Value = _recipes.Count(r => r.Status == "Активна");
            ws.Cell(5, 1).Value = "Произведено партий:"; ws.Cell(5, 2).Value = _batches.Count;
            ws.Cell(6, 1).Value = "Объем производства, кг:"; ws.Cell(6, 2).Value = _batches.Sum(b => b.Quantity);
            ws.Cell(7, 1).Value = "Одобрено лабораторией:"; ws.Cell(7, 2).Value = _batches.Count(b => b.Lab == "Одобрена");
            ws.Cell(8, 1).Value = "Заблокировано:"; ws.Cell(8, 2).Value = _batches.Count(b => b.Lab == "Заблокирована");
            ws.Cell(9, 1).Value = "Всего отклонений:"; ws.Cell(9, 2).Value = _deviations.Count;
            ws.Cell(10, 1).Value = "Устранено отклонений:"; ws.Cell(10, 2).Value = _deviations.Count(d => d.IsResolved);
        }

        // ==================== ВСПОМОГАТЕЛЬНЫЕ МЕТОДЫ ====================
        private Button CreateButton(string text, string colorHex, int marginLeft, int height, int width)
        {
            Button btn = new Button
            {
                Content = text,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(colorHex)),
                Foreground = Brushes.White,
                FontSize = 13,
                Height = height,
                Width = width,
                Margin = new Thickness(marginLeft, 0, 0, 0),
                Cursor = Cursors.Hand
            };
            return btn;
        }

        private DataGrid CreateDataGrid()
        {
            return new DataGrid
            {
                Background = Brushes.White,
                BorderBrush = Brushes.LightGray,
                BorderThickness = new Thickness(1),
                RowHeight = 35,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                HorizontalGridLinesBrush = Brushes.LightGray,
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                SelectionMode = DataGridSelectionMode.Single,
                SelectionUnit = DataGridSelectionUnit.FullRow
            };
        }

        private Border CreateKpiCard(string title, string value, string icon, int column)
        {
            Border border = new Border { Background = Brushes.White, BorderBrush = Brushes.LightGray, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(8), Margin = new Thickness(5), Padding = new Thickness(15) };
            StackPanel stack = new StackPanel();
            stack.Children.Add(new TextBlock { Text = icon + " " + title, FontSize = 12, Foreground = Brushes.Gray });
            stack.Children.Add(new TextBlock { Text = value, FontSize = 28, FontWeight = FontWeights.Bold, Foreground = new SolidColorBrush(Color.FromRgb(46, 125, 50)) });
            border.Child = stack;
            Grid.SetColumn(border, column);
            return border;
        }
    }
}