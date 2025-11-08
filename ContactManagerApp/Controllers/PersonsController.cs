using ContactManagerApp.Filters.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Rotativa.AspNetCore;
using ServiceContracts;
using ServiceContracts.DTO;
using ServiceContracts.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace ContactManagerApp.Controllers
{
    
    [TypeFilter(typeof(PersonsListActionFilter), Arguments = new object[] {"controller", 3},Order =3)]
    public class PersonsController : Controller
    {
        private readonly IPersonsService _personsService;
        private readonly ICountriesService _countriesService;
        private readonly ILogger<PersonsController> _logger;

        public PersonsController(IPersonsService personsService, 
            ICountriesService countriesService,
            ILogger<PersonsController> logger)
        {
            _personsService = personsService;
            _countriesService = countriesService;
            _logger = logger;
        }

        [Route("persons/index")]
        [Route("/")]
        [TypeFilter(typeof(PersonsListActionFilter), Arguments = 
            new object[] {"action",1},Order =1)]
        public async Task<IActionResult> Index(
              string searchBy,string? searchString
            , string sortBy = nameof(PersonResponse.PersonName), SortOrderOptions sortOrderOption = SortOrderOptions.ASC)
        {

            _logger.LogInformation("Index action of persons controller");

            _logger.LogDebug($"searchBy:{searchBy},searchString: {searchString}, sortBy : {sortBy}, sortOrderOption:{sortOrderOption}");


            ViewBag.SearchOptions = new Dictionary<string, string>()
            {
                { nameof(PersonResponse.PersonName),"Person Name" },
                { nameof(PersonResponse.Email),"Email" },
                { nameof(PersonResponse.Gender),"Gender"},
                { nameof(PersonResponse.Address),"Address" },
                { nameof(PersonResponse.CountryID),"Country" },
                {nameof(PersonResponse.DateOfBirth),"Date of birth" }
            };

            ViewBag.CurrentSearchBy = searchBy;
            ViewBag.CurrentSearchString = searchString;          
            List<PersonResponse?> persons = await _personsService.GetFilteredPersons(searchBy,searchString);

            ViewBag.CurrentSortBy = sortBy;
            ViewBag.CurrentSortOrder = sortOrderOption.ToString();
            List<PersonResponse> sortedPersons = await _personsService.GetSortedPersons(persons,sortBy,sortOrderOption);

            return View(sortedPersons);
        }

        [Route("persons/create")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(c =>
            new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });            

            return View();
        }

        [Route("persons/create")]
        [HttpPost]
        public async Task<IActionResult> Create(PersonAddRequest personAddRequest)
        {
            if(!ModelState.IsValid)
            {
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries;
                ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                return View(personAddRequest);
            }

            PersonResponse personAdded = await _personsService.AddPerson(personAddRequest);
            return RedirectToAction("Index","Persons");
        }

        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Edit(Guid personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByID(personID);
            if(personResponse == null)
            {
                return RedirectToAction("Index");
            }

            PersonUpdateRequest personUpdateRequest = personResponse.ToPersonUpdateRequest();

            List<CountryResponse> countries = await _countriesService.GetAllCountries();
            ViewBag.Countries = countries.Select(c =>
            new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });

            return View(personUpdateRequest);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Edit(PersonUpdateRequest request)
        {
            if(!ModelState.IsValid)
            {
               ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage).ToList();
                List<CountryResponse> countries = await _countriesService.GetAllCountries();
                ViewBag.Countries = countries.Select(c =>
                new SelectListItem() { Text = c.CountryName, Value = c.CountryID.ToString() });
                return View();
            }

             await _personsService.UpdatePerson(request);          

                      

            return RedirectToAction("Index", "Persons");
        }


        [HttpGet]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid? personID)
        {
            PersonResponse? personResponse = await _personsService.GetPersonByID(personID);
            if (personResponse == null)
            {
                return RedirectToAction("Index");
            }

             return View(personResponse);
        }

        [HttpPost]
        [Route("[action]/{personID}")]
        public async Task<IActionResult> Delete(Guid personID)
        {
            await _personsService.DeletePerson(personID);
            return RedirectToAction("Index", "Persons");
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> PersonsPDF()
        {
           List<PersonResponse?> persons= await _personsService.GetAllPersons();

            return new ViewAsPdf("PersonsPDF", persons, ViewData)
            {
                PageMargins = new Rotativa.AspNetCore.Options.Margins() { Top = 20, Bottom = 20, Left = 20, Right = 20 },
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
            };
        }

        [Route("[controller]/[action]")]
        public async Task<IActionResult> PersonsCSV()
        {
           MemoryStream memoryStream = await _personsService.GetPersonsCSV();
            return File(memoryStream, "application/octet-stream","persons.csv");
        }


        [Route("[controller]/[action]")]
        public async Task<IActionResult> PersonsExcel()
        {
            MemoryStream memoryStream = await _personsService.GetPersonsExcel();
            return File(memoryStream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "persons.xlsx");
                                     
        }

    }
}
