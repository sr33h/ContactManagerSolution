using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fizzler;
using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;

namespace ContactManagerAppTests
{
    public class PersonControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public PersonControllerIntegrationTests(CustomWebApplicationFactory factory)
        {
           _client = factory.CreateClient();
        }


        [Fact]
        public async Task Index_ShouldReturnView()
        {

           HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

            response.IsSuccessStatusCode.Should().BeTrue();

            string responseBody = await  response.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();

            html.LoadHtml(responseBody);

            var document = html.DocumentNode;

            document.QuerySelectorAll("table.persons").Should().NotBeNull();

        }
    }
}
