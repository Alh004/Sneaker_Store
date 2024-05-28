﻿using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Sneaker_Store.Model;
using System.Collections.Generic;

namespace Sneaker_Store.Services
{
    public class DB_Kunde : IKundeRepository
    {
        private const string ConnectionString =
            "Data Source=mssql13.unoeuro.com;Initial Catalog=sirat_dk_db_thread;User ID=sirat_dk;Password=m5k6BgDhAzxbprH49cyE;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public DB_Kunde(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public Kunde? KundeLoggedIn 
        { 
            get
            {
                var email = _httpContextAccessor.HttpContext.Session.GetString("UserEmail");
                if (string.IsNullOrEmpty(email))
                {
                    return null;
                }
                return GetByEmail(email);
            }
        }

        private const string InsertSql = "INSERT INTO Kunder (Fornavn, Efternavn, Email, Adgangskode, Postnr, Adresse, Admin) VALUES (@navn, @efternavn, @email, @kode, @postnr, @addrese, @admin)";

        public Kunde Add(Kunde newKunde)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(InsertSql, connection))
                {
                    cmd.Parameters.AddWithValue("@navn", newKunde.Navn);
                    cmd.Parameters.AddWithValue("@efternavn", newKunde.Efternavn);
                    cmd.Parameters.AddWithValue("@email", newKunde.Email);
                    cmd.Parameters.AddWithValue("@kode", newKunde.Kode);
                    cmd.Parameters.AddWithValue("@postnr", newKunde.Postnr);
                    cmd.Parameters.AddWithValue("@addrese", newKunde.Adresse);
                    cmd.Parameters.AddWithValue("@admin", newKunde.Admin);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        throw new ArgumentException("Kunne ikke oprette Kunde = " + newKunde);
                    }
                }
            }
            return newKunde;
        }

        public void AddKunde(Kunde kunde)
        {
            Add(kunde);
        }

        public LoginResult? CheckKunde(string email, string kode)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT KundeId, Fornavn, Efternavn, Email, Adresse, Postnr, Adgangskode, Admin FROM Kunder WHERE Email = @Email AND Adgangskode = @kode";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@kode", kode);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var kunde = new Kunde
                            {
                                KundeId = reader.GetInt32(0),
                                Navn = reader.GetString(1),
                                Efternavn = reader.GetString(2),
                                Email = reader.GetString(3),
                                Adresse = reader.GetString(4),
                                Postnr = reader.GetInt32(5),
                                Kode = reader.GetString(6),
                                Admin = reader.GetBoolean(7)
                            };
                            _httpContextAccessor.HttpContext.Session.SetString("UserEmail", email); // Store email in session
                            return new LoginResult
                            {
                                IsAdmin = kunde.Admin
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }
        }

        public List<Kunde> GetAll()
        {
            List<Kunde> kunder = new List<Kunde>();

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT KundeId, Fornavn, Efternavn, Email, Adresse, Postnr, Adgangskode, Admin FROM Kunder";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Kunde kunde = new Kunde
                            {
                                KundeId = reader.GetInt32(0),
                                Navn = reader.GetString(1),
                                Efternavn = reader.GetString(2),
                                Email = reader.GetString(3),
                                Adresse = reader.GetString(4),
                                Postnr = reader.GetInt32(5),
                                Kode = reader.GetString(6),
                                Admin = reader.GetBoolean(7)
                            };
                            kunder.Add(kunde);
                        }
                    }
                }
            }

            return kunder;
        }

        public Kunde GetById(int kundeId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT KundeId, Fornavn, Efternavn, Email, Adresse, Postnr, Adgangskode, Admin FROM Kunder WHERE KundeId = @KundeId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@KundeId", kundeId);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Kunde
                            {
                                KundeId = reader.GetInt32(0),
                                Navn = reader.GetString(1),
                                Efternavn = reader.GetString(2),
                                Email = reader.GetString(3),
                                Adresse = reader.GetString(4),
                                Postnr = reader.GetInt32(5),
                                Kode = reader.GetString(6),
                                Admin = reader.GetBoolean(7)
                            };
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                }
            }
        }

        public void RemoveKunde(Kunde kunde)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string sql = "DELETE FROM Kunder WHERE KundeId = @KundeId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@KundeId", kunde.KundeId);
                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        throw new ArgumentException("Kunne ikke slette Kunde = " + kunde);
                    }
                }
            }
        }

        public Kunde Opdater(Kunde kunde)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string sql = "UPDATE Kunder SET Fornavn = @navn, Efternavn = @efternavn, Email = @email, Adgangskode = @kode, Postnr = @postnr, Adresse = @addrese, Admin = @admin WHERE KundeId = @KundeId";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@navn", kunde.Navn);
                    cmd.Parameters.AddWithValue("@efternavn", kunde.Efternavn);
                    cmd.Parameters.AddWithValue("@email", kunde.Email);
                    cmd.Parameters.AddWithValue("@kode", kunde.Kode);
                    cmd.Parameters.AddWithValue("@postnr", kunde.Postnr);
                    cmd.Parameters.AddWithValue("@addrese", kunde.Adresse);
                    cmd.Parameters.AddWithValue("@admin", kunde.Admin);
                    cmd.Parameters.AddWithValue("@KundeId", kunde.KundeId);

                    int rows = cmd.ExecuteNonQuery();
                    if (rows == 0)
                    {
                        throw new ArgumentException("Kunne ikke opdatere Kunde = " + kunde);
                    }
                }
            }
            return kunde;
        }

        public Kunde Remove(int kundeId)
        {
            Kunde kunde = GetById(kundeId);
            RemoveKunde(kunde);
            return kunde;
        }

        public Kunde Update(int nytKundeId, Kunde kunde)
        {
            if (nytKundeId != kunde.KundeId)
            {
                throw new ArgumentException("Kan ikke opdatere KundeId, de er forskellige.");
            }

            return Opdater(kunde);
        }

        public Kunde GetByEmail(string email)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                string sql = "SELECT KundeId, Fornavn, Efternavn, Email, Adresse, Postnr, Adgangskode, Admin FROM Kunder WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@Email", email);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Kunde
                            {
                                KundeId = reader.GetInt32(0),
                                Navn = reader.GetString(1),
                                Efternavn = reader.GetString(2),
                                Email = reader.GetString(3),
                                Adresse = reader.GetString(4),
                                Postnr = reader.GetInt32(5),
                                Kode = reader.GetString(6),
                                Admin = reader.GetBoolean(7)
                            };
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                }
            }
        }

        private const String selectAllSqlSortedByNavn = "select * from Kunder order by Fornavn DESC";
        public List<Kunde> GetAllKunderSortedByNavnReversed()
        {
            return GetAllWithParameterSQL(selectAllSqlSortedByNavn);
        }

        private List<Kunde> GetAllWithParameterSQL(string sql)
        {
            List<Kunde> kunder = new List<Kunde>();

            SqlConnection connection = new SqlConnection(DB_Kunde.ConnectionString);
            connection.Open();

            SqlCommand cmd = new SqlCommand(sql, connection);

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                Kunde kunde = ReadKunde(reader);
                kunder.Add(kunde);
            }

            connection.Close();
            return kunder;
        }

        public Kunde GetByEmail(string email)
        {
            _httpContextAccessor.HttpContext.Session.Remove("UserEmail");
        }
    }
}
