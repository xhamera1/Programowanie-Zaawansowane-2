using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; 
using MyApp.Models;             
using MyApp.Services;         
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MyApp.Controllers
{
    public class DataController : Controller
    {
        private readonly DatabaseService _dbService;

        public DataController(DatabaseService dbService)
        {
            _dbService = dbService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new DataViewModel();
            model.Entries = _dbService.GetAllDataEntries();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(DataViewModel model)
        {
            if (ModelState.IsValid) 
            {
                bool success = _dbService.AddDataEntry(model.NewDataText!);

                if (success)
                {
                    TempData["SuccessMessage"] = "Data added successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to add new entry!";
                    model.Entries = _dbService.GetAllDataEntries();
                    return View(model);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Validation error!";
                model.Entries = _dbService.GetAllDataEntries();
                return View(model);
            }

        }
    }
}