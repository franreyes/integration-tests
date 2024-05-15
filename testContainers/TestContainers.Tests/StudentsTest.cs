using Microsoft.Data.SqlClient;
using NUnit.Framework;
using Testcontainers.MsSql;

namespace TestContainers.Tests;

public class StudentsTest 
{
    private MsSqlContainer _msSqlContainer;

    [OneTimeSetUp]
    public Task Initialize()
    {
        _msSqlContainer = new MsSqlBuilder().WithName("Courses").Build();
        return _msSqlContainer.StartAsync();
    }

    [SetUp]
    public void Setup()
    {
        CreateSchemaAndData();
    }

    [OneTimeTearDown]
    public Task Dispose()
    {
       return _msSqlContainer.DisposeAsync().AsTask(); 
    }
    
    [Test]
    public void Retrieve_Students()
    {
        var repository = new MsSqlStudenRepository(_msSqlContainer.GetConnectionString());
        
        var students = repository.GetAll();
        
        Assert.That(students.Count, Is.EqualTo(3));
        Assert.That(students, Has.Member(new Student(1, "Santiago", true)));
    }

    private void CreateSchemaAndData()
    {
        using var connection = new SqlConnection(_msSqlContainer.GetConnectionString());
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"CREATE TABLE students(
                        id int,
                        name varchar(255),
                        payment bit,
                    );
            INSERT INTO students VALUES (1, 'Santiago', 1);
            INSERT INTO students VALUES (2, 'Alejandro', 0);
            INSERT INTO students VALUES (3, 'Sara', 1);";
        command.ExecuteNonQuery();
    }  
}