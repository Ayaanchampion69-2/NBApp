using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NBApp.Areas.Identity.Data;
using NBApp.Models;
using NBApp.ViewModels;

namespace NBApp.Controllers
{
    public class CitySuburbController : Controller
    {
        private readonly NBAppContext _db;

        public CitySuburbController(NBAppContext db)
        {
            _db = db;
        }

        private async Task<CitySuburbViewModel> BuildViewModel(
            CityFormModel? cityForm = null,
            SuburbFormModel? suburbForm = null)
        {
            return new CitySuburbViewModel
            {
                Cities = await _db.Cities
                    .Include(c => c.Suburbs)
                    .OrderBy(c => c.CityName)
                    .ToListAsync(),
                Suburbs = await _db.Suburbs
                    .Include(s => s.City)
                    .OrderBy(s => s.SuburbName)
                    .ToListAsync(),
                CityForm = cityForm ?? new CityFormModel(),
                SuburbForm = suburbForm ?? new SuburbFormModel()
            };
        }

        // GET: /CitySuburb
        public async Task<IActionResult> Index()
        {
            return View(await BuildViewModel());
        }

        // ── CITY CRUD ──────────────────────────────────────────────────────────

        // POST: /CitySuburb/AddCity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCity(CityFormModel CityForm)
        {
            ModelState.Remove("SuburbForm.SuburbName");
            ModelState.Remove("SuburbForm.CityID");
            ModelState.Remove("SuburbForm.DeliveryCost");

            if (!ModelState.IsValid)
            {
                TempData["ActiveSection"] = "city";
                return View("Index", await BuildViewModel(cityForm: CityForm));
            }

            _db.Cities.Add(new City { CityName = CityForm.CityName });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"City '{CityForm.CityName}' added.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /CitySuburb/EditCity/5
        public async Task<IActionResult> EditCity(int id)
        {
            var city = await _db.Cities.FindAsync(id);
            if (city == null) return NotFound();

            var form = new CityFormModel { CityID = city.CityID, CityName = city.CityName };
            var vm = await BuildViewModel(cityForm: form);
            TempData["ActiveSection"] = "city";
            TempData["EditCityID"] = id;
            return View("Index", vm);
        }

        // POST: /CitySuburb/UpdateCity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCity(CityFormModel CityForm)
        {
            ModelState.Remove("SuburbForm.SuburbName");
            ModelState.Remove("SuburbForm.CityID");
            ModelState.Remove("SuburbForm.DeliveryCost");

            if (!ModelState.IsValid)
            {
                TempData["ActiveSection"] = "city";
                TempData["EditCityID"] = CityForm.CityID;
                return View("Index", await BuildViewModel(cityForm: CityForm));
            }

            var city = await _db.Cities.FindAsync(CityForm.CityID);
            if (city == null) return NotFound();

            city.CityName = CityForm.CityName;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"City updated to '{CityForm.CityName}'.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /CitySuburb/DeleteCity/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _db.Cities.Include(c => c.Suburbs).FirstOrDefaultAsync(c => c.CityID == id);
            if (city == null) return NotFound();

            if (city.Suburbs != null && city.Suburbs.Any())
            {
                TempData["Error"] = $"Cannot delete '{city.CityName}' — it still has suburbs assigned.";
                return RedirectToAction(nameof(Index));
            }

            _db.Cities.Remove(city);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"City '{city.CityName}' deleted.";
            return RedirectToAction(nameof(Index));
        }

        // ── SUBURB CRUD ────────────────────────────────────────────────────────

        // POST: /CitySuburb/AddSuburb
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddSuburb(SuburbFormModel SuburbForm)
        {
            ModelState.Remove("CityForm.CityName");

            if (!ModelState.IsValid)
            {
                TempData["ActiveSection"] = "suburb";
                return View("Index", await BuildViewModel(suburbForm: SuburbForm));
            }

            _db.Suburbs.Add(new Suburb
            {
                SuburbName = SuburbForm.SuburbName,
                DeliveryCost = SuburbForm.DeliveryCost,
                CityID = SuburbForm.CityID
            });
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Suburb '{SuburbForm.SuburbName}' added.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /CitySuburb/EditSuburb/5
        public async Task<IActionResult> EditSuburb(int id)
        {
            var suburb = await _db.Suburbs.FindAsync(id);
            if (suburb == null) return NotFound();

            var form = new SuburbFormModel
            {
                SuburbID = suburb.SuburbID,
                SuburbName = suburb.SuburbName,
                DeliveryCost = suburb.DeliveryCost ?? 0m,
                CityID = suburb.CityID
            };
            var vm = await BuildViewModel(suburbForm: form);
            TempData["ActiveSection"] = "suburb";
            TempData["EditSuburbID"] = id;
            return View("Index", vm);
        }

        // POST: /CitySuburb/UpdateSuburb
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSuburb(SuburbFormModel SuburbForm)
        {
            ModelState.Remove("CityForm.CityName");

            if (!ModelState.IsValid)
            {
                TempData["ActiveSection"] = "suburb";
                TempData["EditSuburbID"] = SuburbForm.SuburbID;
                return View("Index", await BuildViewModel(suburbForm: SuburbForm));
            }

            var suburb = await _db.Suburbs.FindAsync(SuburbForm.SuburbID);
            if (suburb == null) return NotFound();

            suburb.SuburbName = SuburbForm.SuburbName;
            suburb.DeliveryCost = SuburbForm.DeliveryCost;
            suburb.CityID = SuburbForm.CityID;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Suburb updated to '{SuburbForm.SuburbName}'.";
            return RedirectToAction(nameof(Index));
        }

        // POST: /CitySuburb/DeleteSuburb/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSuburb(int id)
        {
            var suburb = await _db.Suburbs.Include(s => s.City).FirstOrDefaultAsync(s => s.SuburbID == id);
            if (suburb == null) return NotFound();

            _db.Suburbs.Remove(suburb);
            await _db.SaveChangesAsync();
            TempData["Success"] = $"Suburb '{suburb.SuburbName}' deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}