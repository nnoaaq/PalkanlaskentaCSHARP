using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
class FinalPaymentSystem
{
    static void Main()
    {
        List<Employee> Employees = new List<Employee>();
        PayrollSystem payrollSystem = new PayrollSystem();
        int Id = 1; // Aloitus ID
        string fileName = "employees.csv"; // "Tietokanta"
        string[] CorrectMainMenuSelections = {"1","2","3","4","0"};
        while (true)
        {
            Console.Write("Palkanlaskenta\n==============\n(1) Lisää työntekijöitä\n(2) Kirjoita työntekijät tiedostoon\n(3) Lue työntekijät tiedostosta\n(4) Tulosta palkkalaskelma\n(0) Lopeta\nValitse toiminto: ");
            string MainMenuSelection = Console.ReadLine() ?? "";
            while (string.IsNullOrEmpty(MainMenuSelection) || !CorrectMainMenuSelections.Contains(MainMenuSelection))
            {
                Console.WriteLine("Anna kelvollinen valinta");
                Console.Write("Valitse toiminto:");
                MainMenuSelection = Console.ReadLine() ?? "";
            }
            switch (MainMenuSelection)
            {
                case "0":
                    return;
                case "1":
                    bool NewEmployeeMenuOpen = true;
                    Console.WriteLine();
                    while (NewEmployeeMenuOpen)
                    {
                    Console.Write("Palkkatyyppi\n------------\n(1) Kuukausi\n(2) Tunti\n(3) Komissio\n(0) Lopeta\nAnna palkkatyyppi: ");
                    string NewEmployeeSelection = Console.ReadLine() ?? "";
                        switch (NewEmployeeSelection)
                        {
                            case "0":
                                NewEmployeeMenuOpen = false;
                                break;
                            case "1":
                                Console.Write("Anna työntekijän nimi: ");
                                string MonthlyEmployeeName = Console.ReadLine() ?? "";
                                Console.Write("Anna kuukausipalkka: ");
                                int MonthlyEmployeeSalary = Convert.ToInt32(Console.ReadLine());
                                Employee monthlyEmployee = new MonthlyEmployee(Id,MonthlyEmployeeName,MonthlyEmployeeSalary);
                                Employees.Add(monthlyEmployee);
                                Id++;
                                break;
                            case "2":
                                Console.Write("Anna työntekijän nimi: ");
                                string HourlyEmployeeName = Console.ReadLine() ?? "";
                                Employee hourlyEmployee = new HourlyEmployee(Id,HourlyEmployeeName);
                                hourlyEmployee.AskSalary();
                                Employees.Add(hourlyEmployee);
                                Id++;
                                break;
                            case "3":
                                Console.Write("Anna työntekijän nimi: ");
                                string CommissionEmployeeName = Console.ReadLine() ?? "";
                                Console.Write("Anna kuukausipalkka: ");
                                int CommissionEmployeeSalary = Convert.ToInt32(Console.ReadLine());
                                Employee commissionEmployee = new CommissionEmployee(Id,CommissionEmployeeName,CommissionEmployeeSalary);
                                commissionEmployee.AskSalary();
                                Employees.Add(commissionEmployee);
                                Id++;
                                break;
                        }
                    }
                    break;
                case "2":
                    using(StreamWriter sw = new StreamWriter(fileName))
                    {
                        foreach(Employee employee in Employees)
                        {
                            sw.WriteLine(employee.ConvertToCSV());
                        }
                        sw.Close();
                    }
                    Console.WriteLine($"\n{Employees.Count()} työntekijä(ä) lisätty tiedostoon {fileName}");
                    break;
                case "3":
                    Employees.Clear();
                    string[] DataLines = File.ReadAllLines(fileName);
                    foreach(string DataLine in DataLines)
                    {
                        string[] SplittedData = DataLine.Split(','); // Aina sama muoto, ID, NIMI,TYYPPI,KK-PALKKA,TUNNIT,TUNTIPALKKA,KOMISSIO
                        switch (SplittedData[2])
                        {
                            case "M":
                                Employee monthlyEmployee = new MonthlyEmployee(Convert.ToInt32(SplittedData[0]),SplittedData[1],Convert.ToInt32(SplittedData[3]));
                                Employees.Add(monthlyEmployee);
                                break;
                            case "H":
                                HourlyEmployee hourlyEmployee = new HourlyEmployee(Convert.ToInt32(SplittedData[0]),SplittedData[1]); 
                                hourlyEmployee.HoursWorked = Convert.ToInt32(SplittedData[4]);
                                hourlyEmployee.HourRate = Convert.ToInt32(SplittedData[5]);
                                Employees.Add(hourlyEmployee);
                                break;
                            case "C":
                                CommissionEmployee commissionEmployee = new CommissionEmployee(Convert.ToInt32(SplittedData[0]),SplittedData[1],Convert.ToInt32(SplittedData[3]));
                                commissionEmployee.Commission = Convert.ToInt32(SplittedData[6]);
                                Employees.Add(commissionEmployee);
                                break;
                        }
                    }
                    Console.Write($"\n{Employees.Count()} työntekijä(ä) luettu tiedostosta {fileName}\n");
                    break;
                case "4":
                    payrollSystem.CreatePayroll(Employees);
                    break;
            }
        }
    }
}
abstract class Employee
{
    public int Id;
    public string Name;
    public Employee(int Id, string Name)
    {
        this.Id = Id;
        this.Name = Name;
    }
    public abstract void AskSalary();
    public abstract int CalculateSalary();
    public abstract string ConvertToCSV();
}
class MonthlyEmployee : Employee
{
    public int MonthlySalary;
    public MonthlyEmployee(int Id, string Name, int Salary) : base(Id, Name)
    {
        this.MonthlySalary=Salary;
    }  
    public override void AskSalary(){}
    public override int CalculateSalary()
    {
        return this.MonthlySalary;
    }
    public override string ConvertToCSV()
    {
        return $"{Id},{Name},M,{MonthlySalary},,,";
    }
}
class HourlyEmployee : Employee
{
    public int HoursWorked;
    public int HourRate;
    public HourlyEmployee(int Id, string Name):base(Id,Name){}
    public override void AskSalary()
    {
        Console.Write("Anna tuntipalkka: ");
        this.HourRate = Convert.ToInt32(Console.ReadLine());
        Console.Write("Anna tehdyt tunnit: ");
        this.HoursWorked = Convert.ToInt32(Console.ReadLine());
    }
    public override int CalculateSalary()
    {
        return this.HourRate * this.HoursWorked;
    }
    public override string ConvertToCSV()
    {
        return $"{Id},{Name},H,,{HoursWorked},{HourRate},";
    }
}
class CommissionEmployee : MonthlyEmployee
{
    public int Commission;
    public CommissionEmployee(int Id, string Name, int Salary):base(Id,Name,Salary){}
    public override void AskSalary()
    {
        Console.Write("Anna komissio: ");
        this.Commission = Convert.ToInt32(Console.ReadLine());
    }
    public override int CalculateSalary()
    {
        return this.MonthlySalary +this.Commission;
    }
    public override string ConvertToCSV()
    {
        return $"{Id},{Name},C,{MonthlySalary},,,{Commission}";
    }
}
class PayrollSystem
{
    public void CreatePayroll(List<Employee> Employees)
    {
        foreach(Employee employee in Employees)
        {
            Console.WriteLine($"\nPalkkalaskelma\n==============\nHenkilölle: {employee.Id} - {employee.Name}\n- Maksetaan: {employee.CalculateSalary()}\n");
        }   
    }
}