namespace QRCodeEncoder.Core.Tests;

using System;
using FluentAssertions;
using NUnit.Framework;

public class QREncoder_Tests
{
  [Test]
  public void Constructor_Succeeds()
  {
    Action act = () => Create();

    act.Should().NotThrow();
  }

  [Test]
  public void ErrorCorrection_Default_IsM()
  {
    var qrenc = Create();

    qrenc.ErrorCorrection.Should().Be(ErrorCorrection.M);
  }

  [Test]
  public void ErrorCorrection_InRange_Succeeds([Values]ErrorCorrection val)
  {
    var qrenc = Create();

    qrenc.ErrorCorrection = val;

    qrenc.ErrorCorrection.Should().Be(val);
  }

  [Test]
  public void ECIAssignValue_InRange_Succeeds(

    [Values(-1, 0, 1, 10, 100, 1000, 10000, 100000, 999999)] int val)
  {
    var qrenc = Create();

    qrenc.ECIAssignValue = val;

    qrenc.ECIAssignValue.Should().Be(val);
  }

  [Test]
  public void ECIAssignValue_OutOfRange_Throws(
    [Values(-10, -2, 1000000)] int val)
  {
    var qrenc = Create();

    Action act = () => qrenc.ECIAssignValue = val;

    act.Should().Throw<Exception>();
  }

  private QREncoder Create()
  {
    return new QREncoder();
  }
}