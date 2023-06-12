using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// the below 3 match RoomRepository.cs
using Microsoft.Data.SqlClient;
using Roommates.Models;
using System.Collections.Generic;


namespace Roommates.Repositories
{

    /// <summary>
    ///  This class is responsible for interacting with Chore data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    /// Changed from internal to public
    public class ChoreRepository : BaseRepository
    {
        /// <summary>
        /// Get a list of all Chores in the database
        /// </summary>
        /// passing in the connection string
        public ChoreRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        ///  Get a list of all Chores in the database
        /// </summary>
        public List<Chore> GetAll()
        {
            //  We must "use" the database connection.
            //  Because a database is a shared resource (other applications may be using it too) we must
            //  be careful about how we interact with it. Specifically, we Open() connections when we need to
            //  interact with the database and we Close() them when we're finished.
            //  In C#, a "using" block ensures we correctly disconnect from a resource even if there is an error.
            //  For database connections, this means the connection will be properly closed.
            // "Connection" is inherited from base repository, it's passed from program to each of the repositories
            // program class, connection_string was defined, passed into each of the repositories, on ChoreRepository it takes that connection string and passes it to the base class. Base class then creates computed property called Connection, we use that Connection property from the base class to create our Sql connection to open the tunnel to write and execute our queries
            // additional benefits to using the "using" statements with error handling, if it occurs during execution of the code, "dispose method" ??, controlling access to SQL connection class
            using (SqlConnection conn = Connection)
            {
                // Note, we must Open() the connection, the "using" block doesn't do that for us. Once we close the using block, C# closes it for us
                conn.Open();

                // We must "use" commands too.
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // Here we setup the command with the SQL we want to execute before we execute it.
                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    // Execute the SQL in the database and get a "reader" that will give us access to the data.
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // A list to hold the chores we retrieve from the database.
                        List<Chore> chores = new List<Chore>();

                        // Read() will return true if there's more data to read
                        while (reader.Read())
                        {
                            // The "ordinal" is the numeric position of the column in the query results.
                            //  For our query, "Id" has an ordinal value of 0 and "Name" is 1.
                            int idColumnPosition = reader.GetOrdinal("Id");

                            // We user the reader's GetXXX methods to get the value for a particular ordinal.
                            int idValue = reader.GetInt32(idColumnPosition);
                            int nameColumnPosition = reader.GetOrdinal("Name");
                            string nameValue = reader.GetString(nameColumnPosition);

                            // Now let's create a new chore object using the data from the database.
                            Chore chore = new Chore
                            {
                                Id = idValue,
                                Name = nameValue,
                            };

                            // ...and add that chore object to our list.
                            chores.Add(chore);
                        }
                        // Return the list of chores who whomever called this method.
                        return chores;
                    }
                }
            }
        }


        /// <summary>
        ///  Returns a single chore with the given id.
        /// </summary>
        public Chore GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Chore chore = null;

                        // If we only expect a single row back from the database, we don't need a while loop.
                        if (reader.Read())
                        {
                            chore = new Chore
                            {
                                Id = id,
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            };
                        }
                        return chore;
                    }
                }
            }
        }


        /// <summary>
        ///  Add a new chore to the database
        ///   NOTE: This method sends data to the database,
        ///   it does not get anything from the database, so there is nothing to return.
        /// </summary>
        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    // executes query and returns the first column of first row in result set (inserted ID)
                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                }
            }

            // when this method is finished we can look in the database and see the new chore.


        }


        // Add a method to ChoreRepository called GetUnassignedChores. It should not accept any parameters and should return a list of chores that don't have any roommates already assigned to them. After implementing this method, add an option to the menu so the user can see the list of unassigned chores.

        public List<Chore> GetUnassignedChores()
        {
            using (SqlConnection conn = Connection) 
            { 
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * " +
                        "FROM Chore c " +
                        "LEFT JOIN RoommateChore rc ON c.Id = rc.ChoreID " +
                        "WHERE rc.ChoreId IS NULL";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> chores = new List<Chore>();
                        while (reader.Read())
                        {
                            int idColumnPosition = reader.GetOrdinal("Id");                           
                            int idValue = reader.GetInt32(idColumnPosition);

                            int nameColumnPosition = reader.GetOrdinal("Name");
                            string nameValue = reader.GetString(nameColumnPosition);

                            Chore chore = new Chore
                            {
                                Id = idValue,
                                Name = nameValue,
                            };

                            chores.Add(chore);

                        }
                        return chores;
                    }
                }
            }
        }


        public int AssignChore(int roommateId, int choreId)
        {
            using (SqlConnection conn = Connection) 
            { 
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO RoommateChore (RoommateId, ChoreId)
                                        OUTPUT INSERTED.Id
                                        VALUES (@roommateId, @choreId)";
                    cmd.Parameters.AddWithValue("@roommateId", roommateId);
                    cmd.Parameters.AddWithValue("@choreId", choreId);
                    int id = (int)cmd.ExecuteScalar();
                    return id;

                }
            }
            
        }


        public List<NumOfChores> GetChoreCounts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT rm.FirstName, COUNT(rm.Id) AS 'CountOfChores' FROM RoommateChore rc JOIN Roommate rm ON rc.RoommateId = rm.Id GROUP BY rm.Id, rm.FirstName";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<NumOfChores> NumOfChores = new List<NumOfChores>();
                        while (reader.Read())
                        {

                            int firstNameColumnPosition = reader.GetOrdinal("FirstName");
                            string firstNameValue = reader.GetString(firstNameColumnPosition);

                            int countOfChoresColumnPosition = reader.GetOrdinal("CountOfChores");
                            int countOfChoresValue = reader.GetInt32(countOfChoresColumnPosition);

                            NumOfChores numOfChore = new NumOfChores
                            {                                
                                FirstName = firstNameValue,
                                CountOfChores = countOfChoresValue,
                            };

                            // need to .Add single to plural
                            NumOfChores.Add(numOfChore);

                        }
                        // need to return the plural
                        return NumOfChores;







                    }
                }

            }
        }


        /// <summary>
        ///  Updates the chore
        /// </summary>
        public void Update(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Chore
                                        SET Name = @name
                                    WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@name", chore.Name);
                    cmd.Parameters.AddWithValue("@id", chore.Id);

                    // execute SQL code to update row and don't give us anything back
                    cmd.ExecuteNonQuery();
                }
            }
        }


        /// <summary>
        ///  Delete the chore with the given id
        /// </summary>
        public void Delete(int id)
        {
            using (SqlConnection conn = Connection) 
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Chore WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();

                }
            }
        }


    }



}

