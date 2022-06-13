using DALMSSQL.Exceptions;
using InterfaceLib;
using System.Data.SqlClient;

namespace DALMSSQL
{
    public class MedewerkerDAL : IMedewerkerContainer
    {
        ConnectionDb db = new ConnectionDb();
        VaardigheidDAL vaardigheidDAL = new VaardigheidDAL();

        /// <summary>
        /// Een medewerker aanmaken
        /// </summary>
        /// <param name="medewerker">De medewerker die wordt meegegeven</param>
        /// <param name="newWachtwoord">Het wachtwoord die wordt meegegeven</param>
        public void Create(MedewerkerDTO medewerker, string newWachtwoord)
        {
            try
            {
                if (medewerker.Tussenvoegsel == null)
                {
                    medewerker.Tussenvoegsel = "";
                }
                string wachtwoordHash = BCrypt.Net.BCrypt.EnhancedHashPassword(newWachtwoord, 13);
                db.OpenConnection();
                string query = "INSERT INTO Medewerker (Voornaam, Tussenvoegsel, Achternaam, Email, Wachtwoord, TeamId) VALUES (@Voornaam, @Tussenvoegsel, @Achternaam, @Email, @Wachtwoord, @TeamId)";
                SqlCommand cmd = new SqlCommand(query, db.connection);
                cmd.Parameters.AddWithValue("@Voornaam", medewerker.Voornaam);
                cmd.Parameters.AddWithValue("@Tussenvoegsel", medewerker.Tussenvoegsel);
                cmd.Parameters.AddWithValue("@Achternaam", medewerker.Achternaam);
                cmd.Parameters.AddWithValue("@Email", medewerker.Email);
                cmd.Parameters.AddWithValue("@Wachtwoord", wachtwoordHash);
                cmd.Parameters.AddWithValue("@TeamId", medewerker.MijnTeam.Id);
                cmd.ExecuteNonQuery();
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


        /// <summary>
        /// Logt een medewerker in
        /// </summary>
        /// <param name="email">De email die wordt meegegeven</param>
        /// <param name="wachtwoord">De wachtwoord die wordt meegegeven</param>
        /// <returns>Return een nmedewerker</returns>
        public MedewerkerDTO? Inloggen(string email, string wachtwoord)
        {
            try
            {
                bool isValid = false;
                db.OpenConnection();
                MedewerkerDTO med = null;
                string query = @"SELECT Wachtwoord, Id FROM Medewerker WHERE Email = @email";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@email", email);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        med = new MedewerkerDTO(
                        reader["Wachtwoord"].ToString(),
                        Convert.ToInt32(reader["Id"]));
                        isValid = BCrypt.Net.BCrypt.EnhancedVerify(wachtwoord, med.WachtwoordHash);
                    }
                    db.CloseConnetion();
                }
                if (isValid)
                {
                    MedewerkerDTO dto = FindById(med.Id);
                    //dto.MijnTeam = GetTeamVanMedewerker(dto);
                    return dto;
                }
                return null;
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
        /// Zoekt een medewerker op naam
        /// </summary>
        /// <param name="naam">De naam die wordt meegegeven</param>
        /// <returns>Return een medewerker</returns>
        public List<MedewerkerDTO> ZoekOpNaam(string naam)
        {
            try
            {
                List<MedewerkerDTO> medewerkers = new List<MedewerkerDTO>();
                db.OpenConnection();
                string query = @"SELECT * FROM Medewerker WHERE CONCAT_WS(' ', Voornaam, ISNULL(Tussenvoegsel, ' '), Achternaam) LIKE '%' + @naam + '%' ";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@naam", naam);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        medewerkers.Add(new MedewerkerDTO(reader["Email"].ToString(),
                            reader["Voornaam"].ToString(),
                            reader["Achternaam"].ToString(),
                            reader["Tussenvoegsel"].ToString(),
                            Convert.ToInt32(reader["Id"])));
                    }
                    return medewerkers;
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

        /// <summary>
        /// Zoekt een medewerker op vaardigheid
        /// </summary>
        /// <param name="naam">De naam die wordt meegegeven</param>
        /// <returns>Return een medewerker</returns>
        public List<MedewerkerDTO> ZoekMedewerkerOpVaardigheid(string naam)
        {
            try
            {
                List<MedewerkerDTO> medewerkers = new List<MedewerkerDTO>();
                db.OpenConnection();
                string query = @"SELECT * FROM Medewerker 
                 INNER JOIN MedewerkerVaardigheid on Medewerker.Id = MedewerkerVaardigheid.MedewerkerId
                 INNER JOIN Vaardigheid on Vaardigheid.Id = MedewerkerVaardigheid.VaardigheidId
                 WHERE Vaardigheid.Naam LIKE '%' + @naam + '%'";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@naam", naam);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        medewerkers.Add(new MedewerkerDTO(
                            reader["Email"].ToString(),
                            reader["Voornaam"].ToString(),
                            reader["Achternaam"].ToString(),
                            reader["Tussenvoegsel"].ToString(),
                            Convert.ToInt32(reader["Id"])));

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


        /// <summary>
        /// Haalt alle medewerkers op die er zijn
        /// </summary>
        /// <returns>Return een lijst van alle medewerkers</returns>
        public List<MedewerkerDTO> HaalAlleMedewerkersOp()
        {
            try
            {
                List<MedewerkerDTO> medewerkers = new List<MedewerkerDTO>();
                db.OpenConnection();
                SqlCommand command = new SqlCommand(@"SELECT * FROM Medewerker", db.connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        medewerkers.Add(new MedewerkerDTO(
                            reader["Email"].ToString(),
                            reader["Voornaam"].ToString(),
                            reader["Achternaam"].ToString(),
                            reader["Tussenvoegsel"].ToString(),
                            Convert.ToInt32(reader["Id"])));
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


        /// <summary>
        /// Zoekt een medewerker op ID
        /// </summary>
        /// <param name="id">De ID die wordt meegegeven</param>
        /// <returns>Return een medewerker</returns>
        public MedewerkerDTO? FindById(int id)
        {
            try
            {
                MedewerkerDTO? medewerker = null;
                db.OpenConnection();
                string query = @"SELECT * FROM Medewerker WHERE Id = @id";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@id", id);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        medewerker = new MedewerkerDTO(
                            reader["Email"].ToString(),
                            reader["Voornaam"].ToString(),
                            reader["Tussenvoegsel"].ToString(),
                            reader["Achternaam"].ToString(),
                            Convert.ToInt32(reader["Id"]));
                    }
                }
                db.CloseConnetion();
                return medewerker;
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
        /// Koppelt een medewerker aan een leidinggevende
        /// </summary>
        /// <param name="med">De medewerker die wordt meegegeven</param>
        /// <param name="leid">De Leidinggevende die wordt meegegeven</param>
        public void KoppelMedewerkerAanLeidinggevenden(MedewerkerDTO med,LeidingGevendeDTO leid)
        {
            try
            {
                db.OpenConnection();
                string query = @"INSERT INTO LeidinggevendeMedewerker VALUES (@leidId,@medId)";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@leidId", leid.UserID);
                command.Parameters.AddWithValue("@medId", med.Id);
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

        /// <summary>
        /// Haalt de team op van een bepaalde medewerker
        /// </summary>
        /// <param name="dto">De medewerker die wordt meegegeven</param>
        /// <returns>Return een team</returns>
        public TeamDTO? GetTeamVanMedewerker(MedewerkerDTO dto)
        {
            try
            {
                TeamDTO? team = null;
                db.OpenConnection();
                string query = @"SELECT * FROM Team T 
            INNER JOIN Medewerker M ON M.TeamId = T.Id WHERE M.Id = @id";
                SqlCommand command = new SqlCommand(query, db.connection);
                command.Parameters.AddWithValue("@id", dto.Id);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        team = new TeamDTO(
                        Convert.ToInt32(reader["Id"]),
                        reader["TeamKleur"].ToString(),
                        reader["Taak"].ToString(),
                        (float)Convert.ToDouble(reader["Gem_Rating"]));
                    }
                }
                db.CloseConnetion();
                return team;
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
