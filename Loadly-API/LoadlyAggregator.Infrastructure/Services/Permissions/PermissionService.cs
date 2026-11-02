using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using LoadlyAggregator.Domain.Common.Entities;
using LoadlyAggregator.Domain.Common.Enums;
using LoadlyAggregator.Domain.Common.Interfaces;
using LoadlyAggregator.Infrastructure.Data;
using LoadlyAggregator.Shared.Exceptions;

namespace LoadlyAggregator.Infrastructure.Services.Permissions;

public class PermissionService : IPermissionService
{
    private readonly ApplicationDbContext _context;
    private readonly IPermissionRepository _permissionRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IMemoryCache _cache;
    private readonly IConfiguration _configuration;
    private readonly int _cacheExpirationMinutes;

    public PermissionService(
        ApplicationDbContext context,
        IPermissionRepository permissionRepository,
        IRoleRepository roleRepository,
        IMemoryCache cache,
        IConfiguration configuration)
    {
        _context = context;
        _permissionRepository = permissionRepository;
        _roleRepository = roleRepository;
        _cache = cache;
        _configuration = configuration;
        _cacheExpirationMinutes = int.Parse(configuration["AuthSettings:PermissionCacheMinutes"] ?? "60");
    }

    #region Permission Management

    public async Task<Permission> CreatePermissionAsync(string name, string guardName, long? permissionGroupId = null)
    {
        var existing = await _permissionRepository.GetByNameAndGuardAsync(name, guardName);
        if (existing != null)
        {
            throw new InvalidOperationException($"Permission '{name}' already exists for guard '{guardName}'");
        }

        var permission = new Permission
        {
            Name = name,
            GuardName = guardName,
            PermissionGroupId = permissionGroupId ?? 0,
        };

        var result = await _permissionRepository.CreateAsync(permission);
        await _context.SaveChangesAsync();
        ClearAllCache();
        return result;
    }

    public async Task<List<Permission>> GetPermissionsByGuardAsync(string guardName)
    {
        var cacheKey = $"permissions:guard:{guardName}";

        if (!_cache.TryGetValue(cacheKey, out List<Permission>? permissions))
        {
            permissions = await _permissionRepository.GetByGuardAsync(guardName);
            _cache.Set(cacheKey, permissions, TimeSpan.FromMinutes(_cacheExpirationMinutes));
        }

        return permissions ?? new List<Permission>();
    }

    public async Task<Permission?> GetPermissionByNameAsync(string name, string guardName)
    {
        return await _permissionRepository.GetByNameAndGuardAsync(name, guardName);
    }

    #endregion

    #region User Permission Methods

    public async Task GivePermissionToAsync<T>(T user, string permissionName) where T : class, IHasRoles
    {
        var permission = await _permissionRepository.GetByNameAndGuardAsync(permissionName, user.GuardName);
        if (permission == null)
        {
            throw new InvalidOperationException($"Permission '{permissionName}' not found for guard '{user.GuardName}'");
        }

        await GivePermissionToAsync(user, permission.Id);
    }

    public async Task GivePermissionToAsync<T>(T user, long permissionId) where T : class, IHasRoles
    {
        var modelType = typeof(T).Name;
        var exists = await _context.ModelHasPermissions
            .AnyAsync(mhp => mhp.PermissionId == permissionId
                && mhp.ModelId == user.Id
                && mhp.ModelType == modelType);

        if (!exists)
        {
            var modelHasPermission = new ModelHasPermission
            {
                PermissionId = permissionId,
                ModelId = user.Id,
                ModelType = modelType
            };

            _context.ModelHasPermissions.Add(modelHasPermission);
            await _context.SaveChangesAsync();
            ClearUserCache(user);
        }
    }

    public async Task GivePermissionsToAsync<T>(T user, List<string> permissionNames) where T : class, IHasRoles
    {
        var permissions = await _context.Permissions
            .Where(p => permissionNames.Contains(p.Name) && p.GuardName == user.GuardName)
            .ToListAsync();

        var modelType = typeof(T).Name;
        var existingPermissions = await _context.ModelHasPermissions
            .Where(mhp => mhp.ModelId == user.Id && mhp.ModelType == modelType)
            .Select(mhp => mhp.PermissionId)
            .ToListAsync();

        var newPermissions = permissions
            .Where(p => !existingPermissions.Contains(p.Id))
            .Select(p => new ModelHasPermission
            {
                PermissionId = p.Id,
                ModelId = user.Id,
                ModelType = modelType
            })
            .ToList();

        if (newPermissions.Count != 0)
        {
            _context.ModelHasPermissions.AddRange(newPermissions);
            await _context.SaveChangesAsync();
            ClearUserCache(user);
        }
    }

    public async Task GivePermissionsToAsync<T>(T user, List<long> permissionIds) where T : class, IHasRoles
    {
        var modelType = typeof(T).Name;
        var existingPermissions = await _context.ModelHasPermissions
            .Where(mhp => mhp.ModelId == user.Id && mhp.ModelType == modelType)
            .Select(mhp => mhp.PermissionId)
            .ToListAsync();

        var newPermissions = permissionIds
            .Where(id => !existingPermissions.Contains(id))
            .Select(id => new ModelHasPermission
            {
                PermissionId = id,
                ModelId = user.Id,
                ModelType = modelType
            })
            .ToList();

        if (newPermissions.Count != 0)
        {
            _context.ModelHasPermissions.AddRange(newPermissions);
            await _context.SaveChangesAsync();
            ClearUserCache(user);
        }
    }

    public async Task GiveAllGuardPermissionsAsync<T>(T user) where T : class, IHasRoles
    {
        List<Permission> permissions = await GetPermissionsByGuardAsync(user.GuardName);
        if (permissions.Count == 0)
        {
            return;
        }

        List<string> permissionNames = permissions.Select(static p => p.Name).ToList();
        await GivePermissionsToAsync(user, permissionNames);
    }

    public async Task RevokePermissionFromAsync<T>(T user, string permissionName) where T : class, IHasRoles
    {
        var permission = await _permissionRepository.GetByNameAndGuardAsync(permissionName, user.GuardName);
        if (permission == null)
        {
            return;
        }

        await RevokePermissionFromAsync(user, permission.Id);
    }

    public async Task RevokePermissionFromAsync<T>(T user, long permissionId) where T : class, IHasRoles
    {
        var modelType = typeof(T).Name;
        var modelHasPermission = await _context.ModelHasPermissions
            .FirstOrDefaultAsync(mhp => mhp.PermissionId == permissionId
                && mhp.ModelId == user.Id
                && mhp.ModelType == modelType);

        if (modelHasPermission != null)
        {
            _context.ModelHasPermissions.Remove(modelHasPermission);
            await _context.SaveChangesAsync();
            ClearUserCache(user);
        }
    }

    public async Task SyncPermissionsAsync<T>(T user, List<string> permissionNames) where T : class, IHasRoles
    {
        var permissions = await _context.Permissions
            .Where(p => permissionNames.Contains(p.Name) && p.GuardName == user.GuardName)
            .ToListAsync();

        var modelType = typeof(T).Name;
        var existingModelHasPermissions = await _context.ModelHasPermissions
            .Where(mhp => mhp.ModelId == user.Id && mhp.ModelType == modelType)
            .ToListAsync();


        var permissionsToRemove = existingModelHasPermissions
            .Where(mhp => !permissions.Any(p => p.Id == mhp.PermissionId))
            .ToList();

        if (permissionsToRemove.Any())
        {
            _context.ModelHasPermissions.RemoveRange(permissionsToRemove);
        }


        var existingPermissionIds = existingModelHasPermissions.Select(mhp => mhp.PermissionId).ToList();
        var newPermissions = permissions
            .Where(p => !existingPermissionIds.Contains(p.Id))
            .Select(p => new ModelHasPermission
            {
                PermissionId = p.Id,
                ModelId = user.Id,
                ModelType = modelType
            })
            .ToList();

        if (newPermissions.Any())
        {
            _context.ModelHasPermissions.AddRange(newPermissions);
        }

        await _context.SaveChangesAsync();
        ClearUserCache(user);
    }

    public async Task<bool> HasPermissionToAsync<T>(T user, string permissionName) where T : class, IHasRoles
    {
        return await CanAsync(user, permissionName);
    }

    public async Task<bool> CanAsync<T>(T user, string permissionName) where T : class, IHasRoles
    {
        var cacheKey = GetUserPermissionsCacheKey(user);

        if (!_cache.TryGetValue(cacheKey, out HashSet<string>? userPermissions))
        {
            userPermissions = await LoadUserPermissionsAsync(user);
            _cache.Set(cacheKey, userPermissions, TimeSpan.FromMinutes(_cacheExpirationMinutes));
        }

        if (userPermissions == null || userPermissions.Count == 0)
        {
            return false;
        }


        if (userPermissions.Contains(permissionName, StringComparer.OrdinalIgnoreCase))
        {
            return true;
        }


        return userPermissions.Any(p => IsWildcardMatch(p, permissionName));
    }

    public async Task<bool> HasAnyPermissionAsync<T>(T user, params string[] permissions) where T : class, IHasRoles
    {
        foreach (var permission in permissions)
        {
            if (await CanAsync(user, permission))
            {
                return true;
            }
        }
        return false;
    }

    public async Task<bool> HasAllPermissionsAsync<T>(T user, params string[] permissions) where T : class, IHasRoles
    {
        foreach (var permission in permissions)
        {
            if (!await CanAsync(user, permission))
            {
                return false;
            }
        }
        return true;
    }

    public async Task<List<string>> GetPermissionNamesAsync<T>(T user) where T : class, IHasRoles
    {
        var cacheKey = GetUserPermissionsCacheKey(user);

        if (!_cache.TryGetValue(cacheKey, out HashSet<string>? userPermissions))
        {
            userPermissions = await LoadUserPermissionsAsync(user);
            _cache.Set(cacheKey, userPermissions, TimeSpan.FromMinutes(_cacheExpirationMinutes));
        }

        return (userPermissions ?? new HashSet<string>()).OrderBy(p => p).ToList();
    }

    public async Task<List<Permission>> GetPermissionsAsync<T>(T user) where T : class, IHasRoles
    {
        var modelType = typeof(T).Name;


        var directPermissions = await _context.ModelHasPermissions
            .Where(mhp => mhp.ModelId == user.Id && mhp.ModelType == modelType)
            .Include(mhp => mhp.Permission)
            .ThenInclude(p => p.PermissionGroup)
            .Select(mhp => mhp.Permission)
            .ToListAsync();


        var rolePermissions = await _context.ModelHasRoles
            .Where(mhr => mhr.ModelId == user.Id && mhr.ModelType == modelType)
            .Include(mhr => mhr.Role)
                .ThenInclude(r => r.RolePermissions)
                    .ThenInclude(rhp => rhp.Permission)
                        .ThenInclude(p => p.PermissionGroup)
            .SelectMany(mhr => mhr.Role.RolePermissions.Select(rhp => rhp.Permission))
            .ToListAsync();


        return directPermissions
            .Union(rolePermissions)
            .DistinctBy(p => p.Id)
            .OrderBy(p => p.Name)
            .ToList();
    }

    public async Task<Dictionary<string, List<string>>> GetPermissionsGroupedByGroupAsync<T>(T user) where T : class, IHasRoles
    {
        var permissions = await GetPermissionsAsync(user);

        return permissions
            .Where(p => p.PermissionGroup != null && !string.IsNullOrEmpty(p.PermissionGroup.Name))
            .GroupBy(p => p.PermissionGroup!.Name!)
            .ToDictionary(
                g => g.Key,
                g => g.Select(p => p.Name).OrderBy(n => n).ToList()
            );
    }

    #endregion

    #region Role Management

    public async Task<Role> CreateRoleAsync(string name, string guardName)
    {
        var existing = await _roleRepository.GetByNameAndGuardAsync(name, guardName);
        if (existing != null)
        {
            throw new InvalidOperationException($"Role '{name}' already exists for guard '{guardName}'");
        }

        var role = new Role
        {
            Name = name,
            GuardName = guardName,
        };

        var result = await _roleRepository.CreateAsync(role);
        await _context.SaveChangesAsync();
        ClearAllCache();
        return result;
    }

    public async Task<Role?> GetRoleByNameAsync(string name, string guardName)
    {
        return await _roleRepository.GetByNameAndGuardAsync(name, guardName);
    }

    public async Task AssignRoleAsync<T>(T user, string roleName) where T : class, IHasRoles
    {
        var role = await _roleRepository.GetByNameAndGuardAsync(roleName, user.GuardName);
        if (role == null)
        {
            throw new InvalidOperationException($"Role '{roleName}' not found for guard '{user.GuardName}'");
        }

        await AssignRoleAsync(user, role.Id);
    }

    public async Task AssignRoleAsync<T>(T user, long roleId) where T : class, IHasRoles
    {
        var modelType = typeof(T).Name;
        var exists = await _context.ModelHasRoles
            .AnyAsync(mhr => mhr.RoleId == roleId
                && mhr.ModelId == user.Id
                && mhr.ModelType == modelType);

        if (!exists)
        {
            var modelHasRole = new ModelHasRole
            {
                RoleId = roleId,
                ModelId = user.Id,
                ModelType = modelType
            };

            _context.ModelHasRoles.Add(modelHasRole);
            await _context.SaveChangesAsync();
            ClearUserCache(user);
        }
    }
    public async Task AssignRolesAsync<T>(T user, IEnumerable<long> roleIds) where T : class, IHasRoles
    {
        if (!roleIds.Any())
            return;

        var modelType = typeof(T).Name;
        var roleIdsList = roleIds.ToList();

        var selectedRoles = await _context.Roles
            .Where(r => roleIdsList.Contains(r.Id))
            .ToListAsync();

        if (selectedRoles.Count != roleIdsList.Count)
        {
            throw new InvalidOperationException("One or more selected roles do not exist.");
        }

        if (user is Admin && selectedRoles.Any(role => role.GuardName != user.GuardName))
        {
            throw new LogicalException("roles.cannot_assign_non_admin");
        }

        if (user is Company companyUser)
        {
            var resolvedCompanyId = companyUser.Type == CompanyType.COMPANY
                ? companyUser.Id
                : companyUser.CompanyId;

            if (!resolvedCompanyId.HasValue)
            {
                throw new InvalidOperationException("Invalid company association for role assignment.");
            }

            if (selectedRoles.Any(role => role.GuardName != "company" || role.CompanyId != resolvedCompanyId.Value))
            {
                throw new InvalidOperationException("Cannot assign roles that belong to another company.");
            }
        }

        var existingRoleIds = await _context.ModelHasRoles
            .Where(mhr => roleIdsList.Contains(mhr.RoleId)
                && mhr.ModelId == user.Id
                && mhr.ModelType == modelType)
            .Select(mhr => mhr.RoleId)
            .ToListAsync();


        var newRoleIds = roleIdsList.Except(existingRoleIds).ToList();

        if (newRoleIds.Any())
        {
            var modelHasRoles = newRoleIds.Select(roleId => new ModelHasRole
            {
                RoleId = roleId,
                ModelId = user.Id,
                ModelType = modelType
            }).ToList();

            _context.ModelHasRoles.AddRange(modelHasRoles);
            await _context.SaveChangesAsync();
            ClearUserCache(user);
        }
    }

    public async Task AssignRolesAsync<T>(T user, List<string> roleNames) where T : class, IHasRoles
    {
        var roles = await _context.Roles
            .Where(r => roleNames.Contains(r.Name) && r.GuardName == user.GuardName)
            .ToListAsync();

        var modelType = typeof(T).Name;
        var existingRoles = await _context.ModelHasRoles
            .Where(mhr => mhr.ModelId == user.Id && mhr.ModelType == modelType)
            .Select(mhr => mhr.RoleId)
            .ToListAsync();

        var newRoles = roles
            .Where(r => !existingRoles.Contains(r.Id))
            .Select(r => new ModelHasRole
            {
                RoleId = r.Id,
                ModelId = user.Id,
                ModelType = modelType
            })
            .ToList();

        if (newRoles.Any())
        {
            _context.ModelHasRoles.AddRange(newRoles);
            await _context.SaveChangesAsync();
            ClearUserCache(user);
        }
    }

    public async Task RemoveRoleAsync<T>(T user, string roleName) where T : class, IHasRoles
    {
        var role = await _roleRepository.GetByNameAndGuardAsync(roleName, user.GuardName);
        if (role == null)
        {
            return;
        }

        await RemoveRoleAsync(user, role.Id);
    }

    public async Task RemoveRoleAsync<T>(T user, long roleId) where T : class, IHasRoles
    {
        var modelType = typeof(T).Name;
        var modelHasRole = await _context.ModelHasRoles
            .FirstOrDefaultAsync(mhr => mhr.RoleId == roleId
                && mhr.ModelId == user.Id
                && mhr.ModelType == modelType);

        if (modelHasRole != null)
        {
            _context.ModelHasRoles.Remove(modelHasRole);
            await _context.SaveChangesAsync();
            ClearUserCache(user);
        }
    }
    public async Task RemoveRolesAsync<T>(T user, IEnumerable<long> roleIds) where T : class, IHasRoles
    {
        if (!roleIds.Any())
            return;

        var modelType = typeof(T).Name;
        var roleIdsList = roleIds.ToList();

        var modelHasRoles = await _context.ModelHasRoles
            .Where(mhr => roleIdsList.Contains(mhr.RoleId)
                && mhr.ModelId == user.Id
                && mhr.ModelType == modelType)
            .ToListAsync();

        if (modelHasRoles.Any())
        {
            _context.ModelHasRoles.RemoveRange(modelHasRoles);
            await _context.SaveChangesAsync();
            ClearUserCache(user);
        }
    }

    public async Task SyncRolesAsync<T>(T user, List<string> roleNames) where T : class, IHasRoles
    {
        var roles = await _context.Roles
            .Where(r => roleNames.Contains(r.Name) && r.GuardName == user.GuardName)
            .ToListAsync();

        var modelType = typeof(T).Name;
        var existingModelHasRoles = await _context.ModelHasRoles
            .Where(mhr => mhr.ModelId == user.Id && mhr.ModelType == modelType)
            .ToListAsync();


        var rolesToRemove = existingModelHasRoles
            .Where(mhr => !roles.Any(r => r.Id == mhr.RoleId))
            .ToList();

        if (rolesToRemove.Any())
        {
            _context.ModelHasRoles.RemoveRange(rolesToRemove);
        }


        var existingRoleIds = existingModelHasRoles.Select(mhr => mhr.RoleId).ToList();
        var newRoles = roles
            .Where(r => !existingRoleIds.Contains(r.Id))
            .Select(r => new ModelHasRole
            {
                RoleId = r.Id,
                ModelId = user.Id,
                ModelType = modelType
            })
            .ToList();

        if (newRoles.Any())
        {
            _context.ModelHasRoles.AddRange(newRoles);
        }

        await _context.SaveChangesAsync();
        ClearUserCache(user);
    }

    public async Task<bool> HasRoleAsync<T>(T user, string roleName) where T : class, IHasRoles
    {
        var cacheKey = GetUserRolesCacheKey(user);

        if (!_cache.TryGetValue(cacheKey, out HashSet<string>? userRoles))
        {
            userRoles = await LoadUserRolesAsync(user);
            _cache.Set(cacheKey, userRoles, TimeSpan.FromMinutes(_cacheExpirationMinutes));
        }

        return userRoles != null && userRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<bool> HasAnyRoleAsync<T>(T user, params string[] roles) where T : class, IHasRoles
    {
        foreach (var role in roles)
        {
            if (await HasRoleAsync(user, role))
            {
                return true;
            }
        }
        return false;
    }

    public async Task<bool> HasAllRolesAsync<T>(T user, params string[] roles) where T : class, IHasRoles
    {
        foreach (var role in roles)
        {
            if (!await HasRoleAsync(user, role))
            {
                return false;
            }
        }
        return true;
    }

    public async Task<List<string>> GetRoleNamesAsync<T>(T user) where T : class, IHasRoles
    {
        var cacheKey = GetUserRolesCacheKey(user);

        if (!_cache.TryGetValue(cacheKey, out HashSet<string>? userRoles))
        {
            userRoles = await LoadUserRolesAsync(user);
            _cache.Set(cacheKey, userRoles, TimeSpan.FromMinutes(_cacheExpirationMinutes));
        }

        return (userRoles ?? new HashSet<string>()).OrderBy(r => r).ToList();
    }

    public async Task<List<Role>> GetRolesAsync<T>(T user) where T : class, IHasRoles
    {
        var modelType = typeof(T).Name;

        return await _context.ModelHasRoles
            .Where(mhr => mhr.ModelId == user.Id && mhr.ModelType == modelType)
            .Include(mhr => mhr.Role)
            .Select(mhr => mhr.Role)
            .OrderBy(r => r.Name)
            .ToListAsync();
    }

    #endregion

    #region Role-Permission Management

    public async Task GivePermissionToRoleAsync(string roleName, string permissionName, string guardName)
    {
        var role = await _roleRepository.GetByNameAndGuardAsync(roleName, guardName);
        var permission = await _permissionRepository.GetByNameAndGuardAsync(permissionName, guardName);

        if (role == null || permission == null)
        {
            throw new InvalidOperationException("Role or permission not found");
        }

        await GivePermissionToRoleAsync(role.Id, permission.Id);
    }

    public async Task GivePermissionToRoleAsync(long roleId, long permissionId)
    {
        var exists = await _context.RoleHasPermissions
            .AnyAsync(rhp => rhp.RoleId == roleId && rhp.PermissionId == permissionId);

        if (!exists)
        {
            var roleHasPermission = new RoleHasPermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            };

            _context.RoleHasPermissions.Add(roleHasPermission);
            await _context.SaveChangesAsync();

            await ClearCacheForRoleUsersAsync(roleId);
        }
    }

    public async Task GivePermissionsToRoleAsync(string roleName, List<string> permissionNames, string guardName)
    {
        var role = await _roleRepository.GetByNameAndGuardAsync(roleName, guardName);
        if (role == null)
        {
            throw new InvalidOperationException($"Role '{roleName}' not found");
        }

        var permissions = await _context.Permissions
            .Where(p => permissionNames.Contains(p.Name) && p.GuardName == guardName)
            .ToListAsync();

        var existingPermissions = await _context.RoleHasPermissions
            .Where(rhp => rhp.RoleId == role.Id)
            .Select(rhp => rhp.PermissionId)
            .ToListAsync();

        var newPermissions = permissions
            .Where(p => !existingPermissions.Contains(p.Id))
            .Select(p => new RoleHasPermission
            {
                RoleId = role.Id,
                PermissionId = p.Id
            })
            .ToList();

        if (newPermissions.Any())
        {
            _context.RoleHasPermissions.AddRange(newPermissions);
            await _context.SaveChangesAsync();
            ClearAllCache();
        }
    }

    public async Task RevokePermissionFromRoleAsync(string roleName, string permissionName, string guardName)
    {
        var role = await _roleRepository.GetByNameAndGuardAsync(roleName, guardName);
        var permission = await _permissionRepository.GetByNameAndGuardAsync(permissionName, guardName);

        if (role == null || permission == null)
        {
            return;
        }

        var roleHasPermission = await _context.RoleHasPermissions
            .FirstOrDefaultAsync(rhp => rhp.RoleId == role.Id && rhp.PermissionId == permission.Id);

        if (roleHasPermission != null)
        {
            _context.RoleHasPermissions.Remove(roleHasPermission);
            await _context.SaveChangesAsync();

            await ClearCacheForRoleUsersAsync(role.Id);
        }
    }

    public async Task SyncPermissionsToRoleAsync(string roleName, List<string> permissionNames, string guardName)
    {
        var role = await _roleRepository.GetByNameAndGuardAsync(roleName, guardName);
        if (role == null)
        {
            throw new InvalidOperationException($"Role '{roleName}' not found");
        }

        var permissions = await _context.Permissions
            .Where(p => permissionNames.Contains(p.Name) && p.GuardName == guardName)
            .ToListAsync();

        var existingRoleHasPermissions = await _context.RoleHasPermissions
            .Where(rhp => rhp.RoleId == role.Id)
            .ToListAsync();


        var permissionsToRemove = existingRoleHasPermissions
            .Where(rhp => !permissions.Any(p => p.Id == rhp.PermissionId))
            .ToList();

        if (permissionsToRemove.Any())
        {
            _context.RoleHasPermissions.RemoveRange(permissionsToRemove);
        }


        var existingPermissionIds = existingRoleHasPermissions.Select(rhp => rhp.PermissionId).ToList();
        var newPermissions = permissions
            .Where(p => !existingPermissionIds.Contains(p.Id))
            .Select(p => new RoleHasPermission
            {
                RoleId = role.Id,
                PermissionId = p.Id
            })
            .ToList();

        if (newPermissions.Any())
        {
            _context.RoleHasPermissions.AddRange(newPermissions);
        }

        await _context.SaveChangesAsync();

        await ClearCacheForRoleUsersAsync(role.Id);
    }
    public async Task SyncPermissionsToRoleAsync(long roleId, List<long> permissionIds)
    {

        var role = await _roleRepository.GetByIdAsync(roleId);
        if (role == null)
        {
            throw new InvalidOperationException($"Role with ID {roleId} not found");
        }


        var permissions = await _permissionRepository.GetByIdsAsync(permissionIds);


        if (permissions.Count != permissionIds.Count)
        {
            var foundIds = permissions.Select(p => p.Id).ToHashSet();
            var missingIds = permissionIds.Where(id => !foundIds.Contains(id)).ToList();
            throw new InvalidOperationException($"Permissions not found: {string.Join(", ", missingIds)}");
        }


        var invalidPermissions = permissions.Where(p => p.GuardName != role.GuardName).ToList();
        if (invalidPermissions.Any())
        {
            var invalidNames = string.Join(", ", invalidPermissions.Select(p => $"{p.Name} (GuardName: {p.GuardName})"));
            throw new InvalidOperationException(
                $"Role and permission must have the same GuardName. " +
                $"Role GuardName: {role.GuardName}. Invalid permissions: {invalidNames}");
        }


        var existingRoleHasPermissions = await _context.RoleHasPermissions
            .Where(rhp => rhp.RoleId == roleId)
            .ToListAsync();


        if (existingRoleHasPermissions.Any())
        {
            _context.RoleHasPermissions.RemoveRange(existingRoleHasPermissions);
        }


        var newRoleHasPermissions = permissions
            .Select(p => new RoleHasPermission
            {
                RoleId = roleId,
                PermissionId = p.Id
            })
            .ToList();

        if (newRoleHasPermissions.Any())
        {
            _context.RoleHasPermissions.AddRange(newRoleHasPermissions);
        }

        await _context.SaveChangesAsync();


        await ClearCacheForRoleUsersAsync(roleId);
    }
    #endregion

    #region Cache Management

    public void ClearUserCache<T>(T user) where T : class, IHasRoles
    {
        ClearModelCache(user.Id, typeof(T).Name);
    }

    private void ClearModelCache(long modelId, string modelType)
    {
        var permissionsCacheKey = $"permissions:{modelType}:{modelId}";
        var rolesCacheKey = $"roles:{modelType}:{modelId}";

        _cache.Remove(permissionsCacheKey);
        _cache.Remove(rolesCacheKey);
    }

    public void ClearAllCache()
    {




    }





    public async Task ClearCacheForRoleUsersAsync(long roleId)
    {
        var usersWithRole = await _context.ModelHasRoles
            .Where(mhr => mhr.RoleId == roleId)
            .Select(mhr => new { mhr.ModelId, mhr.ModelType })
            .ToListAsync();




        foreach (var user in usersWithRole)
        {
            var permissionsKey = $"permissions:{user.ModelType}:{user.ModelId}";
            var rolesKey = $"roles:{user.ModelType}:{user.ModelId}";
            _cache.Remove(permissionsKey);
            _cache.Remove(rolesKey);
        }
    }

    #endregion

    #region Permission Synchronization






    public async Task SyncRolePermissionsToUserAsync<T>(T user) where T : class, IHasRoles
    {
        await SyncRolePermissionsAsync(user.Id, typeof(T).Name);
    }







    public async Task SyncRolePermissionsAsync(long modelId, string modelType)
    {

        var modelRoles = await _context.ModelHasRoles
            .Where(mhr => mhr.ModelId == modelId && mhr.ModelType == modelType)
            .Include(mhr => mhr.Role)
            .Select(mhr => mhr.RoleId)
            .ToListAsync();


        var expectedPermissionIds = new HashSet<long>();

        if (modelRoles.Any())
        {
            var allRolePermissions = await _context.RoleHasPermissions
                .Where(rhp => modelRoles.Contains(rhp.RoleId))
                .Select(rhp => rhp.PermissionId)
                .Distinct()
                .ToListAsync();

            foreach (var permissionId in allRolePermissions)
            {
                expectedPermissionIds.Add(permissionId);
            }
        }


        var existingModelPermissions = await _context.ModelHasPermissions
            .Where(mhp => mhp.ModelId == modelId && mhp.ModelType == modelType)
            .ToListAsync();

        var existingPermissionIds = existingModelPermissions
            .Select(mhp => mhp.PermissionId)
            .ToHashSet();


        var permissionIdsToAdd = expectedPermissionIds
            .Except(existingPermissionIds)
            .ToList();

        if (permissionIdsToAdd.Any())
        {
            var newModelHasPermissions = permissionIdsToAdd.Select(permissionId => new ModelHasPermission
            {
                PermissionId = permissionId,
                ModelId = modelId,
                ModelType = modelType
            }).ToList();

            _context.ModelHasPermissions.AddRange(newModelHasPermissions);
        }



        var permissionsToRemove = existingModelPermissions
            .Where(mhp => !expectedPermissionIds.Contains(mhp.PermissionId))
            .ToList();

        if (permissionsToRemove.Any())
        {
            _context.ModelHasPermissions.RemoveRange(permissionsToRemove);
        }


        if (permissionIdsToAdd.Any() || permissionsToRemove.Any())
        {
            await _context.SaveChangesAsync();


            ClearModelCache(modelId, modelType);
        }
    }

    public async Task AssignRolesIfProvidedAsync<T>(T user, List<long>? roleIds) where T : class, IHasRoles
    {
        if (roleIds != null && roleIds.Any())
        {
            try
            {
                await AssignRolesAsync(user, roleIds);
            }
            catch (InvalidOperationException ex)
            {
                throw new Shared.Exceptions.LogicalException($"Failed to assign roles: {ex.Message}");
            }

            await SyncRolePermissionsToUserAsync(user);
        }
    }

    public async Task UpdateRolesAsync<T>(T user, List<long>? roleIds) where T : class, IHasRoles
    {
        if (roleIds != null)
        {
            var currentRoleIds = (await GetRolesAsync(user)).Select(r => r.Id).ToList();
            var rolesToAdd = roleIds.Except(currentRoleIds).ToList();
            var rolesToRemove = currentRoleIds.Except(roleIds).ToList();

            if (rolesToRemove.Any())
            {
                try
                {
                    await RemoveRolesAsync(user, rolesToRemove);
                }
                catch (InvalidOperationException ex)
                {
                    throw new Shared.Exceptions.LogicalException($"Failed to remove roles: {ex.Message}");
                }
            }

            if (rolesToAdd.Any())
            {
                try
                {
                    await AssignRolesAsync(user, rolesToAdd);
                }
                catch (InvalidOperationException ex)
                {
                    throw new Shared.Exceptions.LogicalException($"Failed to assign roles: {ex.Message}");
                }
            }

            if (rolesToAdd.Any() || rolesToRemove.Any())
            {
                await SyncRolePermissionsToUserAsync(user);
            }
        }
    }
    #endregion

    #region Audience Resolution

    public async Task<IReadOnlyList<long>> GetModelIdsWithPermissionAsync(
        string permissionName,
        string guardName,
        string modelType,
        CancellationToken cancellationToken = default)
    {
        string cacheKey = $"permission-owners:{permissionName}:{guardName}:{modelType}";

        if (_cache.TryGetValue(cacheKey, out IReadOnlyList<long>? cached))
            return cached!;

        var permission = await _permissionRepository.GetByNameAndGuardAsync(permissionName, guardName);
        if (permission is null)
        {
            var emptyResult = Array.Empty<long>();
            _cache.Set(cacheKey, emptyResult, TimeSpan.FromMinutes(_cacheExpirationMinutes));
            return emptyResult;
        }

        var modelIds = await _context.ModelHasPermissions
            .Where(mhp => mhp.PermissionId == permission.Id && 
                          mhp.ModelType == modelType)
            .Select(mhp => mhp.ModelId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var result = (IReadOnlyList<long>)modelIds;
        _cache.Set(cacheKey, result, TimeSpan.FromMinutes(_cacheExpirationMinutes));
        return result;
    }

    #endregion

    #region Private Helper Methods

    private async Task<HashSet<string>> LoadUserPermissionsAsync<T>(T user) where T : class, IHasRoles
    {
        var modelType = typeof(T).Name;


        var permissions = await _context.ModelHasPermissions
            .Where(mhp => mhp.ModelId == user.Id && mhp.ModelType == modelType)
            .Include(mhp => mhp.Permission)
            .Select(mhp => mhp.Permission.Name)
            .Union(
                _context.ModelHasRoles
                    .Where(mhr => mhr.ModelId == user.Id && mhr.ModelType == modelType)
                    .Include(mhr => mhr.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rhp => rhp.Permission)
                    .SelectMany(mhr => mhr.Role.RolePermissions
                        .Select(rhp => rhp.Permission.Name))
            )
            .ToListAsync();

        return new HashSet<string>(permissions, StringComparer.OrdinalIgnoreCase);
    }

    private async Task<HashSet<string>> LoadUserRolesAsync<T>(T user) where T : class, IHasRoles
    {
        var modelType = typeof(T).Name;

        var roles = await _context.ModelHasRoles
            .Where(mhr => mhr.ModelId == user.Id && mhr.ModelType == modelType)
            .Include(mhr => mhr.Role)
            .Select(mhr => mhr.Role.Name)
            .ToListAsync();

        return new HashSet<string>(roles, StringComparer.OrdinalIgnoreCase);
    }

    private string GetUserPermissionsCacheKey<T>(T user) where T : class, IHasRoles
    {
        return $"permissions:{typeof(T).Name}:{user.Id}";
    }

    private string GetUserRolesCacheKey<T>(T user) where T : class, IHasRoles
    {
        return $"roles:{typeof(T).Name}:{user.Id}";
    }

    private bool IsWildcardMatch(string pattern, string permission)
    {
        if (!pattern.Contains('*'))
        {
            return false;
        }



        var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(pattern)
            .Replace("\\*", ".*") + "$";

        return System.Text.RegularExpressions.Regex.IsMatch(
            permission,
            regexPattern,
            System.Text.RegularExpressions.RegexOptions.IgnoreCase
        );
    }

    #endregion
}

