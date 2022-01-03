using System.Net;
using System.Net.Sockets;
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

        private readonly string _eventsPagePath = "../Pages/EventsPage.xaml";
        private readonly string _examTimetablePagePath = "../Pages/ExamTimetablePage.xaml";
        private readonly string _lessonTimetablePagePath = "../Pages/LessonTimetablePage.xaml";
        private readonly string _MarksPagePath = "../Pages/MarksPage.xaml";

        private string _currentFramePage;
        private string _studentName = "Дмитрий";
        private string _studentLastname = "Тришин";
        private string _studentPatronymic = "Александрович";
        private string _studentGroup = "М8О-311Б-19";
        private string _specialtyNumber = "02.03.02";
        private string _specialtyName = "Фундаментальная информатика и информационные технологии";
        
        private ICommand _eventsButtonCommand;
        private ICommand _examTimetableButtonCommand;
        private ICommand _lessonTimetableButtonCommand;
        private ICommand _marksButtonCommand;

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

        private void GetEvents()
        {
            try
            {
                Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Connect(IPAddress.Loopback, _port);
                _buffer = Encoding.ASCII.GetBytes("Events");
                _socket.Send(_buffer);
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                CurrentFramePage = _eventsPagePath;
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
                _buffer = Encoding.ASCII.GetBytes("Exams");
                _socket.Send(_buffer);
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                CurrentFramePage = _examTimetablePagePath;
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
                _buffer = Encoding.ASCII.GetBytes("Lessons");
                _socket.Send(_buffer);
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
                CurrentFramePage = _lessonTimetablePagePath;
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
                _buffer = Encoding.ASCII.GetBytes("Marks");
                _socket.Send(_buffer);
                _socket.Shutdown(SocketShutdown.Both);
                _socket.Close();
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