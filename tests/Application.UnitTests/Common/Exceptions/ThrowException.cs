// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using FluentAssertions;
using NetCa.Application.Common.Exceptions;
using NUnit.Framework;

namespace NetCa.Application.UnitTests.Common.Exceptions;

/// <summary>
/// NotFoundExceptionTests
/// </summary>
public class ThrowExceptionTests
{
    /// <summary>
    /// DefaultConstructorCreatesAnExceptionWithMessage
    /// </summary>
    /// <param name="message"/>
    [Test]
    [TestCase("Data Not Found")]
    public void DefaultConstructorCreatesAnExceptionWithMessage(string message)
    {
        var actual = new ThrowException(message);

        actual.Message.Should().BeEquivalentTo(message);
    }
}
