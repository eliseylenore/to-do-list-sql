using System.Collections.Generic;
using System.Data.SqlClient;
using System;

namespace ToDoListSql
{
    public class Task
   {
     private int _id;
     private string _description;
     private string _dueDate;
     private int _categoryId;


     public Task(string Description, int CategoryId, string DueDate, int Id = 0)
     {
       _id = Id;
       _description = Description;
       _dueDate = DueDate;
       _categoryId = CategoryId;
     }

     public override bool Equals(System.Object otherTask)
     {
         if (!(otherTask is Task))
         {
           return false;
         }
         else
         {
           Task newTask = (Task) otherTask;
           bool idEquality = this.GetId() == newTask.GetId();
           bool descriptionEquality = this.GetDescription() == newTask.GetDescription();
           bool categoryEquality = this.GetCategoryId() == newTask.GetCategoryId();
           return (idEquality && descriptionEquality && categoryEquality);
         }
     }

     public override int GetHashCode()
     {
         return this.GetDescription().GetHashCode();
    }
    public int GetId()
    {
      return _id;
    }
    public int GetCategoryId()
    {
      return _categoryId;
    }
    public string GetDescription()
    {
      return _description;
    }
    public void SetDescription(string newDescription)
    {
      _description = newDescription;
    }
    public string GetDueDate()
    {
        return _dueDate;
    }

  public static List<Task> GetAll()
  {
    List<Task> AllTasks = new List<Task>{};

    SqlConnection conn = DB.Connection();
    conn.Open();

    SqlCommand cmd = new SqlCommand("SELECT * FROM tasks ORDER BY due_date;", conn);
    SqlDataReader rdr = cmd.ExecuteReader();

    while(rdr.Read())
    {
      int taskId = rdr.GetInt32(0);
      string taskDescription = rdr.GetString(1);
      string taskDueDate = rdr.GetDateTime(3).ToString("mm-dd-yyyy");
      int taskCategoryId = rdr.GetInt32(2);
      Task newTask = new Task(taskDescription, taskCategoryId, taskDueDate, taskId);
      AllTasks.Add(newTask);
    }
    if (rdr != null)
    {
      rdr.Close();
    }
    if (conn != null)
    {
      conn.Close();
    }
    return AllTasks;
  }
  public void Save()
  {
    SqlConnection conn = DB.Connection();
    conn.Open();

    SqlCommand cmd = new SqlCommand("INSERT INTO tasks (description, category_id, due_date) OUTPUT INSERTED.id VALUES (@TaskDescription, @TaskCategoryId, @TaskDueDate);", conn);

    SqlParameter descriptionParameter = new SqlParameter();
    descriptionParameter.ParameterName = "@TaskDescription";
    descriptionParameter.Value = this.GetDescription();

    SqlParameter dueDateParameter = new SqlParameter();
    dueDateParameter.ParameterName = "@TaskDueDate";
    dueDateParameter.Value = this.GetDueDate();

    SqlParameter categoryIdParameter = new SqlParameter();
    categoryIdParameter.ParameterName = "@TaskCategoryId";
    categoryIdParameter.Value = this.GetCategoryId();

    cmd.Parameters.Add(descriptionParameter);
    cmd.Parameters.Add(dueDateParameter);
    cmd.Parameters.Add(categoryIdParameter);

    SqlDataReader rdr = cmd.ExecuteReader();

    while(rdr.Read())
    {
      this._id = rdr.GetInt32(0);
    }
    if (rdr != null)
    {
      rdr.Close();
    }
    if (conn != null)
    {
      conn.Close();
    }
  }

  public static void DeleteAll()
    {
      SqlConnection conn = DB.Connection();
      conn.Open();
      SqlCommand cmd = new SqlCommand("DELETE FROM tasks;", conn);
      cmd.ExecuteNonQuery();
      conn.Close();
    }

  public static Task Find(int id)
  {
    SqlConnection conn = DB.Connection();
    conn.Open();

    SqlCommand cmd = new SqlCommand("SELECT * FROM tasks WHERE id = @TaskId;", conn);
    SqlParameter taskIdParameter = new SqlParameter();
    taskIdParameter.ParameterName = "@TaskId";
    taskIdParameter.Value = id.ToString();
    cmd.Parameters.Add(taskIdParameter);
    SqlDataReader rdr = cmd.ExecuteReader();

    int foundTaskId = 0;
    string foundTaskDescription = null;
    string foundTaskDueDate = null;
    int foundTaskCategoryId = 0;

    while(rdr.Read())
    {
      foundTaskId = rdr.GetInt32(0);
      foundTaskDescription = rdr.GetString(1);
      foundTaskDueDate = rdr.GetDateTime(3).ToString("mm-dd-yyyy");
      foundTaskCategoryId = rdr.GetInt32(2);
    }
    Task foundTask = new Task(foundTaskDescription, foundTaskCategoryId, foundTaskDueDate, foundTaskId);

    if (rdr != null)
    {
      rdr.Close();
    }
    if (conn != null)
    {
      conn.Close();
    }
    return foundTask;
  }
  }
}
