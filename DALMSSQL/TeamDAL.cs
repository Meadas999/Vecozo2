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
        private readonly ConnectionDb db;
        private readonly string connectionString;

        public TeamDAL(string con)
        {
            this.connectionString = con;
            this.db = new(this.connectionString);
        }
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

        public List<MedewerkerDTO> GetMedewerkersFromTeam(int teamid)
        {
            try
            {
                List<MedewerkerDTO> medewerkers = new List<MedewerkerDTO>();
                db.OpenConnection();
                string query = @"SELECT * FROM Medewerker AS M INNER JOIN Team AS T ON T.Id = M.TeamID WHERE M.TeamId = @id";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@id", teamid);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        medewerkers.Add(new MedewerkerDTO(reader["Email"].ToString(),
                        reader["Voornaam"].ToString(), reader["Achternaam"].ToString(),
                        reader["Tussenvoegsel"].ToString(), Convert.ToInt32(reader["Id"]),
                        new TeamDTO(Convert.ToInt32(reader["TeamId"]), reader["TeamKleur"].ToString(),
                        reader["Taak"].ToString(), (float)Convert.ToDouble(reader["Gem_Rating"]))));
                    }
                }
                db.CloseConnetion();
                return medewerkers;
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

        public void UpdateTeamMedewerker(MedewerkerDTO medewerker, TeamDTO team)
        {
            try
            {
                db.OpenConnection();
                string query = @"UPDATE Medewerker SET TeamId = @id WHERE Id = @userID";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@id", team.Id);
                command.Parameters.AddWithValue("@userID", medewerker.Id);
                command.ExecuteNonQuery();
                db.CloseConnetion();
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

        public void Update(TeamDTO team)
        {
            try
            {
                db.OpenConnection();
                string query = @"UPDATE Team SET TeamKleur = @kleur, Taak = @taak, Gem_Rating = @rating WHERE Id = @id";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("id", team.Id);
                command.Parameters.AddWithValue("kleur", team.Kleur);
                command.Parameters.AddWithValue("rating", team.GemRating);
                command.Parameters.AddWithValue("taak", team.Taak);
                command.ExecuteNonQuery();
                db.CloseConnetion();
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

        public void Delete(TeamDTO dto)
        {
            try
            {
                db.OpenConnection();
                string query = @"DELETE FROM Team WHERE Id = @id";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@id", dto.Id);
                command.ExecuteNonQuery();
                db.CloseConnetion();
            }
            catch (SqlException)
            {
                throw new TemporaryException("Kan geen verbinding maken met de server");
            }
            catch (Exception)
            {
                throw new PermanentException("Er is een fout opgetreden");
            }
        }

        public void Create(TeamDTO dto)
        {
            try
            {
                db.OpenConnection();
                string query = @"INSERT INTO Team VALUES (@kleur, @taak, @rating)";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@kleur", dto.Kleur);
                command.Parameters.AddWithValue("@taak", dto.Taak);
                command.Parameters.AddWithValue("@rating", dto.GemRating);
                command.ExecuteNonQuery();
                db.CloseConnetion();
            }
            catch (SqlException)
            {
                throw new TemporaryException("Kan geen verbinding maken met de server");
            }
            catch (Exception)
            {
                throw new PermanentException("Er is een fout opgetreden");
            }
        }

        public TeamDTO FindByID(int id)
        {
            try
            {
                TeamDTO? dto = null;
                db.OpenConnection();
                string query = @"SELECT * FROM Team WHERE Id = @id";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    dto = new TeamDTO(
                        Convert.ToInt32(reader["Id"]),
                        reader["TeamKleur"].ToString(),
                        reader["Taak"].ToString(),
                        (float)Convert.ToDouble(reader["Gem_Rating"]));
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
    }
}
