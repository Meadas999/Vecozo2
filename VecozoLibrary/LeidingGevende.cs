﻿using InterfaceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusnLogicVecozo
{
    public class LeidingGevende : Gebruiker
    {
        /// <summary>
        /// Een Leidinggevende heeft een Email, voornaam, tussenvoegsels, achternaam en een ID net zoals een gebruiker
        /// Ook is hij de baas van een aantal medewerkers
        /// </summary>
        public List<Medewerker> Medewerkers { get; set; } = new List<Medewerker>();

        public LeidingGevende(string email, string voornaam, string achternaam, int userID = 0, string tussenvoegsel = "") : base(email, voornaam, achternaam, userID, tussenvoegsel)
        {
        }


        public LeidingGevende(LeidingGevendeDTO dto) : base(dto.Email, dto.Voornaam, dto.Achternaam, dto.UserID, dto.Tussenvoegsel)
        {
            //Medewerkers = dto.Medewerkers.Select(m => new Medewerker(m)).ToList();
        }
        
        public LeidingGevendeDTO GetDTO()
        {
            return new LeidingGevendeDTO(this.Email, this.Voornaam, this.Achternaam, this.UserID, this.Tussenvoegsel);
        }


        public override string ToString()
        {
            return base.ToString();
        }
    }
}
