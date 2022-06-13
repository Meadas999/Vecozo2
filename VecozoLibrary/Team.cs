﻿using InterfaceLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusnLogicVecozo
{
    public class Team
    {
        /// <summary>
        /// Elk team heeft een unieke ID, een teamkleur, een taak en een gemiddelde rating.
        /// Ook horen bij een team natuurlijk bepaalde leden
        /// </summary>
        public int Id { get; set; }
        public string Kleur { get; }
        public string Taak { get; }
        public double GemRating { get; }
        public List<Medewerker> GroepLeden { get; } = new List<Medewerker>();

        public Team(int id, string kleur, string taak, double gemRating)
        {
            Id = id;
            Kleur = kleur;
            Taak = taak;
            GemRating = gemRating;
        }

        public Team(TeamDTO dto)
        {
            this.Id = dto.Id;
            this.Kleur = dto.Kleur;
            this.Taak = dto.Taak;
            this.GemRating = dto.GemRating;
            //this.GroepLeden = dto.GroepLeden.Select(x => new Medewerker(x)).ToList();
        }

        public TeamDTO GetDTO()
        {
            TeamDTO dto = new TeamDTO(Id,Kleur,Taak,GemRating);
            return dto;
        }
        
        public override string ToString()
        {
            return $"Teamkleur: {this.Kleur}\nTaak: {this.Taak}";
        }

    }
}
