﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InterfaceLib
{
    public interface ITeamContainer
    {
        public List<TeamDTO> GetAll();
        public TeamDTO? FindByUserId(int userid);
        public List<MedewerkerDTO> GetMedewerkersFromTeam(int teamid);
        public void UpdateTeamMedewerker(MedewerkerDTO medewerker, TeamDTO team);
        public void Update(TeamDTO team);
        public void Create(TeamDTO team);
        public void Delete(TeamDTO team);
        public TeamDTO FindByID(int id);
    }
}
