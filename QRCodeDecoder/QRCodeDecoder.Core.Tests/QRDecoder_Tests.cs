namespace QRCodeDecoder.Core.Tests
{
  using FluentAssertions;
  using Moq;
  using NUnit.Framework;
  using Serilog;
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Reflection;

  public class QRDecoder_Tests
  {
    private Mock<ILogger> _mocklogger;

    [SetUp]
    public void Setup()
    {
      _mocklogger = new Mock<ILogger>();
    }

    [Test]
    public void Constructor_Succeeds()
    {
      Action act = () => Create();

      act.Should().NotThrow();
    }

    [Test]
    public void ImageDecoder_Succeeds([ValueSource(nameof(DataFiles))] string filePath)
    {
      var qrdec = Create();

      var res = qrdec.ImageDecoder(filePath);

      res.Should().NotBeEmpty();
    }

    private static IEnumerable<string> DataFiles()
    {
      var currAssy = Assembly.GetExecutingAssembly().Location;
      var currDir = Path.GetDirectoryName(currAssy);
      var dataDir = Path.Combine(currDir, "Data");
      var pngFiles = Directory.EnumerateFiles(dataDir, "*.png", SearchOption.AllDirectories);
      var jpgFiles = Directory.EnumerateFiles(dataDir, "*.jpg", SearchOption.AllDirectories);
      var allFiles = pngFiles.Concat(jpgFiles);
      return allFiles;
    }

    private QRDecoder Create()
    {
      return new QRDecoder(_mocklogger.Object);
    }
  }
}