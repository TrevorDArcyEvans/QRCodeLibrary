namespace QRCodeEncoder.Core.Tests;

using System;
using FluentAssertions;
using NUnit.Framework;

public sealed class QRSaveImagePixels_Tests : QRSaveImage_Base
{
  [Test]
  public void Constructor_Succeeds()
  {
    var matrix = GetMatrix();

    Action act = () => Create(matrix);

    act.Should().NotThrow();
  }

  [Test]
  public void ConvertQRCodeMatrixToPixels_Returns()
  {
    var matrix = GetMatrix();
    var saver = Create(matrix);

    var res= saver.ConvertQRCodeMatrixToPixels();

    res.Should().NotBeNull();
  }

  private QRSaveImagePixels Create(bool[,] matrix)
  {
    return new (matrix);
  }
}
