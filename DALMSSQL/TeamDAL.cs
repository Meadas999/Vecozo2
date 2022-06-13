using DALMSSQL.Exceptions;
using InterfaceLib;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DALMSSQL
{
    public class TeamDAL : ITeamContainer
    {
        ConnectionDb db = new();

        /// <summary>
        /// Zoekt een team op basis van de meegegeven ID
        /// </summary>
        /// <param name="userid">De ID die wordt meegegeven</param>
        /// <returns>Return een team</returns>
        public TeamDTO? FindByUserId(int userid)
        {
            try
            {
                int teamid = GetTeamIdByUserid(userid);
                TeamDTO? dto = null;
                db.OpenConnection();
                string query = @"SELECT * FROM Team WHERE Id = @id";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@id", teamid);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    dto = new TeamDTO(
                        Convert.ToInt32(reader["Id"]),
                        reader["TeamKleur"].ToString(),
                        reader["Taak"].ToString(),
                        Convert.ToDouble(reader["Gem_Rating"]));
                }
                db.CloseConnetion();
                return dto;
            }
            catch (SqlException sqlex)
            {
                throw new TemporaryException("Kan geen verbinding maken met de server");
            }
            catch (Exception ex)
            {
                throw new PermanentException("Er is een fout opgetreden");
            }
        }

        /// <summary>
        /// Haalt alle teams op die er zijn
        /// </summary>
        /// <returns>Return een lijst van teams</returns>
        public List<TeamDTO> GetAll()
        {
            try
            {
                List<TeamDTO> dtos = new();
                db.OpenConnection();
                string query = @"SELECT * FROM Team";
                SqlCommand command = new SqlCommand(query, db.connection);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    dtos.Add(new TeamDTO(
                        Convert.ToInt32(reader["Id"]),
                        reader["TeamKleur"].ToString(),
                        reader["Taak"].ToString(),
                        (float)Convert.ToDouble(reader["Gem_Rating"])));
                }
                db.CloseConnetion();
                return dtos;
            }
            catch (SqlException sqlex)
            {
                throw new TemporaryException("Kan geen verbinding maken met de server");
            }
            catch (Exception ex)
            {
                throw new PermanentException("Er is een fout opgetreden");
            }
        }

        /// <summary>
        /// Haalt een team op met een bepaalde gebruikersID
        /// </summary>
        /// <param name="userid">De ID die wordt meegegeven</param>
        /// <returns>Return een team</returns>
        private int GetTeamIdByUserid(int userid)
        {
            try
            {
                db.OpenConnection();
                string query = @"SELECT TeamId FROM Medewerker WHERE Id = @id";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@id", userid);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int teamid = Convert.ToInt32(reader["TeamId"]);
                        return teamid;
                    }
                }
                db.CloseConnetion();
                return -1;
            }
            catch (SqlException sqlex)
            {
                throw new TemporaryException("Kan geen verbinding maken met de server");
            }
            catch (Exception ex)
            {
                throw new PermanentException("Er is een fout opgetreden");
            }
        }

    }
}
