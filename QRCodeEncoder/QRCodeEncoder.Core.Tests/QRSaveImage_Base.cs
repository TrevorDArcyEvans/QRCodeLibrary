namespace QRCodeEncoder.Core.Tests;

public abstract class QRSaveImage_Base
{
  private const string Text1 = "0123456789";
  private const string Text2 = "some arbitrary text SOME OTHER ARBITRARY TEXT 0123456789 $ % * + - . / : []{}()&^£!?@|";

  protected bool[,] GetMatrix()
  {
    var qrenc = CreateEncoder();
    var data = new[] { Text1, Text2 };
    var matrix = qrenc.Encode(data);
    return matrix;
  }

  private QREncoder CreateEncoder()
  {
    return new();
  }
}
