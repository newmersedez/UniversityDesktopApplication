using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
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
        private readonly string _emptyPage = "../Pages/EmptyPage.xaml";

        private Student _student = new Student();
        private readonly StudentAuthentication _auth = new StudentAuthentication();
        private bool _authStatus = false;
        private string _currentFramePage;

        private ICommand _eventsButtonCommand;
        private ICommand _examTimetableButtonCommand;
        private ICommand _lessonTimetableButtonCommand;
        private ICommand _marksButtonCommand;
        private ICommand _authenticationCommand;
        private ICommand _logoutCommand;
        
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

        private bool AuthStatus
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
            _authenticationCommand = new RelayCommand(param => Authentication(param), _ => !AuthStatus);

        public ICommand LogoutCommand =>
            _logoutCommand = new RelayCommand(_ => Logout(), _ => AuthStatus);

        #endregion
        
        #region Functions

        private void Authentication(object param)
        {
            var passwordBox = param as PasswordBox;
            StudentPassword = passwordBox.Password;
            
            if ( String.IsNullOrEmpty(_auth.StudentLogin) || String.IsNullOrEmpty(_auth.StudentPassword))
                MessageBox.Show("Поля логина и пароля не должны быть пустыми", "Ошибка");
            else if (StudentLogin == "admin" && StudentPassword == "admin")
                MessageBox.Show("Данный клиент не поддерживает админ-панель, используйте другой", "Ошибка");
            else
            {
                try
                {
                    // Create new server request
                    ServerRequest request = new ServerRequest();
                    request.RequestName = "Auth";
                    request.Args = new List<string>();
                    request.Args.Add(StudentLogin);
                    request.Args.Add(StudentPassword);
                    var jsonAuthString = JsonConvert.SerializeObject(request);
                    
                    // Send request to server
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.ConnectAsync(IPAddress.Loopback, Port);
                    _buffer = Encoding.ASCII.GetBytes(jsonAuthString);
                    socket.Send(_buffer);
                
                    // Recieve answer from server
                    byte[] recvBuffer = new byte[1024];
                    int recvNumber = socket.Receive(recvBuffer);
                    char[] chars = new char[recvNumber];
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                    string jsonString = new string(chars);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    // Auth
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
                }
                catch (SocketException)
                {
                    MessageBox.Show("Failed to get server response", "Error");
                }
            }
        }

        private void Logout()
        {
            AuthStatus = !AuthStatus;
            CurrentFramePage = _emptyPage;
            
            StudentLastname = "";
            StudentName = "";
            StudentPatronymic = "";
            StudentGroup = "";
            StudentFormOfEducation = "";
            StudentDegree = "";
            SpecialtyNumber = "";
            SpecialtyName = "";
        }

        private void GetEvents()
        {
            if (CurrentFramePage != _eventsPagePath)
            {
                try
                {
                    // Create new server request
                    ServerRequest request = new ServerRequest();
                    request.RequestName = "Events";
                    var jsonEventsString = JsonConvert.SerializeObject(request);

                    // Send request to server
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.ConnectAsync(IPAddress.Loopback, Port);
                    _buffer = Encoding.ASCII.GetBytes(jsonEventsString);
                    socket.Send(_buffer);

                    // Recieve answer from server
                    byte[] recvBuffer = new byte[10000];
                    int recvNumber = socket.Receive(recvBuffer);
                    char[] chars = new char[recvNumber];
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                    string jsonString = new string(chars);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    // Load events page
                    string jsonFilePath = "\\Temp\\tmp.json";
                    string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
                                      jsonFilePath;
                    File.WriteAllTextAsync(fullPath, jsonString);
                    CurrentFramePage = _eventsPagePath;
                }
                catch (SocketException)
                {
                    MessageBox.Show("Failed to get server response", "Error");
                }
            }
        }

        private void GetLessons()
        {
            if (CurrentFramePage != _lessonTimetablePagePath)
            {
                try
                {
                    // Create new server request
                    ServerRequest request = new ServerRequest();
                    request.RequestName = "Lessons";
                    request.Args = new List<string>();
                    request.Args.Add(StudentGroup);
                    var jsonServerRequestString = JsonConvert.SerializeObject(request);

                    // Send request to server
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.ConnectAsync(IPAddress.Loopback, Port);
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    _buffer = encoding.GetBytes(jsonServerRequestString);
                    socket.Send(_buffer);

                    // Recieve answer from server
                    byte[] recvBuffer = new byte[10000];
                    int recvNumber = socket.Receive(recvBuffer);
                    char[] chars = new char[recvNumber];
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                    string jsonString = new string(chars);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    // Load lessons page
                    string jsonFilePath = "\\Temp\\tmp.json";
                    string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
                                      jsonFilePath;
                    File.WriteAllTextAsync(fullPath, jsonString);
                    CurrentFramePage = _lessonTimetablePagePath;
                }
                catch (SocketException)
                {
                    MessageBox.Show("Failed to get server response", "Error");
                }
            }
        }

        private void GetExams()
        {
            if (CurrentFramePage != _examTimetablePagePath)
            {
                try
                {
                    // Create new server request
                    ServerRequest request = new ServerRequest();
                    request.RequestName = "Exams";
                    request.Args = new List<string>();
                    request.Args.Add(StudentGroup);
                    var jsonServerRequestString = JsonConvert.SerializeObject(request);

                    // Send request to server
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.ConnectAsync(IPAddress.Loopback, Port);
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    _buffer = encoding.GetBytes(jsonServerRequestString);
                    socket.Send(_buffer);

                    // Recieve answer from server
                    byte[] recvBuffer = new byte[10000];
                    int recvNumber = socket.Receive(recvBuffer);
                    char[] chars = new char[recvNumber];
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                    string jsonString = new string(chars);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    // Load exams page
                    string jsonFilePath = "\\Temp\\tmp.json";
                    string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
                                      jsonFilePath;
                    File.WriteAllTextAsync(fullPath, jsonString);
                    CurrentFramePage = _examTimetablePagePath;
                }
                catch (SocketException)
                {
                    MessageBox.Show("Failed to get server response", "Error");
                }
            }
        }

        private void GetMarks()
        {
            if (CurrentFramePage != _MarksPagePath)
            {
                try
                {
                    // Create new server request
                    ServerRequest request = new ServerRequest();
                    request.RequestName = "Marks";
                    request.Args = new List<string>();
                    request.Args.Add(StudentLastname);
                    request.Args.Add(StudentName);
                    request.Args.Add(StudentPatronymic);
                    var jsonServerRequestString = JsonConvert.SerializeObject(request);

                    // Send request to server
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    socket.ConnectAsync(IPAddress.Loopback, Port);
                    System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding();
                    _buffer = encoding.GetBytes(jsonServerRequestString);
                    socket.Send(_buffer);

                    // Recieve answer from server
                    byte[] recvBuffer = new byte[10000];
                    int recvNumber = socket.Receive(recvBuffer);
                    char[] chars = new char[recvNumber];
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                    string jsonString = new string(chars);
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();

                    string jsonFilePath = "\\Temp\\tmp.json";
                    string fullPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName +
                                      jsonFilePath;
                    File.WriteAllTextAsync(fullPath, jsonString);
                    CurrentFramePage = _MarksPagePath;
                }
                catch (SocketException)
                {
                    MessageBox.Show("Failed to get server response", "Error");
                }
            }
        }

        #endregion
    } 
}