using Microsoft.AspNetCore.Mvc.Filters;
using ServiceContracts.DTO;

namespace ContactManagerApp.Filters.ActionFilters
{
    public class PersonsListActionFilter : IActionFilter, IOrderedFilter
    {
        private readonly ILogger<PersonsListActionFilter> _logger;
        private readonly string _testvalue;
        public int Order { get; set; }


        public PersonsListActionFilter(ILogger<PersonsListActionFilter> logger,
            string testvalue, int order)
        {
            _logger = logger;
            _testvalue = testvalue;
            Order = order;
           
        }

        

        public void OnActionExecuted(ActionExecutedContext context)
        {
            _logger.LogInformation("PersonsListOnActionExecuted method");
            context.HttpContext.Response.Headers["testvalue"] = _testvalue;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            _logger.LogInformation("PersonsListOnActionExecuting method");
            context.HttpContext.Response.Headers["testvalue"] = _testvalue;

            if (context.ActionArguments.ContainsKey("searchBy"))
            {
                string? searchBy = Convert.ToString(context.ActionArguments["searchBy"]);

                if(!string.IsNullOrEmpty(searchBy) )
                {
                    var searchByOptions = new List<string>()
                    {
                        nameof(PersonResponse.PersonName),
                        nameof(PersonResponse.Email)
                    };

                    if(searchByOptions.Any(x => x == searchBy))
                    {
                        _logger.LogInformation("searchBy actual value is {searchBy}", context.ActionArguments["searchBy"]);
                    }
                }
            }

        }
    }
}
