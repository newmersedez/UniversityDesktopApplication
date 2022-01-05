﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using UniversityDesktop.Classes;
using UniversityDesktop.MVVM.Core.Command;
using UniversityDesktop.MVVM.Core.ViewModel;
using UniversityDesktop.Pages;

namespace UniversityDesktop.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        #region Variables

        private readonly string _eventsPagePath = "../Pages/EventsPage.xaml";
        private readonly string _examTimetablePagePath = "../Pages/ExamTimetablePage.xaml";
        private readonly string _lessonTimetablePagePath = "../Pages/LessonTimetablePage.xaml";
        private readonly string _MarksPagePath = "../Pages/MarksPage.xaml";

        private Student _student = new Student();
        private readonly StudentAuthentication _auth = new StudentAuthentication();
        private bool _authStatus = false;
        private string _currentFramePage;

        private ICommand _eventsButtonCommand;
        private ICommand _examTimetableButtonCommand;
        private ICommand _lessonTimetableButtonCommand;
        private ICommand _marksButtonCommand;
        private ICommand _authenticationCommand;
        
        private const int Port = 5430;
        private byte[] _buffer = new byte[1024];

        #endregion

        #region Properties

        public string CurrentFramePage
        {
            get =>
                _currentFramePage;
            set
            {
                _currentFramePage = value;
                RaisePropertyChanged(nameof(CurrentFramePage));
            }
        }

        public bool AuthStatus
        {
            get =>
                _authStatus;
            set =>
                _authStatus = value;
        }

        public string StudentName
        {
            get =>
                _student.StudentName;
            set 
            { 
                _student. StudentName = value;
                RaisePropertyChanged(nameof(StudentName));
            }
        }

        public string StudentLastname
        {
            get =>
                _student.StudentLastname;
            set
            {
                _student.StudentLastname = value;
                RaisePropertiesChanged(nameof(StudentLastname));
            }
        }

        public string StudentPatronymic
        {
            get =>
                _student.StudentPatronymic;
            set
            {
                _student.StudentPatronymic = value;
                RaisePropertyChanged(nameof(StudentPatronymic));
            }
        }

        public string StudentGroup
        {
            get =>
                _student.StudentGroup;
            set
            {
                _student.StudentGroup = value;
                RaisePropertyChanged(nameof(StudentGroup));
            }
        }
        
        public string StudentDegree
        {
            get =>
                _student.StudentDegree;
            set
            {
                _student.StudentDegree = value;
                RaisePropertyChanged(nameof(StudentDegree));
            }
        }

        public string StudentFormOfEducation
        {
            get =>
                _student.StudentFormOfEducation;
            set
            {
                _student.StudentFormOfEducation = value;
                RaisePropertyChanged(nameof(StudentFormOfEducation));
            }
        }
        
        public string SpecialtyNumber
        {
            get =>
                _student.SpecialtyNumber;
            set
            {
                _student.SpecialtyNumber = value;
                RaisePropertyChanged(nameof(SpecialtyNumber));
            }
        }

        public string SpecialtyName
        {
            get =>
                _student.SpecialtyName;
            set
            {
                _student.SpecialtyName = value;
                RaisePropertyChanged(nameof(SpecialtyName));
            }
        }

        public string StudentLogin
        {
            get =>
                _auth.StudentLogin;
            set
            {
                _auth.StudentLogin = value;
                RaisePropertyChanged(nameof(StudentLogin));
            }
        }

        public string StudentPassword
        {
            get =>
                _auth.StudentPassword;
            set
            {
                _auth.StudentPassword = value;
                RaisePropertyChanged(nameof(StudentPassword));
            }
        }

        #endregion

        #region Commands

        public ICommand EventsButtonCommand =>
            _eventsButtonCommand = new RelayCommand(_ => GetEvents(), _=>AuthStatus);

        public ICommand ExamButtonCommand =>
            _examTimetableButtonCommand = new RelayCommand(_ => GetExams(), _ => AuthStatus);

        public ICommand LessonTimetableButtonCommand =>
            _lessonTimetableButtonCommand = new RelayCommand(_ => GetLessons(), _ => AuthStatus);

        public ICommand MarksButtonCommand =>
            _marksButtonCommand = new RelayCommand(_ => GetMarks(), _ => AuthStatus);
        
        public ICommand AuthenticationCommand =>
            _authenticationCommand = new RelayCommand(_ => Authentication());

        #endregion
        
        #region Functions

        private void Authentication()
        {
            if ( String.IsNullOrEmpty(_auth.StudentLogin) || String.IsNullOrEmpty(_auth.StudentPassword))
                MessageBox.Show("Поля логина и пароля не должны быть пустыми", "Ошибка");
            else
            {
                try
                {
                    var jsonAuthString = JsonConvert.SerializeObject(_auth);                
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.Connect(IPAddress.Loopback, Port);
                    _buffer = Encoding.ASCII.GetBytes(jsonAuthString);
                    socket.Send(_buffer);
                
                    byte[] recvBuffer = new byte[1024];
                    int recvNumber = socket.Receive(recvBuffer);
                    char[] chars = new char[recvNumber];
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                    string jsonString = new string(chars);

                    if (jsonString == "[]")
                        MessageBox.Show("Неверный логин или пароль", "Ошибка");
                    else
                    {
                        List<Student> account = (List<Student>)Newtonsoft.Json.JsonConvert.DeserializeObject(jsonString, typeof(List<Student>));
                        _student = account[0];
                        StudentLastname = _student.StudentLastname;
                        StudentName = _student.StudentName;
                        StudentPatronymic = _student.StudentPatronymic;
                        StudentGroup = _student.StudentGroup;
                        StudentDegree = _student.StudentDegree;
                        StudentFormOfEducation = _student.StudentFormOfEducation;
                        SpecialtyNumber = _student.SpecialtyNumber;
                        SpecialtyName = _student.SpecialtyName;
                        AuthStatus = true;
                    }
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                catch (SocketException)
                {
                    MessageBox.Show("Failed to get server response", "Error");
                }
            }
        }

        private void GetEvents()
        {
            try
            {
                Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(IPAddress.Loopback, Port);
                _buffer = Encoding.ASCII.GetBytes("Events");
                _socket.Send(_buffer);

                byte[] recvBuffer = new byte[10000];
                int recvNumber = _socket.Receive(recvBuffer);
                char[] chars = new char[recvNumber];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                string jsonString = new string(chars);

                string jsonFilePath = "\\Temp\\tmp.json";
                string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + jsonFilePath;
                File.WriteAllText(fullPath, jsonString);
                CurrentFramePage = _eventsPagePath;
            }
            catch (SocketException)
            {
                MessageBox.Show("Failed to get server response", "Error");
            }
        }
        
        private void GetLessons()
        {
            try
            {
                Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(IPAddress.Loopback, Port);
                _buffer = Encoding.ASCII.GetBytes("Lessons");
                _socket.Send(_buffer);

                byte[] recvBuffer = new byte[10000];
                int recvNumber = _socket.Receive(recvBuffer);
                char[] chars = new char[recvNumber];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                string jsonString = new string(chars);

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                
                string jsonFilePath = "\\Temp\\tmp.json";
                string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + jsonFilePath;
                File.WriteAllText(fullPath, jsonString);
                CurrentFramePage = _lessonTimetablePagePath;
            }
            catch (SocketException)
            {
                MessageBox.Show("Failed to get server response", "Error");
            }
        }

        
        private void GetExams()
        {
            try
            {
                Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(IPAddress.Loopback, Port);
                _buffer = Encoding.ASCII.GetBytes("Exams");
                _socket.Send(_buffer);

                byte[] recvBuffer = new byte[10000];
                int recvNumber = _socket.Receive(recvBuffer);
                char[] chars = new char[recvNumber];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                string jsonString = new string(chars);

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                
                string jsonFilePath = "\\Temp\\tmp.json";
                string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + jsonFilePath;
                File.WriteAllText(fullPath, jsonString);
                CurrentFramePage = _examTimetablePagePath;
            }
            catch (SocketException)
            {
                MessageBox.Show("Failed to get server response", "Error");
            }
        }
        
        private void GetMarks()
        {
            try
            {
                Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(IPAddress.Loopback, Port);
                _buffer = Encoding.ASCII.GetBytes("Marks");
                _socket.Send(_buffer);

                byte[] recvBuffer = new byte[10000];
                int recvNumber = _socket.Receive(recvBuffer);
                char[] chars = new char[recvNumber];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                string jsonString = new string(chars);

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                
                string jsonFilePath = "\\Temp\\tmp.json";
                string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName + jsonFilePath;
                File.WriteAllText(fullPath, jsonString);
                CurrentFramePage = _MarksPagePath;
            }
            catch (SocketException)
            {
                MessageBox.Show("Failed to get server response", "Error");
            }
        }

        #endregion
    } 
}