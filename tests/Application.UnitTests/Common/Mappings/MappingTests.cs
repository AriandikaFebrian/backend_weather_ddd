// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using AutoMapper;
using FluentAssertions;
using NetCa.Application.Common.Mappings;
using NUnit.Framework;

namespace NetCa.Application.UnitTests.Common.Mappings;

/// <summary>
/// MappingTests
/// </summary>
public class MappingTests
{
    private readonly IConfigurationProvider _configuration;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="MappingTests"/> class.
    /// </summary>
    public MappingTests()
    {
        _configuration = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<MappingProfile>();
        });

        _mapper = _configuration.CreateMapper();
    }

    /// <summary>
    /// ShouldHaveValidConfiguration
    /// </summary>
    [Test]
    public void ShouldHaveValidConfiguration()
    {
        _configuration.AssertConfigurationIsValid();
    }

    /// <summary>
    /// ShouldSupportMappingFromSourceToDestination
    /// </summary>
    /// <param name="source"/>
    /// <param name="destination"/>
    [Test]
    [TestCase(null, null)]
    public void ShouldSupportMappingFromSourceToDestination(Type source, Type destination)
    {
        if (source is null || destination is null)
        {
            return;
        }

        var test = _mapper.Map(GetInstanceOf(source), source, destination);
        test.Should().BeAssignableTo(destination);
    }

    private static object GetInstanceOf(Type type)
    {
        if (type.GetConstructor(Type.EmptyTypes) != null)
        {
            return Activator.CreateInstance(type);
        }

        return FormatterServices.GetUninitializedObject(type);
    }
}
