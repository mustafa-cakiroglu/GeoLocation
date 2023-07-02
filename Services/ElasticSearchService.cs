using AutoMapper;
using GeoLocationTest.Configuration.Interface;
using GeoLocationTest.Data;
using GeoLocationTest.Helper;
using GeoLocationTest.Models;
using GeoLocationTest.Models.Input;
using GeoLocationTest.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Nest;

namespace GeoLocationTest.Services
{
    public class ElasticSearchService : IElasticSearchService
    {
        private readonly IElasticClient _elasticClient;
        private readonly IMapper _mapper;
        private readonly IElasticSearchConfigration _elasticSearchConfigration;
        private readonly LocationContext dbContext;

        public ElasticSearchService(IElasticSearchConfigration elasticSearchConfigration,
            IUnitOfWork context)
        {
            _elasticSearchConfigration = elasticSearchConfigration ?? throw new ArgumentNullException(nameof(elasticSearchConfigration));
            dbContext = context as LocationContext ?? throw new ArgumentNullException(nameof(context));
            _elasticClient = GetClient();
        }

        public List<CustomerModel> GetGeoLocations(LocationDto locationDto)
        {

            var geoResult = _elasticClient.Search<CustomerModel>(s =>
                s.From(0)
                 .Size(10000)
                 .Query(query => query.Bool(b => b.Filter
                    (filter => filter
                        .GeoDistance
                            (
                              geo => geo
                                .Field(f => f.Location)
                                .Distance(locationDto.Distance).Location(locationDto.Latitude, locationDto.Longitude)
                                .DistanceType(GeoDistanceType.Plane)
                             )
                    )
                  )));

            foreach (var customer in geoResult.Documents)
            {
                customer.DistanceKm = CalculateDistance.GetDistanceFromLatLonInKm(
                    locationDto.Latitude,
                    locationDto.Longitude,
                    customer.Location.Latitude,
                    customer.Location.Longitude);
            }

            return geoResult.Documents.ToList();
        }

        public async Task<bool> LoadBulkDataFromSqlAsync()
        {

            var customerLocationList = dbContext.CustomerLocations.Where(s => !s.IsDeleted)
            .Include(s => s.Customer)
            .Select(s => new CustomerModel
            {
                CustomerId = s.CustomerId,
                CustomerName = s.Customer.Name,
                LocationId = s.Id,
                LocationName = s.Name,
                Location = new GeoLocation(Convert.ToDouble(s.Latitude), Convert.ToDouble(s.Longitude))
            }).ToList();


            var defaultIndex = "customerlocation";


            var existIndice = await _elasticClient.Indices.ExistsAsync(defaultIndex);

            if (existIndice.Exists)
            {
                var anyIndex = await _elasticClient.Indices.DeleteAsync(defaultIndex);
            }

            var defaultAlias = "location_alias";
            var existAlias = await _elasticClient.Indices.ExistsAsync(defaultAlias);

            if (!existAlias.Exists)
            {
               await _elasticClient.Indices.CreateAsync(defaultIndex, c => c
                    .Mappings(m => m
                        .Map<CustomerModel>(mm => mm
                            .AutoMap()
                        )
                    ).Aliases(a => a.Alias(defaultAlias))
                );
            }

            var bulkIndexer = new BulkDescriptor();

            foreach (var document in customerLocationList)
            {
                bulkIndexer.Index<CustomerModel>(i => i
                    .Document(document)
                    .Id(document.LocationId)
                    .Index(defaultIndex));
            }

            _elasticClient.Bulk(bulkIndexer);

            return true;
        }

        private ElasticClient GetClient()
        {
            var str = _elasticSearchConfigration.ConnectionString;
            var strs = str.Split('|');
            var nodes = strs.Select(s => new Uri(s)).ToList();

            var connectionString = new ConnectionSettings(new Uri(str))
                .DisablePing()
                .SniffOnStartup(false)
                .DefaultIndex("customerlocation")
                .DefaultMappingFor<CustomerModel>(m => m.IndexName("customerlocation"))
                .SniffOnConnectionFault(false);

            return new ElasticClient(connectionString);
        }
    }
}
