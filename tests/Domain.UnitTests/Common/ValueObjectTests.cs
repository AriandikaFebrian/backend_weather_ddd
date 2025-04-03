// -----------------------------------------------------------------------------------
// Copyright DAD RnD 2024. All rights reserved.
// United Tractors DAD Mobile Web Help Desk (helpdesk.mobweb@unitedtractors.com)
// -----------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Globalization;
using FluentAssertions;
using NetCa.Domain.Common;
using NUnit.Framework;

namespace NetCa.Domain.UnitTests.Common;

/// <summary>
/// ValueObjectTests
/// </summary>
public class ValueObjectTests
{
    /// <summary>
    /// Equals_GivenDifferentValues_ShouldReturnFalse
    /// </summary>
    [Test]
    public void Equals_GivenDifferentValues_ShouldReturnFalse()
    {
        var point1 = new Point(1, 2);
        var point2 = new Point(2, 1);

        point1.Should().NotBe(point2);
    }

    /// <summary>
    /// Equals_GivenMatchingValues_ShouldReturnTrue
    /// </summary>
    [Test]
    public void Equals_GivenMatchingValues_ShouldReturnTrue()
    {
        var point1 = new Point(1, 2);
        var point2 = new Point(1, 2);

        point1.Should().Be(point2);
    }

    private class Point(int X, int Y) : ValueObject
    {
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return X.ToString(CultureInfo.InvariantCulture);
            yield return Y.ToString(CultureInfo.InvariantCulture);
        }
    }
}
