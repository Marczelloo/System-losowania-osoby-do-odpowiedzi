using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System_losowania_osoby_do_odpowiedzi.Controls
{
    class StudentClass
    {
        public string ClassName { get; set; }
        public List<Student> Students { get; set; }

        public StudentClass(string className)
        {
            ClassName = className;
            Students = new List<Student>();
        }

        public void AddStudent(Student student)
        {
            Students.Add(student);
        }
        public void RemoveStudent(Student student)
        {
            Students.Remove(student);
        }
        public void ClearStudents()
        {
            Students.Clear();
        }
    }
}
