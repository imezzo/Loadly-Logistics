using System.ComponentModel.DataAnnotations;

namespace LoadlyAggregator.Domain.Common.Enums;

public enum LicenseCategory
{
    [Display(Name = "Private License")]
    PrivateLicense = 1,

    [Display(Name = "1st Professional License")]
    FirstProfessionalLicense = 2,

    [Display(Name = "2nd Professional License")]
    SecondProfessionalLicense = 3,

    [Display(Name = "3rd Professional License")]
    ThirdProfessionalLicense = 4
}
