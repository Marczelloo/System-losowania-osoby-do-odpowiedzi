using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System_losowania_osoby_do_odpowiedzi.Controls;

public partial class Student : ContentView
{
    protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        if(propertyName == IsPresentProperty.PropertyName)
		{
            IsPresentDisplay = IsPresent ? "Present" : "Absent";
        };

		base.OnPropertyChanged(propertyName);
    }
    public Student()
	{
		InitializeComponent();

		Number = 0;
		Name = "Default Name";
		Surname = "Default Surname";
		Class = "Default Class";
		IsPresent = true;
		RoundsToDraw = 0;

	}

	public Student(int Number, string Name, string Surname, string CLass, bool Present, int RoundsToDraw)
	{ 
		InitializeComponent();
	
		this.Number = Number;
	    this.Name = Name;
	    this.Surname = Surname;
	    this.Class = CLass;
	    this.IsPresent = Present;
	    this.RoundsToDraw = RoundsToDraw;
		this.IsPresentDisplay = IsPresent ? "Present" : "Absent";

		Debug.WriteLine("Student created: " + Name + " " + Surname + " " + CLass + " " + Present + " " + RoundsToDraw);
	}

	public static readonly BindableProperty NumberProperty =
		BindableProperty.Create(nameof(Number), typeof(int), typeof(Student), default(int), BindingMode.TwoWay);

	public int Number
	{
		get => (int)GetValue(NumberProperty);
		set => SetValue(NumberProperty, value);
	}

	public static readonly BindableProperty IsPresentDisplayProperty =
		BindableProperty.Create(nameof(IsPresentDisplay), typeof(string), typeof(Student), default(string), BindingMode.TwoWay);
	public string IsPresentDisplay
	{
        get
		{
			return IsPresent ? "Present" : "Absent";
		}
        set => SetValue(IsPresentDisplayProperty, value);
    }

	public static readonly BindableProperty NameProperty = 
		BindableProperty.Create(nameof(Name), typeof(string), typeof(Student), default(string), BindingMode.TwoWay);

	public string Name
	{
        get => (string)GetValue(NameProperty);
        set => SetValue(NameProperty, value);
    }

	public static readonly BindableProperty SurnameProperty = 
        BindableProperty.Create(nameof(Surname), typeof(string), typeof(Student), default(string), BindingMode.TwoWay);	

	public string Surname
	{
        get => (string)GetValue(SurnameProperty);
        set => SetValue(SurnameProperty, value);
    }

	public static readonly BindableProperty ClassProperty = 
		        BindableProperty.Create(nameof(Class), typeof(string), typeof(Student), default(string), BindingMode.TwoWay);

	public string Class
	{
        get => (string)GetValue(ClassProperty);
        set => SetValue(ClassProperty, value);
    }

	public static readonly BindableProperty IsPresentProperty = 
		BindableProperty.Create(nameof(IsPresent), typeof(bool), typeof(Student), default(bool), BindingMode.TwoWay);

	public bool IsPresent
	{
        get => (bool)GetValue(IsPresentProperty);
        set => SetValue(IsPresentProperty, value);
    }

	public static readonly BindableProperty RoundsToDrawProperty =
        BindableProperty.Create(nameof(RoundsToDraw), typeof(int), typeof(Student), default(int), BindingMode.TwoWay);

	public int RoundsToDraw
	{
        get => (int)GetValue(RoundsToDrawProperty);
        set => SetValue(RoundsToDrawProperty, value);
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
		IsPresent = !IsPresent;
    }
}