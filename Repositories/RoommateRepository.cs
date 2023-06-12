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
    ///  This class is responsible for interacting with Roommate data.
    ///  It inherits from the BaseRepository class so that it can use the BaseRepository's Connection property
    /// </summary>
    /// Changed from internal to public
    public class RoommateRepository : BaseRepository
    {
        /// <summary>
        /// Get a list of all Chores in the database
        /// </summary>
        public RoommateRepository(string connectionString) : base(connectionString) { }

        /// <summary>
        ///  Get a list of all Rooms in the database
        /// </summary>
        public List<Roommate> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = "SELECT * " +
                        "FROM Roommate rm " +
                        "JOIN Room r ON rm.RoomId = r.Id;";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Roommate> roommates = new List<Roommate>();
                        //Roommate roommate = null;

                        while (reader.Read())
                        {
                            int idColumnPosition = reader.GetOrdinal("Id");
                            int idValue = reader.GetInt32(idColumnPosition);
                            // Id = reader.GetInt32(reader.GetOrdinal("Id));

                            int firstNameColumnPosition = reader.GetOrdinal("FirstName");
                            string firstNameValue = reader.GetString(firstNameColumnPosition);

                            int lastNameColumnPosition = reader.GetOrdinal("LastName");
                            string lastNameValue = reader.GetString(lastNameColumnPosition);

                            int rentPortionColumnPosition = reader.GetOrdinal("RentPortion");
                            int rentPortionValue = reader.GetInt32(rentPortionColumnPosition);

                            int moveInDateColumnPosition = reader.GetOrdinal("MoveInDate");
                            DateTime moveInDateValue = reader.GetDateTime(moveInDateColumnPosition);

                            int roomIdColumnPosition = reader.GetOrdinal("RoomId");
                            int roomIdValue = reader.GetInt32(roomIdColumnPosition);

                            int roomNameColumnPosition = reader.GetOrdinal("Name");
                            string roomNameValue = reader.GetString(roomNameColumnPosition);

                            int roomMaxOccupancyColumnPosition = reader.GetOrdinal("MaxOccupancy");
                            int roomMaxOccupancyValue = reader.GetInt32(roomMaxOccupancyColumnPosition);

                            Roommate roommate = new Roommate
                            {
                                Id = idValue,
                                FirstName = firstNameValue,
                                LastName = lastNameValue,
                                RentPortion = rentPortionValue,
                                MovedInDate = moveInDateValue,
                                Room = new Room
                                {
                                    Id = roomIdValue,
                                    Name = roomNameValue,
                                    MaxOccupancy = roomMaxOccupancyValue,
                                }
                                
                            };
                            roommates.Add(roommate);

                        }

                        return roommates;
                    }
                }
            }
        }

        /// <summary>
        ///  Returns a single roommate with the given id.
        /// </summary>
        public Roommate GetById(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * " +
                        "FROM Roommate rm " +
                        "JOIN Room r ON rm.RoomId = r.Id " +
                        "WHERE rm.Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader()) 
                    {
                        Roommate roommate = null;

                        // If we only expect a single row back from the database, we don't need a while loop.  
                        if (reader.Read())
                        {
                            roommate = new Roommate
                            {
                                Id = id,
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                RentPortion = reader.GetInt32(reader.GetOrdinal("RentPortion")),
                                MovedInDate = reader.GetDateTime(reader.GetOrdinal("MoveInDate")),

                                Room = new Room
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("RoomId")),
                                    Name = reader.GetString(reader.GetOrdinal("Name")),
                                    MaxOccupancy = reader.GetInt32(reader.GetOrdinal("MaxOccupancy")),
                                }
                            };
                        }
                        return roommate;
                    }

                }
            }
        }

    }
}
