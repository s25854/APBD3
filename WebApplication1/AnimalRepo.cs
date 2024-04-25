using System.Data.SqlClient;

namespace WebApplication1
{
    public interface IAnimalsRepo
    {
        IEnumerable<Animal> GetRepoAnimals();
        int AddAnimal(Animal newAnimal);
        int UpdateAnimal(Animal updatedAnimal);
        int DeleteAnimal(int idAnimal);
    }

    public class AnimalRepo : IAnimalsRepo
    {
        private readonly IConfiguration _configuration;

        public AnimalRepo(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IEnumerable<Animal> GetRepoAnimals()
        {
            return ExecuteQuery<Animal>("SELECT IdAnimal, Name, Description, CATEGORY, AREA FROM Animal ORDER BY Name",
                                reader => new Animal
                                {
                                    IdAnimal = (int)reader["IdAnimal"],
                                    Name = reader["Name"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Category = reader["CATEGORY"].ToString(),
                                    Area = reader["AREA"].ToString()
                                });
}

        public int AddAnimal(Animal newAnimal)
        {
            return ExecuteNonQuery("INSERT INTO Animal (Name, Description, Category, Area) VALUES (@Name, @Description, @Category, @Area)",
                            new SqlParameter("@Name", newAnimal.Name),
                            new SqlParameter("@Description", newAnimal.Description),
                            new SqlParameter("@Category", newAnimal.Category),
                            new SqlParameter("@Area", newAnimal.Area));
        }

        public int UpdateAnimal(Animal updatedAnimal)
        {
            return ExecuteNonQuery("UPDATE Animal SET Name=@Name, Description=@Description, CATEGORY=@Category, AREA=@Area WHERE IdAnimal = @IdAnimal",
                                    new SqlParameter("@IdAnimal", updatedAnimal.IdAnimal),
                                    new SqlParameter("@Name", updatedAnimal.Name),
                                    new SqlParameter("@Description", updatedAnimal.Description),
                                    new SqlParameter("@Category", updatedAnimal.Category),
                                    new SqlParameter("@Area", updatedAnimal.Area));
        }

        public int DeleteAnimal(int idAnimal)
        {
            return ExecuteNonQuery("DELETE FROM Animal WHERE IdAnimal = @IdAnimal",
                                    new SqlParameter("@IdAnimal", idAnimal));
        }

        private IEnumerable<T> ExecuteQuery<T>(string query, Func<SqlDataReader, T> mapFunction, params SqlParameter[] parameters)
        {
            using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            con.Open();

            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddRange(parameters);

            using var reader = cmd.ExecuteReader();

            var result = new List<T>();
            while (reader.Read())
            {
                result.Add(mapFunction(reader));
            }
            return result;
        }

        private int ExecuteNonQuery(string query, params SqlParameter[] parameters)
        {
            using var con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            con.Open();

            using var cmd = new SqlCommand(query, con);
            cmd.Parameters.AddRange(parameters);

            return cmd.ExecuteNonQuery();
        }
    }
}