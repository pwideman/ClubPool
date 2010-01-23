using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NUnit.Framework;
using SharpArch.Testing.NUnit;

using ClubPool.Framework.Extensions;

namespace Tests.ClubPool.Framework.Extensions
{
  [TestFixture]
  public class StringExtensionsTests
  {
    // IsNullOrEmptyOrBlank tests

    [Test]
    public void IsNullOrEmptyOrBlank_returns_true_for_null() {
      string test = null;
      test.IsNullOrEmptyOrBlank().ShouldBeTrue();
    }

    [Test]
    public void IsNullOrEmptyOrBlank_returns_true_for_empty() {
      string test = string.Empty;
      test.IsNullOrEmptyOrBlank().ShouldBeTrue();
    }

    [Test]
    public void IsNullOrEmptyOrBlank_returns_true_for_blank() {
      string test = "   ";
      test.IsNullOrEmptyOrBlank().ShouldBeTrue();
    }

    [Test]
    public void IsNullOrEmptyOrBlank_returns_false_for_valid_string() {
      string test = "some string";
      test.IsNullOrEmptyOrBlank().ShouldBeFalse();
    }

    // IsNonEmptyWithLengthLessThan tests

    //[Test]
    //public void IsNonEmptyWithLengthLessThan_returns_false_for_null() {
    //  string test = null;
    //  test.IsNonEmptyWithLengthLessThan(1).ShouldBeFalse();
    //}

    //[Test]
    //public void IsNonEmptyWithLengthLessThan_returns_false_for_empty() {
    //  string test = string.Empty;
    //  test.IsNonEmptyWithLengthLessThan(1).ShouldBeFalse();
    //}

    //[Test]
    //public void IsNonEmptyWithLengthLessThan_returns_false_for_blank() {
    //  string test = "   ";
    //  test.IsNonEmptyWithLengthLessThan(1).ShouldBeFalse();
    //}

    //[Test]
    //public void IsNonEmptyWithLengthLessThan_returns_false_for_string_that_is_too_long() {
    //  string test = "Some long string";
    //  test.IsNonEmptyWithLengthLessThan(16).ShouldBeFalse();
    //}

    //[Test]
    //public void IsNonEmptyWithLengthLessThan_returns_true_for_valid_string() {
    //  string test = "Some string";
    //  test.IsNonEmptyWithLengthLessThan(12).ShouldBeTrue();
    //}
  }
}
