namespace QRCodeDecoder.Core.Tests
{
  using System;
  using FluentAssertions;
  using Moq;
  using NUnit.Framework;
  using Serilog;

  public class QRDecoder_Tests
    {
        private Mock<ILogger> _mocklogger;
        
        [SetUp]
        public void Setup()
        {
          _mocklogger = new Mock<ILogger>();
        }

        [Test]
        public void Test1()
        {
          Action act = () => Create();

          act.Should().NotThrow();
        }

        private QRDecoder Create()
        {
          return new QRDecoder(_mocklogger.Object);
        }
    }
}