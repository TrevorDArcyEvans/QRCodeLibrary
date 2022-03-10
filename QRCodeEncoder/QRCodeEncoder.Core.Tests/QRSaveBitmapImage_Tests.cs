namespace QRCodeEncoder.Core.Tests;

using FluentAssertions;
using NUnit.Framework;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using System;
using System.Collections.Generic;
using System.IO;

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
  public void SaveQRCodeToImageFile_Returns([ValueSource(nameof(GetImageFormat))] IImageFormat imgFmt)
  {
    var matrix = GetMatrix();
    var saver = Create(matrix);
    using var stream = new MemoryStream();

    Action act = () => saver.SaveQRCodeToImageFile(stream, imgFmt);

    act.Should().NotThrow();
  }

  private static IEnumerable<IImageFormat> GetImageFormat => new IImageFormat[]
  {
    BmpFormat.Instance,
    GifFormat.Instance,
    JpegFormat.Instance,
    PngFormat.Instance,
    TiffFormat.Instance
  };

  private QRSaveBitmapImage Create(bool[,] matrix)
  {
    return new(matrix);
  }
}
