using EdFi.Roster.Models;

namespace EdFi.Roster.Explorer.Models
{
    public class RosterViewModel
    {
        public RosterViewModel(LocalEducationAgencyRoster localEducationAgencyRoster)
        {
            this.Roster = localEducationAgencyRoster;
        }
        public LocalEducationAgencyRoster Roster { get; }
    }
}
