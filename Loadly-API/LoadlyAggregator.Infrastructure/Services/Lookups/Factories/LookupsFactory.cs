using Microsoft.Extensions.DependencyInjection;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;
using LoadlyAggregator.Infrastructure.Services.Lookups.Entities;
using LoadlyAggregator.Shared.DTOs.Common;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Factories;





public class LookupFactory : ILookupFactory
{
    private readonly IServiceProvider _serviceProvider;

    public LookupFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<LookupResponse> ExecuteLookupAsync(string entity, string lookup, LookupRequest request)
    {
        var lookupInstance = GetLookupInstance(entity, lookup);
        return await lookupInstance.ExecuteAsync(request);
    }








    private Lookup GetLookupInstance(string entity, string lookup)
    {
        var mapping = LookupsMapping();

        if (!mapping.ContainsKey(entity))
        {
            throw new LogicalException("lookup.invalid_entity", entity, string.Join(", ", mapping.Keys));
        }

        if (!mapping[entity].ContainsKey(lookup))
        {
            throw new LogicalException(
                "lookup.invalid_lookup_for_entity",
                lookup,
                entity,
                string.Join(", ", mapping[entity].Keys));
        }

        var lookupType = mapping[entity][lookup];
        return (Lookup)_serviceProvider.GetRequiredService(lookupType);
    }




    private static Dictionary<string, Dictionary<string, Type>> LookupsMapping()
    {
        return new Dictionary<string, Dictionary<string, Type>>(StringComparer.OrdinalIgnoreCase)
        {

            ["cities"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(CitiesDefaultLookup)
            },
            ["districts"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(DistrictsDefaultLookup)
            },
            ["governerates"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(GoverneratesDefaultLookup)
            },
            ["sectors"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(SectorsDefaultLookup)
            },
            ["industrytypes"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(IndustryTypesDefaultLookup)
            },
            ["servicerequirements"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(ServiceRequirementsDefaultLookup)
            },
            ["communicationmethods"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(CommunicationMethodsDefaultLookup)
            },
            ["operatinghours"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(OperatingHoursDefaultLookup)
            },
            ["documentstatuses"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(DocumentStatusesDefaultLookup)
            },


            ["admins"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(AdminsDefaultLookup)
            },
            ["companies"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(CompaniesDefaultLookup)
            },
            ["drivers"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(DriversDefaultLookup)
            },
            ["contractors"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(ContractorsDefaultLookup)
            },
            ["collectors"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(CollectorsDefaultLookup)
            },
            ["contractvehicletypes"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(ContractVehicleTypesDefaultLookup)
            },
            ["vehicletypes"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(VehicleTypesDefaultLookup)
            },
            ["vehiclebrands"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(VehicleBrandsDefaultLookup)
            },
            ["vehiclebrandmodels"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(VehicleBrandModelsDefaultLookup)
            },
            ["vehiclecapacityunits"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(VehicleCapacityUnitsDefaultLookup)
            },
            ["vehicleinsurances"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(VehicleInsurancesDefaultLookup)
            },
            ["roles"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(RolesDefaultLookup)
            },
            ["extrafeetypes"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(ExtraFeeTypesLookup)
            },
            ["contractreferencenumbers"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(ContractReferenceNumberDefaultLookup)
            }
            ,
            ["taxes"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(TaxesDefaultLookup)
            },
            ["leads"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(LeadsDefaultLookup)
            },
            ["discounts"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(DiscountsDefaultLookup)
            },
            ["fueltypes"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(FuelTypesDefaultLookup)
            },
            ["tags"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(TagsDefaultLookup)
            },
            ["waypoints"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(WaypointsDefaultLookup)
            },
            ["contractdestinations"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(ContractDestinationsDefaultLookup)
            },
            ["contractbillingsnapshots"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(ContractBillingSnapshotsDefaultLookup)
            },
            ["vehiclefacilities"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(VehicleFacilitiesDefaultLookup)
            },
            ["ports"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(PortsDefaultLookup)
            },
            ["containers"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(ContainersDefaultLookup)
            },
            ["shippinglines"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["default"] = typeof(ShippingLinesDefaultLookup)
            }
        };
    }
}

