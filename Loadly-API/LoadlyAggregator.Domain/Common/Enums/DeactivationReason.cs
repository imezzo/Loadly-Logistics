using LoadlyAggregator.Shared.Api;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum DeactivationReason
{
    [Display("Client Request", "بناء على طلب العميل")]
    CLIENT_REQUEST = 1,
    [Display("Breach of Terms", "مخالفة الشروط")]
    BREACH_OF_TERMS = 2,
    [Display("Other", "أخرى")]
    OTHER = 3
}
