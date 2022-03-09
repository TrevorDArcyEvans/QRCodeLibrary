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
    public void ImageDecoder_Single_Succeeds([ValueSource(nameof(SingleDataFiles))] string filePath)
    {
      var qrdec = Create();

      var res = qrdec.ImageDecoder(filePath);

      res.Count().Should().Be(1);
    }

    [Test]
    public void ImageDecoder_Multiple_Succeeds([ValueSource(nameof(MultipleDataFiles))] string filePath)
    {
      var qrdec = Create();

      var res = qrdec.ImageDecoder(filePath);

      res.Count().Should().BeGreaterThan(1);
    }

    private static IEnumerable<string> GetDataFiles(string subDir)
    {
      var currAssy = Assembly.GetExecutingAssembly().Location;
      var currDir = Path.GetDirectoryName(currAssy);
      var dataDir = Path.Combine(currDir, "Data", subDir);
      var directory = new DirectoryInfo(dataDir);
      var masks = new[] { "*.png", "*.jpg" };
      var files = masks.SelectMany(directory.EnumerateFiles).Select(fi => fi.FullName);
      return files;
    }

    private static IEnumerable<string> SingleDataFiles()
    {
      return GetDataFiles("Single");
    }

    private static IEnumerable<string> MultipleDataFiles()
    {
      return GetDataFiles("Multiple");
    }

    private QRDecoder Create()
    {
      return new(_mocklogger.Object);
    }
  }
}