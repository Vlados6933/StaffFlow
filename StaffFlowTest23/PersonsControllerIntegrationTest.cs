using Fizzler.Systems.HtmlAgilityPack;
using HtmlAgilityPack;
using StaffFlowTest;
using Xunit;

namespace StaffFlow.Controllers
{
    public class PersonsControllerIntegrationTest(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client = factory.CreateClient();

        #region Index
        [Fact]
        public async Task Index_ToReturnView()
        {
            HttpResponseMessage response = await _client.GetAsync("/Persons/Index");

            Assert.True(response.IsSuccessStatusCode);

            string responseBody = await response.Content.ReadAsStringAsync();

            HtmlDocument html = new HtmlDocument();
            html.LoadHtml(responseBody);
            var document = html.DocumentNode;

            Assert.NotNull(document.QuerySelectorAll("table.persons"));
        }

        #endregion
    }
}
