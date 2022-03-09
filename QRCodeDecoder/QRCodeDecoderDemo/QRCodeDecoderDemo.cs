﻿/////////////////////////////////////////////////////////////////////
//
//	QR Code Decoder Library
//
//	QR Code Decoder test/demo application.
//
//	Author: Uzi Granot
//
//	Current Version: 3.0.0
//	Date: March 1, 2022
//
//	Original Version: 1.0
//	Date: June 30, 2018
//
//	Copyright (C) 2018-2022 Uzi Granot. All Rights Reserved
//
//	QR Code Library C# class library and the attached test/demo
//  applications are free software.
//	Software developed by this author is licensed under CPOL 1.02.
//	Some portions of the QRCodeVideoDecoder are licensed under GNU Lesser
//	General Public License v3.0.
//
//	The video decoder is using some of the source modules of
//	Camera_Net project published at CodeProject.com:
//	https://www.codeproject.com/Articles/671407/Camera_Net-Library
//	and at GitHub: https://github.com/free5lot/Camera_Net.
//	This project is based on DirectShowLib.
//	http://sourceforge.net/projects/directshownet/
//	This project includes a modified subset of the source modules.
//
//	The main points of CPOL 1.02 subject to the terms of the License are:
//
//	Source Code and Executable Files can be used in commercial applications;
//	Source Code and Executable Files can be redistributed; and
//	Source Code can be modified to create derivative works.
//	No claim of suitability, guarantee, or any warranty whatsoever is
//	provided. The software is provided "as-is".
//	The Article accompanying the Work may not be distributed or republished
//	without the Author's consent
//
//	For version history please refer to QRDecoder.cs
/////////////////////////////////////////////////////////////////////

namespace QRCodeDecoderDemo
{
  using System.Diagnostics;
  using System.Drawing.Imaging;
  using System.Runtime.InteropServices.ComTypes;
  using System.Text;
  using QRCodeDecoder.Core;
  using QRCodeDecoder.Windows;
  using Serilog;
  using Timer = System.Windows.Forms.Timer;

  /// <summary>
  /// Test QR Code Decoder
  /// </summary>
  public partial class QRCodeDecoderDemo : Form
  {
    // QR decoder variables
    private QRDecoder QRCodeDecoder;
    private Bitmap QRCodeInputImage;

    // video camera variables
    private bool VideoCameraExists;
    private FrameSize FrameSize;
    private Camera VideoCamera;
    private IMoniker CameraMoniker;
    private Timer QRCodeTimer;
    private Panel CameraPanel;

    private readonly ILogger _logger;

    /// <summary>
    /// Constructor
    /// </summary>
    public QRCodeDecoderDemo()
    {
      InitializeComponent();

      // open trace file
      _logger = new LoggerConfiguration()
           .WriteTo.Console()
           .WriteTo.File("QRCodeDecoderTrace.txt")
           .CreateLogger();
      _logger.Information("QRCodeDecoder");
    }

    /// <summary>
    /// QR Decode program initialization
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    private void OnLoad(object sender, EventArgs e)
    {
      // program title
      Text = "QRCodeDecoder " + QRDecoder.VersionNumber + " \u00a9 2018-2022 Uzi Granot. All rights reserved.";

      // disable go to url button
      GoToUrlButton.Enabled = false;

      // create decoder
      QRCodeDecoder = new QRDecoder(_logger);

      // test for video camera
      VideoCameraExists = TestForVideoCamera();
      VideoCameraButton.Enabled = VideoCameraExists;

      // resize window
      OnResize(sender, e);
    }

    /// <summary>
    /// Load image from file and scan for QR codes
    /// </summary>
    /// <param name="Sender">Sender</param>
    /// <param name="e">Event arguments</param>
    private void OnLoadImage(object sender, EventArgs e)
    {
      // set header label to file name without path
      HeaderLabel.Text = "Load Image File";

      // the application is in video camera mode
      // viewing panel is hidden
      if (!ViewingPanel.Visible)
      {
        // disable timer
        QRCodeTimer.Enabled = false;

        // pause video camera
        VideoCamera.PauseGraph();

        // hide camera panel
        CameraPanel.Visible = false;

        // restore viewing image panel
        ViewingPanel.Visible = true;

        // enable video camera button
        VideoCameraButton.Enabled = true;
      }

      // get file name to decode
      OpenFileDialog Dialog = new()
      {
        Filter = "Image Files(*.png;*.jpg;*.gif;*.tif)|*.png;*.jpg;*.gif;*.tif;*.bmp)|All files (*.*)|*.*",
        Title = "Load QR Code Image",
        InitialDirectory = Directory.GetCurrentDirectory(),
        RestoreDirectory = true,
        FileName = string.Empty
      };

      // display dialog box
      if (Dialog.ShowDialog() != DialogResult.OK)
      {
        return;
      }

      // set header label to file name without path
      HeaderLabel.Text = Dialog.SafeFileName;

      // clear text boxes
      ClearTextBoxes();

      // disable buttons
      ImageFileButton.Enabled = false;
      VideoCameraButton.Enabled = false;
      GoToUrlButton.Enabled = false;

      // dispose previous image
      if (QRCodeInputImage != null) QRCodeInputImage.Dispose();

      // convert image file to bitmap
      QRCodeInputImage = new(Dialog.FileName);

      // decode image into array of QR codes
      // each QR code matrix is made of one byte per module
      QRCodeResult[] QRCodeResultArray = QRCodeDecoder.ImageDecoder(QRCodeInputImage);

      // trace
      _logger.Information("****");
      _logger.Information($"Decode image: {Dialog.FileName}");
      _logger.Information($"Image width: {QRCodeInputImage.Width}, Height: {QRCodeInputImage.Height}");

      // we have at least one QR Code
      if (QRCodeResultArray != null)
      {
        // display dimension value
        QRCodeDimensionLabel.Text = QRCodeResultArray[0].QRCodeDimension.ToString();

        // display error correction code
        ErrorCodeLabel.Text = QRCodeResultArray[0].ErrorCorrection.ToString();

        // display ECI value
        ECIValueLabel.Text = QRCodeResultArray[0].ECIAssignValue >= 0 ? QRCodeResultArray[0].ECIAssignValue.ToString() : null;

        // convert results to text
        DataTextBox.Text = ConvertResultToDisplayString(QRCodeResultArray);

        if (QRCodeResultArray != null && QRCodeResultArray.Length > 0)
        {
          byte[] Data = QRCodeResultArray[0].DataArray;
          for (int Index = 0; Index < Data.Length; Index++)
          {
            _logger.Information($"{Index}: Dec {Data[Index]}, Hex {Data[Index]:x2}");
          }
        }

        // if the text is a valid url
        if (IsValidUrl(DataTextBox.Text))
        {
          GoToUrlButton.Enabled = true;
        }

        {
          byte[] Data = QRCodeResultArray[0].DataArray;
          for (int Index = 0; Index < Data.Length; Index++)
          {
            _logger.Information($"{Index}: Dec {Data[Index]}, Hex {Data[Index]:x2}");
          }
        }
      }

      // enable buttons
      ImageFileButton.Enabled = true;
      VideoCameraButton.Enabled = VideoCameraExists;

      // force repaint
      ViewingPanel.Invalidate();
    }

    /// <summary>
    /// Activate video camera
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnVideoCamera(object sender, EventArgs e)
    {
      // clear text boxes
      ClearTextBoxes();

      // set header label to file name without path
      HeaderLabel.Text = "Video Camera Mode";

      // change to video camera from load image state
      if (ViewingPanel.Visible)
      {
        // dispose previous image
        if (QRCodeInputImage != null)
        {
          QRCodeInputImage.Dispose();
          QRCodeInputImage = null;
        }

        // disable viewing panel
        ViewingPanel.Visible = false;

        // video camera not defined yet
        if (VideoCamera == null)
        {
          // camera panel
          CameraPanel = new Panel();
          Controls.Add(CameraPanel);
          CameraPanel.Name = "CameraPanel";
          CameraPanel.TabIndex = 20;

          // Set selected camera to camera control with selected frame size
          // Create camera object
          VideoCamera = new Camera(CameraPanel, CameraMoniker, FrameSize);

          // create grab frame timer
          QRCodeTimer = new Timer
          {
            Interval = 200
          };
          QRCodeTimer.Tick += QRCodeTimer_Tick;
          QRCodeTimer.Enabled = true;

          // resize viewing panel and camera panel
          OnResize(sender, e);
        }
        else
        {
          CameraPanel.Visible = true;
          VideoCamera.RunGraph();
          QRCodeTimer.Enabled = true;
        }
      }

      // restart video camera after QR code was detected
      else
      {
        VideoCamera.RunGraph();
        QRCodeTimer.Enabled = true;
      }

      // disable video camera button
      VideoCameraButton.Enabled = false;
      GoToUrlButton.Enabled = false;
    }

    /// <summary>
    /// Video camera timer tick
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    private void QRCodeTimer_Tick(object sender, EventArgs e)
    {
      // disable timer
      QRCodeTimer.Enabled = false;

      // snapshot camera image
      Bitmap QRCodeImage;

      // get one camera frame image
      try
      {
        // get frame image
        QRCodeImage = VideoCamera.SnapshotSourceImage();

        // trace
        _logger.Information($"Image width: {QRCodeImage.Width}, Height: {QRCodeImage.Height}");
      }
      catch (Exception EX)
      {
        DataTextBox.Text = "Decode exception.\r\n" + EX.Message;
        QRCodeTimer.Enabled = true;
        return;
      }

      // decode image
      QRCodeResult[] DataByteArray = QRCodeDecoder.ImageDecoder(QRCodeImage);
      string Text = ConvertResultToDisplayString(DataByteArray);

      // save image for debugging
      QRCodeImage.Save("VideoCaptureImage.png", ImageFormat.Png);

      // dispose bitmap
      QRCodeImage.Dispose();

      // we have no QR code
      if (Text.Length == 0)
      {
        QRCodeTimer.Enabled = true;
        return;
      }

      // pause the camera
      VideoCamera.PauseGraph();

      // display the QR code text
      DataTextBox.Text = Text;

      // if the text is a valid url
      if (IsValidUrl(DataTextBox.Text))
      {
        GoToUrlButton.Enabled = true;
      }

      // enable video camera button
      VideoCameraButton.Enabled = true;
    }

    /// <summary>
    /// Test this computer for video camera
    /// </summary>
    /// <returns>Result</returns>
    private bool TestForVideoCamera()
    {
      // get an array of web camera devices
      DsDevice[] CameraDevices = DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice);

      // make sure at least one is available
      if (CameraDevices == null || CameraDevices.Length == 0)
      {
        return false;
      }

      // select the first camera
      DsDevice CameraDevice = CameraDevices[0];

      // Device moniker
      CameraMoniker = CameraDevice.Moniker;

      // get a list of frame sizes available
      FrameSize[] FrameSizes = Camera.GetFrameSizeList(CameraMoniker);

      // make sure there is at least one frame size
      if (FrameSizes == null || FrameSizes.Length == 0)
      {
        CameraMoniker = null;
        return false;
      }

      // test if our frame size is available
      int Index;
      for (Index = 0;
           Index < FrameSizes.Length &&
           (FrameSizes[Index].Width != 640 || FrameSizes[Index].Height != 480);
           Index++)
      {
        // DO_NOTHING
        ;
      }

      // our default frame size is available
      if (Index < FrameSizes.Length)
      {
        FrameSize = new FrameSize(640, 480);
      }
      // select first frame size
      else
      {
        FrameSize = FrameSizes[0];
      }

      // we have a video camera
      return true;
    }

    /// <summary>
    /// Convert byte array to string using UTF8 encoding
    /// </summary>
    /// <param name="DataArray">Input array</param>
    /// <returns>Output string</returns>
    private static string ByteArrayToStr(byte[] DataArray)
    {
      Decoder Decoder = Encoding.UTF8.GetDecoder();
      int CharCount = Decoder.GetCharCount(DataArray, 0, DataArray.Length);
      char[] CharArray = new char[CharCount];
      Decoder.GetChars(DataArray, 0, DataArray.Length, CharArray, 0);
      return new string(CharArray);
    }

    /// <summary>
    /// Format result for display
    /// </summary>
    /// <param name="DataByteArray">QR Decoded byte arrays</param>
    /// <returns>Display string</returns>
    private static string ConvertResultToDisplayString(QRCodeResult[] DataByteArray)
    {
      // no QR code
      if (DataByteArray == null)
      {
        return string.Empty;
      }

      // image has one QR code
      if (DataByteArray.Length == 1)
      {
        return SingleQRCodeResult(ByteArrayToStr(DataByteArray[0].DataArray));
      }

      // image has more than one QR code
      StringBuilder Str = new();
      for (int Index = 0; Index < DataByteArray.Length; Index++)
      {
        if (Index != 0)
        {
          Str.Append("\r\n");
        }
        Str.AppendFormat("QR Code {0}\r\n", Index + 1);
        Str.Append(SingleQRCodeResult(ByteArrayToStr(DataByteArray[Index].DataArray)));
      }
      return Str.ToString();
    }

    /// <summary>
    /// Single QR Code result
    /// </summary>
    /// <param name="Result">Input string</param>
    /// <returns>Output display string</returns>
    private static string SingleQRCodeResult(string Result)
    {
      int Index;
      for (Index = 0;
           Index < Result.Length && (Result[Index] >= ' ' && Result[Index] <= '~' || Result[Index] >= 160);
           Index++)
      {
        // DO_NOTHING
        ;
      }

      if (Index == Result.Length)
      {
        return Result;
      }

      StringBuilder Display = new(Result[..Index]);
      for (; Index < Result.Length; Index++)
      {
        char OneChar = Result[Index];
        if (OneChar >= ' ' && OneChar <= '~' || OneChar >= 160)
        {
          Display.Append(OneChar);
          continue;
        }

        if (OneChar == '\r')
        {
          Display.Append("\r\n");
          if (Index + 1 < Result.Length && Result[Index + 1] == '\n') Index++;
          continue;
        }

        if (OneChar == '\n')
        {
          Display.Append("\r\n");
          continue;
        }

        Display.Append('¿');
      }
      return Display.ToString();
    }

    /// <summary>
    /// Start web browser with decoded URL
    /// </summary>
    /// <param name="sender">Sender (this class)</param>
    /// <param name="e">Standard event arguments</param>
    private void OnGoToUrl(object sender, EventArgs e)
    {
      // start image editor
      Process Proc = new();
      Proc.StartInfo = new ProcessStartInfo(DataTextBox.Text) { UseShellExecute = true };
      Proc.Start();
    }

    /// <summary>
    /// Test string to be a valid url
    /// </summary>
    /// <param name="Url">Url string</param>
    /// <returns>Result</returns>
    private static bool IsValidUrl(string Url)
    {
      if (Uri.IsWellFormedUriString(Url, UriKind.Absolute) &&
        Uri.TryCreate(Url, UriKind.Absolute, out Uri TempUrl))
      {
        return TempUrl.Scheme == System.Uri.UriSchemeHttp || TempUrl.Scheme == System.Uri.UriSchemeHttps;
      }
      return false;
    }

    /// <summary>
    /// Clear text boxes
    /// </summary>
    private void ClearTextBoxes()
    {
      // clear text boxes
      QRCodeDimensionLabel.Text = string.Empty;
      ErrorCodeLabel.Text = string.Empty;
      ECIValueLabel.Text = string.Empty;
      DataTextBox.Text = string.Empty;
    }

    /// <summary>
    /// paint QR Code image
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Paint event arguments</param>
    private void OnViewingPanelPaint(object sender, PaintEventArgs e)
    {
      // no image
      if (QRCodeInputImage == null)
      {
        return;
      }

      // calculate image height to preserve aspect ratio
      int ImageHeight = (ViewingPanel.Width * QRCodeInputImage.Height) / QRCodeInputImage.Width;
      int ImageWidth;

      if (ImageHeight <= ViewingPanel.Height)
      {
        ImageWidth = ViewingPanel.Width;
      }
      else
      {
        ImageWidth = (ViewingPanel.Height * QRCodeInputImage.Width) / QRCodeInputImage.Height;
        ImageHeight = ViewingPanel.Height;
      }

      // calculate position
      int ImageX = (ViewingPanel.Width - ImageWidth) / 2;
      int ImageY = (ViewingPanel.Height - ImageHeight) / 2;

      // draw image
      e.Graphics.DrawImage(QRCodeInputImage, new Rectangle(ImageX, ImageY, ImageWidth, ImageHeight));
    }

    /// <summary>
    /// On user resize
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">Event arguments</param>
    private void OnResize(object sender, EventArgs e)
    {
      // minimize
      if (ClientSize.Width == 0)
      {
        return;
      }

      // center header label
      HeaderLabel.Left = (ClientSize.Width - HeaderLabel.Width) / 2;

      // data text box
      DataTextBox.Top = ClientSize.Height - DataTextBox.Height - 8;
      DataTextBox.Width = ClientSize.Width - 2 * DataTextBox.Left;

      // decoded data label
      DecodedDataLabel.Top = DataTextBox.Top - DecodedDataLabel.Height - 8;

      // image area
      ViewingPanel.Width = ClientSize.Width - ViewingPanel.Left - 4;
      ViewingPanel.Height = DecodedDataLabel.Top - ViewingPanel.Top - 4;

      if (CameraPanel != null)
      {
        CameraPanel.Location = new Point(ViewingPanel.Left, ViewingPanel.Top);
        CameraPanel.Size = new Size(ViewingPanel.Width, ViewingPanel.Height);
      }

      // if there is an image force repaint
      if (QRCodeInputImage != null)
      {
        ViewingPanel.Invalidate();
      }
    }

    /// <summary>
    /// Application is closing
    /// </summary>
    /// <param name="sender">Sender</param>
    /// <param name="e">ClosingEventArguments</param>
    private void OnClosing(object sender, FormClosingEventArgs e)
    {
      QRCodeInputImage?.Dispose();
      VideoCamera?.Dispose();
      Log.CloseAndFlush();
    }
  }
}
