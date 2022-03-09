namespace QRCodeEncoder.Core.Tests;

using FluentAssertions;
using NUnit.Framework;
using System;
using System.IO;

public sealed class QRSavePngImage_Tests : QRSaveImage_Base
{
  [Test]
  public void Constructor_Succeeds()
  {
    var matrix = GetMatrix();

    Action act = () => Create(matrix);

    act.Should().NotThrow();
  }

  [Test]
  public void SaveQRCodeToPngFile_Returns()
  {
    var matrix = GetMatrix();
    var saver = Create(matrix);
    using var stream = new MemoryStream();

    Action act = () => saver.SaveQRCodeToPngFile(stream);

    act.Should().NotThrow();
  }

  private QRSavePngImage Create(bool[,] matrix)
  {
    return new QRSavePngImage(matrix);
  }
}
