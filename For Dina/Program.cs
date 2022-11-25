using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.WebSockets;

/* To Do:
 * 1. Возможность отказаться от проекта(Тимлид либо переназначает, лиюо удаляет задачу)
 * 2. Функция которая меняет статус проекта
 * 3. Функцкия, меняющая статус задачи
 * 4. Функция отслеживающая дедлайн
 * 5. Функция отправляющая отчет
 * 5. Возможность тимлида утвердить отчет или вернуть на доработку
 * 
 * Don't mention it:
 * 1. Функция периодичности отчета
 * 1.1. Каждый день
 * 1.2 Раз в неделю
 * 1.3 Раз в месяц
 * 1.4 Выбрать день
 * 
 * Current progress:
 *  1. Список файлов с задачами находятся в files.txt
 *  2. Каждая из t1.txt содержит в себе суть задачи и дедлайн
 *  3. Тимлид может дать задачу работнику
 *  4. У работника есть функция принять задачу
 *  5. Частично реализованы классы: проект, задачи, отчета, тимлида, работника
 *  6. Работники могут обмениваться задачами
*/

internal class Project
{
    //private List<string> about_project;
    private string about_project;
    private DateTime deadline;
    private string customer;
    private string teamlead;
    private List<Task> tasks;
    private string status = "Проект";

    public List<Task> Tasks
    {
        get { return tasks; }
    }

    internal Project(string path_to_about_project, DateTime deadline, string customer, string teamlead, string path_to_tasks)
    {
        tasks = new List<Task>();
        FromFileToString(path_to_about_project, out about_project);
        PullingToTasks(path_to_tasks, tasks);
        this.deadline = deadline;
        this.customer = customer;
        this.teamlead = teamlead;
    }

    public void Printing()
    {
        for (int i = 0; i < tasks.Count; i++)
        {
            tasks[i].Put();
        }
    }

    public void Printing_1()
    {
        if (about_project != null)
        {
            Console.WriteLine(about_project);
        }
    }

    private void FromFileToString(string path_to_about_project, out string about_project)
    {
        StreamReader sr = new StreamReader(path_to_about_project);
        about_project = sr.ReadToEnd();
        sr.Close();
    }

    private void PullingToTasks(string path_to_about_tasks, List<Task> tasks)
    {
        StreamReader sr = new StreamReader(path_to_about_tasks);
        List<string> list_of_tasks_files = new List<string>();

        string line = sr.ReadLine();
        while (line != null)
        {
            list_of_tasks_files.Add(line);
            line = sr.ReadLine();
        }
        sr.Close();

        for (int i = 0; i < list_of_tasks_files.Count(); i++)
        {
            CreatingTask(list_of_tasks_files[i], this.tasks);
        }
    }

    private void CreatingTask(string list_of_tasks_files, List<Task> tasks)
    {
        StreamReader sr = new StreamReader(list_of_tasks_files);
        //Console.WriteLine(list_of_tasks_files);
        List<string> lines = sr.ReadToEnd().Split(';').ToList();

        Task task_example = new Task(lines[0], DateTime.Parse(lines[1]), lines[2]);

        tasks.Add(task_example);
        sr.Close();
    }
}

internal class Task
{
    private string about_task;
    private DateTime deadline;
    private string customer;
    private string executor;
    private string status;
    private TaskReport report;

    internal Task(string about_task, DateTime deadline, string customer)
    {
        this.about_task = about_task;
        this.deadline = deadline;
        this.customer = customer;
    }

    internal Task()
    {

    }

    internal string AboutTask
    {
        get { return about_task; }
    }

    internal string Executor
    {
        get { return executor; }
        set { executor = value; }
    }

    internal string Status
    {
        get { return status; }
        set { status = value; }
    }

    internal TaskReport Report
    {
        get { return report; }
        set { report = value; }
    }

    internal DateTime Deadline
    {
        get { return deadline; }
    }

    public void Put()
    {
        Console.WriteLine(about_task + " " + deadline + " " + customer);
    }
}

internal class TeamLead
{
    private string name;
    internal TeamLead(string name)
    {
        this.name = name;
    }

    internal void GiveTask(List<Task> tasks, Developer dev)
    {
        int choice;
        Console.WriteLine("Нынешние задачи:");
        int i = 0;
        for (i = 0; i < tasks.Count; i++)
        {
            Console.Write(i + ". ");
            tasks[i].Put();
        }
        Console.Write($"Напишите, какую задачу хотите дать работнику {dev.Name}: ");
        choice = int.Parse(Console.ReadLine());
        if (choice <= i && choice >= 0)
        {
            dev.GetTask(tasks[choice]);
            tasks.Remove(tasks[choice]);
        }
    }
}

internal class Report
{
    private string report;
    private DateTime deadline;
    private string executor;
}

internal class Developer
{
    private string name;
    public Task DevTask { get; set; }

    public string Name
    {
        get
        {
            return name;
        }
        set
        {
            name = value;
        }
    }

    internal Developer(string name)
    {
        DevTask = new Task();
        this.name = name;
    }

    internal TaskReport TaskIsDone()
    {
        TaskReport report = new TaskReport(Console.ReadLine(), name);
        List<object> returning = new List<object>();
        returning.Add(DevTask);
        returning.Add(report);

        return report;
    }

    internal void GetTask(Task task)
    {
        DevTask = task;
    }

    internal void SwitchTask(Developer developer_switch)
    {
        Task new_task = DevTask;
        DevTask = developer_switch.DevTask;
        developer_switch.DevTask = new_task;
        DevTask.Executor = name;
        developer_switch.DevTask.Executor = developer_switch.name;
    }

    internal void Currenttask()
    {
        Console.WriteLine(DevTask.AboutTask + " " + DevTask.Deadline.ToString());
    }

}

internal class TaskReport
{
    private string report;
    DateTime work_is_done { get; }
    private string executor;

    internal TaskReport(string report, string executor)
    {
        this.report = report;
        work_is_done = DateTime.Now;
        this.executor = executor;
    }
}


// TeamLead (1) - 1
// SystemMid(2), DevMid(2) - 1, 1
// SystemJun(3), DevJun(3) - 3, 4


// Задачи по проекту (Файл с названиями файлов задач).
// Задача проекта (Файл - разделение на определенные символы)

// 

internal class program
{
    static void Main()
    {
        Project project = new Project("a1.txt", DateTime.Parse("26/07/2023"), "Ilnaz", "Bulat", "files.txt");
        TeamLead teamlead = new TeamLead("Ilnaz");
        Developer developer = new Developer("Bulat");
        Developer developer1 = new Developer("Denis");
        //while (project.Tasks != null)
        //{
        //    teamlead.GiveTask(project.Tasks, developer);
        //    teamlead.GiveTask(project.Tasks, developer1);
        //    Console.WriteLine("Задача " + developer.Name + "а: ");
        //    Console.WriteLine("Задача " + developer1.Name + "а: ");
        //    developer.Currenttask();
        //    developer1.Currenttask();
        //    developer.SwitchTask(developer1);
        //    developer.Currenttask();
        //    developer1.Currenttask();
        //}
        teamlead.GiveTask(project.Tasks, developer);
        teamlead.GiveTask(project.Tasks, developer1);
        Console.ReadKey();
        Console.WriteLine("Задача " + developer.Name + "а: ");
        developer.Currenttask();
        Console.WriteLine("Задача " + developer1.Name + "а: ");
        developer1.Currenttask();
        Console.ReadKey();
        developer.SwitchTask(developer1);
        Console.WriteLine("Задача " + developer.Name + "а: ");
        developer.Currenttask();
        Console.WriteLine("Задача " + developer1.Name + "а: ");
        developer1.Currenttask();
    }
}
