namespace QRCodeEncoder.Core.Tests;

using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

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
  public void ErrorCorrection_InRange_Succeeds([Values] ErrorCorrection val)
  {
    var qrenc = Create();

    qrenc.ErrorCorrection = val;

    qrenc.ErrorCorrection.Should().Be(val);
  }

  [Test]
  public void ErrorCorrection_OutOfRange_Throws([Values(-1, 4, 5, 10)] int val)
  {
    var qrenc = Create();

    Action act = () => qrenc.ErrorCorrection = (ErrorCorrection) val;

    act.Should().Throw<ArgumentOutOfRangeException>();
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

    act.Should().Throw<ArgumentOutOfRangeException>();
  }

  [Test]
  public void Encode_EmptyNull_Throws(
    [Values(null, "")] string val)
  {
    var qrenc = Create();

    Action act = () => qrenc.Encode(val);

    act.Should().Throw<ArgumentException>();
  }

  [Test]
  public void Encode_Bytes_Returns(
    [Values] ErrorCorrection err,
    [ValueSource(nameof(ECIrange))] int eci)
  {
    var qrenc = Create(err, eci);

    var data = Encoding.UTF8.GetBytes(Text1);
    var res = qrenc.Encode(data);

    res.Should().NotBeNull();
  }

  [Test]
  public void Encode_SingleString_Returns(
    [Values] ErrorCorrection err,
    [ValueSource(nameof(ECIrange))] int eci)
  {
    var qrenc = Create(err, eci);

    var data = Text1;
    var res = qrenc.Encode(data);

    res.Should().NotBeNull();
  }

  [Test]
  public void Encode_MultipleString_Returns(
    [Values] ErrorCorrection err,
    [ValueSource(nameof(ECIrange))] int eci)
  {
    var qrenc = Create(err, eci);

    var data = new[] { Text1, Text2 };
    var res = qrenc.Encode(data);

    res.Should().NotBeNull();
  }

  private static IEnumerable<int> ECIrange => new[] { -1, 0, 127, 16383, 32767 };

  private const string Text1 = "0123456789";
  private const string Text2 = "some arbitrary text SOME OTHER ARBITRARY TEXT 0123456789 $ % * + - . / : []{}()&^£!?@|";

  private QREncoder Create(int eci = 0)
  {
    return new QREncoder
    {
      ECIAssignValue = eci
    };
  }

  private QREncoder Create(
    ErrorCorrection err,
    int eci = 0)
  {
    return new QREncoder
    {
      ECIAssignValue = eci,
      ErrorCorrection = err
    };
  }
}
