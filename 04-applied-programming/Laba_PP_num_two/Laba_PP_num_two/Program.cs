using System;
using System.Collections.Generic;

namespace Laba_PP_num_two
{
    // Вспомогательные классы
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Department(string name) { Name = name; }
    }
    public class JobVacancy
    {
        private static int idCounter = 0;
        public int Id { get; private set; }
        public string Title { get; set; }
        public JobVacancy(string title)
        {
            Id = ++idCounter;
            Title = title;
        }
    }
    public class JobTitle
    {
        public string Name { get; set; }
        public JobTitle(string name) { Name = name; }
    }
    public class Person
    {
        public string FullName { get; set; }
        public Person(string fullName) { FullName = fullName; }
    }
    public class Employee
    {
        private static int idCounter = 0;
        public int EmployeeId { get; private set; }
        public Person Person { get; set; }
        public string JobTitle { get; set; }
        public Employee(Person person, string jobTitle)
        {
            EmployeeId = ++idCounter;
            Person = person;
            JobTitle = jobTitle;
        }
    }
    public class Reason
    {
        public string Description { get; set; }
        public Reason(string description) { Description = description; }
    }

    // Интерфейс IStaff
    public interface IStaff2
    {
        int AddJobTitle(JobTitle jobTitle);
    }

    public interface IStaff
    {
        List<JobVacancy> GetJobVacancies();
        List<Employee> GetEmployees();
        List<JobTitle> GetJobTitles();
        int AddJobTitle(JobTitle jobTitle);
        string PrintJobVacancies();
        bool DelJobTitle(int id);
        int OpenJobVacancy(JobVacancy vacancy);
        bool CloseJobVacancy(int id);
        Employee Recruit(JobVacancy vacancy, Person person);
        bool Dismiss(int id, Reason reason);
    }

    // Класс Organization
    public class Organization : IStaff, IStaff2
    {
        private int id;
        protected string name;
        protected string shortName;
        protected string address;
        protected DateTime timeStamp;

        protected List<JobVacancy> jobVacancies = new List<JobVacancy>();
        protected List<JobTitle> jobTitles = new List<JobTitle>();
        protected List<Employee> employees = new List<Employee>();

        public int Id 
        { 
            get => id; 
            private set => id = value; 
        }
        public string Name 
        {
            get => name; 
            protected set => name = value; 
        }
        public string ShortName 
        { 
            get => shortName; 
            protected set => shortName = value; 
        }
        public string Address 
        { 
            get => address; 
            protected set => address = value; 
        }
        public DateTime TimeStamp 
        { 
            get => timeStamp; 
            protected set => timeStamp = value; 
        }
        public Organization()
        {
            Id = new Random().Next(1000, 9999);
            TimeStamp = DateTime.Now;
            Name = "Анонимная организация";
            ShortName = "Неизвестно";
            Address = "Неизвестно";
        }

        public Organization(string name, string shortName, string address)
        {
            Id = new Random().Next(1000, 9999);
            TimeStamp = DateTime.Now;
            Name = name;
            ShortName = shortName;
            Address = address;
        }

        public virtual void PrintInfo()
        {
            Console.WriteLine($"Организация: {Name} ({ShortName})");
            Console.WriteLine($"Адрес: {Address}");
            Console.WriteLine($"Создана: {TimeStamp}");
            Console.WriteLine($"Сотрудников: {employees.Count}, вакансий: {jobVacancies.Count}, должностей: {jobTitles.Count}\n");
        }

        // Реализация интерфейса IStaff
        public virtual List<JobVacancy> GetJobVacancies()
        {
            return new List<JobVacancy>(jobVacancies);
        }
        public virtual List<Employee> GetEmployees()
        {
            return new List<Employee>(employees);
        }
        public virtual List<JobTitle> GetJobTitles()
        {
            return new List<JobTitle>(jobTitles);
        }

        int IStaff.AddJobTitle(JobTitle jobTitle)
        {
            jobTitles.Add(jobTitle);
            return jobTitles.Count - 1;
        }

        int IStaff2.AddJobTitle(JobTitle jobTitle)
        {
            jobTitles.Add(jobTitle);
            return jobTitles.Count - 1;
        }

        public virtual string PrintJobVacancies()
        {
            if (jobVacancies.Count == 0) return "Нет открытых вакансий.";
            string result = "Открытые вакансии:\n";
            foreach (var v in jobVacancies)
                result += $"{v.Id}. {v.Title}\n";
            return result;
        }

        public virtual bool DelJobTitle(int id)
        {
            if (id >= 0 && id < jobTitles.Count)
            {
                jobTitles.RemoveAt(id);
                return true;
            }
            return false;
        }

        public virtual int OpenJobVacancy(JobVacancy vacancy)
        {
            jobVacancies.Add(vacancy);
            return vacancy.Id;
        }

        public virtual bool CloseJobVacancy(int id)
        {
            var vacancy = jobVacancies.Find(v => v.Id == id);
            if (vacancy != null)
            {
                jobVacancies.Remove(vacancy);
                return true;
            }
            return false;
        }

        public virtual Employee Recruit(JobVacancy vacancy, Person person)
        {
            if (jobVacancies.Contains(vacancy))
            {
                var emp = new Employee(person, vacancy.Title);
                employees.Add(emp);
                CloseJobVacancy(vacancy.Id);
                return emp;
            }
            return null;
        }
        public virtual bool Dismiss(int id, Reason reason)
        {
            var emp = employees.Find(e => e.EmployeeId == id);
            if (emp != null)
            {
                employees.Remove(emp);
                Console.WriteLine($"Сотрудник {emp.Person.FullName} (ID: {emp.EmployeeId}) уволен. Причина: {reason.Description}");
                return true;
            }
            return false;
        }

    }

    // Класс Faculty
    public class Faculty : Organization
    {
        protected List<Department> departments = new();

        public Faculty(string name, string shortName, string address) : base(name, shortName, address) { }

        public int AddDepartment(Department dep)
        {
            departments.Add(dep);
            return departments.Count - 1;
        }

        public List<Department> GetDepartments() => new(departments);

        public override void PrintInfo()
        {
            Console.WriteLine($"Факультет: {Name} ({ShortName}), кафедр: {departments.Count}");
            base.PrintInfo();
        }
    }

    // Класс University
    public class University : Organization
    {
        protected List<Faculty> faculties = new();

        public University(string name, string shortName, string address) : base(name, shortName, address) { }

        public int AddFaculty(Faculty f)
        {
            faculties.Add(f);
            return faculties.Count - 1;
        }

        public List<Faculty> GetFaculties() => new(faculties);

        public override void PrintInfo()
        {
            Console.WriteLine($"Университет: {Name} ({ShortName}), факультетов: {faculties.Count}");
            base.PrintInfo();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // --- Задание 1 ---
            Console.WriteLine("---Задание №1. Демонстрация создания объектов---");
            University bstu = new University("Белорусский государственный технологический университет", "БГТУ", "г. Минск, ул. Свердлова, 13а");
            Faculty f1 = new Faculty("Лесохозяйственный факультет", "ЛХФ", "г. Минск");
            Faculty f2 = new Faculty("Факультет информационных технологий", "ФИТ", "г. Минск");
            Console.WriteLine("Объекты успешно созданы.\n");

            //---Задание 2---
            Console.WriteLine("---Задание №2. Демонстрация наследования и свойств---");
            Console.WriteLine($"Название университета: {bstu.Name}");
            Console.WriteLine($"Адресс факультета: {f1.Address}\n");

            bstu.AddFaculty(f1);
            bstu.AddFaculty(f2);

            f1.AddDepartment(new Department("Кафедра ЛХ"));
            f1.AddDepartment(new Department("Кафедра ТиП"));

            Console.WriteLine("Информация об университете:");
            bstu.PrintInfo();
            Console.WriteLine("Информация о факультете:");
            f1.PrintInfo();

            // --- Задание 3 ---
            Console.WriteLine("---Задание №3. Демонстрация реализации интерфейса---");
            IStaff staffManager = bstu;

            var deanTitle = new JobTitle("Декан");
            var profTitle = new JobTitle("Профессор");
            staffManager.AddJobTitle(deanTitle);
            staffManager.AddJobTitle(profTitle);

            var deanVacancy = new JobVacancy(deanTitle.Name);
            var profVacancy = new JobVacancy(profTitle.Name);
            staffManager.OpenJobVacancy(deanVacancy);
            staffManager.OpenJobVacancy(profVacancy);

            Console.WriteLine(staffManager.PrintJobVacancies());

            Person candidate = new Person("Иванов Иван Иванович");
            Employee newDean = staffManager.Recruit(deanVacancy, candidate);

            Console.WriteLine(staffManager.PrintJobVacancies());

            staffManager.Dismiss(newDean.EmployeeId, new Reason("Просто уволили"));
        }
    }
}
