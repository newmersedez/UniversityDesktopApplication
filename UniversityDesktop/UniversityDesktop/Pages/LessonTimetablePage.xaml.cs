﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using UniversityDesktop.Classes;

namespace UniversityDesktop.Pages
{
    public partial class LessonTimetablePage : Page
    {
        public LessonTimetablePage()
        {
            InitializeComponent();
            
            string jsonFilePath = "\\Temp\\tmp.json";
            string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + jsonFilePath;
            string jsonString = File.ReadAllText(fullPath); 
            List<Lesson> lessons = (List<Lesson>)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, typeof(List<Lesson>));
            this.LessonsDataGrid.ItemsSource = lessons;
        }
    }
}