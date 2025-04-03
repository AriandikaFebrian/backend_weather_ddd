// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using FluentAssertions;
using FluentValidation.Results;
using NetCa.Application.Common.Exceptions;
using NUnit.Framework;

namespace NetCa.Application.UnitTests.Common.Exceptions;

/// <summary>
/// ValidationExceptionTests
/// </summary>
public class ValidationExceptionTests
{
    /// <summary>
    /// DefaultConstructorCreatesAnEmptyErrorDictionary
    /// </summary>
    [Test]
    public void DefaultConstructorCreatesAnEmptyErrorDictionary()
    {
        var actual = new ValidationException().Errors;

        actual.Keys.Should().BeEquivalentTo(Array.Empty<string>());
    }

    /// <summary>
    /// SingleValidationFailureCreatesASingleElementErrorDictionary
    /// </summary>
    [Test]
    public void SingleValidationFailureCreatesASingleElementErrorDictionary()
    {
        var failures = new List<ValidationFailure> { new("Age", "must be over 18"), };

        var actual = new ValidationException(failures).Errors;

        actual.Keys.Should().BeEquivalentTo("Age");
        actual["Age"].Should().BeEquivalentTo("must be over 18");
    }

    /// <summary>
    /// MulitpleValidationFailureForMultiplePropertiesCreatesAMultipleElementErrorDictionaryEachWithMultipleValues
    /// </summary>
    [Test]
    public void MulitpleValidationFailureForMultiplePropertiesCreatesAMultipleElementErrorDictionaryEachWithMultipleValues()
    {
        var failures = new List<ValidationFailure>
        {
            new ("Age", "must be 18 or older"),
            new ("Age", "must be 25 or younger"),
            new ("Password", "must contain at least 8 characters"),
            new ("Password", "must contain a digit"),
            new ("Password", "must contain upper case letter"),
            new ("Password", "must contain lower case letter"),
        };

        var actual = new ValidationException(failures).Errors;

        actual.Keys.Should().BeEquivalentTo("Password", "Age");

        actual["Age"].Should().BeEquivalentTo("must be 25 or younger", "must be 18 or older");

        actual["Password"]
            .Should()
            .BeEquivalentTo(
                "must contain lower case letter",
                "must contain upper case letter",
                "must contain at least 8 characters",
                "must contain a digit");
    }
}
