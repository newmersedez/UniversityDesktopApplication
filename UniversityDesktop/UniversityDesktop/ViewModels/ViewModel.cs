using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Input;
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
        private string _currentFramePage;
        private string _studentName;
        private string _studentLastname;
        private string _studentPatronymic;
        private string _studentGroup;
        private string _specialtyNumber;
        private string _specialtyName;
        
        // Login window info
        private string _studentLogin;
        private string _studentPassword;
        private string _errorMsg;

        // Commands
        private ICommand _eventsButtonCommand;
        private ICommand _examTimetableButtonCommand;
        private ICommand _lessonTimetableButtonCommand;
        private ICommand _marksButtonCommand;
        private ICommand _loginButtonCommand;

        // Connection info
        private const string _host = "localhost";
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
                _studentName;
            set =>
                _studentName = value;
        }

        public string StudentLastname
        {
            get =>
                _studentLastname;
            set =>
                _studentLastname = value;
        }

        public string StudentPatronymic
        {
            get =>
                _studentPatronymic;
            set =>
                _studentPatronymic = value;
        }

        public string StudentGroup
        {
            get =>
                _studentGroup;
            set =>
                _studentGroup = value;
        }

        public string SpecialtyNumber
        {
            get =>
                _specialtyNumber;
            set =>
                _specialtyNumber = value;
        }

        public string SpecialtyName
        {
            get =>
                _specialtyName;
            set =>
                _specialtyName = value;
        }

        public string StudentLogin
        {
            get =>
                _studentLogin;
            set
            {
                _studentLogin = value;
                RaisePropertyChanged(nameof(Login));
            }
        }

        public string StudentPassword
        {
            get =>
                _studentPassword;
            set
            {
                _studentPassword = value;
                RaisePropertyChanged(nameof(Login));
            }
        }
        
        public string ErrorMsg
        {
            get =>
                _errorMsg;
            set
            {
                _errorMsg = value;
                RaisePropertyChanged(nameof(Login));
            }
        }

        public ICommand LoginButtonCommand =>
            _loginButtonCommand = new RelayCommand(_ => Login());
        
        public ICommand EventsButtonCommand =>
            _eventsButtonCommand = new RelayCommand(_ => GetEvents());

        public ICommand ExamButtonCommand =>
            _examTimetableButtonCommand = new RelayCommand(_ => GetExams());
        
        public ICommand LessonTimetableButtonCommand =>
            _lessonTimetableButtonCommand = new RelayCommand(_ => GetLessons());
        
        public ICommand MarksButtonCommand =>
            _marksButtonCommand = new RelayCommand(_ => GetMarks());
        
        #endregion

        #region Functions

        private void Login()
        {
            RegistrationWindow regWindow = new RegistrationWindow();
            regWindow.Show();
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