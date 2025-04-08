// See https://aka.ms/new-console-template for more information

//Console.WriteLine("Hello, World!");
//Car.cs - Model
using System.Text.Json;
using System.Xml.Serialization;
using System.IO;

public class Car
{
    public string Name { get; set; }
    public string Manufacturer { get; set; }

    public int Year  {get; set; }
    public string Drivetrain { get; set; } // RWD and AWD
    public string ImagePath { get; set; }

    public override string ToString()
    {
        return $"{Name} ({{Year}}) - {{Manufacturer}} [{{Drivetrain}}]";
    }
}

//DataSerializer - which handles Save/load on products



public static class DataSerializer
{
    public static void SaveToJson(List<Car> cars, string filePath)
    {
        string json = JsonSerializer.Serialize(cars);
        File.WriteAllText(filePath, json);
    }

    public static List<Car> LoadFromJson(string filePath)
    {
        string json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Car>>(json) ?? new List<Car>();
    }
    
    public static void SaveToXml(List<Car> cars, string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Car>));
        using var stream = File.Create(filePath);
        serializer.Serialize(stream, cars);
    }

    public static List<Car> LoadFromXml(string filePath)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(List<Car>));
        using var stream = File.OpenRead(filePath);
        return (List<Car>)serializer.Deserialize(stream);
    }
}

// CarController
public class CarController
{
    public List<Car> Cars { get; set; } = new List<Car>();

    public void AddCar(Car car) => Cars.Add(car);

    public void RemoveCar(int index)
    {
        if (index >= 0 && index < Cars.Count)
            Cars.RemoveAt(index);
    }

    public void EditCar(int index, Car updatedCar)
    {
        if (index >= 0 && index < Cars.Count)
            Cars[index] = updatedCar;
    }
}

// The View 
public static class ConsoleView
{
    public static void ShowMainMenu()
    {
        Console.WriteLine("\n===== CAR COLLECTION APP =====");
        Console.WriteLine("1. View Cars");
        Console.WriteLine("2. Add Car");
        Console.WriteLine("3. Edit Car");
        Console.WriteLine("4. Remove Car");
        Console.WriteLine("5. Save Cars");
        Console.WriteLine("6. Load Cars");
        Console.WriteLine("7. About");
        Console.WriteLine("0. Exit");
    }

    public static Car GetCarFromUser()
    {
        Console.Write("Enter Car Name: ");
        string name = Console.ReadLine();

        Console.Write("Enter Manufacturer: ");
        string make = Console.ReadLine();

        Console.Write("Enter Year: ");
        int year = int.Parse(Console.ReadLine() ?? "0");

        Console.Write("Enter Drivetrain (RWD, AWD, etc.): ");
        string drivetrain = Console.ReadLine();

        Console.Write("Enter Image Path (optional): ");
        string path = Console.ReadLine();

        return new Car { Name = name, Manufacturer = make, Year = year, Drivetrain = drivetrain, ImagePath = path };
    }

    public static void ShowCars(List<Car> cars)
    {
        if (cars.Count == 0)
        {
            Console.WriteLine("No cars available.");
            return;
        }

        for (int i = 0; i < cars.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {cars[i]}");
        }
    }

    public static void ShowAbout()
    {
        Console.WriteLine("\nThis app was built by Joshua to manage cool cars like:");
        Console.WriteLine("- Lexus LFA\n- Lamborghini Revuelto\n- Nissan GT-R R34 Skyline");
    }
}
// Program.cs 
class Program
{
    static void Main(string[] args)
    {
        var controller = new CarController();

        bool running = true;
        while (running)
        {
            ConsoleView.ShowMainMenu();
            Console.Write("Select an option: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    ConsoleView.ShowCars(controller.Cars);
                    break;
                case "2":
                    controller.AddCar(ConsoleView.GetCarFromUser());
                    break;
                case "3":
                    ConsoleView.ShowCars(controller.Cars);
                    Console.Write("Select car to edit: ");
                    int editIndex = int.Parse(Console.ReadLine() ?? "0") - 1;
                    controller.EditCar(editIndex, ConsoleView.GetCarFromUser());
                    break;
                case "4":
                    ConsoleView.ShowCars(controller.Cars);
                    Console.Write("Select car to remove: ");
                    int removeIndex = int.Parse(Console.ReadLine() ?? "0") - 1;
                    controller.RemoveCar(removeIndex);
                    break;
                case "5":
                    Console.Write("Save as JSON or XML? (j/x): ");
                    string type = Console.ReadLine()?.ToLower();
                    Console.Write("Enter file path: ");
                    string savePath = Console.ReadLine();
                    try
                    {
                        if (type == "j")
                            DataSerializer.SaveToJson(controller.Cars, savePath);
                        else if (type == "x")
                            DataSerializer.SaveToXml(controller.Cars, savePath);
                        Console.WriteLine("Saved successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving: {ex.Message}");
                    }
                    break;
                case "6":
                    Console.Write("Load JSON or XML? (j/x): ");
                    string loadType = Console.ReadLine()?.ToLower();
                    Console.Write("Enter file path: ");
                    string loadPath = Console.ReadLine();
                    try
                    {
                        if (loadType == "j")
                            controller.Cars = DataSerializer.LoadFromJson(loadPath);
                        else if (loadType == "x")
                            controller.Cars = DataSerializer.LoadFromXml(loadPath);
                        Console.WriteLine("Loaded successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error loading: {ex.Message}");
                    }
                    break;
                case "7":
                    ConsoleView.ShowAbout();
                    break;
                case "0":
                    Console.Write("Are you sure you want to quit? (y/n): ");
                    running = Console.ReadLine()?.ToLower() != "y" ? true : false;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }
        }
        Console.WriteLine("Goodbye!");
    }
}

