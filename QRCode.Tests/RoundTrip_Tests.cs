namespace QRCode.Tests;

using FluentAssertions;
using FluentAssertions.Execution;
using Moq;
using NUnit.Framework;
using QRCodeDecoder.Core;
using QRCodeEncoder.Core;
using Serilog;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class RoundTrip_Tests
{
  private Mock<ILogger> _mocklogger;

  [SetUp]
  public void Setup()
  {
    _mocklogger = new Mock<ILogger>();
  }

  [Test]
  public void RoundTrip_Bitmap_Succeeds(
    [Values] QRCodeEncoder.Core.ErrorCorrection err,
    [ValueSource(nameof(ECIrange))] int eci)
  {
    var qrenc = CreateEncoder(err, eci);
    var data = new[] { Text1, Text2 };
    var matrix = qrenc.Encode(data);
    var saver = CreateBitmapSaver(matrix);
    var bmp = saver.CreateQRCodeBitmap();
    var qrdec = CreateDecoder();

    var res = qrdec.ImageDecoder(bmp);

    using (new AssertionScope())
    {
      res.Count().Should().Be(1);

      var text = Encoding.UTF8.GetString(res.Single().DataArray);
      text.Should().Be(Text1 + Text2);
    }
  }

  [Test]
  public void RoundTrip_Png_Succeeds(
    [Values] QRCodeEncoder.Core.ErrorCorrection err,
    [ValueSource(nameof(ECIrange))] int eci)
  {
    var qrenc = CreateEncoder(err, eci);
    var data = new[] { Text1, Text2 };
    var matrix = qrenc.Encode(data);
    var saver = CreatePngSaver(matrix);
    var pngData = saver.QRCodeToPngFormat();
    var png = Image.Load<Rgba32>(pngData, out _);
    var qrdec = CreateDecoder();

    var res = qrdec.ImageDecoder(png);

    using (new AssertionScope())
    {
      res.Count().Should().Be(1);

      var text = Encoding.UTF8.GetString(res.Single().DataArray);
      text.Should().Be(Text1 + Text2);
    }
  }

  private static IEnumerable<int> ECIrange => new[] { -1, 0, 127, 16383, 32767 };

  private const string Text1 = "0123456789";
  private const string Text2 = "some arbitrary text SOME OTHER ARBITRARY TEXT 0123456789 $ % * + - . / : []{}()&^£!?@|";

  private QREncoder CreateEncoder(
    QRCodeEncoder.Core.ErrorCorrection err,
    int eci = 0)
  {
    return new()
    {
      ECIAssignValue = eci,
      ErrorCorrection = err
    };
  }

  private QRSaveBitmapImage CreateBitmapSaver(bool[,] matrix)
  {
    return new(matrix);
  }

  private QRSavePngImage CreatePngSaver(bool[,] matrix)
  {
    return new(matrix);
  }

  private QRDecoder CreateDecoder()
  {
    return new(_mocklogger.Object);
  }
}
