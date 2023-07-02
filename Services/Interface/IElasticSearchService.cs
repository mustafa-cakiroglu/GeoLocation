using GeoLocationTest.Models.Input;
using GeoLocationTest.Models;

namespace GeoLocationTest.Services.Interface
{
    public interface IElasticSearchService
    {
        List<CustomerModel> GetGeoLocations(LocationDto locationDto);
        Task<bool> LoadBulkDataFromSqlAsync();
    }
}
