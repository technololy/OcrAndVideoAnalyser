using System;
using Microsoft.Data.SqlClient;

namespace SqlDataClientLib
{
    public class Class1
    {
        public int ExecuteDbAction(string ConnStr, string query)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        return command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Something went wrong");
                    return 0;
                }
            }
        }


        public int ReturnFromDB(string ConnStr, string query)
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            {
                con.Open();
                try
                {
                    using (SqlCommand command = new SqlCommand(query, con))
                    {
                        return command.ExecuteNonQuery();
                    }
                }
                catch
                {
                    Console.WriteLine("Something went wrong");
                    return 0;
                }
            }
        }




        public string ReturnSingle(string connStr, string query)
        {

            try
            {
                using SqlConnection con = new SqlConnection(connStr);
                con.Open();

                using SqlCommand command = new SqlCommand(query, con);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {

                    string id = reader.GetString(0);  // Name string
                    return id;
                    //dogs.Add(new Dog() { Weight = weight, Name = name, Breed = breed });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return "";
            }
            return "";
        }



    }




}
