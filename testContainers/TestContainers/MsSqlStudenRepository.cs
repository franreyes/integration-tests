using Microsoft.Data.SqlClient;

namespace TestContainers;

public class MsSqlStudenRepository
{
    private readonly string _connection;

    public MsSqlStudenRepository(string connection)
    {
        _connection = connection;
    }

    public List<Student> GetAll()
    {
        using var connection = new SqlConnection(_connection);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "Select id, name, payment from students;";
        var students = new List<Student>();
        using var reader = command.ExecuteReader();
        if (reader.HasRows)
        {
            while (reader.Read())
            {
                var id = reader.GetInt32(0);
                var name = reader.GetString(1);
                var payment = reader.GetBoolean(2);
                students.Add(new Student(id, name, payment));
            }
        }

        return students;
    }

}

public record Student(int Id, string Name, bool Payment);