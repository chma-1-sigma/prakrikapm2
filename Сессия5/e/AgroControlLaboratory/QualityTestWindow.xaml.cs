using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using AgroControlLaboratory.Models;

namespace AgroControlLaboratory.Views
{
    public partial class QualityTestWindow : Window
    {
        private BatchForQuality _batch;
        private ObservableCollection<QualityParameter> _parameters;

        public QualityTestWindow(BatchForQuality batch)
        {
            InitializeComponent();
            _batch = batch;

            txtBatchNumber.Text = batch.BatchNumber;
            txtProductName.Text = batch.ProductName;
            txtSampleType.Text = (batch.SampleType == "raw_material") ? "Сырье" : "Готовая продукция";

            LoadParameters();

            btnSave.Click += (s, e) => SaveResults();
            btnApprove.Click += (s, e) => ApproveBatch();
            btnBlock.Click += (s, e) => BlockBatch();
            btnCancel.Click += (s, e) => Close();
        }

        private void LoadParameters()
        {
            _parameters = new ObservableCollection<QualityParameter>();
            _parameters.Add(new QualityParameter { ParameterName = "Концентрация", StandardValue = "≥97%", Unit = "%", ToleranceMin = 97, IsMandatory = true });
            _parameters.Add(new QualityParameter { ParameterName = "pH", StandardValue = "6.5-7.0", Unit = "", ToleranceMin = 6.5m, ToleranceMax = 7.0m, IsMandatory = true });
            _parameters.Add(new QualityParameter { ParameterName = "Влажность", StandardValue = "≤2.5%", Unit = "%", ToleranceMax = 2.5m, IsMandatory = false });
            _parameters.Add(new QualityParameter { ParameterName = "Внешний вид", StandardValue = "Однородный", Unit = "", IsMandatory = true });

            dgParameters.ItemsSource = _parameters;
        }

        private void SaveResults()
        {
            foreach (QualityParameter param in _parameters)
            {
                if (param.MeasuredValue.HasValue)
                {
                    param.Result = CheckParameter(param);
                }
            }
            dgParameters.Items.Refresh();
            MessageBox.Show("Результаты сохранены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ApproveBatch()
        {
            var missingParams = new System.Collections.Generic.List<QualityParameter>();
            foreach (QualityParameter param in _parameters)
            {
                if (param.IsMandatory && !param.MeasuredValue.HasValue)
                {
                    missingParams.Add(param);
                }
            }

            if (missingParams.Count > 0)
            {
                string names = "";
                foreach (QualityParameter p in missingParams)
                {
                    names += p.ParameterName + ", ";
                }
                names = names.TrimEnd(',', ' ');
                MessageBox.Show("Заполните обязательные параметры: " + names, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Партия " + _batch.BatchNumber + " ОДОБРЕНА!\n\nКомментарий: " + txtComment.Text, "Решение принято", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private void BlockBatch()
        {
            if (string.IsNullOrWhiteSpace(txtComment.Text))
            {
                MessageBox.Show("Укажите причину блокировки!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            MessageBox.Show("Партия " + _batch.BatchNumber + " ЗАБЛОКИРОВАНА!\n\nПричина: " + txtComment.Text, "Решение принято", MessageBoxButton.OK, MessageBoxImage.Information);
            Close();
        }

        private string CheckParameter(QualityParameter param)
        {
            if (!param.MeasuredValue.HasValue) return "❓ Не измерено";

            decimal value = param.MeasuredValue.Value;

            if (param.ToleranceMin.HasValue && value < param.ToleranceMin.Value)
                return "❌ Не пройден";

            if (param.ToleranceMax.HasValue && value > param.ToleranceMax.Value)
                return "❌ Не пройден";

            return "✅ Пройден";
        }
    }
}