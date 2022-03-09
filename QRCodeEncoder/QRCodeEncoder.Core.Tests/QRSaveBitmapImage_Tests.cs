namespace QRCodeEncoder.Core.Tests;

using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using FluentAssertions;
using NUnit.Framework;

public sealed class QRSaveBitmapImage_Tests : QRSaveImage_Base
{
  [Test]
  public void Constructor_Succeeds()
  {
    var matrix = GetMatrix();

    Action act = () => Create(matrix);

    act.Should().NotThrow();
  }

  [Test]
  public void SaveQRCodeToImageFile_Returns([ValueSource(nameof(GetImageFormat))] ImageFormat imgFmt)
  {
    var matrix = GetMatrix();
    var saver = Create(matrix);
    using var stream = new MemoryStream();

    Action act = () => saver.SaveQRCodeToImageFile(stream, imgFmt);

    act.Should().NotThrow();
  }

  private static IEnumerable<ImageFormat> GetImageFormat => new[]
  {
    ImageFormat.Bmp,
    ImageFormat.Gif,
    ImageFormat.Jpeg,
    ImageFormat.Png,
    ImageFormat.Tiff
  };

  private QRSaveBitmapImage Create(bool[,] matrix)
  {
    return new(matrix);
  }
}
