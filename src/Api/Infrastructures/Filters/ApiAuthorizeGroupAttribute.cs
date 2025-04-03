// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

namespace NetCa.Api.Infrastructures.Filters;

/// <summary>
/// ApiAuthorizeGroupAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiAuthorizeGroupAttribute : Attribute
{
    /// <summary>
    /// Gets or sets group
    /// </summary>
    public string[] Group { get; set; }
}

/// <summary>
/// ApiAuthorizeRoleAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiAuthorizeRoleAttribute : Attribute
{
    /// <summary>
    /// Gets or sets role
    /// </summary>
    public string[] Role { get; set; }
}

/// <summary>
/// ApiUserRoleCustomAttribute
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class ApiAuthorizeRoleLevelAttribute : Attribute
{
    /// <summary>
    /// Gets or sets level
    /// </summary>
    public int[] Level { get; set; }
}
