using Xunit;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SqlClient;

namespace ToDoListSql
{
  public class ToDoTest : IDisposable
  {
    public ToDoTest()
    {
      DBConfiguration.ConnectionString = "Data Source=(localdb)\\mssqllocaldb;Initial Catalog=todo_test;Integrated Security=SSPI;";
    }

    [Fact]
    public void Test_DatabaseEmptyAtFirst()
    {
      //Arrange, Act
      int result = Task.GetAll().Count;

      //Assert
      Assert.Equal(0, result);
    }
    [Fact]
   public void Test_EqualOverrideTrueForSameDescription()
   {
     //Arrange, Act
     Task firstTask = new Task("Mow the lawn", 1, "01-01-2017");
     Task secondTask = new Task("Mow the lawn", 1, "01-01-2017");

     //Assert
     Assert.Equal(firstTask, secondTask);
   }

   [Fact]
   public void Test_Save()
   {
     //Arrange
     Task testTask = new Task("Mow the lawn", 1, "01-01-2017");
     testTask.Save();

     //Act
     List<Task> result = Task.GetAll();
     List<Task> testList = new List<Task>{testTask};

     //Assert
     Assert.Equal(testList, result);
   }

   [Fact]
   public void Test_SaveAssignsIdToObject()
   {
     //Arrange
     Task testTask = new Task("Mow the lawn", 1, "01-01-2017");
     testTask.Save();

     //Act
     Task savedTask = Task.GetAll()[0];

     int result = savedTask.GetId();
     int testId = testTask.GetId();

     //Assert
     Assert.Equal(testId, result);
   }

   [Fact]
   public void Test_FindFindsTaskInDatabase()
   {
     //Arrange
     Task testTask = new Task("Mow the lawn", 1, "01-01-2017");
     testTask.Save();

     //Act
     Task foundTask = Task.Find(testTask.GetId());

     //Assert
     Assert.Equal(testTask, foundTask);
   }

   [Fact]
    public void Test_GetTasks_RetrievesAllTasksWithCategory()
    {
      Category testCategory = new Category("Household chores");
      testCategory.Save();

      Task firstTask = new Task("Mow the lawn", testCategory.GetId(), "01-01-2017");
      firstTask.Save();
      Task secondTask = new Task("Do the dishes", testCategory.GetId(), "01-01-2017");
      secondTask.Save();


      List<Task> testTaskList = new List<Task> {firstTask, secondTask};
      List<Task> resultTaskList = testCategory.GetTasks();

      Assert.Equal(testTaskList, resultTaskList);
    }

    [Fact]
    public void Test_Save_SaveDateTimeAsProperty()
    {
        //Arrange
        Task testTask = new Task("Mow the lawn", 1, "01-01-2017");
        testTask.Save();
        string testString = "01-01-2017";

        //Assert
        Assert.Equal(testTask.GetDueDate(), testString);
    }
    [Fact]
    public void Test_GetTasks_OrdersAllTasksByDate()
    {
        Category testCategory = new Category("Household chores");
        testCategory.Save();

        Task firstTask = new Task("Mow the pool", testCategory.GetId(), "05-02-2017");
        firstTask.Save();
        Task secondTask = new Task("Clean the lawn", testCategory.GetId(), "03-15-2017");
        secondTask.Save();
        Task thirdTask = new Task("Walk the Cat", testCategory.GetId(), "04-15-2017");
        thirdTask.Save();

        List<Task> testTaskList = new List<Task> {secondTask, thirdTask, firstTask};
        List<Task> resultTaskList = Task.GetAll();

        foreach (Task task in testTaskList)
        {
            Console.WriteLine("TEST: " + task.GetDescription());
        }

        foreach (Task task in resultTaskList)
        {
            Console.WriteLine("RESULT: " + task.GetDescription());
        }


        Assert.Equal(testTaskList, resultTaskList);
    }

    public void Dispose()
    {
        Task.DeleteAll();
    }
  }
}
