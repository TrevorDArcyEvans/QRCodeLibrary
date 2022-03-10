namespace QRCodeEncoderDemo;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

/// <summary>
/// stolen from:
///   https://codeblog.vurdalakov.net/2019/06/imagesharp-convert-image-to-system-drawing-bitmap-and-back.html
/// </summary>
public static class ImageSharpExtensions
{
  public static Bitmap ToBitmap<TPixel>(this Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
  {
    using var memoryStream = new MemoryStream();
    var imageEncoder = image.GetConfiguration().ImageFormatsManager.FindEncoder(PngFormat.Instance);
    image.Save(memoryStream, imageEncoder);

    memoryStream.Seek(0, SeekOrigin.Begin);

    return new Bitmap(memoryStream);
  }

  public static Image<TPixel> ToImageSharpImage<TPixel>(this Bitmap bitmap) where TPixel : unmanaged, IPixel<TPixel>
  {
    using var memoryStream = new MemoryStream();
    bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

    memoryStream.Seek(0, SeekOrigin.Begin);

    return Image.Load<TPixel>(memoryStream);
  }
}
