using System.Diagnostics;
using System_losowania_osoby_do_odpowiedzi.Controls;

namespace System_losowania_osoby_do_odpowiedzi.Pages;

public partial class PersonDrawPage : ContentPage
{
	List<Student> allStudents;
	List<StudentClass> studentClass;
	List<string> classes;
	string pickedClass;
	int luckyNumber;

	public PersonDrawPage()
	{
		InitializeComponent();
		allStudents = new List<Student>();
		studentClass = new List<StudentClass>();
		classes = new List<string>();

		LoadStudents();
		ClassToDraw.ItemsSource = null;
		ClassToDraw.ItemsSource = classes;
		Debug.WriteLine("Loaded students");

        int highestNumber = allStudents.Max(student => student.Number);
        int randomNumber = new Random().Next(1, highestNumber + 1);
		luckyNumber = randomNumber;
        LuckyNumberLabel.Text = "Lucky number: " + luckyNumber;

        if (classes == null || classes.Count == 0) return;
        pickedClass = classes[0];
		ClassToDraw.SelectedItem = pickedClass;
		UpdateList();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        LoadStudents();
        ClassToDraw.ItemsSource = null;
        ClassToDraw.ItemsSource = classes;
        Debug.WriteLine("Loaded students");

        int highestNumber = allStudents.Max(student => student.Number);
        int randomNumber = new Random().Next(1, highestNumber + 1);
        luckyNumber = randomNumber;
        LuckyNumberLabel.Text = "Lucky number: " + luckyNumber;

        if (classes == null || classes.Count == 0) return;
        pickedClass = classes[0];
        ClassToDraw.SelectedItem = pickedClass;
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
		
		foreach (var className in classes)
		{
            StudentClass newClass = new StudentClass(className);
            foreach (var student in allStudents)
			{
                if (student.Class == className)
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
		foreach(var sc in studentClass)
		{
			if(pickedClass == sc.ClassName)
			{
				Label label = new Label();
				label.Text = "Student list of class: " + sc.ClassName;
				ClassList.Children.Add(label);
				foreach(var student in sc.Students)
				{
					ClassList.Children.Add(student);
				}
			}
		}
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
		Debug.WriteLine("Drawing student");
		foreach (var sc in studentClass)
		{
			if (pickedClass == sc.ClassName)
			{
				Debug.WriteLine("Picked class: " + pickedClass);
				Student randomPerson = new Student();

				bool roundsToDrawHigher = allStudents.All(student => student.RoundsToDraw > 0);
                bool allStudentsAreAbsent = !allStudents.Any(student => student.Class == pickedClass && student.IsPresent);
				Debug.WriteLine("TEST: " + allStudentsAreAbsent);

				if (roundsToDrawHigher)
				{
					Debug.WriteLine("All students have rounds to draw protection.");
					await DisplayAlert("Error", "All students have rounds to draw protection.", "OK");
					return;
				}

				if (allStudentsAreAbsent)
				{
					Debug.WriteLine("All students are absent.");
					await DisplayAlert("Error", "All students are absent.", "OK");
					return;
				}

				var random = new Random();
				int index = 0;
				bool drawNext = true;
				int maxDraws = 0;

				while (drawNext)
				{
					index = random.Next(sc.Students.Count);
					randomPerson = sc.Students[index];
					if (randomPerson.RoundsToDraw == 0 && randomPerson.IsPresent && randomPerson.Number != luckyNumber)
					{
						drawNext = false;
					}
					else
					{
						Debug.WriteLine("Drawed student has lucky number or is absent or has rounds to draw protection.");
					}

					if (maxDraws > 10)
					{
						await DisplayAlert("Error", "Cannot draw student. All students have rounds to draw protection.", "OK");
						drawNext = false;
					}

					maxDraws++;
				}

				foreach (var s in sc.Students)
				{
					if (s.RoundsToDraw > 0) s.RoundsToDraw--;
				}

				if (maxDraws < 10)
				{
					sc.Students[index].RoundsToDraw = 3;
				}

                UpdateList();
                SaveStudents();

                if (maxDraws < 10)
				{
					await DisplayAlert("Drawed student: ", randomPerson.Name + " " + randomPerson.Surname, "OK");
				}

				
			}
		}
	}

    private void ClassToDraw_SelectedIndexChanged(object sender, EventArgs e)
    {
		Debug.WriteLine("Selected class: " + ClassToDraw.SelectedItem);
		string selectedClass = ClassToDraw.SelectedItem as string;
		pickedClass = selectedClass;
		UpdateList();
        Debug.WriteLine("Drawing lucky number");
        int highestNumber = allStudents.Where(student => student.Class == pickedClass).Count();
        int randomNumber = new Random().Next(1, highestNumber + 1);
        luckyNumber = randomNumber;
        LuckyNumberLabel.Text = "Lucky number: " + luckyNumber;
    }

	private void DrawNumberButton_Clicked(object sender, EventArgs e)
	{
		Debug.WriteLine("Drawing lucky number");
        int highestNumber = allStudents.Where(student => student.Class == pickedClass).Count();
		int randomNumber = new Random().Next(1, highestNumber + 1);
        luckyNumber = randomNumber;
		LuckyNumberLabel.Text = "Lucky number: " + luckyNumber;
    }

	private void ClearRoundProtectionButton_Clicked(object sender, EventArgs e)
	{
		Debug.WriteLine("Clearing round protection for class: " + pickedClass);
		studentClass.Find(sc => sc.ClassName == pickedClass).Students.ForEach(student => student.RoundsToDraw = 0);
		SaveStudents();
	}
}