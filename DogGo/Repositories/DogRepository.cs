using DogGo.Models;
using DogGo.Utils;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DogGo.Repositories
{
    public class DogRepository : IDogRepository
    {
        private readonly IConfiguration _config;
        public DogRepository(IConfiguration cofig)
        {
            _config = cofig;
        }
        public SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }
        public List<Dog> GetAllDogs()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT d.Id,d.[Name], Breed, OwnerId, Notes, ImageUrl
                        FROM Dog d
                        JOIN Owner o ON d.OwnerId = o.Id
                    ";

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Dog> dogs = new List<Dog>();
                    while (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Notes = ReaderUtils.GetNullableString(reader, "Notes"),
                            ImageUrl = ReaderUtils.GetNullableString(reader, "ImageUrl"),
                            Owner = new Owner()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("OwnerId")),
                            }
                        };

                        dogs.Add(dog);
                    }

                    reader.Close();

                    return dogs;
                }
            }
        }
        public void AddDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    INSERT INTO Dog ([Name], OwnerId, Breed, Notes, ImageUrl)
                    OUTPUT INSERTED.ID
                    VALUES (@name, @ownerId, @breed, @notes, @imageUrl);
                ";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);                    
                    cmd.Parameters.Add(new SqlParameter("@notes", dog.Notes ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@imageUrl", dog.ImageUrl ?? (object)DBNull.Value));
                    int id = (int)cmd.ExecuteScalar();
                    dog.Id = id;
                }
            }
        }
        public Dog GetDogById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id,[Name],  OwnerId, Breed, Notes, ImageUrl
                        FROM Dog
                        WHERE Id = @id
                    ";

                    cmd.Parameters.AddWithValue("@id", id);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        Dog dog = new Dog
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            OwnerId = reader.GetInt32(reader.GetOrdinal("OwnerId")),                           
                            Breed = reader.GetString(reader.GetOrdinal("Breed")),
                            Notes = ReaderUtils.GetNullableString(reader, "Notes"),
                            ImageUrl = ReaderUtils.GetNullableString(reader, "ImageUrl")
                        };

                        reader.Close();
                        return dog;
                    }
                    else
                    {
                        reader.Close();
                        return null;
                    }
                }
            }
        }
        public void UpdateDog(Dog dog)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            UPDATE Dog
                            SET 
                                [Name] = @name, 
                                OwnerId = @ownerId, 
                                Breed = @breed, 
                                Notes = @notes, 
                                ImageUrl = @imageUrl
                            WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@name", dog.Name);
                    cmd.Parameters.AddWithValue("@ownerId", dog.OwnerId);
                    cmd.Parameters.AddWithValue("@breed", dog.Breed);
                    cmd.Parameters.Add(new SqlParameter("@notes", dog.Notes ?? (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@imageUrl", dog.ImageUrl ?? (object)DBNull.Value));
                    cmd.Parameters.AddWithValue("@id", dog.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void DeleteDog(int dogId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                            DELETE FROM Dog
                            WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", dogId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
