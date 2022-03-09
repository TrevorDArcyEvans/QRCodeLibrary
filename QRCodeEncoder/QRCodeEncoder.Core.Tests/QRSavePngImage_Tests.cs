namespace QRCodeEncoder.Core.Tests;

using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;

public sealed class QRSavePngImage_Tests
{
  [Test]
  public void Constructor_Succeeds()
  {
    var qrenc = CreateEncoder();
    var data = new[] { Text1, Text2 };
    var matrix = qrenc.Encode(data);

    Action act = () => Create(matrix);

    act.Should().NotThrow();
  }

  [Test]
  public void SaveQRCodeToPngFile_Returns()
  {
    var qrenc = CreateEncoder();
    var data = new[] { Text1, Text2 };
    var matrix = qrenc.Encode(data);
    var saver = Create(matrix);
    using var stream = new MemoryStream();

    Action act = () => saver.SaveQRCodeToPngFile(stream);

    act.Should().NotThrow();
  }

  private const string Text1 = "0123456789";
  private const string Text2 = "some arbitrary text SOME OTHER ARBITRARY TEXT 0123456789 $ % * + - . / : []{}()&^£!?@|";

  private QRSavePngImage Create(bool[,] matrix)
  {
    return new QRSavePngImage(matrix);
  }

  private QREncoder CreateEncoder()
  {
    return new QREncoder();
  }
}
