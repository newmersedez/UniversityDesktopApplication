using System.Collections;
using System.Collections.Generic;

namespace UniversityDesktop.Classes
{
    public sealed class StudentAuthentication
    {
        public string StudentLogin { get; set; }
        public string StudentPassword  { get; set; }
    };
    
    public sealed class Student
    {
        public string StudentLastname { get; set; }
        public string StudentName { get; set; }
        public string StudentPatronymic { get; set; }
        public string StudentGroup { get; set; }
        public string StudentDegree { get; set; }
        public string StudentFormOfEducation { get; set; }
        public string SpecialtyNumber { get; set; }
        public string SpecialtyName { get; set; }
    }
}