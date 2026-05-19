using System;
using System.Collections.ObjectModel;

namespace AgroControlOperator.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Shift { get; set; }
    }

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
    }
}