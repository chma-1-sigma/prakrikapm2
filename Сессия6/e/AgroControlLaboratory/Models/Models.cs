using System;
using System.Collections.ObjectModel;

namespace AgroControlLaboratory.Models
{
    public class BatchForQuality
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; }
        public string ProductName { get; set; }
        public string SampleType { get; set; }
        public string Status { get; set; }
        public string LabStatus { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int ActualQuantityKg { get; set; }
        public string Supplier { get; set; }
        public DateTime? ArrivalDate { get; set; }

        public string ArrivalDateStr
        {
            get { return ArrivalDate.HasValue ? ArrivalDate.Value.ToString("dd.MM.yyyy") : ""; }
        }

        public string StartTimeStr
        {
            get { return StartTime.HasValue ? StartTime.Value.ToString("dd.MM.yyyy HH:mm") : ""; }
        }

        public string QuantityStr
        {
            get { return ActualQuantityKg + " кг"; }
        }
    }

    public class QualityParameter
    {
        public int Id { get; set; }
        public string ParameterName { get; set; }
        public string StandardValue { get; set; }
        public string Unit { get; set; }
        public decimal? MeasuredValue { get; set; }
        public string Result { get; set; }
        public decimal? ToleranceMin { get; set; }
        public decimal? ToleranceMax { get; set; }
        public bool IsMandatory { get; set; }
        public string Comment { get; set; }

        public QualityParameter()
        {
            ParameterName = "";
            StandardValue = "";
            Unit = "";
            Result = "⏳ Не проверено";
        }
    }

    public class QualityTestHistory
    {
        public int Id { get; set; }
        public int BatchId { get; set; }
        public string BatchNumber { get; set; }
        public string SampleType { get; set; }
        public DateTime TestDate { get; set; }
        public string AnalystName { get; set; }
        public string Result { get; set; }
        public string Decision { get; set; }
        public string Comment { get; set; }

        public string TestDateStr
        {
            get { return TestDate.ToString("dd.MM.yyyy HH:mm"); }
        }

        public QualityTestHistory()
        {
            BatchNumber = "";
            SampleType = "";
            AnalystName = "";
            Result = "";
            Decision = "";
            Comment = "";
        }
    }
}