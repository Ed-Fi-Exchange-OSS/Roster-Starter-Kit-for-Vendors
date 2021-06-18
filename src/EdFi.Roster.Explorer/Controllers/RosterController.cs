using System.Threading.Tasks;
using EdFi.Roster.Explorer.Models;
using EdFi.Roster.Explorer.Services;
using Microsoft.AspNetCore.Mvc;

namespace EdFi.Roster.Explorer.Controllers
{
    public class RosterController : Controller
    {
        private readonly RosterService _rosterService;

        public RosterController(RosterService rosterService)
        {
            _rosterService = rosterService;
        }

        // GET: RosterController
        public ActionResult Index()
        {
            return View();
        }

        // GET: RosterController/Details/5
        public async Task<ActionResult> HierarchyWithTerms()
        {
            var leaRoster = await _rosterService.GetRosterAsync();
            var returnRoster = new RosterViewModel(leaRoster);
            return View(returnRoster);
        }
        public async Task<ActionResult> HierarchyByStaff()
        {
            var leaRoster = await _rosterService.GetRosterAsync();
            var returnRoster = new RosterViewModel(leaRoster);
            return View(returnRoster);
        }

        public async Task<ActionResult> HierarchyBySchoolSection()
        {
            var leaRoster = await _rosterService.GetRosterAsync();
            var returnRoster = new RosterViewModel(leaRoster);
            return View(returnRoster);
        }
    }
}
