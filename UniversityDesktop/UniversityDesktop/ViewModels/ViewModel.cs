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
            _eventsButtonCommand = new RelayCommand(_ => { CurrentFramePage = _eventsPagePath; });

        public ICommand ExamButtonCommand =>
            _examTimetableButtonCommand = new RelayCommand(_ => { CurrentFramePage = _examTimetablePagePath; });
        
        public ICommand LessonTimetableButtonCommand =>
            _lessonTimetableButtonCommand = new RelayCommand(_ => { CurrentFramePage = _lessonTimetablePagePath; });
        
        public ICommand MarksButtonCommand =>
            _marksButtonCommand = new RelayCommand(_ => { CurrentFramePage = _MarksPagePath; });
        

        #endregion
    }
}