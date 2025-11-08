using Microsoft.AspNetCore.Mvc;
using ServiceContracts;
using Services;

namespace ContactManagerApp.Controllers
{
    [Route("[controller]")]
    public class CountriesController : Controller
    {
        private readonly ICountriesService _countriesService;

        public CountriesController(ICountriesService countriesService)
        {
            _countriesService = countriesService;
        }

        [Route("UploadFromExcel")]
        public IActionResult UploadFromExcel()
        {
            return View();
        }

        [Route("UploadFromExcel")]
        [HttpPost]
        public async Task<IActionResult> UploadFromExcel(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
            {
                ViewBag.ErrorMessage = "File cannot be empty";
                return View();
            }

            if (!Path.GetExtension(formFile.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.ErrorMessage = "File is not xlsx";
                return View();
            }

            int countriesInserted = await _countriesService.UploadFromExcelFile(formFile);

            ViewBag.Message = $"{countriesInserted} countries uploaded";
            return View();

        }



    }


   
}
