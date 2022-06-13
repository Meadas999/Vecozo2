﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceLib;

namespace BusnLogicVecozo
{
    public class VaardigheidContainer
    {
        private readonly IVaardigheidContainer container;
        public VaardigheidContainer(IVaardigheidContainer container)
        {
            this.container = container;
        }
        public Vaardigheid Create(Vaardigheid vaardigheid)
        {
            VaardigheidDTO dto = vaardigheid.GetDTO();
            VaardigheidDTO dto2 = container.Create(dto);
            return new Vaardigheid(dto2);
        }
        public void Delete(Vaardigheid vaardigheid)
        {
            VaardigheidDTO dto = vaardigheid.GetDTO();
            container.Delete(dto);
        }
        public void VerwijderVaarigheidVanMedewerker(Medewerker medewerker, int vaardigheidId)
        {
            MedewerkerDTO dto = medewerker.GetDTO();
            container.VerwijderVaarigheidVanMedewerker(dto, vaardigheidId);
        }
        public void Update(Vaardigheid vaardigheid)
        {
            VaardigheidDTO dto = vaardigheid.GetDTO();
            container.Update(dto);
        }
        public List<Rating> FindByMedewerker(int id)
        {
            List<RatingDTO> dtos = container.FindByMedewerker(id);
            return dtos.Select(v => new Rating(v)).ToList();

        }
        public List<Vaardigheid> GetAll()
        {
            List<VaardigheidDTO> dtos = container.GetAll();
            return dtos.Select(v => new Vaardigheid(v)).ToList();
        }
        public void VoegVaardigheidToeAanMedewerker(Medewerker medewerker, Rating rating)
        {
            MedewerkerDTO dto = medewerker.GetDTO();
            RatingDTO dto3 = rating.GetDTO();
            container.VoegVaardigheidToeAanMedewerker(dto, dto3);
        }
        public Vaardigheid? BestaandeVaardigeheid(string naam)
        {
            VaardigheidDTO dto = container.BestaandeVaardigeheid(naam);
            if (dto == null)
            {
                return null;
            }
            return new Vaardigheid(dto);
        }
        public void UpdateRating(Medewerker med, Rating rating)
        {
            MedewerkerDTO dto = med.GetDTO();
            RatingDTO dto2 = rating.GetDTO();
            container.UpdateRating(dto, dto2);
        }
        public Rating FindRating(int MedewerkerId, int VaardigheidId)
        {
            RatingDTO dto = container.FindRating(MedewerkerId, VaardigheidId);
            dto.vaardigheidDTO = container.FindVaardigheid(VaardigheidId);
            return new Rating(dto);
        }
        public Vaardigheid FindVaardigheid(int id)
        {
            VaardigheidDTO dto = container.FindVaardigheid(id);
            return new Vaardigheid(dto);
        }

    }
}
