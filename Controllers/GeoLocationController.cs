using GeoLocationTest.Models;
using GeoLocationTest.Models.Input;
using GeoLocationTest.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace GeoLocationTest.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GeoLocationController : ControllerBase
    {
        private readonly IElasticSearchService _elasticSearchService;

        public GeoLocationController(IElasticSearchService elasticSearchService)
        {
            _elasticSearchService = elasticSearchService;
        }

        [HttpPost]
        public List<CustomerModel> GetListofLocations(LocationDto locationDto)
        {
            var result = _elasticSearchService.GetGeoLocations(locationDto);

            return result;
        }

        [HttpGet]
        public async Task<bool> LoadLocations()
        {
            var result = await _elasticSearchService.LoadBulkDataFromSqlAsync();

            return result;
        }

    }
}
