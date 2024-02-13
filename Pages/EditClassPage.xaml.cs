using System.Diagnostics;
using System_losowania_osoby_do_odpowiedzi.Controls;

namespace System_losowania_osoby_do_odpowiedzi.Pages;

public partial class EditClassPage : ContentPage
{
    List<Student> allStudents;
    List<StudentClass> studentClass;
    List<string> classes;
    string pickedClass;

    string newStudentName;
    string newStudentSurname;

    Student studentToDelete;

    public EditClassPage()
	{
		InitializeComponent();
        allStudents = new List<Student>();
        studentClass = new List<StudentClass>();
        classes = new List<string>();

        LoadStudents();
        if (classes == null || classes.Count == 0) return;
        Debug.WriteLine("Loaded students");

        ClassPicker.ItemsSource = null;
        ClassPicker.ItemsSource = classes;

        pickedClass = classes[0];
        ClassPicker.SelectedItem = pickedClass;
        UpdateList();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadStudents();
        if (classes == null || classes.Count == 0) return;
        Debug.WriteLine("Loaded students");

        ClassPicker.ItemsSource = null;
        ClassPicker.ItemsSource = classes;

        pickedClass = classes[0];
        ClassPicker.SelectedItem = pickedClass;
        UpdateList();
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        SaveStudents();
    }

    private void LoadStudents()
    {
        allStudents.Clear();
        studentClass.Clear();
        classes.Clear();
        ClassList.Children.Clear();

        string appDataPath = FileSystem.AppDataDirectory;
        string filePath = Path.Combine(appDataPath, "students.txt");
        Debug.WriteLine("Loading students from file: " + filePath);

        if (!File.Exists(filePath)) return;

        try
        {
            string[] lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                string[] data = line.Split(',');
                Student student = new Student(int.Parse(data[0]), data[1], data[2], data[3], bool.Parse(data[4]), int.Parse(data[5]));
                allStudents.Add(student);
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("File operation failed " + e.Message);
        }

        foreach (var student in allStudents)
        {
            if (!classes.Contains(student.Class))
            {
                classes.Add(student.Class);
            }
        }

        foreach(var className in classes)
        {
            StudentClass newClass = new StudentClass(className);
            foreach(var student in allStudents)
            {
                if(student.Class == className)
                {
                    newClass.AddStudent(student);
                }
            }
            studentClass.Add(newClass);
        }
    }

    private void SaveStudents()
    {
        string appDataPath = FileSystem.AppDataDirectory;
        string filePath = Path.Combine(appDataPath, "students.txt");

        List<string> lines = new List<string>();
        foreach (var student in allStudents)
        {
            lines.Add(student.Number + "," + student.Name + "," + student.Surname + "," + student.Class + "," + student.IsPresent + "," + student.RoundsToDraw);
        }

        try
        {
            Debug.WriteLine("Saving students to file: " + filePath);
            File.WriteAllLines(filePath, lines);
        }
        catch (UnauthorizedAccessException ex)
        {
            Debug.WriteLine("UnauthorizedAccessException: " + ex.Message);
            DisplayAlert("Error", "Unauthorized access to file. Cannot save students.", "OK");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Exception: " + ex.Message);
            DisplayAlert("Error", "An error occurred while saving students.", "OK");
        }
    }

    private void UpdateList()
    {
        ClassList.Children.Clear();
        foreach (var sc in studentClass)
        {
            if(pickedClass == sc.ClassName)
            {
                Label label = new Label();
                label.Text = "Student list of class: " + sc.ClassName;
                ClassList.Children.Add(label);
                foreach (var student in sc.Students)
                {
                    ClassList.Children.Add(student);
                }
            }
        }

        var selectedStudents = studentClass.Find(sc => sc.ClassName == pickedClass)?.Students;
        if (selectedStudents != null)
        {
            var studentNamesAndSurnames = selectedStudents.Select(student => $"{student.Name} {student.Surname}").ToArray();
            StudentPicker.ItemsSource = studentNamesAndSurnames;
        }
        else
        {
            StudentPicker.ItemsSource = null; 
        }
    }
    private void ClassPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ClassPicker.SelectedItem != null)
        {
            pickedClass = ClassPicker.SelectedItem.ToString();
            UpdateList();
        }
        else
        {
            Debug.WriteLine("No item selected in the Picker.");
        }
    }

    private async void AddClass_Clicked(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Add new class", "Class Name: ");

        if (result == null) return;

        if (classes.Contains(result))
        {
            await DisplayAlert("Error", "Class already exists", "OK");
            return;
        }

        Debug.WriteLine("Adding class: " + result);

        classes.Add(result);
        studentClass.Add(new StudentClass(result));

        ClassPicker.ItemsSource = null;
        ClassPicker.ItemsSource = classes;

        ClassPicker.SelectedItem = result;
        pickedClass = result;
    }

    private void RemoveClass_Clicked(object sender, EventArgs e)
    {
        if (classes.Count == 0) return;
        Debug.WriteLine("Removing class: " + pickedClass);
        classes.Remove(pickedClass);

        allStudents.RemoveAll(student => student.Class == pickedClass);

        ClassPicker.ItemsSource = null;
        ClassPicker.ItemsSource = classes;
        
        pickedClass = classes.FirstOrDefault();
        ClassPicker.SelectedItem = pickedClass;

        UpdateList();
        SaveStudents();
    }

    private void AddStudent_Clicked(object sender, EventArgs e)
    {
        if (newStudentName == null || newStudentSurname == null || newStudentName == "" || newStudentSurname == "")
        {
            DisplayAlert("Error", "Name and Surname cannot be empty", "OK");
            return;
        }
        Debug.WriteLine("Adding student: " + newStudentName + " " + newStudentSurname + " to class: " + pickedClass);
        if(allStudents.Exists(student => student.Name == newStudentName && student.Surname == newStudentSurname && student.Class == pickedClass))
        {
            DisplayAlert("Error", "Student already exists in this class", "OK");
            return;
        }

        int number = studentClass.Find(sc => sc.ClassName == pickedClass).Students.Count + 1;
        allStudents.Add(new Student(number, newStudentName, newStudentSurname, pickedClass, true, 0));
        studentClass.Find(sc => sc.ClassName == pickedClass).AddStudent(new Student(number, newStudentName, newStudentSurname, pickedClass, true, 0));
        UpdateList();
        SaveStudents();

        EntryName.Text = "";
        EntrySurname.Text = "";
    }
    private void RemoveStudent_Clicked(object sender, EventArgs e)
    {
        if (studentToDelete == null) return;
        Debug.WriteLine("Removing student: " + studentToDelete.Name + " " + studentToDelete.Surname + " from class: " + pickedClass);
        allStudents.Remove(studentToDelete);
        Debug.WriteLine("ALLS: " + allStudents.Count);
        var selectedClass = studentClass.Find(sc => sc.ClassName == pickedClass);
        if (selectedClass != null)
        {
            var studentToRemove = selectedClass.Students.FirstOrDefault(student =>
                student.Name == studentToDelete.Name && student.Surname == studentToDelete.Surname);

            if (studentToRemove != null)
            {
                selectedClass.Students.Remove(studentToRemove);
                Debug.WriteLine("SCS: " + selectedClass.Students.Count);
            }
        }
        UpdateList();
        SaveStudents();
    }

    private void EntryName_TextChanged(object sender, TextChangedEventArgs e)
    {
        newStudentName = e.NewTextValue;
    }

    private void EntrySurname_TextChanged(object sender, TextChangedEventArgs e)
    {
        newStudentSurname = e.NewTextValue;
    }

    private void StudentPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (StudentPicker.SelectedItem == null)
        {
            Debug.WriteLine("No student selected for removal.");
            return;
        }

        string selectedOption = StudentPicker.SelectedItem.ToString();
        string name = selectedOption.Split(' ')[0];
        string surname = selectedOption.Split(' ')[1];
        studentToDelete = allStudents.Find(student => student.Name == name && student.Surname == surname);
        if (studentToDelete != null)
        {
               Debug.WriteLine("Selected student to delete: " + studentToDelete.Name + " " + studentToDelete.Surname);
        }
    }
}