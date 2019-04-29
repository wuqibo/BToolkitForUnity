using UnityEngine;
using System;
using System.Collections.Generic;
using Vuforia;
using ZXing;

namespace BToolkit
{
    public class VuforiaQRCodeScanner
    {

        private static VuforiaQRCodeScanner instance;
        public static VuforiaQRCodeScanner Instance { get { return instance ?? (instance = new VuforiaQRCodeScanner()); } }
        private Action<string> OnScanedCallback;
        private const Image.PIXEL_FORMAT kReadImageFormat = Image.PIXEL_FORMAT.GRAYSCALE;
        private const RGBLuminanceSource.BitmapFormat kDecodeFormat = RGBLuminanceSource.BitmapFormat.Gray8;
        private BarcodeReader _barcodeReader = null;
        private bool _registeredFormat = false;
        private bool _decoding = false;
        private bool _appPaused = false;
        private bool _scanning = false;

        public void StartScanning(Action<string> OnScanedCallback)
        {
            if (!_scanning)
            {
                _scanning = true;
                this.OnScanedCallback = OnScanedCallback;
                if (VuforiaARController.Instance != null)
                {
                    VuforiaARController.Instance.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
                    VuforiaARController.Instance.RegisterOnPauseCallback(OnVuforiaPaused);
                }
                CreateBarcodeReader();
                // Init loom in main thread.
                ThreadHelper.Init();
                _registeredFormat = false;
            }
        }

        public void StopScanning()
        {
            if (_scanning)
            {
                this.OnScanedCallback = null;
                if (VuforiaARController.Instance != null)
                {
                    VuforiaARController.Instance.UnregisterTrackablesUpdatedCallback(OnTrackablesUpdated);
                    VuforiaARController.Instance.UnregisterOnPauseCallback(OnVuforiaPaused);
                }
                if (_registeredFormat)
                {
                    CameraDevice.Instance.SetFrameFormat(kReadImageFormat, false);
                }
                _barcodeReader = null;
                _scanning = false;
            }
        }

        private void CreateBarcodeReader()
        {
            if (_barcodeReader == null)
            {
                _barcodeReader = new BarcodeReader();
                List<BarcodeFormat> formatList = new List<BarcodeFormat>();
                formatList.Add(BarcodeFormat.QR_CODE);
                _barcodeReader.Options.PossibleFormats = formatList;
                _barcodeReader.AutoRotate = false;
                _barcodeReader.Options.TryHarder = false;
            }
        }

        private void OnVuforiaPaused(bool paused)
        {
            _appPaused = paused;
            if (paused)
            {
                // Delete the reader.
                _barcodeReader = null;
                if (_registeredFormat)
                {
                    CameraDevice.Instance.SetFrameFormat(kReadImageFormat, false);
                }
            }
            else
            {
                // Should register again.
                CreateBarcodeReader();
                _registeredFormat = false;
            }
        }

        private void OnTrackablesUpdated()
        {
            if (_appPaused)
                return;
            if (!_registeredFormat)
            {
                _registeredFormat = CameraDevice.Instance.SetFrameFormat(kReadImageFormat, true);
            }
            if (_registeredFormat && !_decoding)
            {
                var image = CameraDevice.Instance.GetCameraImage(kReadImageFormat);
                if (image != null)
                {
                    _decoding = true;
                    ThreadHelper.RunAsync(() =>
                    {
                        try
                        {
                            Result data = _barcodeReader.Decode(image.Pixels, image.BufferWidth, image.BufferHeight, kDecodeFormat);
                            if (data != null)
                            {
                                ThreadHelper.QueueOnMainThread(() =>
                                {
                                    if (data.BarcodeFormat == BarcodeFormat.QR_CODE)
                                    {
                                        if (OnScanedCallback != null)
                                        {
                                            OnScanedCallback(data.Text);
                                        }
                                    }
                                });
                            }
                        }
                        finally
                        {
                            _decoding = false;
                        }
                    });
                }
            }
        }
    }
}