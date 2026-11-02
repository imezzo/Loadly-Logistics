using System.ComponentModel.DataAnnotations;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum CollectorDocumentStatus
{
    [Display(Name = "Approved")]
    Approved = 1,

    [Display(Name = "Still With Customer Waiting For Signature")]
    StillWithCustomerWaitingForSignature = 2,

    [Display(Name = "Lost By Driver")]
    LostByDriver = 3,

    [Display(Name = "Trip Does Not Exist")]
    TripDoesNotExist = 4,

    [Display(Name = "Will Be Sent Later")]
    WillBeSentLater = 5,

    [Display(Name = "Customer Refused To Sign")]
    CustomerRefusedToSign = 6,

    [Display(Name = "Other")]
    Other = 7,

    [Display(Name = "Confirmed")]
    Confirmed = 8
}
