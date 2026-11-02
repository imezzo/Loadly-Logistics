# Loadly Aggregator - .NET 8 API Project

A comprehensive, production-ready .NET 8 API project with a clean architecture, implementing RBAC (Role-Based Access Control), multi-factor authentication, and comprehensive audit logging.

## 🏗️ Solution Architecture

```text
LoadlyAggregator.sln
│
├── 📁 LoadlyAggregator.API          # Entry point & Controllers
├── 📁 LoadlyAggregator.Domain       # Business Logic & Actions
├── 📁 LoadlyAggregator.Infrastructure # Data Access & External Services
└── 📁 LoadlyAggregator.Shared       # Common DTOs & Utilities
```

## ✨ Features

### User Management

- **Three Separate User Tables**: Admins, Companies, and Drivers
- **Flexible Login**: Support for login via username, email, or phone
- **Multi-Factor Authentication (MFA)**: Email or SMS-based 6-digit codes with stateless deterministic verification (no tokens, no DB storage)
- **Password Reset**: Secure forget/reset password flow with email notifications
- **Change Password**: Authenticated users can change their password with current password verification
- **Multi-Language Support**: API responses in English and Arabic based on `lang` header

### Security

- **JWT Authentication**: Token-based authentication per domain
- **BCrypt Password Hashing**: Secure password storage
- **RBAC System**: Flexible role and permission management
- **Stateless MFA**: Deterministic code generation using HMAC-SHA256 (no DB storage, no token generation)
- **Audit Logging**: Comprehensive tracking of all operations using Audit.NET

### Architecture

- **Clean Architecture**: Separation of concerns across layers
- **Request → Action → Response → Resource Pattern**: Clean separation of concerns with clear data flow
  - **Request** (API layer): HTTP request models with FluentValidation
  - **Action** (Domain layer): Business logic execution
  - **Response** (Domain layer): Domain response models
  - **Resource** (API layer): Response transformers for API formatting
  - **Pattern Variations**:
    - **CRUD Operations**: Request → Action → Response → Resource (Request used directly in Actions)
    - **Auth Operations**: Request → DTO → Action → Response → Resource (DTO conversion for auth-specific logic)
  - **Consistent Pattern**: All controllers (Admin, Company, Driver) follow the same pattern for consistency
- **Repository Pattern**: Generic repository for data access with soft delete support
- **Unit of Work**: Transaction management
- **Dependency Injection**: Proper IoC container setup
- **Exception Handling**: Global exception middleware
- **Action Pattern**: Business logic encapsulated in reusable action classes
- **Strategy Pattern**: MFA and other extensible behaviors without if statements
- **Service Facade Pattern**: Clean service interfaces delegating to actions
- **IF Statement Elimination**: All services and actions refactored to use design patterns (Builder, Strategy, Resolver, Validator) instead of conditional logic
  - **Validation Services**: `UserValidator` for all user validation checks
  - **Strategy Pattern**: `MfaMethodStrategyFactory` for MFA method selection
  - **Helper Methods**: `ClaimsHelper` for claim extraction without IF statements
  - **Method Extraction**: Private methods for separation of concerns
- **Soft Delete**: Automatic soft delete with timestamp management
- **Activity Logging**: Automatic logging of all CRUD operations
- **Translation System**: Dynamic message translation based on request headers
- **File Upload & Storage**: Agnostic file upload system with multi-cloud provider support
  - **Multiple Storage Providers**: Local, AWS S3, Azure Blob Storage, Google Cloud Storage
  - **Provider Selection**: Switch providers via configuration (no code changes)
  - **File Validation**: FluentValidation with size and MIME type validation
  - **CDN Support**: Automatic URL generation for uploaded files
  - **Organized Structure**: Logical directory organization for files
- **SMS/WhatsApp Service**: Agnostic messaging service with multiple provider support
  - **Multiple SMS Providers**: VictoryLink (SMS), UltraMsg (WhatsApp), Akedly.io (SMS)
  - **Provider Selection**: Switch providers via configuration (no code changes)
  - **Phone Number Normalization**: Automatic formatting per provider requirements
  - **Error Handling**: Comprehensive error mapping and logging
  - **MFA Integration**: Automatic SMS/WhatsApp support for multi-factor authentication
- **Redis Queue System**: Built-in message queue using Redis
  - **Background Processing**: Automatic message processing via background service
  - **Multiple Queues**: Support for multiple named queues (default, emails, notifications, etc.)
  - **Flexible Message Types**: JSON-based message serialization
  - **Configurable Polling**: Adjustable polling intervals and error retry delays
  - **Queue Management**: Enqueue, dequeue, queue length, and clear operations
  - **Processor Pattern**: Extensible message processor interface for custom handling
- **Google Maps Place Extraction**: Extract place data (lat, lng, address, placeId) from Google Maps URLs
  - **Endpoint**: `GET /api/admin/maps/extract-place?url={googleMapsUrl}` (requires `create_trip` permission)
  - **Supported URLs**: Short links (`maps.app.goo.gl`, `goo.gl/maps`) and full URLs (`www.google.com/maps/place/...`)
  - **Geocoding**: Uses [Nominatim](https://nominatim.org/) (OpenStreetMap) for address→coordinates when URL lacks lat/lng
  - **Fallback Geocoder**: [Photon](https://photon.komoot.io/) when Nominatim returns no results
  - **Reverse Geocoding**: Uses Nominatim for coordinates→address when URL has lat/lng but no address
  - **Egyptian Addresses**: Dynamic fallbacks for governorate-style addresses (e.g. "Assiut Governorate, Egypt")

## 🗂️ Database Schema

### User Tables

- **admins**: Administrative users
- **companies**: Company users
- **drivers**: Driver users

Each user table includes:

- Authentication fields (username, email, phone, password)
- MFA fields (mfa) - Code verification is stateless (no DB storage)
- Password reset fields (password_reset_token, password_reset_token_expires_at)
- Status management (email_verified, phone_verified, status)
- Timestamps (created_at, updated_at, deleted_at)
- Soft delete support (deleted_at for soft deletion)

### RBAC Tables

- **roles**: Role definitions per guard
- **permissions**: Permission definitions
- **permission_groups**: Logical grouping of permissions
- **model_has_roles**: User-to-Role assignments (polymorphic)
- **role_has_permissions**: Role-to-Permission mappings
- **model_has_permissions**: Direct user-to-Permission assignments

### Audit & Logs

Both audit logs and notifications logs are stored in a **separate logs database** (`LogsDbContext`) for better performance and separation of concerns:

- **audit_logs**: Comprehensive audit trail of all operations (CREATE, UPDATE, DELETE, LOGIN, LOGOUT)
- **notifications_log**: Log of all SMS and email notifications sent

**Note**: The logs database is configured via `ConnectionStrings:LogsConnection` in `appsettings.json`

## 🔐 Permission System (RBAC)

The project uses an ACL-based permission system where permissions are defined in configuration files and loaded via command line.

### Summary Pattern

For any new API endpoint, follow this pattern:

#### 1. Add Permission to ACL File

Edit the appropriate ACL file (`AdminAcl.cs`, `CompanyAcl.cs`, or `DriverAcl.cs`):

```csharp
public static Dictionary<string, List<string>> Permissions => new()
{
    ["group_name"] = new List<string>
    {
        "permission_name",
    }
}
```

**Example:**

```csharp
// AdminAcl.cs
["activity_logs"] = new List<string>
{
    "view_activity_log",
}
```

#### 2. Add to Request Class

Implement `IRequiresPermission` interface and add permission constant:

```csharp
using LoadlyAggregator.Domain.Common.Interfaces;

public class YourRequest : IRequiresPermission
{
    /// <summary>
    /// Required permission constant for this request
    /// </summary>
    public const string Permission = "permission_name";  // Must match ACL
    
    // ... other properties
    
    /// <summary>
    /// Required permission to access this endpoint
    /// </summary>
    public string RequiredPermission => Permission;
}
```

**Example:**

```csharp
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

public class GetActivityLogsRequest : BasePagedRequest, IRequiresPermission
{
    public const string Permission = "view_activity_log";
    
    public GetActivityLogsRequest()
    {
        PageSize = 15; // Override default PageSize for log requests
    }
    
    public string? EventType { get; set; }
    // ... other properties
    
    public string RequiredPermission => Permission;
}
```

#### 3. Use in Controller

Add `[HasPermission]` attribute referencing the request's permission constant:

```csharp
[HttpGet]
[HasPermission(YourRequest.Permission)]
public async Task<ActionResult> YourEndpoint([FromQuery] YourRequest request)
{
    // ... implementation
}
```

**Example:**

```csharp
[HttpGet]
[HasPermission(GetActivityLogsRequest.Permission)]
public async Task<ActionResult> GetActivityLogs([FromQuery] GetActivityLogsRequest request)
{
    // ... implementation
}
```

#### 4. Load Permissions

Run the permission loader command to create permissions in the database and assign them to super admin:

```bash
# Load admin permissions
dotnet run --project LoadlyAggregator.API -- --load permission admin

# Load company permissions
dotnet run --project LoadlyAggregator.API -- --load permission company

# Load driver permissions
dotnet run --project LoadlyAggregator.API -- --load permission driver

# Load collector permissions
dotnet run --project LoadlyAggregator.API -- --load permission collector

# Load all permissions
dotnet run --project LoadlyAggregator.API -- --load permission all
```

### Benefits

- ✅ **Single Source of Truth**: Permission defined in Request class
- ✅ **Type-Safe**: Using constants prevents typos
- ✅ **Discoverable**: Easy to see what permission is required
- ✅ **Reusable**: Can be referenced from middleware, validators, or other places
- ✅ **Centralized**: All permissions defined in ACL files

The permission is defined in the Request class, referenced in the controller, and loaded from the ACL file.

## 📄 Pagination System

The project uses a unified pagination pattern through the `BasePagedRequest` base class. All list request classes inherit from this base class to ensure consistent pagination behavior across the API.

### BasePagedRequest

All paginated request classes inherit from `BasePagedRequest` which provides:

- `Page` (int): Page number (1-based, default: 1)
- `PageSize` (int): Number of items per page (default: 10)

**Location**: `LoadlyAggregator.Shared/DTOs/Common/BasePagedRequest.cs`

### Usage Pattern

#### Basic List Request

```csharp
using LoadlyAggregator.Shared.DTOs.Common;

public class ListAdminsRequest : BasePagedRequest
{
    public string? Search { get; set; }
    public int? Type { get; set; }
    public int? Status { get; set; }
}
```

#### List Request with Permissions

```csharp
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Shared.DTOs.Common;

public class ListDriversRequest : BasePagedRequest, IRequiresPermission
{
    public const string Permission = "view_driver";
    
    public string? Search { get; set; }
    public int? Status { get; set; }
    
    public string RequiredPermission => Permission;
}
```

#### Custom PageSize Override

For requests that need a different default page size (e.g., log requests):

```csharp
using LoadlyAggregator.Shared.DTOs.Common;

public class GetNotificationsLogsRequest : BasePagedRequest
{
    public GetNotificationsLogsRequest()
    {
        PageSize = 15; // Override default PageSize
    }
    
    public int? Type { get; set; }
    // ... other properties
}
```

### Pagination Benefits

- ✅ **DRY Principle**: No duplicate `Page` and `PageSize` properties across request classes
- ✅ **Consistency**: All paginated endpoints use the same pagination properties
- ✅ **Maintainability**: Changes to pagination logic can be made in one place
- ✅ **Type Safety**: Compile-time checks ensure all list requests have pagination
- ✅ **Flexibility**: Easy to override default `PageSize` when needed

### Query Parameters

All paginated endpoints accept:

- `page` (int, optional): Page number (default: 1)
- `pageSize` (int, optional): Items per page (default: 10, or custom default if overridden)

**Example:**

```bash
GET /api/admin/admins?page=2&pageSize=20&search=john
```

### Response Format

Paginated responses include a `Pagination` object:

```json
{
  "success": true,
  "data": [...],
  "pagination": {
    "page": 2,
    "pageSize": 20,
    "totalCount": 150,
    "totalPages": 8
  }
}
```

### Validation

Use `PaginationValidationRules` extension methods in FluentValidation validators:

```csharp
using LoadlyAggregator.API.Validators.Extensions;

public class ListAdminsRequestValidator : AbstractValidator<ListAdminsRequest>
{
    public ListAdminsRequestValidator()
    {
        RuleFor(x => x.Page).IsValidPage();
        RuleFor(x => x.PageSize).IsValidPageSize(maxSize: 100);
    }
}
```

## 🔍 Lookup System

The project implements a flexible lookup pattern for providing paginated lookup data for dropdowns and selectors. Lookups support optional authentication with automatic data separation based on user type.

### How Lookups Work

Lookups follow a factory pattern where:

- **Route**: `GET /api/lookups/{entity}/{lookup}`
- **Factory**: `LookupsFactory` maps entity names to lookup classes
- **Base Class**: `BaseLookup<TEntity>` - Unified base class for all entities with flexible field selection
- **Response Format**: `{ id: number, value: string, phone?: string, email?: string, code?: string }` (optional fields only included when specified)
- **Authentication**: Optional - endpoints work with or without authentication, but data separation applies when authenticated

### Adding a New Lookup

To add a new lookup endpoint, follow these steps:

#### Step 1: Create Lookup Class

Create a new lookup class in `LoadlyAggregator.Infrastructure/Services/Lookups/Entities/`:

**Basic Example (NameEn/NameAr):**

```csharp
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Repositories.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;
using System.Linq.Expressions;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

/// <summary>
/// Default lookup for YourEntity entity
/// Returns NameEn or NameAr based on language header
/// </summary>
public class YourEntityDefaultLookup : BaseLookup<YourEntity>
{
    public YourEntityDefaultLookup(
        Infrastructure.Repositories.Interfaces.IYourEntityRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<YourEntity, string>> GetValueSelector()
    {
        // Language-aware selector
        var language = LanguageService.GetCurrentLanguage();
        return language == "ar" 
            ? x => x.NameAr ?? x.NameEn ?? string.Empty
            : x => x.NameEn ?? x.NameAr ?? string.Empty;
    }
}
```

**Example with FullName:**

```csharp
public class YourEntityDefaultLookup : BaseLookup<YourEntity>
{
    public YourEntityDefaultLookup(
        Infrastructure.Repositories.Interfaces.IYourEntityRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<YourEntity, string>> GetValueSelector()
        => x => x.FullName;
}
```

**Example with Additional Fields (Phone, Email):**

```csharp
public class YourEntityDefaultLookup : BaseLookup<YourEntity>
{
    // ... constructor ...

    protected override Expression<Func<YourEntity, string>> GetValueSelector()
        => x => x.FullName;

    // Optional: Add additional fields to response
    protected override Dictionary<string, Expression<Func<YourEntity, object?>>> GetAdditionalFields()
        => new()
        {
            { "Phone", x => x.Phone },
            { "Email", x => x.Email }
        };
}
```

#### Step 2: Register in Factory

Add your lookup to `LookupsFactory.cs`:

```csharp
// In LookupsMapping() method
["yourentities"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
{
    ["default"] = typeof(YourEntityDefaultLookup)
}
```

**Important:** Use the plural form of your entity name (e.g., `cities`, `admins`, `companies`).

#### Step 3: Register in Dependency Injection

Add your lookup class to `ServiceCollectionExtensions.cs`:

```csharp
// In RegisterLookupServices() method
services.AddScoped<Infrastructure.Services.Lookups.Entities.YourEntityDefaultLookup>();
```

#### Step 4: Add to Postman Collection (Optional)

Add your lookup endpoint to `LoadlyAggregator.postman_collection.json`:

```json
{
    "name": "Your Entity Lookup",
    "request": {
        "method": "GET",
        "header": [
            {
                "key": "lang",
                "value": "{{lang}}",
                "type": "text"
            }
        ],
        "url": {
            "raw": "{{baseUrl}}/api/lookups/yourentities/default?page=1&limit=20",
            "host": ["{{baseUrl}}"],
            "path": ["api", "lookups", "yourentities", "default"],
            "query": [
                {
                    "key": "page",
                    "value": "1"
                },
                {
                    "key": "limit",
                    "value": "20"
                }
            ]
        },
        "description": "Get your entity lookup (returns NameEn or NameAr based on lang header)"
    },
    "response": []
}
```

### Example: Adding a "Colors" Lookup

**1. Create Lookup Class:**

```csharp
// LoadlyAggregator.Infrastructure/Services/Lookups/Entities/ColorsDefaultLookup.cs
using Microsoft.AspNetCore.Http;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Repositories.Interfaces;
using LoadlyAggregator.Infrastructure.Services.Lookups.Base;
using System.Linq.Expressions;

namespace LoadlyAggregator.Infrastructure.Services.Lookups.Entities;

public class ColorsDefaultLookup : BaseLookup<Color>
{
    public ColorsDefaultLookup(
        Infrastructure.Repositories.Interfaces.IColorRepository repository,
        ILanguageService languageService,
        IHttpContextAccessor httpContextAccessor)
        : base(repository, languageService, httpContextAccessor)
    {
    }

    protected override Expression<Func<Color, string>> GetValueSelector()
    {
        var language = LanguageService.GetCurrentLanguage();
        return language == "ar" 
            ? x => x.NameAr ?? x.NameEn ?? string.Empty
            : x => x.NameEn ?? x.NameAr ?? string.Empty;
    }
}
```

**2. Add to Factory:**

```csharp
// In LookupsFactory.LookupsMapping()
["colors"] = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
{
    ["default"] = typeof(ColorsDefaultLookup)
}
```

**3. Register in DI:**

```csharp
// In ServiceCollectionExtensions.RegisterLookupServices()
services.AddScoped<Infrastructure.Services.Lookups.Entities.ColorsDefaultLookup>();
```

**4. Usage:**

```bash
# Get colors lookup (English)
GET /api/lookups/colors/default?page=1&limit=20
Header: lang: en

# Get colors lookup (Arabic)
GET /api/lookups/colors/default?page=1&limit=20
Header: lang: ar
```

**Response:**

```json
{
    "success": true,
    "data": {
        "data": [
            { "id": 1, "value": "Red" },      // or "أحمر" if lang=ar
            { "id": 2, "value": "Blue" }      // or "أزرق" if lang=ar
        ],
        "pagination": {
            "page": 1,
            "limit": 20,
            "total": 2
        }
    },
    "message": "lookup.retrieved_successfully"
}
```

**Response with Additional Fields** (e.g., Admins lookup):

```json
{
    "success": true,
    "data": {
        "data": [
            { 
                "id": 1, 
                "value": "John Doe",
                "phone": "01000665931",
                "email": "john@example.com"
            }
        ],
        "pagination": {
            "page": 1,
            "limit": 20,
            "total": 1
        }
    },
    "message": "lookup.retrieved_successfully"
}
```

**Note**: Optional fields (`phone`, `email`, `code`) are only included when specified in `GetAdditionalFields()` and are automatically excluded from JSON when null.

### Supported Entities

**Entities with NameEn/NameAr:**

- `cities` - Cities lookup
- `districts` - Districts lookup
- `governerates` - Governerates lookup
- `industrytypes` - Industry Types lookup
- `servicerequirements` - Service Requirements lookup
- `communicationmethods` - Communication Methods lookup
- `packages` - Packages lookup

**Entities with FullName:**

- `admins` - Admins lookup
- `companies` - Companies lookup
- `drivers` - Drivers lookup

### Features

- ✅ **Unified Base Class**: Single `BaseLookup<TEntity>` class for all entities
- ✅ **Flexible Field Selection**: Override `GetValueSelector()` to specify which property to use
- ✅ **Optional Additional Fields**: Add Phone, Email, Code via `GetAdditionalFields()` method
- ✅ **Language-Aware**: Automatically returns `NameEn` or `NameAr` based on `lang` header
- ✅ **Pagination**: Built-in pagination support via `BasePagedRequest` (`page`, `pageSize`)
- ✅ **Filtering**: Query parameters automatically applied as filters
- ✅ **Sorting**: Support for `sortBy` and `sortOrder` query parameters
- ✅ **Optional Authentication**: Endpoints work with or without authentication
- ✅ **Data Separation**: Automatic filtering based on user type when authenticated
- ✅ **Factory Pattern**: Easy to extend with new entities
- ✅ **Type-Safe**: Compile-time checks for entity mappings

### Lookup Query Parameters

All lookup endpoints support pagination via `LookupRequest` which inherits from `BasePagedRequest`:

- `page` (int): Page number (default: 1)
- `limit` (int): Items per page (default: 10) - Note: Lookups use `limit` for backward compatibility, while other endpoints use `pageSize`
- `sortBy` (string): Field to sort by (e.g., `NameEn`, `Id`)
- `sortOrder` (string): Sort order (`asc` or `desc`)
- Entity-specific filters: Any property name as query parameter (e.g., `governerateId`, `cityId`, `status`)

### Language Support

Set the `lang` header to control the response language:

- `lang: en` → Returns `NameEn` or `FullName`
- `lang: ar` → Returns `NameAr` or `FullName`

If `NameAr` is null, it falls back to `NameEn` or empty string (handled automatically).

### Authentication & Data Separation

Lookups support optional authentication. When a valid JWT token is provided:

**Non-User Entities** (cities, districts, etc.):

- All authenticated users see all data

**User Entities** (admins, companies, drivers):

- **Admin**: Sees all data
- **Company**: Sees only their own company, all drivers
- **Driver**: Sees only their own driver record
- **No Token**: Returns empty results for user entities

**Note**: Null fields (Phone, Email, Code) are automatically excluded from JSON responses when not populated.

## 🚀 Getting Started

### Prerequisites

Before you begin, ensure you have the following installed:

- **.NET 8 SDK** - [Download here](https://dotnet.microsoft.com/download/dotnet/8.0)
- **PostgreSQL 17.6** - [Download here](https://www.postgresql.org/download/)
- **Redis** - [Download here](https://redis.io/download) (for queue functionality)
- **dotnet-ef tool** - For Entity Framework Core migrations

### Step 1: Install .NET 8 SDK

Verify your installation:

```bash
dotnet --version
# Should output: 8.0.x or higher
```

### Step 2: Install PostgreSQL

**macOS (using Homebrew):**

```bash
brew install postgresql@17
brew services start postgresql@17
```

**Linux (Ubuntu/Debian):**

```bash
# Add PostgreSQL official repository for version 17
sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt $(lsb_release -cs)-pgdg main" > /etc/apt/sources.list.d/pgdg.list'
wget --quiet -O - https://www.postgresql.org/media/keys/ACCC4CF8.asc | sudo apt-key add -
sudo apt update
sudo apt install postgresql-17 postgresql-contrib-17
sudo systemctl start postgresql@17-main
sudo systemctl enable postgresql@17-main
```

**Windows:**
Download and install PostgreSQL 17.6 from [PostgreSQL official website](https://www.postgresql.org/download/windows/)

**Verify PostgreSQL is running:**

```bash
psql --version
# Should output: psql (PostgreSQL) 17.6 or higher
```

### Step 3: Install Redis

**macOS (using Homebrew):**

```bash
brew install redis
brew services start redis
```

**Linux (Ubuntu/Debian):**

```bash
sudo apt update
sudo apt install redis-server
sudo systemctl start redis-server
sudo systemctl enable redis-server
```

**Windows:**
Download and install Redis from [Redis official website](https://redis.io/download) or use WSL2.

**Verify Redis is running:**

```bash
redis-cli ping
# Should output: PONG
```

**Note:** Redis is required for the queue system. If you don't need queue functionality, you can skip this step, but the application will fail to start if Redis is not available.

### Step 4: Create PostgreSQL Database

**Connect to PostgreSQL:**

```bash
# macOS/Linux
psql -U postgres

# Or if you have a specific user
psql -U your_username
```

**Create the databases:**

```sql
-- Main application database
CREATE DATABASE loadly;

-- Logs database (for audit logs and notifications)
CREATE DATABASE loadly_logs;
```

**Verify database creation:**

```sql
\l
-- You should see 'loadly' and 'loadly_logs' in the list
```

**Exit PostgreSQL:**

```sql
\q
```

### Step 5: Install Entity Framework Core Tools

```bash
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
```

**Note for macOS/Linux:** If the command is not found, add dotnet tools to your PATH:

```bash
export PATH="$PATH:$HOME/.dotnet/tools"
# Add this to your ~/.zshrc or ~/.bashrc to make it permanent
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zshrc
```

### Step 6: Clone and Setup Project

**Clone the repository:**

```bash
git clone <repository-url>
cd Loadly
```

**Restore NuGet packages:**

```bash
dotnet restore
```

### Step 7: Configure Database Connection

Edit `LoadlyAggregator.API/appsettings.json` and update the connection string and other settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=loadly;Username=your_username;Password=your_password",
    "LogsConnection": "Host=localhost;Database=loadly_logs;Username=your_username;Password=your_password",
    "Redis": "localhost:6379"
  },
  "JwtSettings": {
    "SecretKey": "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong",
    "Issuer": "LoadlyAggregator",
    "Audience": "LoadlyAggregator",
    "ExpirationInMinutes": 60
  },
  "SmtpSettings": {
    "Host": "sandbox.smtp.mailtrap.io",
    "Port": 2525,
    "Username": "your_smtp_username",
    "Password": "your_smtp_password",
    "FromEmail": "noreply@loadlyaggregator.com",
    "FromName": "Loadly Aggregator",
    "EnableSsl": true,
    "UseDefaultCredentials": false
  },
  "Sms": {
    "Provider": "victory_link",
    "VictoryLink": {
      "Username": "your-victory-link-username",
      "Password": "your-victory-link-password",
      "SenderName": "your-sender-name",
      "ApiUrl": "https://api.victorylink.com/send"
    },
    "UltraMsg": {
      "Token": "your-ultramsg-token",
      "InstanceId": "your-ultramsg-instance-id"
    },
    "Akedly": {
      "ApiKey": "your-akedly-api-key",
      "PipelineId": "your-akedly-pipeline-id",
      "ApiUrl": "https://api.akedly.io/api/v1"
    }
  },
  "Media": {
    "StorageProvider": "local",
    "ImagesServerDomain": "http://localhost:5000",
    "CdnMainFolder": "loadly-images",
    "StoragePath": "wwwroot/uploads"
  },
  "Queue": {
    "Queues": [ "default", "emails", "notifications" ],
    "PollingIntervalMs": 1000,
    "ErrorRetryDelayMs": 5000
  }
}
```

**SMS/WhatsApp Configuration:**

For SMS and WhatsApp messaging, configure the provider:

```json
{
  "Sms": {
    "Provider": "victory_link",
    "VictoryLink": {
      "Username": "your-username",
      "Password": "your-password",
      "SenderName": "your-sender-name",
      "ApiUrl": "https://api.victorylink.com/send"
    },
    "UltraMsg": {
      "Token": "your-token",
      "InstanceId": "your-instance-id"
    },
    "Akedly": {
      "ApiKey": "your-api-key",
      "PipelineId": "your-pipeline-id",
      "ApiUrl": "https://api.akedly.io/api/v1"
    }
  }
}
```

**SMS Provider Options:**

- `"victory_link"` - VictoryLink SMS provider (default)
- `"ultramsg"` - UltraMsg WhatsApp provider
- `"akedly"` - Akedly.io SMS provider

**Media Configuration:**

For file uploads, configure the storage provider:

```json
{
  "Media": {
    "StorageProvider": "local",
    "ImagesServerDomain": "http://localhost:5000",
    "CdnMainFolder": "loadly-images",
    "StoragePath": "wwwroot/uploads"
  }
}
```

**Storage Provider Options:**

- `"local"` - Local filesystem (default, for development)
- `"s3"` - AWS S3 (requires AWS SDK and configuration)
- `"azure"` - Azure Blob Storage (requires Azure SDK and configuration)
- `"gcs"` - Google Cloud Storage (requires GCS SDK and configuration)

See `STORAGE_PROVIDERS.md` for detailed cloud provider setup instructions.

**Connection String Format:**

```text
Host=localhost;Database=loadly;Username=postgres;Password=your_password
```

**Redis Configuration:**

The queue system requires Redis. The Redis connection string is configured in the `ConnectionStrings` section (see above). Queue processing is configured in the `Queue` section:

```json
{
  "Queue": {
    "Queues": [ "default", "emails", "notifications" ],
    "PollingIntervalMs": 1000,
    "ErrorRetryDelayMs": 5000
  }
}
```

**Queue Configuration Options:**

- `Queues`: List of queue names to process (default: `["default"]`)
- `PollingIntervalMs`: Delay in milliseconds when queue is empty (default: `1000`)
- `ErrorRetryDelayMs`: Delay in milliseconds before retrying after an error (default: `5000`)

**Redis Connection String:**

- Default: `localhost:6379`
- With password: `localhost:6379,password=your_password`
- With database: `localhost:6379,defaultDatabase=0`

**Common PostgreSQL connection strings:**

- **Local default:** `Host=localhost;Database=loadly;Username=postgres;Password=postgres`
- **Custom user:** `Host=localhost;Database=loadly;Username=moaaz;Password=`
- **With port:** `Host=localhost;Port=5432;Database=loadly;Username=postgres;Password=postgres`

**Note:**

- The main application database name is `loadly` by default (configurable in `DefaultConnection`)
- The logs database name is `loadly_logs` by default (configurable in `LogsConnection`)
- Update these database names in `appsettings.json` to match your PostgreSQL setup

### Step 8: Apply Database Migrations

Run the following command to create all tables in the primary application database:

```bash
dotnet ef database update --project LoadlyAggregator.Infrastructure --startup-project LoadlyAggregator.API
```

**Logs database (audit & notifications):**

Audit logs and notification logs are stored in a separate database (`LogsDbContext`) for better performance. After setting `ConnectionStrings:LogsConnection` in `appsettings.json`, apply the LogsDb migrations:

```bash
dotnet ef database update \
  --context LogsDbContext \
  --project LoadlyAggregator.Infrastructure \
  --startup-project LoadlyAggregator.API
```

**Note**: Make sure `LogsConnection` is configured in `appsettings.json` before running this command.

**Expected output:**

```text
Applying migration 'CreateNotificationsLogTable'.
Applying migration 'CreateAuditLogsTable'.
Done.
```

**If you encounter issues:**

**Migration not found error:**

```bash
# Create a new migration
dotnet ef migrations add InitialCreate --project LoadlyAggregator.Infrastructure --startup-project LoadlyAggregator.API
# Then apply it
dotnet ef database update --project LoadlyAggregator.Infrastructure --startup-project LoadlyAggregator.API
```

**Connection string error:**

- Verify your PostgreSQL is running: `pg_isready`
- Check your connection string in `appsettings.json`
- Test connection: `psql -U your_username -d loadly`

### Step 9: Build the Project

```bash
dotnet build
```

**Expected output:**

```text
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Step 10: Run the Application

```bash
dotnet run --project LoadlyAggregator.API
```

**Expected output:**

```text
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5050
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:5051
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
```

### Step 11: Access the API

**Swagger UI (API Documentation):**

- **HTTP:** <http://localhost:5050/swagger>
- **HTTPS:** <https://localhost:5051/swagger>

**API Base URL:**

- **HTTP:** <http://localhost:5050>
- **HTTPS:** <https://localhost:5051>

**Language Support:**

All API endpoints support multi-language responses via the `lang` header:

- `lang: en` - English messages
- `lang: ar` - Arabic messages

Example:

```bash
curl -X POST http://localhost:5050/api/admin/auth/logout \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "lang: ar"
# Response will be in Arabic: "تم تسجيل الخروج بنجاح"
```

### Step 12: Verify Seeded Data

On first run, the application automatically seeds a super admin user:

**Credentials:**

- **Email:** `super@sw-ag.com`
- **Username:** `super-admin`
- **Phone:** `01000665931`
- **Password:** `password@sw-ag`

**Verify in database:**

```bash
psql -U your_username -d loadly
```

```sql
SELECT id, full_name, user_name, email, phone, status, verified FROM admins;
-- You should see the super admin user
```

### Quick Start Commands Summary

```bash
# 1. Install EF Core tools
dotnet tool install --global dotnet-ef

# 2. Restore packages
dotnet restore

# 3. Update database connection in appsettings.json
# Edit: LoadlyAggregator.API/appsettings.json

# 4. Apply migrations
dotnet ef database update --project LoadlyAggregator.Infrastructure --startup-project LoadlyAggregator.API

# 5. Build
dotnet build

# 6. Run
dotnet run --project LoadlyAggregator.API
```

### Common Issues and Solutions

#### Issue: Port already in use

```bash
# Find process using port 5050
lsof -i :5050

# Kill the process
kill -9 <PID>
```

#### Issue: Database connection failed

```bash
# Check PostgreSQL is running
pg_isready

# Test connection manually
psql -U your_username -d loadly -h localhost
```

#### Issue: Migration errors

```bash
# Remove all migrations and recreate
dotnet ef migrations remove --project LoadlyAggregator.Infrastructure --startup-project LoadlyAggregator.API
dotnet ef migrations add InitialCreate --project LoadlyAggregator.Infrastructure --startup-project LoadlyAggregator.API
dotnet ef database update --project LoadlyAggregator.Infrastructure --startup-project LoadlyAggregator.API
```

#### Issue: dotnet-ef command not found (macOS/Linux)

```bash
# Add to PATH
export PATH="$PATH:$HOME/.dotnet/tools"

# Make permanent
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zshrc
source ~/.zshrc
```

### Testing the API

**Using Postman:**

1. Import the `LoadlyAggregator.postman_collection.json` file
2. Create a new environment named "Local"
3. Set the following variables:
   - `baseUrl`: `http://localhost:5050`
   - `lang`: `en` (or `ar` for Arabic)
4. Test the Admin Login endpoint with the super admin credentials
5. The collection automatically saves tokens to environment variables:
   - `adminToken` - After admin login
   - `companyToken` - After company login
   - `driverToken` - After driver login
6. All authenticated endpoints use Bearer token authentication
7. All requests include the `lang` header for multi-language support

**Using cURL:**

```bash
# Admin Login (English)
curl -X POST http://localhost:5050/api/admin/auth/login \
  -H "Content-Type: application/json" \
  -H "lang: en" \
  -d '{
    "login": "super-admin",
    "password": "password@sw-ag"
  }'

# Admin Login (Arabic)
curl -X POST http://localhost:5050/api/admin/auth/login \
  -H "Content-Type: application/json" \
  -H "lang: ar" \
  -d '{
    "login": "super-admin",
    "password": "password@sw-ag"
  }'

# Get Profile (after login, use the accessToken from response)
curl -X GET http://localhost:5050/api/admin/auth/profile \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "lang: en"

# Change Password
curl -X POST http://localhost:5050/api/admin/auth/change-password \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -H "lang: en" \
  -d '{
    "currentPassword": "password@sw-ag",
    "newPassword": "newpassword123",
    "confirmPassword": "newpassword123"
  }'

# Enable MFA
curl -X POST http://localhost:5050/api/admin/auth/enable-mfa \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -H "lang: en" \
  -d '{
    "method": "email"
  }'

# Send MFA Code
curl -X POST http://localhost:5050/api/admin/auth/send-mfa-code \
  -H "Authorization: Bearer YOUR_ACCESS_TOKEN" \
  -H "Content-Type: application/json" \
  -H "lang: en" \
  -d '{
    "method": "email"
  }'

# Verify MFA (requires Authorization Bearer token from login response when MFA is enabled)
curl -X POST http://localhost:5050/api/admin/auth/verify-mfa \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -H "Content-Type: application/json" \
  -H "lang: en" \
  -d '{
    "code": "123456"
  }'
```

### Development Workflow

**Making changes to the database:**

```bash
# 1. Make changes to entities in Domain layer
# 2. Create a new migration
dotnet ef migrations add YourMigrationName --project LoadlyAggregator.Infrastructure --startup-project LoadlyAggregator.API

# 3. Apply migration
dotnet ef database update --project LoadlyAggregator.Infrastructure --startup-project LoadlyAggregator.API
```

**Hot reload during development:**

```bash
dotnet watch run --project LoadlyAggregator.API
```

This will automatically restart the application when you make code changes.

## 📁 Project Structure

### API Layer (`LoadlyAggregator.API`)

- **Controllers**: Admin, Company, Driver auth controllers
  - Handle HTTP requests/responses
  - Convert Request → Action → Response → Resource
  - All controllers follow the same consistent pattern
- **Resources**: Response transformers for API formatting
  - Transform domain responses to API format
  - Located in `Resources/Admin/`, `Resources/Company/`, `Resources/Driver/`
  - Each resource has `FromResponse()` static method
- **Helpers**: Utility classes for common operations
  - `ClaimsHelper`: Extract userId from JWT claims without IF statements
- **Validators**: FluentValidation validators for request validation
- **Middleware**: Exception handling, language detection, audit logging
- **Filters**: Validation, authorization
- **Extensions**: Service registration, middleware pipeline

### Domain Layer (`LoadlyAggregator.Domain`)

- **Entities**: Admin, Company, Driver, Role, Permission, etc.
- **Base Classes**: BaseEntity, BaseUserEntity, BaseAction
- **Enums**: UserStatus, PermissionStatus, AuditEventType
- **Actions**: Business logic operations (Login, MFA, Password Reset, etc.)
  - Work with DTOs, not HTTP-bound Requests
  - Located in `{UserType}/Actions/Auth/`
  - All actions (Admin, Company, Driver) follow the same pattern
  - Use validation services and strategy patterns instead of IF statements
- **DTOs**: Data Transfer Objects (Request → DTO conversion)
  - Separate API concerns from business logic
  - Located in `{UserType}/DTOs/Auth/`
  - Each DTO has `FromRequest()` static method
  - Consistent across all user types (Admin, Company, Driver)
- **Responses**: Domain response models
  - Located in `{UserType}/Actions/Auth/Responses/`
- **Services**: Domain services (Auth, MFA Strategy, Validation, Activity Log)
  - `AuthService`: Credential validation
  - `MfaMethodStrategyFactory`: Strategy pattern for MFA methods
  - `MfaCodeService`: Stateless deterministic code generation and verification (no DB storage)
  - `UserValidator`: Validation service eliminating IF statements
- **Interfaces**: Domain contracts for infrastructure implementations

### Infrastructure Layer (`LoadlyAggregator.Infrastructure`)

- **Data**: ApplicationDbContext, Entity configurations, Interceptors
- **Repositories**: Generic repository with soft delete support
- **Services**: Auth, email, SMS/WhatsApp, permissions, activity logging, media/file upload, queue
  - **Media Service**: File upload/download/delete with storage provider abstraction
  - **Storage Providers**: Local, S3, Azure Blob, Google Cloud Storage implementations
  - **SMS/WhatsApp Service**: Messaging with provider abstraction
  - **SMS Providers**: VictoryLink (SMS), UltraMsg (WhatsApp), Akedly.io (SMS) implementations
  - **Queue Service**: Redis-based message queue with background processing
  - **Queue Processors**: Extensible message processing interface
- **Extensions**: Service collection extensions
- **Interceptors**: EF Core interceptors for automatic activity logging

### Shared Layer (`LoadlyAggregator.Shared`)

- **DTOs**: Request/Response models
- **Exceptions**: Custom exception classes
- **Constants**: Status codes, messages
- **Utilities**: Helper classes
- **Resources**: Translation files (Messages.en.json, Messages.ar.json)

## 🔄 Code Flow Example

### Architecture Layers

```text
API → Domain → Infrastructure
     ↓
   Shared (DTOs, Exceptions)
```

### Login Flow Example (Request → DTO → Action → Response → Resource)

**Note:** Auth endpoints use Request → DTO → Action pattern, while CRUD endpoints use Request → Action pattern directly.

```text
1. HTTP Request
   POST /api/{user-type}/auth/login
   Body: { "login": "user@example.com", "password": "password123" }
   Headers: { "lang": "en" }
   └─→ {UserType}AuthController.Login({UserType}LoginRequest request)

2. Validation (FluentValidation)
   └─→ {UserType}LoginRequestValidator (automatic validation)
       ├─ Validates Login (min 3 chars)
       └─ Validates Password (min 6 chars)

3. Request → DTO Conversion (Auth endpoints only)
   └─→ {UserType}LoginDto.FromRequest(request)
       └─→ Converts Request to DTO for domain layer

4. Domain Action (Business Logic) - DTO → Response
   {UserType}LoginAction.cs
   └─→ Execute: _loginAction.ExecuteAsync(dto)
       ├─→ Gets user: _repository.GetByLoginAsync(dto.Login)
       ├─→ Validates: _authService.ValidateCredentials(user, dto.Password)
       │   └─→ Uses UserValidator (no IF statements)
       └─→ Returns: {UserType}LoginResponse (Domain response)
           ├─ If MFA enabled → Returns empty tokens (user must verify MFA code)
           └─ If no MFA → Returns JWT access token + refresh token

5. Infrastructure Services
   ├─ Repository → Database query (GetByLoginAsync)
   ├─ AuthService → Password validation (uses UserValidator)
   └─ JwtTokenService → Token generation

6. Controller - Response → Resource Transformation
   {UserType}AuthController.cs
   └─→ Transform Response to Resource:
       var resource = {UserType}LoginResource.FromResponse(response)
       └─→ Resource formats response for API

7. API Response
   └─→ ApiResponseService wraps resource
       └─→ Returns: { success: true, data: {...}, message: "auth.login_success" }
       └─→ Message translated based on "lang" header
```

**Complete Pattern Flow:**

**Auth Endpoints:**

```text
HTTP Request → Request (FluentValidation) → DTO (FromRequest) → Action (ExecuteAsync) 
→ Response → Resource (FromResponse) → ApiResponseService → JSON Response
```

**CRUD Endpoints:**

```text
HTTP Request → Request (FluentValidation) → Action (ExecuteAsync with Request directly)
→ Response → Resource (FromResponse) → ApiResponseService → JSON Response
```

**Note:**

- Auth endpoints use DTO conversion for additional validation and transformation logic
- CRUD endpoints work directly with Request models for simplicity
- Refresh tokens are generated but refresh token endpoint is not yet implemented

### Code Structure Pattern

**Auth Endpoints (Request → DTO → Action):**

```text
📁 API Layer (Controllers & Resources)
   └─ Controllers/Admin/Auth/AdminAuthController.cs
      ├─ Handles HTTP request/response
      ├─ Validates via FluentValidation (auto)
      ├─ Converts Request → DTO (AdminLoginDto.FromRequest)
      ├─ Calls Domain Actions with DTO
      └─ Transforms Response → Resource (AdminLoginResource.FromResponse)
   
   └─ Resources/Admin/AdminLoginResource.cs
      └─ Response transformer for API formatting
          └─ Formats domain responses for API

📁 Domain Layer (Business Logic)
   └─ Admin/Actions/Auth/AdminLoginAction.cs
      ├─ Implements IAction<AdminLoginDto, AdminLoginResponse>
      ├─ Works with DTO classes
      ├─ Uses Domain Interfaces (IAdminRepository)
      └─ Contains pure business logic
   
   └─ Admin/Actions/Auth/Requests/AdminLoginRequest.cs
      └─ Request model with FluentValidation (API layer)
   
   └─ Admin/DTOs/Auth/AdminLoginDto.cs
      └─ DTO model (Domain layer) - converted from Request
   
   └─ Admin/Actions/Auth/Responses/AdminLoginResponse.cs
      └─ Domain response model

📁 Infrastructure Layer (Implementation)
   └─ Repositories/Implementations/AdminRepository.cs
      └─ Implements IAdminRepository (from Domain)
      └─ Uses ApplicationDbContext (EF Core)
```

**CRUD Endpoints (Request → Action directly):**

```text
📁 API Layer (Controllers & Resources)
   └─ Controllers/Admin/Admins/AdminsController.cs
      ├─ Handles HTTP request/response
      ├─ Validates via FluentValidation (auto)
      ├─ Calls Domain Actions with Request directly
      └─ Transforms Response → Resource (AdminResource.FromResponse)

📁 Domain Layer (Business Logic)
   └─ Admin/Actions/Admins/CreateAdminAction.cs
      ├─ Implements IAction<CreateAdminRequest, AdminResponse>
      ├─ Works with Request classes directly
      └─ Contains pure business logic
   
   └─ Admin/Actions/Admins/Requests/CreateAdminRequest.cs
      └─ Request model with FluentValidation (used directly in Action)
```

### Key Design Patterns

- **Request → Action → Response → Resource Pattern**:
  - Clean architecture pattern with clear separation of concerns
  - **Pattern Variations**:
    - **CRUD Operations**: Request → Action → Response → Resource (Request used directly in Actions)
    - **Auth Operations**: Request → DTO → Action → Response → Resource (DTO conversion for auth-specific logic)
  - **Request**: HTTP-bound validation (API layer)
  - **DTO**: Domain Transfer Object (used in auth endpoints for additional validation/transformation)
  - **Action**: Business logic execution (Domain layer)
  - **Response**: Domain response model (Domain layer)
  - **Resource**: API response transformation (API layer)
  - **Consistent**: All user types (Admin, Company, Driver) follow the same pattern
- **Action Pattern**: Business logic encapsulated in `*Action` classes
  - CRUD actions work with Request classes directly
  - Auth actions work with DTO classes (converted from Request)
- **Strategy Pattern**: Eliminates IF statements (MFA methods via `MfaMethodStrategyFactory`)
- **Builder Pattern**: Eliminates IF statements (JWT claims, login responses)
- **Resolver Pattern**: Eliminates nested IF statements (translation resolution, MFA recipient resolution)
- **Validator Pattern**: Eliminates IF statements (validation via `UserValidator`, `ClaimsHelper`)
- **Repository Pattern**: Data access via interfaces
- **Dependency Injection**: Domain depends on interfaces, Infrastructure implements them
- **Validation**: FluentValidation in API layer (automatic validation)
- **Resource Transformers**: Response formatting for API layer
- **Service Registration**: Actions registered in `ServiceCollectionExtensions`
- **IF Statement Elimination**: All services and actions use design patterns instead of conditional logic
  - **Validation Services**: `UserValidator` for all user validation checks
  - **Strategy Factory**: `MfaMethodStrategyFactory` for MFA method selection
  - **Helper Methods**: `ClaimsHelper` for claim extraction
  - **Method Extraction**: Private methods for separation of concerns

**Benefits:**

- ✅ Separation of concerns: API layer (Request/Resource) separated from Domain layer (DTO/Action/Response)
- ✅ Testability: Actions work with DTOs (auth) or Requests (CRUD), keeping domain logic testable
- ✅ Reusability: Actions can be called from different contexts (API, background jobs, etc.)
- ✅ Maintainability: Clear data flow and transformation points

This keeps the Domain layer independent of Infrastructure and API, making the codebase testable and maintainable.

## 🔐 Authentication Endpoints

### Admin

#### Authentication

- `POST /api/admin/auth/login` - Admin login (supports MFA flow, returns JWT access token and refresh token)
- `POST /api/admin/auth/logout` - Admin logout (token blacklisting not yet implemented)
- `POST /api/admin/auth/forget-password` - Request password reset (sends email)
- `POST /api/admin/auth/reset-password` - Reset password with token
- `POST /api/admin/auth/change-password` - Change password (requires current password)
- `POST /api/admin/auth/enable-mfa` - Enable MFA (email or SMS)
- `POST /api/admin/auth/send-mfa-code` - Send MFA verification code
- `POST /api/admin/auth/verify-mfa` - Verify MFA code and get JWT token (requires Authorization Bearer token)
- `GET /api/admin/auth/profile` - Get profile (**Not implemented** - returns placeholder)
- `PUT /api/admin/auth/profile` - Update profile (**Not implemented** - returns placeholder)

#### Admins CRUD

- `POST /api/admin/admins` - Create a new admin (sends welcome email with password reset link)
- `GET /api/admin/admins/{id}` - Get admin by ID
- `GET /api/admin/admins` - List all admins with pagination and filters
  - Query parameters: `page`, `pageSize`, `search`, `type`, `status`
- `PUT /api/admin/admins/{id}` - Update admin (password cannot be updated via CRUD)
- `DELETE /api/admin/admins/{id}` - Delete admin (soft delete)

### Admin Logs

- `GET /api/admin/logs/notifications` - Get notifications log with pagination and filters
  - Query parameters: `page`, `pageSize`, `type` (1=SMS, 2=EMAIL), `status` (0=PENDING, 1=SUCCESS, 2=FAILED), `recipient`, `startDate`, `endDate`
  - Returns paginated list of SMS/Email notifications sent
  - Requires admin authentication
- `GET /api/admin/logs/activity` - Get activity/audit log with pagination and filters
  - Query parameters: `page`, `pageSize`, `eventType`, `entityType`, `userId`, `userType`, `startDate`, `endDate`
  - Returns paginated list of all system activities (CREATE, UPDATE, DELETE, LOGIN, LOGOUT)
  - Requires admin authentication

### Company

- Similar endpoints at `/api/company/auth/*`
  - Login, logout, forget/reset password, change password
  - Enable MFA, send MFA code, verify MFA
  - Profile management (**Not implemented** - returns placeholder)
  - **Pattern**: Follows the same Request → DTO → Action → Response → Resource pattern as Admin

### Driver

- Similar endpoints at `/api/driver/auth/*`
  - Login, logout, forget/reset password, change password
  - Enable MFA, send MFA code, verify MFA
  - Profile management (**Not implemented** - returns placeholder)
  - **Pattern**: Follows the same Request → DTO → Action → Response → Resource pattern as Admin

**Note:** All endpoints support multi-language responses via `lang` header (`en` or `ar`)

### Log Endpoints Examples

**Get Notifications Log:**

```bash
curl -X GET "http://localhost:5050/api/admin/logs/notifications?page=1&pageSize=15&type=1&status=1" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "lang: en"
```

**Get Activity Log:**

```bash
curl -X GET "http://localhost:5050/api/admin/logs/activity?page=1&pageSize=15&eventType=CREATE&userType=Admin" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "lang: en"
```

**Filter by Date Range:**

```bash
curl -X GET "http://localhost:5050/api/admin/logs/activity?startDate=2025-01-01T00:00:00Z&endDate=2025-12-31T23:59:59Z" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "lang: en"
```

## 🛠️ Technology Stack

- **Framework**: .NET 8
- **Database**: PostgreSQL 17.6
- **ORM**: Entity Framework Core 9.x
- **Authentication**: JWT Bearer Tokens
- **Password Hashing**: BCrypt.Net-Next
- **Email**: MailKit (SMTP)
- **SMS/WhatsApp**: Agnostic messaging service with VictoryLink, UltraMsg, and Akedly.io support
- **Queue**: Redis (StackExchange.Redis) for message queuing
- **Logging**: Serilog with pluggable exporters (Console, File, Seq, Elasticsearch)
- **Audit**: Audit.NET
- **Validation**: FluentValidation
- **API Documentation**: Swagger/OpenAPI
- **Translation**: JSON-based translation system (en/ar)
- **File Storage**: Multi-provider storage abstraction (Local, S3, Azure, GCS)

## 📊 Logging & Observability

The project uses **Serilog** for structured logging with a pluggable exporter pattern, similar to the storage provider pattern. Logs are automatically enriched with correlation IDs, request context, and structured properties.

### Log Exporters

The application supports multiple log exporters that can be enabled via configuration:

- **Console**: Real-time log output to console (default)
- **File**: Rolling file logs with configurable retention
- **Seq**: Structured log server with powerful search and dashboards
- **Elasticsearch**: Distributed search and analytics

### Quick Setup

**1. Configure Exporters in `appsettings.json`:**

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "Microsoft.EntityFrameworkCore.Query": "None",
        "System": "Warning"
      }
    },
    "Exporters": [ "console", "file" ],
    "Console": {
      "Enabled": true
    },
    "File": {
      "Enabled": true,
      "Path": "logs/app-.log",
      "RollingInterval": "Day",
      "RetainedFileCountLimit": 30,
      "FileSizeLimitBytes": 10485760
    },
    "Seq": {
      "Enabled": false,
      "ServerUrl": "http://localhost:5341",
      "ApiKey": ""
    },
    "Elasticsearch": {
      "Enabled": false,
      "NodeUris": "http://localhost:9200",
      "IndexFormat": "loadly-aggregator-logs-{0:yyyy.MM.dd}"
    }
  }
}
```

**2. Enable Seq (Optional):**

```bash
# Run Seq server with Docker
docker run -d \
  --name seq \
  -p 5341:80 \
  -e ACCEPT_EULA=Y \
  datalust/seq:latest
```

Then update `appsettings.json`:

```json
{
  "Serilog": {
    "Exporters": [ "console", "file", "seq" ],
    "Seq": {
      "Enabled": true,
      "ServerUrl": "http://localhost:5341"
    }
  }
}
```

**Access Seq UI:** <http://localhost:5341>

### Correlation IDs

All logs from the same HTTP request are automatically grouped using **Correlation IDs**. This makes it easy to trace all logs related to a specific request.

**Features:**

- Automatic correlation ID generation per request
- Request context enrichment (path, method, query string, user agent, IP)
- Distributed tracing support (pass `X-Correlation-Id` header)
- Response header includes correlation ID for client tracking

**Search logs by correlation ID:**

```sql
CorrelationId = '0HNIB2PVK48SF'
```

### Log Properties

Each log entry includes structured properties:

- `CorrelationId`: Unique request identifier
- `RequestPath`: API endpoint path
- `RequestMethod`: HTTP method (GET, POST, etc.)
- `Application`: "LoadlyAggregator.API"
- `Environment`: Development/Staging/Production
- `MachineName`: Server name
- `ThreadId`: Thread identifier
- Custom properties from your code

### Using Seq

**Search Logs:**

```sql
Application = 'LoadlyAggregator.API'
Level = 'Error'
CorrelationId = '0HNIB2PVK48SF'
@Message like '%password%'
```

**Find all errors:**

```sql
Level = 'Error'
```

**Find logs from specific service:**

```sql
@SourceContext like '%MediaService%'
```

**Find logs in time range:**

```sql
@Timestamp > '2026-01-04' and @Timestamp < '2026-01-05'
```

### Docker Compose (Optional)

Create `docker-compose.yml` for easier Seq management:

```yaml
version: '3.8'

services:
  seq:
    image: datalust/seq:latest
    container_name: seq
    ports:
      - "5341:80"
      - "5342:5341"
    environment:
      - ACCEPT_EULA=Y
      - SEQ_FIRSTRUN_ADMINPASSWORD=admin
    volumes:
      - seq-data:/data
    restart: unless-stopped

volumes:
  seq-data:
```

Run with:

```bash
docker-compose up -d
```

### Production Considerations

1. **Use Seq Cloud** for managed hosting
2. **Set up authentication** with ApiKey
3. **Configure retention policies** to manage storage
4. **Set up alerts** for critical errors
5. **Use HTTPS** for secure connections

For detailed setup instructions, see **[SEQ_SETUP.md](SEQ_SETUP.md)** and **[CORRELATION_ID_GUIDE.md](CORRELATION_ID_GUIDE.md)**.

## 📝 Naming Conventions

- **Tables**: `snake_case` (e.g., `admin_users`)
- **Controllers**: `{UserType}{Entity}Controller.cs`
- **Actions**: `{UserType}{Verb}{Entity}Action.cs`
- **DTOs**: `{Entity}Dto.cs`
- **Repositories**: `{Entity}Repository.cs`
- **Services**: `{Purpose}Service.cs`
- **JSON**: `camelCase`

## 🔄 Next Steps

The project foundation is complete. Current status:

✅ **Completed:**

1. ✅ **Authentication Logic**: Login, logout fully implemented
2. ✅ **JWT Service**: Token generation and validation
3. ✅ **Seed Data**: Super admin user seeded on startup
4. ✅ **Language Support**: Multi-language support via headers (en/ar) with translation system
5. ✅ **Action Pattern**: Clean architecture with Domain Actions for all business logic
6. ✅ **MFA Implementation**: Complete stateless email/SMS MFA flow
   - Enable MFA (email or SMS)
   - Send 6-digit verification codes (deterministic, time-window based)
   - Verify codes using Authorization Bearer token (no temporary tokens)
   - Stateless code generation (no DB storage, no token generation)
   - Code valid for 10-minute time windows
7. ✅ **Password Reset**: Complete forget/reset password flow
   - Forget password with email notification
   - Reset password with secure token validation
   - Token expiration (24 hours)
8. ✅ **Change Password**: Authenticated password change
   - Current password verification
   - New password confirmation
   - Password strength validation
9. ✅ **Email Service**: SMTP-based notification service (MailKit)
   - Configurable via appsettings.json
   - HTML email support
10. ✅ **SMS/WhatsApp Service**: Agnostic messaging service
    - Multiple provider support: VictoryLink (SMS), UltraMsg (WhatsApp), Akedly.io (SMS)
    - Provider selection via configuration (no code changes)
    - Automatic phone number normalization per provider
    - Comprehensive error handling and logging
    - Integrated with MFA system

11. ✅ **Soft Delete**: Automatic soft delete implementation
    - All entities support soft delete
    - Automatic filtering of deleted entities in queries
    - Timestamp management (DeletedAt)

12. ✅ **Activity Logging**: Automatic activity logging
    - Logs all create/update/delete operations
    - Captures user context, IP address, user agent
    - Stores changes as JSON for audit trail

13. ✅ **Code Refactoring - IF Statement Elimination**
    - All services and actions refactored to eliminate IF statements using design patterns
    - **Strategy Pattern**: `MfaMethodStrategyFactory` for MFA method selection
    - **MFA Code Service**: `MfaCodeService` for stateless deterministic code generation (HMAC-based, time-window based)
    - **Validator Services**: `UserValidator` for all user validation checks
    - **Helper Methods**: `ClaimsHelper` for claim extraction without IF statements
    - **Method Extraction**: Private methods for separation of concerns
    - **LoginResponseBuilder**: Builder + Strategy pattern for MFA/login responses
    - **MessageTranslatorFactory**: Strategy pattern for message translation
    - **ClaimsBuilder**: Builder pattern for JWT claims
    - **TranslationResolver**: Resolver pattern for translation fallback
    - **Pattern Matching**: C# pattern matching for type checks
    - Services and actions now use: Strategy, Builder, Resolver, Validator patterns instead of conditional logic

14. ✅ **Request → Action → Response → Resource Pattern**
    - Clean architecture pattern with clear separation of concerns
    - **Consistent Pattern**: All user types (Admin, Company, Driver) follow the same pattern
    - Request classes used directly in Actions (simplified architecture)
    - Resource transformers format responses for API
    - Clear separation between API and Domain layers
    - Actions work with Request classes for better simplicity and maintainability
    - **Company & Driver**: Fully refactored to match Admin pattern
      - All Company and Driver Actions use Request classes directly
      - All Company and Driver Controllers use Resource transformers
      - All Company and Driver Actions use validation services and strategy patterns

15. ✅ **Admin Log APIs**
    - **Notifications Log API**: `GET /api/admin/logs/notifications`
      - Paginated list of all SMS/Email notifications sent
      - Filtering by type (SMS/EMAIL), status (PENDING/SUCCESS/FAILED), recipient, date range
      - Stored in separate logs database for better performance
    - **Activity Log API**: `GET /api/admin/logs/activity`
      - Paginated list of all system activities (CREATE, UPDATE, DELETE, LOGIN, LOGOUT)
      - Filtering by event type, entity type, user ID, user type, date range
      - Comprehensive audit trail of all operations
    - Both endpoints support pagination, filtering, and multi-language responses
    - Follow the same Request → Action → Response → Resource pattern

16. ✅ **Admin CRUD APIs**
    - **Create Admin**: `POST /api/admin/admins`
      - Creates admin without password
      - Sends welcome email with verification code and password reset link
      - Email includes reset token valid for 24 hours
    - **Get Admin**: `GET /api/admin/admins/{id}`
      - Retrieves admin by ID
    - **List Admins**: `GET /api/admin/admins`
      - Paginated list with search, type, and status filters
      - Query parameters: `page`, `pageSize`, `search`, `type`, `status`
    - **Update Admin**: `PUT /api/admin/admins/{id}`
      - Updates admin information
      - Password cannot be updated via CRUD
    - **Delete Admin**: `DELETE /api/admin/admins/{id}`
      - Soft delete operation
    - All endpoints follow Request → Action → Response → Resource pattern
    - Actions work directly with Request models (no separate DTOs per operation)

17. ✅ **File Upload & Storage System**
    - **Agnostic Storage Provider**: Supports Local, AWS S3, Azure Blob, Google Cloud Storage
    - **Provider Selection**: Switch providers via configuration (no code changes)
    - **Media Service**: `IMediaService` interface for upload, read, delete operations
    - **Storage Provider Abstraction**: `IStorageProvider` interface for cloud provider implementations
    - **File Validation**: FluentValidation extension methods for file size and MIME type validation
    - **CDN URL Generation**: Automatic full URL generation for uploaded files
    - **Organized Structure**: Logical directory organization (`{entity}/{context}/{id}`)
    - **Static File Serving**: Configured for local development
    - **Documentation**: Complete usage guides in `FILE_UPLOAD_USAGE.md` and `STORAGE_PROVIDERS.md`

18. ✅ **Redis Queue System**

- **Queue Service**: `IQueueService` interface for queue operations (enqueue, dequeue, length, clear)
- **Redis Implementation**: `RedisQueueService` using StackExchange.Redis
- **Background Processing**: `RedisQueueBackgroundService` automatically processes messages from configured queues
- **Multiple Queues**: Support for multiple named queues (default, emails, notifications, etc.)
- **Message Serialization**: JSON-based message serialization with camelCase naming
- **Configurable Polling**: Adjustable polling intervals and error retry delays
- **Processor Pattern**: `IQueueProcessor` interface for custom message handling
- **Error Handling**: Comprehensive error handling with logging and retry logic
- **Service Registration**: Automatic registration in `ServiceCollectionExtensions`
- **Configuration**: Queue configuration via `appsettings.json` (queues list, polling intervals)

🔄 **In Progress / To Do:**

1. 🔄 **Profile Management**: Complete profile retrieval and update functionality (endpoints exist but return placeholders)
2. 🔄 **Logout Token Management**: Implement token blacklisting or refresh token invalidation for logout
3. 🔄 **Refresh Token Endpoint**: Implement refresh token endpoint to renew access tokens
4. 🔄 **CRUD Actions**: Implement admin actions for managing companies and drivers
5. 🔄 **Permission Checking**: Permission authorization logic is implemented but may need additional testing
6. 🔄 **Role Management**: Complete RBAC role and permission assignment UI/workflows
7. 🔄 **Unit Tests**: Add comprehensive test coverage
8. 🔄 **Queue Processors**: Queue processors exist (EmailQueueProcessor, SmsQueueProcessor) but may need additional customization

## 📚 Additional Documentation

- **[ARCHITECTURE.md](ARCHITECTURE.md)**: Comprehensive architecture and patterns documentation
  - Project structure and organization
  - Design patterns used (Action, Strategy, Repository, Builder, Resolver, etc.)
  - **Request → Action → Response → Resource Pattern**
  - **Service Refactoring Patterns** (IF Statement Elimination)
  - Soft delete implementation details
  - Activity logging system
  - Best practices and coding guidelines
  - Pattern implementation details and best practices

- **[FILE_UPLOAD_USAGE.md](FILE_UPLOAD_USAGE.md)**: Complete file upload system usage guide
  - Configuration setup
  - Request validation patterns
  - Request and Action patterns for file uploads
  - File retrieval and URL generation
  - Complete examples and best practices

- **[STORAGE_PROVIDERS.md](STORAGE_PROVIDERS.md)**: Storage provider configuration guide
  - Local storage setup
  - AWS S3 configuration
  - Azure Blob Storage setup
  - Google Cloud Storage setup
  - Switching between providers
  - Implementation examples

- **[CRUD_OPERATIONS_GUIDE.md](CRUD_OPERATIONS_GUIDE.md)**: Complete CRUD operations implementation guide
  - Step-by-step implementation from Entity to Controller
  - Create, Read, Update, Delete operations
  - Request → Action → Response → Resource pattern
  - Repository pattern with soft delete
  - Validation, pagination, and error handling
  - Complete Product entity example
  - Best practices and testing examples

- **[SEQ_SETUP.md](SEQ_SETUP.md)**: Seq logging server setup guide
  - What is Seq and why use it
  - Docker setup instructions
  - Configuration options
  - Search and query examples
  - Troubleshooting guide
  - Production considerations

- **[CORRELATION_ID_GUIDE.md](CORRELATION_ID_GUIDE.md)**: Request correlation ID guide
  - How correlation IDs work
  - Request context enrichment
  - Distributed tracing
  - Search examples
  - Advanced usage patterns

## 📦 Redis Queue Usage

### Basic Usage

**Enqueue a message:**

```csharp
// Inject IQueueService in your service/controller
private readonly IQueueService _queueService;

// Enqueue a message
await _queueService.EnqueueAsync("emails", new {
    To = "user@example.com",
    Subject = "Welcome",
    Body = "Welcome to our platform!"
});
```

**Dequeue a message:**

```csharp
// Dequeue a message
var message = await _queueService.DequeueAsync<EmailMessage>("emails");
if (message != null)
{
    // Process the message
    await ProcessEmail(message);
}
```

**Get queue length:**

```csharp
var length = await _queueService.GetQueueLengthAsync("emails");
```

**Clear a queue:**

```csharp
await _queueService.ClearQueueAsync("emails");
```

### Creating a Custom Queue Processor

Implement `IQueueProcessor` to process messages automatically:

```csharp
public class EmailQueueProcessor : IQueueProcessor
{
    private readonly ILogger<EmailQueueProcessor> _logger;
    private readonly INotificationService _notificationService;

    public EmailQueueProcessor(
        ILogger<EmailQueueProcessor> logger,
        INotificationService notificationService)
    {
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task ProcessMessageAsync(
        string messageJson, 
        string queueName, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Deserialize message
            var message = JsonSerializer.Deserialize<EmailMessage>(messageJson);
            
            if (message != null)
            {
                // Process the email
                await _notificationService.SendEmailAsync(
                    message.To,
                    message.Subject,
                    message.Body,
                    isHtml: true
                );
                
                _logger.LogInformation("Email sent to {To}", message.To);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process email message from queue {QueueName}", queueName);
            throw;
        }
    }
}

// Register in ServiceCollectionExtensions
services.AddScoped<IQueueProcessor, EmailQueueProcessor>();
```

### Queue Configuration

Configure queues in `appsettings.json`:

```json
{
  "Queue": {
    "Queues": [ "default", "emails", "notifications" ],
    "PollingIntervalMs": 1000,
    "ErrorRetryDelayMs": 5000
  },
  "ConnectionStrings": {
    "Redis": "localhost:6379"
  }
}
```

**Configuration Options:**

- `Queues`: Array of queue names to process automatically
- `PollingIntervalMs`: Milliseconds to wait when queue is empty (default: 1000)
- `ErrorRetryDelayMs`: Milliseconds to wait before retrying after an error (default: 5000)

### Queue Service Interface

The `IQueueService` interface provides:

- `EnqueueAsync<T>(queueName, message)` - Add message to queue
- `DequeueAsync<T>(queueName)` - Remove and return message from queue
- `DequeueStringAsync(queueName)` - Remove and return raw JSON string
- `GetQueueLengthAsync(queueName)` - Get number of items in queue
- `ClearQueueAsync(queueName)` - Remove all messages from queue

All methods support `CancellationToken` for async cancellation.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Copyright (c) 2025 Loadly Aggregator

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.

## 👥 Contributors

[Moaaz Radwan/Loadly]
