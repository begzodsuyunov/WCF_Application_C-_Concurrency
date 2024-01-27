using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace _00010358_Service
{
    // Sets the instance context mode to single, concurrency mode to multiple, and uses the synchronization context
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = true)]
    public class AirportService : IAirportService
    {
        //declaring a connection string to a local SQL Server database file
        private string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\00010358FlightDb.mdf;Integrated Security=True";

        //method to insert flight
        public bool InsertFlight(Flight flight)
        {
            bool success = false;

            //establish connection to the database
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                //open the connection
                connection.Open();

                //create a command to insert the flight information
                SqlCommand command = new SqlCommand("INSERT INTO Flights (FlightCode, Destination, DepartureTime) VALUES (@FlightCode, @Destination, @DepartureTime)", connection);
                command.Parameters.AddWithValue("@FlightCode", flight.FlightCode);
                command.Parameters.AddWithValue("@Destination", flight.Destination);
                command.Parameters.AddWithValue("@DepartureTime", flight.DepartureTime);



                //execute the command
                int rowsAffected = command.ExecuteNonQuery();

                //check if the insertion was successful
                if (rowsAffected > 0)
                {
                    success = true;
                }
            }

            return success;
        }
    }

    
}
