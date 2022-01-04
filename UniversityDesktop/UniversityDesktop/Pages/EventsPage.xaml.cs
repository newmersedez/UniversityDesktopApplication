using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Windows;
using System.Windows.Controls;
using UniversityDesktop.Classes;

namespace UniversityDesktop.Pages
{
    public partial class EventsPage : Page
    {
        public EventsPage()
        {
            InitializeComponent(); 
            string jsonFilePath = "\\Temp\\tmp.json";
            string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + jsonFilePath;
            string jsonString = File.ReadAllText(fullPath); 
            List<Event> events = (List<Event>)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, typeof(List<Event>));
            this.EventsDataGrid.ItemsSource = events;
        }
    }
}