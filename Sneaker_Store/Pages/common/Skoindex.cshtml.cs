using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Sneaker_Store.Model;
using Sneaker_Store.Services;
using System.Collections.Generic;
using System.Linq;

namespace Sneaker_Store.Pages
{
    public class SkoIndexModel : PageModel
    {
        private readonly ISkoRepository _skoRepository;

        public SkoIndexModel(ISkoRepository skoRepository)
        {
            _skoRepository = skoRepository;
        }

        public List<Sko> Skos { get; set; }
        [BindProperty] public int SelectedSkoId { get; set; }
        [BindProperty] public string SelectedBrand { get; set; }
        [BindProperty] public string PriceFilter { get; set; }
        public List<string> Brands { get; set; } = new List<string> { "Nike", "Asics", "Adidas", "SneakerCare" };

        public void OnGet()
        {
            Skos = _skoRepository.GetAll();
        }

        public void OnPostFilter()
        {
            var skos = _skoRepository.GetAll();

            if (!string.IsNullOrEmpty(SelectedBrand))
            {
                skos = skos.Where(s => s.Maerke == SelectedBrand).ToList();
            }

            if (PriceFilter == "Lowest")
            {
                skos = skos.OrderBy(s => s.Pris).ToList();
            }
            else if (PriceFilter == "Highest")
            {
                skos = skos.OrderByDescending(s => s.Pris).ToList();
            }

            Skos = skos;
        }

        public IActionResult OnPostAddToBasket()
        {
            // Get the selected shoe from the repository
            var selectedSko = _skoRepository.GetById(SelectedSkoId);
            if (selectedSko != null)
            {
                // Retrieve the current Kurv from the session or create a new one if it doesn't exist
                Kurv kurv;
                try
                {
                    kurv = Testsession.Get<Kurv>(HttpContext);
                }
                catch (NoSessionObjectException)
                {
                    kurv = new Kurv();
                }

                // Add the selected shoe to the Kurv
                kurv.Tilføj(selectedSko);

                // Save the updated Kurv back to the session
                Testsession.Set(kurv, HttpContext);

                // Redirect to a confirmation page or show a success message
                return RedirectToPage("PurchaseConfirmation", new { id = SelectedSkoId });
            }

            // If the selected shoe is not found, reload the page with an error message
            ModelState.AddModelError(string.Empty, "Selected shoe not found.");
            Skos = _skoRepository.GetAll();
            return Page();
        }

    }
}
