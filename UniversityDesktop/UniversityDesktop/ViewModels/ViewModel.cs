using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Newtonsoft.Json;
using UniversityDesktop.MVVM.Core.Command;
using UniversityDesktop.MVVM.Core.ViewModel;

namespace UniversityDesktop.ViewModels
{
    public class ViewModel : ViewModelBase
    {
        #region Variables

        // Pages
        private readonly string _eventsPagePath = "../Pages/EventsPage.xaml";
        private readonly string _examTimetablePagePath = "../Pages/ExamTimetablePage.xaml";
        private readonly string _lessonTimetablePagePath = "../Pages/LessonTimetablePage.xaml";
        private readonly string _MarksPagePath = "../Pages/MarksPage.xaml";

        // Student info
        private struct Student
        {
            public string StudentLastname;
            public string StudentName;
            public string StudentPatronymic;
            public string StudentGroup;
            public string StudentDegree;
            public string StudentFormOfEducation;
            public string SpecialtyNumber;
            public string SpecialtyName;
        }

        // Login window info
        private struct StudentAuthentication
        {
            public string StudentLogin;
            public string StudentPassword;
        };

        private Student _student = new Student();
        private StudentAuthentication _auth = new StudentAuthentication();
        private string _currentFramePage;
        private string _errorMsg;

        // Commands
        private ICommand _eventsButtonCommand;
        private ICommand _examTimetableButtonCommand;
        private ICommand _lessonTimetableButtonCommand;
        private ICommand _marksButtonCommand;
        private ICommand _loginButtonCommand;
        private ICommand _authenticationCommand;

        // Connection info
        private const string _host = "local_host";
        private const int _port = 5430;
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

        public string StudentName
        {
            get =>
                _student.StudentName;
            set =>
                _student.StudentName = value;
        }

        public string StudentLastname
        {
            get =>
                _student.StudentLastname;
            set =>
                _student.StudentLastname = value;
        }

        public string StudentPatronymic
        {
            get =>
                _student.StudentPatronymic;
            set =>
                _student.StudentPatronymic = value;
        }

        public string StudentGroup
        {
            get =>
                _student.StudentGroup;
            set =>
                _student.StudentGroup = value;
        }
        
        public string StudentDegree
        {
            get =>
                _student.StudentDegree;
            set =>
                _student.StudentDegree = value;
        }

        public string StudentFormOfEducation
        {
            get =>
                _student.StudentFormOfEducation;
            set =>
                _student.StudentFormOfEducation = value;
        }
        
        public string SpecialtyNumber
        {
            get =>
                _student.SpecialtyNumber;
            set =>
                _student.SpecialtyNumber = value;
        }

        public string SpecialtyName
        {
            get =>
                _student.SpecialtyName;
            set =>
                _student.SpecialtyName = value;
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
        
        public string ErrorMsg
        {
            get =>
                _errorMsg;
            set
            {
                _errorMsg = value;
                RaisePropertyChanged(nameof(ErrorMsg));
            }
        }

        public ICommand EventsButtonCommand =>
            _eventsButtonCommand = new RelayCommand(_ => GetEvents());

        public ICommand ExamButtonCommand =>
            _examTimetableButtonCommand = new RelayCommand(_ => GetExams());

        public ICommand LessonTimetableButtonCommand =>
            _lessonTimetableButtonCommand = new RelayCommand(_ => GetLessons());

        public ICommand MarksButtonCommand =>
            _marksButtonCommand = new RelayCommand(_ => GetMarks());

        public ICommand LoginButtonCommand =>
            _loginButtonCommand = new RelayCommand(_ => {
                RegistrationWindow regWindow = new RegistrationWindow();
                regWindow.Show();
                regWindow.Tag = "auth_window";
            });

        public ICommand AuthenticationCommand =>
            _authenticationCommand = new RelayCommand(_ => Authentication());

        #endregion

        #region Functions

        private void Authentication()
        {
            if ( String.IsNullOrEmpty(_auth.StudentLogin) || String.IsNullOrEmpty(_auth.StudentPassword))
                ErrorMsg = "Поля логина и пароля не должны быть пустыми";
            else
            {
                try
                {
                    var jsonAuthString = JsonConvert.SerializeObject(_auth);                
                    Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    _socket.Connect(IPAddress.Loopback, _port);
                    _buffer = Encoding.ASCII.GetBytes(jsonAuthString);
                    _socket.Send(_buffer);
                
                    byte[] recvBuffer = new byte[1024];
                    int recvNumber = _socket.Receive(recvBuffer);
                    char[] chars = new char[recvNumber];
                    System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                    int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                    string jsonString = new string(chars);
                
                    if (jsonString == "[]")
                        ErrorMsg = "Неверный логин или пароль";
                    else
                    {
                        MessageBox.Show(jsonString);
                        ErrorMsg = "";
                    }
                    _socket.Shutdown(SocketShutdown.Both);
                    _socket.Close();
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
                _socket.Connect(IPAddress.Loopback, _port);
                _buffer = Encoding.ASCII.GetBytes("Events");
                _socket.Send(_buffer);

                byte[] recvBuffer = new byte[1024];
                int recvNumber = _socket.Receive(recvBuffer);
                char[] chars = new char[recvNumber];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                string recv = new string(chars);

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                
                CurrentFramePage = _eventsPagePath;
                MessageBox.Show(recv, "RECIEVED");
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
                _socket.Connect(IPAddress.Loopback, _port);
                _buffer = Encoding.ASCII.GetBytes("Events");
                _socket.Send(_buffer);

                byte[] recvBuffer = new byte[1024];
                int recvNumber = _socket.Receive(recvBuffer);
                char[] chars = new char[recvNumber];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                string recv = new string(chars);

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                
                CurrentFramePage = _examTimetablePagePath;
                MessageBox.Show(recv, "RECIEVED");
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
                _socket.Connect(IPAddress.Loopback, _port);
                _buffer = Encoding.ASCII.GetBytes("Events");
                _socket.Send(_buffer);

                byte[] recvBuffer = new byte[1024];
                int recvNumber = _socket.Receive(recvBuffer);
                char[] chars = new char[recvNumber];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                string recv = new string(chars);

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                
                CurrentFramePage = _lessonTimetablePagePath;
                MessageBox.Show(recv, "RECIEVED");
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
                _socket.Connect(IPAddress.Loopback, _port);
                _buffer = Encoding.ASCII.GetBytes("Events");
                _socket.Send(_buffer);

                byte[] recvBuffer = new byte[1024];
                int recvNumber = _socket.Receive(recvBuffer);
                char[] chars = new char[recvNumber];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(recvBuffer, 0, recvNumber, chars, 0);
                string recv = new string(chars);

                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                
                CurrentFramePage = _MarksPagePath;
                MessageBox.Show(recv, "RECIEVED");
            }
            catch (SocketException)
            {
                MessageBox.Show("Failed to get server response", "Error");
            }
        }

        #endregion
    } 
}