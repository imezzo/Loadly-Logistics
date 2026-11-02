using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Infrastructure.Services.Auth;
using LoadlyAggregator.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace LoadlyAggregator.Infrastructure.Data.Seeders;

/// <summary>
/// Fat demo dataset so admin/company dashboards have something to show.
/// Idempotent: skips sections that already have DEMO-prefixed / name-matched rows.
/// </summary>
public class DemoDataSeeder
{
    private readonly ApplicationDbContext _db;
    private readonly IPasswordHashingService _hash;
    private const string DemoPwd = "password123";
    private const string MarkerPrefix = "DEMO-";

    public DemoDataSeeder(ApplicationDbContext db, IPasswordHashingService hash)
    {
        _db = db;
        _hash = hash;
    }

    public async Task SeedAsync()
    {
        Console.WriteLine("📦 Seeding demo dataset...");
        var adminId = await _db.Admins.Select(a => (long?)a.Id).FirstOrDefaultAsync() ?? 1L;
        var pwd = _hash.HashPassword(DemoPwd);

        await SeedLookupsAsync(adminId);
        await SeedUsersAsync(pwd);
        await SeedFacilitiesAndVehiclesAsync(adminId);
        await SeedContractsTripsInvoicesAsync(adminId);
        await SeedTargetsAsync();
        Console.WriteLine("✅ Demo dataset ready.");
    }

    private async Task SeedLookupsAsync(long adminId)
    {
        if (!await _db.FuelTypes.AnyAsync())
        {
            _db.FuelTypes.AddRange(
                new FuelType { NameEn = "Diesel", NameAr = "سولار", UnitPrice = 13.5m, AddedBy = adminId },
                new FuelType { NameEn = "Gasoline 95", NameAr = "بنزين 95", UnitPrice = 15.0m, AddedBy = adminId },
                new FuelType { NameEn = "CNG", NameAr = "غاز طبيعي", UnitPrice = 5.5m, AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + fuel types");
        }

        if (!await _db.Taxes.AnyAsync())
        {
            _db.Taxes.AddRange(
                new Tax { NameEn = "VAT 14%", NameAr = "ضريبة قيمة مضافة 14%", Type = TaxType.Percentage, Value = 14, Status = TaxStatus.ACTIVE, AddedBy = adminId },
                new Tax { NameEn = "Stamp Fixed", NameAr = "دمغة ثابتة", Type = TaxType.Fixed, Value = 50, Status = TaxStatus.ACTIVE, AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + taxes");
        }

        if (!await _db.Discounts.AnyAsync())
        {
            _db.Discounts.AddRange(
                new Discount { NameEn = "Early Pay 5%", NameAr = "خصم سداد مبكر 5%", Type = DiscountType.Percentage, Value = 5, Status = DiscountStatus.ACTIVE, AddedBy = adminId },
                new Discount { NameEn = "Promo 200", NameAr = "عرض 200", Type = DiscountType.Fixed, Value = 200, Status = DiscountStatus.ACTIVE, AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + discounts");
        }

        if (!await _db.Tags.AnyAsync())
        {
            string[] tags = ["Refrigerated", "Hazardous", "Express", "Bulk", "Container", "Local", "Export", "Import"];
            foreach (var t in tags)
                _db.Tags.Add(new Tag { NameEn = t, NameAr = t, IsPricing = t is "Express" or "Hazardous", AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + tags");
        }

        if (!await _db.Ports.AnyAsync())
        {
            _db.Ports.AddRange(
                new Port { NameEn = "Alexandria Port", NameAr = "ميناء الإسكندرية", Country = "EG", PortCode = "EGALY", Type = PortType.SEA, Status = PortStatus.ACTIVE, AddedBy = adminId },
                new Port { NameEn = "Damietta Port", NameAr = "ميناء دمياط", Country = "EG", PortCode = "EGDAM", Type = PortType.SEA, Status = PortStatus.ACTIVE, AddedBy = adminId },
                new Port { NameEn = "Sokhna Port", NameAr = "ميناء السخنة", Country = "EG", PortCode = "EGSOK", Type = PortType.SEA, Status = PortStatus.ACTIVE, AddedBy = adminId },
                new Port { NameEn = "Port Said", NameAr = "بورسعيد", Country = "EG", PortCode = "EGPSD", Type = PortType.SEA, Status = PortStatus.ACTIVE, AddedBy = adminId },
                new Port { NameEn = "Cairo Dry Port", NameAr = "الميناء الجاف بالقاهرة", Country = "EG", PortCode = "EGCAI", Type = PortType.LAND, Status = PortStatus.ACTIVE, AddedBy = adminId },
                new Port { NameEn = "10th of Ramadan Dry Port", NameAr = "الميناء الجاف بالعاشر", Country = "EG", PortCode = "EG10R", Type = PortType.LAND, Status = PortStatus.ACTIVE, AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + ports");
        }

        if (!await _db.Containers.AnyAsync())
        {
            foreach (var n in new[] { "20ft Dry", "40ft Dry", "40ft HC", "20ft Reefer", "40ft Reefer" })
                _db.Containers.Add(new Container { NameEn = n, NameAr = n, Status = ContainerStatus.ACTIVE, AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + containers");
        }

        if (!await _db.ShippingLines.AnyAsync())
        {
            foreach (var n in new[] { "Maersk", "MSC", "CMA CGM", "Hapag-Lloyd", "COSCO" })
                _db.ShippingLines.Add(new ShippingLine { NameEn = n, NameAr = n, Status = ShippingLineStatus.ACTIVE, AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + shipping lines");
        }

        if (!await _db.VehicleCapacityUnits.AnyAsync())
        {
            _db.VehicleCapacityUnits.AddRange(
                new VehicleCapacityUnit { CapacityTypeEn = "Tons", CapacityTypeAr = "طن", Status = UserStatus.ACTIVE, AddedBy = adminId },
                new VehicleCapacityUnit { CapacityTypeEn = "Cubic Meters", CapacityTypeAr = "متر مكعب", Status = UserStatus.ACTIVE, AddedBy = adminId },
                new VehicleCapacityUnit { CapacityTypeEn = "Pallets", CapacityTypeAr = "بالتات", Status = UserStatus.ACTIVE, AddedBy = adminId },
                new VehicleCapacityUnit { CapacityTypeEn = "TEU", CapacityTypeAr = "حاوية مكافئة", Status = UserStatus.ACTIVE, AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + capacity units");
        }

        if (!await _db.OperatingHours.AnyAsync())
        {
            _db.OperatingHours.AddRange(
                new OperatingHour { NameEn = "24/7", NameAr = "على مدار الساعة", AddedBy = adminId },
                new OperatingHour { NameEn = "Business Hours", NameAr = "ساعات العمل", AddedBy = adminId },
                new OperatingHour { NameEn = "Night Shift", NameAr = "وردية ليلية", AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + operating hours");
        }

        if (!await _db.VehicleBrands.AnyAsync())
        {
            var brands = new (string En, string Ar, string[] Models)[]
            {
                ("Mercedes", "مرسيدس", ["Actros", "Atego", "Arocs"]),
                ("Volvo", "فولفو", ["FH16", "FM", "FE"]),
                ("Scania", "سكانيا", ["R450", "S500", "G410"]),
                ("MAN", "مان", ["TGX", "TGS", "TGL"]),
                ("Iveco", "ايفيكو", ["S-Way", "Eurocargo"]),
            };
            foreach (var (en, ar, models) in brands)
            {
                var brand = new VehicleBrand { NameEn = en, NameAr = ar, Status = UserStatus.ACTIVE, AddedBy = adminId };
                _db.VehicleBrands.Add(brand);
                await _db.SaveChangesAsync();
                foreach (var m in models)
                    _db.VehicleBrandModels.Add(new VehicleBrandModel
                    {
                        NameEn = m, NameAr = m, VehicleBrandId = brand.Id, Status = UserStatus.ACTIVE, AddedBy = adminId
                    });
                await _db.SaveChangesAsync();
            }
            Console.WriteLine("  + vehicle brands/models");
        }

        if (!await _db.VehicleTypes.AnyAsync())
        {
            _db.VehicleTypes.AddRange(
                new VehicleType { NameEn = "Tractor Head", NameAr = "رأس جرار", Category = VehicleTypeCategory.VehicleHead, VehicleUnitType = VehicleUnitType.Head, Status = VehicleTypeStatus.ACTIVE, AddedBy = adminId },
                new VehicleType { NameEn = "Flat Trailer", NameAr = "مقطورة مسطحة", Category = VehicleTypeCategory.FlatTrailer, VehicleUnitType = VehicleUnitType.Tail, Status = VehicleTypeStatus.ACTIVE, AddedBy = adminId },
                new VehicleType { NameEn = "Fridge Truck", NameAr = "ثلاجة", Category = VehicleTypeCategory.Fridge, VehicleUnitType = VehicleUnitType.RIGID, Status = VehicleTypeStatus.ACTIVE, AddedBy = adminId },
                new VehicleType { NameEn = "Closed Box", NameAr = "صندوق مغلق", Category = VehicleTypeCategory.ClosedBox, VehicleUnitType = VehicleUnitType.RIGID, Status = VehicleTypeStatus.ACTIVE, AddedBy = adminId },
                new VehicleType { NameEn = "Light Truck", NameAr = "شاحنة خفيفة", Category = VehicleTypeCategory.LightTruck, VehicleUnitType = VehicleUnitType.RIGID, Status = VehicleTypeStatus.ACTIVE, AddedBy = adminId },
                new VehicleType { NameEn = "Side Trailer", NameAr = "مقطورة جانبية", Category = VehicleTypeCategory.TrailerWithSide, VehicleUnitType = VehicleUnitType.Tail, Status = VehicleTypeStatus.ACTIVE, AddedBy = adminId },
                new VehicleType { NameEn = "Pallet Carrier", NameAr = "ناقل بالتات", Category = VehicleTypeCategory.Pallet, VehicleUnitType = VehicleUnitType.RIGID, Status = VehicleTypeStatus.ACTIVE, AddedBy = adminId },
                new VehicleType { NameEn = "Trailer", NameAr = "مقطورة", Category = VehicleTypeCategory.Trailer, VehicleUnitType = VehicleUnitType.Tail, Status = VehicleTypeStatus.ACTIVE, AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + vehicle types");
        }

        if (!await _db.VehicleInsurances.AnyAsync())
        {
            var now = DateTime.UtcNow;
            _db.VehicleInsurances.AddRange(
                new VehicleInsurance { PolicyNo = "POL-DEMO-001", InsuranceProvider = "GIG Egypt", StartDate = now.AddYears(-1), EndDate = now.AddYears(1), Amount = 25000, Status = UserStatus.ACTIVE, AddedBy = adminId },
                new VehicleInsurance { PolicyNo = "POL-DEMO-002", InsuranceProvider = "Allianz", StartDate = now.AddMonths(-6), EndDate = now.AddMonths(18), Amount = 32000, Status = UserStatus.ACTIVE, AddedBy = adminId },
                new VehicleInsurance { PolicyNo = "POL-DEMO-003", InsuranceProvider = "Misr Insurance", StartDate = now.AddMonths(-3), EndDate = now.AddMonths(9), Amount = 18000, Status = UserStatus.ACTIVE, AddedBy = adminId });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + vehicle insurances");
        }
    }

    private async Task SeedUsersAsync(string pwd)
    {
        // Extra companies
        for (var i = 1; i <= 12; i++)
        {
            var user = $"demo-co-{i:D2}";
            if (await _db.Companies.AnyAsync(c => c.UserName == user)) continue;
            _db.Companies.Add(new Company
            {
                FullName = $"Demo Company {i}",
                UserName = user,
                Email = $"{user}@loadly.test",
                Phone = $"0102{i:D7}",
                Password = pwd,
                Status = i % 7 == 0 ? UserStatus.SUSPENDED : UserStatus.ACTIVE,
                EmailVerified = true,
                PhoneVerified = true,
                MfaChannel = CommunicationChannel.None,
                Type = CompanyType.COMPANY,
                CompanyId = null
            });
        }
        await _db.SaveChangesAsync();

        // Contractors
        for (var i = 1; i <= 8; i++)
        {
            var user = $"demo-contractor-{i:D2}";
            if (await _db.Contractors.AnyAsync(c => c.UserName == user)) continue;
            _db.Contractors.Add(new Contractor
            {
                FullName = $"Demo Contractor {i}",
                UserName = user,
                Email = $"{user}@loadly.test",
                Phone = $"0103{i:D7}",
                Password = pwd,
                NationalId = $"2990101{i:D7}",
                Status = UserStatus.ACTIVE,
                EmailVerified = true,
                PhoneVerified = true,
                MfaChannel = CommunicationChannel.None,
                ContractorStatus = ContractorStatus.ACTIVE,
                ApprovedAt = DateTime.UtcNow.AddDays(-i),
                Address = $"Cairo Industrial Zone {i}"
            });
        }
        await _db.SaveChangesAsync();

        // Collectors
        for (var i = 1; i <= 5; i++)
        {
            var user = $"demo-collector-{i:D2}";
            if (await _db.Collectors.AnyAsync(c => c.UserName == user)) continue;
            _db.Collectors.Add(new Collector
            {
                FullName = $"Demo Collector {i}",
                UserName = user,
                Email = $"{user}@loadly.test",
                Phone = $"0104{i:D7}",
                Password = pwd,
                Status = UserStatus.ACTIVE,
                EmailVerified = true,
                PhoneVerified = true,
                MfaChannel = CommunicationChannel.None
            });
        }
        await _db.SaveChangesAsync();

        // Drivers (many)
        var contractorIds = await _db.Contractors.OrderBy(c => c.Id).Select(c => c.Id).ToListAsync();
        for (var i = 1; i <= 40; i++)
        {
            var phone = $"0105{i:D7}";
            if (await _db.Drivers.AnyAsync(d => d.Phone == phone)) continue;
            _db.Drivers.Add(new Driver
            {
                FullName = $"Demo Driver {i}",
                FullNameAr = $"سائق تجريبي {i}",
                NationalId = $"2980101{i:D7}",
                UserName = $"driver-demo-{i:D2}",
                Email = $"driver-demo-{i:D2}@loadly.test",
                Phone = phone,
                Password = pwd,
                Status = UserStatus.ACTIVE,
                EmailVerified = true,
                PhoneVerified = true,
                MfaChannel = CommunicationChannel.None,
                WorkingStatus = DriverWorkingStatus.Available,
                ContractorId = contractorIds.Count > 0 ? contractorIds[(i - 1) % contractorIds.Count] : null
            });
        }
        await _db.SaveChangesAsync();

        // Leads
        if (await _db.Leads.CountAsync() < 20)
        {
            var adminId = await _db.Admins.Select(a => (long?)a.Id).FirstOrDefaultAsync();
            for (var i = 1; i <= 25; i++)
            {
                var phone = $"0106{i:D7}";
                if (await _db.Leads.AnyAsync(l => l.Phone == phone)) continue;
                _db.Leads.Add(new Lead
                {
                    FullName = $"Demo Lead {i}",
                    Phone = phone,
                    Email = $"lead{i}@loadly.test",
                    AddedBy = adminId
                });
            }
            await _db.SaveChangesAsync();
            Console.WriteLine("  + leads");
        }

        Console.WriteLine("  + users (companies/contractors/collectors/drivers)");
    }

    private async Task SeedFacilitiesAndVehiclesAsync(long adminId)
    {
        var companyId = await _db.Companies.Where(c => c.CompanyId == null).Select(c => c.Id).FirstOrDefaultAsync();
        if (companyId == 0) return;

        if (!await _db.VehicleFacilities.AnyAsync())
        {
            _db.VehicleFacilities.AddRange(
                new VehicleFacility { Name = "Cairo Main Garage", Address = "Nasr City", Lat = 30.05m, Lng = 31.34m, AddedBy = adminId, CompanyId = companyId, Status = VehicleFacilityStatus.ACTIVE, FacilityType = VehicleFacilityType.Garage },
                new VehicleFacility { Name = "Alex Parking", Address = "Alexandria", Lat = 31.20m, Lng = 29.92m, AddedBy = adminId, CompanyId = companyId, Status = VehicleFacilityStatus.ACTIVE, FacilityType = VehicleFacilityType.Parking },
                new VehicleFacility { Name = "10th Ramadan Workshop", Address = "10th of Ramadan", Lat = 30.30m, Lng = 31.74m, AddedBy = adminId, CompanyId = companyId, Status = VehicleFacilityStatus.ACTIVE, FacilityType = VehicleFacilityType.Workshop },
                new VehicleFacility { Name = "Giza Agency", Address = "Giza", Lat = 30.01m, Lng = 31.21m, AddedBy = adminId, CompanyId = companyId, Status = VehicleFacilityStatus.ACTIVE, FacilityType = VehicleFacilityType.Agency });
            await _db.SaveChangesAsync();
            Console.WriteLine("  + facilities");
        }

        if (await _db.Vehicles.CountAsync(v => v.PlateNo.StartsWith("DM")) >= 30) return;

        var types = await _db.VehicleTypes.OrderBy(t => t.Id).ToListAsync();
        var models = await _db.VehicleBrandModels.OrderBy(m => m.Id).Select(m => m.Id).ToListAsync();
        var fuels = await _db.FuelTypes.OrderBy(f => f.Id).Select(f => f.Id).ToListAsync();
        var colors = await _db.Colors.OrderBy(c => c.Id).Select(c => c.Id).ToListAsync();
        var units = await _db.VehicleCapacityUnits.OrderBy(u => u.Id).Select(u => u.Id).ToListAsync();
        var insurances = await _db.VehicleInsurances.OrderBy(i => i.Id).Select(i => i.Id).ToListAsync();
        var drivers = await _db.Drivers.OrderBy(d => d.Id).Select(d => new { d.Id, d.ContractorId }).ToListAsync();
        var facilities = await _db.VehicleFacilities.OrderBy(f => f.Id).Select(f => f.Id).ToListAsync();
        var sectors = await _db.Sectors.OrderBy(s => s.Id).Select(s => s.Id).ToListAsync();
        if (types.Count == 0 || drivers.Count == 0) return;

        var letters = new[] { "أ ب ج", "د ه و", "ز ح ط", "ك ل م", "ن س ع" };
        for (var i = 1; i <= 35; i++)
        {
            var plate = $"DM{i:D4}";
            if (await _db.Vehicles.AnyAsync(v => v.PlateNo == plate)) continue;
            var driver = drivers[(i - 1) % drivers.Count];
            var type = types[(i - 1) % types.Count];
            _db.Vehicles.Add(new Vehicle
            {
                VehicleTypeId = type.Id,
                PlateNo = plate,
                PlateLetters = letters[(i - 1) % letters.Length],
                MakeYear = 2018 + (i % 7),
                InsuranceId = insurances.Count > 0 ? insurances[(i - 1) % insurances.Count] : null,
                PurchaseDate = DateTime.UtcNow.AddYears(-(i % 5)).AddMonths(-(i % 11)),
                InitialOdometerReading = 50000 + i * 1375,
                VinNo = $"VINDEMO{i:D10}",
                ColorId = colors.Count > 0 ? colors[(i - 1) % colors.Count] : null,
                CapacityUnitId = units.Count > 0 ? units[(i - 1) % units.Count] : null,
                DriverId = driver.Id,
                AssignedParkingFacilityId = facilities.Count > 0 ? facilities[(i - 1) % facilities.Count] : null,
                ContractorId = driver.ContractorId,
                SectorId = sectors.Count > 0 ? sectors[(i - 1) % sectors.Count] : null,
                BrandModelID = models.Count > 0 ? models[(i - 1) % models.Count] : null,
                TankCapacity = 300 + i * 5,
                Capacity = 10 + (i % 20),
                FuelTypeId = fuels.Count > 0 ? fuels[(i - 1) % fuels.Count] : null,
                AddedBy = adminId,
                Status = i % 11 == 0 ? VehicleStatus.OutOfService : VehicleStatus.Available
            });
        }
        await _db.SaveChangesAsync();
        Console.WriteLine("  + vehicles");
    }

    private async Task SeedContractsTripsInvoicesAsync(long adminId)
    {
        if (await _db.Contracts.AnyAsync(c => c.ReferenceNumber.StartsWith(MarkerPrefix)))
        {
            Console.WriteLine("  ⏭️  DEMO contracts already present");
            return;
        }

        var companies = await _db.Companies.Where(c => c.CompanyId == null && c.Status == UserStatus.ACTIVE).OrderBy(c => c.Id).Take(8).ToListAsync();
        var vehicleTypes = await _db.VehicleTypes.OrderBy(t => t.Id).ToListAsync();
        var vehicles = await _db.Vehicles.Where(v => v.Status == VehicleStatus.Available).OrderBy(v => v.Id).ToListAsync();
        var drivers = await _db.Drivers.OrderBy(d => d.Id).ToListAsync();
        var contractors = await _db.Contractors.OrderBy(c => c.Id).ToListAsync();
        var collectors = await _db.Collectors.OrderBy(c => c.Id).ToListAsync();
        var taxId = await _db.Taxes.Select(t => (long?)t.Id).FirstOrDefaultAsync();
        var discountId = await _db.Discounts.Select(d => (long?)d.Id).FirstOrDefaultAsync();
        var leads = await _db.Leads.OrderBy(l => l.Id).ToListAsync();
        if (companies.Count == 0 || vehicleTypes.Count == 0) return;

        var contractTypes = new[] { ContractType.Included, ContractType.Excluded, ContractType.OnDemand, ContractType.Seasonal };
        var contracts = new List<Contract>();
        for (var i = 1; i <= 10; i++)
        {
            var company = companies[(i - 1) % companies.Count];
            var start = DateTime.UtcNow.Date.AddMonths(-2);
            var contract = new Contract
            {
                ReferenceNumber = $"{MarkerPrefix}CTR-{i:D3}",
                ReferenceSequence = 1000 + i,
                NameEn = $"Demo Contract {i}",
                NameAr = $"عقد تجريبي {i}",
                CompanyId = company.Id,
                ContractType = contractTypes[(i - 1) % contractTypes.Length],
                Price = 50000 + i * 7500,
                PaymentMethod = PaymentMethod.BankTransfer,
                PaymentDueDays = 30,
                BillingCycle = ContractBillingCycles.Monthly,
                StartDate = start,
                EndDate = start.AddYears(1),
                ConsumptionType = i % 2 == 0 ? ContractConsumptionTypes.Trips : ContractConsumptionTypes.KMs,
                CargoCommission = 0,
                MonthlyWorkingDays = 22,
                Status = i <= 7 ? ContractStatus.ACTIVE : (i == 8 ? ContractStatus.DRAFT : ContractStatus.PENDING_APPROVAL),
                TermsAndConditions = "Demo terms",
                AddedBy = adminId,
                SalesRepId = adminId,
                AutoAssignMode = i % 3 == 0,
                VehiclesCount = 3
            };
            _db.Contracts.Add(contract);
            contracts.Add(contract);
        }
        await _db.SaveChangesAsync();

        var slots = new List<ContractSlot>();
        var cvts = new List<ContractVehicleType>();
        foreach (var contract in contracts.Where(c => c.Status == ContractStatus.ACTIVE))
        {
            var vt = vehicleTypes[(int)(contract.Id % vehicleTypes.Count)];
            var cvt = new ContractVehicleType
            {
                ContractId = contract.Id,
                VehicleTypeId = vt.Id,
                ConsumptionRate = 1.2m,
                TripsPerMonth = 40,
                TripsPerDay = 2,
                MaxKms = 5000,
                MonthlyRental = 25000,
                DailyValue = 1200,
                OverConsumption = 5,
                Quantity = 3,
                MinBillableWeightTons = 5,
                MaxPayloadTons = 25,
                ExtraWorkingDayFees = 800,
                AddedBy = adminId
            };
            _db.ContractVehicleTypes.Add(cvt);
            cvts.Add(cvt);
        }
        await _db.SaveChangesAsync();

        var vehicleIdx = 0;
        foreach (var cvt in cvts)
        {
            for (var s = 0; s < 2; s++)
            {
                Vehicle? vehicle = null;
                if (vehicleIdx < vehicles.Count)
                {
                    vehicle = vehicles[vehicleIdx++];
                    vehicle.ContractId = cvt.ContractId;
                }
                var slot = new ContractSlot
                {
                    AssignedAt = DateTime.UtcNow.AddDays(-10),
                    ContractId = cvt.ContractId,
                    VehicleTypeId = cvt.VehicleTypeId,
                    VehicleId = vehicle?.Id,
                    Status = ContractSlotStatus.ACTIVE,
                    ConsumptionRate = cvt.ConsumptionRate,
                    TripsPerMonth = cvt.TripsPerMonth,
                    TripsPerDay = cvt.TripsPerDay,
                    MaxKms = cvt.MaxKms,
                    MonthlyRental = cvt.MonthlyRental,
                    DailyValue = cvt.DailyValue,
                    OverConsumption = cvt.OverConsumption,
                    MinBillableWeightTons = cvt.MinBillableWeightTons,
                    MaxPayloadTons = cvt.MaxPayloadTons,
                    ExtraWorkingDayFees = cvt.ExtraWorkingDayFees,
                    AddedBy = adminId
                };
                _db.ContractSlots.Add(slot);
                slots.Add(slot);
            }
        }
        await _db.SaveChangesAsync();
        Console.WriteLine($"  + {contracts.Count} contracts / {slots.Count} slots");

        // Trips across statuses
        var statuses = new[]
        {
            TripStatus.Pending, TripStatus.Scheduled, TripStatus.InProgress,
            TripStatus.Completed, TripStatus.Completed, TripStatus.Completed,
            TripStatus.Cancelled, TripStatus.Pending, TripStatus.InProgress, TripStatus.Scheduled
        };
        var activeContracts = contracts.Where(c => c.Status == ContractStatus.ACTIVE).ToList();
        var tripCount = 0;
        for (var i = 1; i <= 60; i++)
        {
            var contract = activeContracts[(i - 1) % activeContracts.Count];
            var slot = slots.FirstOrDefault(s => s.ContractId == contract.Id);
            var cvt = cvts.FirstOrDefault(c => c.ContractId == contract.Id);
            var driver = drivers[(i - 1) % drivers.Count];
            var vehicle = vehicles.Count > 0 ? vehicles[(i - 1) % vehicles.Count] : null;
            var contractor = contractors.Count > 0 ? contractors[(i - 1) % contractors.Count] : null;
            var status = statuses[(i - 1) % statuses.Length];
            var start = DateTime.UtcNow.AddDays(-(i % 25)).AddHours(i % 12);
            _db.Trips.Add(new Trip
            {
                ContractId = contract.Id,
                ContractVehicleTypeId = cvt?.Id,
                ContractSlotId = slot?.Id,
                VehicleId = vehicle?.Id,
                DriverId = driver.Id,
                ContractorId = contractor?.Id ?? driver.ContractorId,
                StartDate = start,
                EndDate = status is TripStatus.Completed or TripStatus.Cancelled ? start.AddHours(4 + i % 6) : null,
                FinancialDate = DateOnly.FromDateTime(start),
                Status = status,
                TripDocumentStatus = status == TripStatus.Completed ? TripDocumentStatus.Approved : TripDocumentStatus.Pending,
                DispatcherId = adminId,
                DispatcherNotes = $"Demo trip {i}",
                CompanyUserId = contract.CompanyId,
                ContractDistanceKm = 80 + i * 3,
                ActualDistanceKm = status == TripStatus.Completed ? 75 + i * 3 : null,
                ActualCargoWeightTons = 8 + (i % 10),
                CargoPrice = 1500 + i * 50,
                SpotPrice = null,
                ContactPersonName = $"Contact {i}",
                ContactPersonMobile = $"0107{i:D7}",
                HelpersCount = i % 3,
                NumberOfDays = 1,
                Type = TripType.OneWay,
                TotalTripCost = 2000 + i * 75,
                ContractorPrice = 1500 + i * 40,
                CollectorId = collectors.Count > 0 ? collectors[(i - 1) % collectors.Count].Id : null,
                LeadId = leads.Count > 0 && i % 4 == 0 ? leads[(i - 1) % leads.Count].Id : null,
                IsExternalTrip = false
            });
            tripCount++;
        }
        await _db.SaveChangesAsync();
        Console.WriteLine($"  + {tripCount} trips");

        // Invoices for completed-ish contracts
        var invSeq = 1;
        foreach (var contract in activeContracts.Take(6))
        {
            var start = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-1));
            var end = DateOnly.FromDateTime(DateTime.UtcNow);
            var sub = 40000 + invSeq * 3500m;
            var taxAmt = Math.Round(sub * 0.14m, 2);
            _db.Invoices.Add(new Invoice
            {
                InvoiceNumber = $"{MarkerPrefix}INV-{invSeq:D4}",
                Sequence = 2000 + invSeq,
                InvoiceType = InvoiceType.Trips,
                ContractId = contract.Id,
                StartDate = start,
                EndDate = end,
                IssueDate = end,
                DueDate = end.AddDays(30),
                SubTotal = sub,
                TripsSubTotal = sub,
                SlotChargesSubTotal = 0,
                TaxId = taxId,
                DiscountId = discountId,
                TaxType = TaxType.Percentage,
                TaxValue = 14,
                TaxAmount = taxAmt,
                DiscountType = DiscountType.Percentage,
                DiscountValue = 5,
                DiscountAmount = Math.Round(sub * 0.05m, 2),
                TotalAmount = sub + taxAmt - Math.Round(sub * 0.05m, 2),
                Status = invSeq % 3 == 0 ? InvoiceStatus.Paid : (invSeq % 3 == 1 ? InvoiceStatus.PartiallyPaid : InvoiceStatus.UnPaid),
                AddedBy = adminId
            });
            invSeq++;
        }
        await _db.SaveChangesAsync();
        Console.WriteLine($"  + {invSeq - 1} invoices");
    }

    private async Task SeedTargetsAsync()
    {
        var year = DateTime.UtcNow.Year;
        if (await _db.Targets.AnyAsync(t => t.Year == year)) return;
        for (var m = 1; m <= 12; m++)
        {
            _db.Targets.Add(new Target
            {
                Month = (TargetMonth)m,
                Year = year,
                TargetType = TargetType.Trips,
                TargetValue = 80 + m * 5
            });
            _db.Targets.Add(new Target
            {
                Month = (TargetMonth)m,
                Year = year,
                TargetType = TargetType.Cost,
                TargetValue = 200000 + m * 15000
            });
        }
        await _db.SaveChangesAsync();
        Console.WriteLine("  + dashboard targets");
    }
}
