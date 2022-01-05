using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using UniversityDesktop.Classes;

namespace UniversityDesktop.Pages
{
    public partial class MarksPage : Page
    {
        public MarksPage()
        {
            InitializeComponent();
            
            string jsonFilePath = "\\Temp\\tmp.json";
            string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + jsonFilePath;
            string jsonString = File.ReadAllText(fullPath); 
            List<Mark> events = (List<Mark>)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, typeof(List<Mark>));
            this.MarksDataGrid.ItemsSource = events;
        }
    }
}