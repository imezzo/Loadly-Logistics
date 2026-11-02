using LoadlyAggregator.Domain.Admin.DTOs.Trips;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Helpers;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.Constants;

namespace LoadlyAggregator.Domain.Admin.Helpers;

public static class TripDtoDocumentsExtensions
{
    public static void EnrichTripDocuments(this TripDto trip, IMediaService mediaService)
    {
        ArgumentNullException.ThrowIfNull(trip);
        ArgumentNullException.ThrowIfNull(mediaService);

        TripRecordingDocumentExtractor.PopulateDocumentsFromRecordings(trip);
        ApplyDriverDocumentFileUrls(trip.Documents, mediaService);
        ApplyCollectorDocumentFileUrls(trip.CollectorDocuments, mediaService);
    }

    public static void EnrichAll(this IEnumerable<TripDto> trips, IMediaService mediaService)
    {
        ArgumentNullException.ThrowIfNull(trips);
        ArgumentNullException.ThrowIfNull(mediaService);

        foreach (var trip in trips)
            trip.EnrichTripDocuments(mediaService);
    }

    public static void EnrichContractConditions(this TripDto trip, Trip tripEntity, IMediaService mediaService)
    {
        ArgumentNullException.ThrowIfNull(trip);
        ArgumentNullException.ThrowIfNull(tripEntity);
        ArgumentNullException.ThrowIfNull(mediaService);

        trip.ContractConditions = BuildContractConditions(tripEntity, mediaService);
    }

    public static void EnrichAllContractConditions(
        this IList<TripDto> trips,
        IList<Trip> tripEntities,
        IMediaService mediaService)
    {
        ArgumentNullException.ThrowIfNull(trips);
        ArgumentNullException.ThrowIfNull(tripEntities);
        ArgumentNullException.ThrowIfNull(mediaService);

        if (trips.Count == 0)
            return;

        var conditionsByContractId = new Dictionary<long, ContractConditionsDocumentDto?>();

        if (trips.Count == tripEntities.Count)
        {
            for (var i = 0; i < trips.Count; i++)
                trips[i].ContractConditions = ResolveContractConditions(tripEntities[i], mediaService, conditionsByContractId);
            return;
        }

        var entityById = tripEntities.ToDictionary(t => t.Id);
        foreach (var trip in trips)
        {
            trip.ContractConditions = entityById.TryGetValue(trip.Id, out var entity)
                ? ResolveContractConditions(entity, mediaService, conditionsByContractId)
                : null;
        }
    }

    private static ContractConditionsDocumentDto? ResolveContractConditions(
        Trip tripEntity,
        IMediaService mediaService,
        Dictionary<long, ContractConditionsDocumentDto?> conditionsByContractId)
    {
        if (tripEntity.ContractId is not long contractId)
            return null;

        if (conditionsByContractId.TryGetValue(contractId, out var existingConditions))
            return existingConditions;

        var conditions = BuildContractConditions(tripEntity, mediaService);
        conditionsByContractId[contractId] = conditions;
        return conditions;
    }

    private static ContractConditionsDocumentDto? BuildContractConditions(Trip tripEntity, IMediaService mediaService)
    {
        if (tripEntity.ContractId is null || tripEntity.Contract is null)
            return null;

        if (tripEntity.Contract.ContractDocuments is not { Count: > 0 } contractDocuments)
            return null;

        var contractDocument = contractDocuments.FirstOrDefault(cd =>
            cd.Document?.NameEn == ContractDocumentNames.ContractConditions
            && !string.IsNullOrWhiteSpace(cd.Path));

        if (contractDocument?.Document is null)
            return null;

        return new ContractConditionsDocumentDto
        {
            DocumentId = contractDocument.DocumentId,
            NameEn = contractDocument.Document.NameEn,
            NameAr = contractDocument.Document.NameAr,
            FileUrl = mediaService.Read(
                ContractDocumentPathHelper.BuildFilePath(
                    tripEntity.ContractId.Value,
                    contractDocument.Document.NameEn,
                    contractDocument.Path!))
        };
    }

    private static void ApplyDriverDocumentFileUrls(List<TripDriverDocumentDto>? documents, IMediaService mediaService)
    {
        if (documents is null)
            return;
        foreach (var doc in documents)
        {
            doc.FileUrls = doc.Paths.Count == 0
                ? []
                : doc.Paths.ConvertAll(mediaService.Read);
        }
    }

    private static void ApplyCollectorDocumentFileUrls(List<TripDocumentDto>? documents, IMediaService mediaService)
    {
        if (documents is null)
            return;
        foreach (var doc in documents)
        {
            doc.FileUrls = doc.Paths.Count == 0
                ? []
                : doc.Paths.ConvertAll(mediaService.Read);
        }
    }
}
