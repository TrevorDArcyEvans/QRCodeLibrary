# QR Encoding and Decoding Library
QR Code libraries to allow program to create QR Code image or read image containing one or more QR Codes

Based on code from:
[QR Code Encoder and Decoder C# Class Library](https://www.codeproject.com/Articles/1250071/QR-Code-Encoder-and-Decoder-Csharp-Class-Library-f)

![encoder](docs/encoder.png)
<p/>

![encoder](docs/encoder.png)
<p/>

## Prerequisites
* Visual Studio 2022
* .NET 6
* Windows

## Getting started
* open `QRCodeLibrary.sln`

## Changes from original project
* used [Serilog](https://serilog.net/) for logging
* ported to .NET Core 6
* refactored
* added encoder + decoder unit tests
* used [ImageSharp](https://github.com/SixLabors/ImageSharp) for graphics operations

## Notes
* _QRCodeDecoder.Core_ + _QRCodeEncode.Core_ run on _Linux_
* _QRCodeDecoder.Windows_ only runs on _Windows_
* demo program only runs on _Windows_

## Further work
* ~~use [ImageSharp](https://github.com/SixLabors/ImageSharp) for graphics operations~~
* ~~support _Linux_~~
* fixed decoder unit tests
  * currently 2 failed detections after switching to _ImageSharp_


