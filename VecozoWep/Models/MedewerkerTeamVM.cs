using BusnLogicVecozo;
using DALMSSQL;
using VecozoWep.Models;

namespace VecozoWeb.Models
{
    public class MedewerkerTeamVM
    {
        public MedewerkerVM MedewerkerVM { get; set; }
        public List<MedewerkerVM> MedewerkersVM { get; set; } = new List<MedewerkerVM>();
        public TeamVM TeamVM { get; set; }
        public List<int> MedewerkersInTeam { get; set; } = new List<int>();

        public MedewerkerTeamVM(MedewerkerVM medewerkerVM, List<MedewerkerVM> medewerkersVM, TeamVM teamVM)
        {
            this.MedewerkerVM = medewerkerVM;
            this.MedewerkersVM = medewerkersVM;
            this.TeamVM = teamVM;
        }

        public MedewerkerTeamVM()
        {

        }
    }
}
