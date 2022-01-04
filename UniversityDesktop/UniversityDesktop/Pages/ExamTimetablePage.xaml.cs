using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using UniversityDesktop.Classes;

namespace UniversityDesktop.Pages
{
    public partial class ExamTimetablePage : Page
    {
        public ExamTimetablePage()
        {
            InitializeComponent();
            
            string jsonFilePath = "\\Temp\\tmp.json";
            string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + jsonFilePath;
            string jsonString = File.ReadAllText(fullPath); 
            List<Exam> events = (List<Exam>)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, typeof(List<Exam>));
            this.ExamDataGrid.ItemsSource = events;
        }
    }
}