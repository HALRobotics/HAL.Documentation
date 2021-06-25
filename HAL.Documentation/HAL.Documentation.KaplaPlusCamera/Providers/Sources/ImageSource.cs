using System;
using Emgu.CV;
using HAL.ImageAnalysis.Features.Sources;
using HAL.ImageAnalysis.Implementation.Features.Sources;

namespace HAL.Documentation.KaplaPlusCamera.Providers.Sources
{ 
    /// <summary> Wrapper of Emgu.CV <see cref=VideoCapture/>. </summary>
    public class ImageSource : VideoCapture, IImageSource, IDisposable
    {
        private Image _image;

        /// <summary>Create a new <see cref="VideoCapture"/> from its camera index. </summary>
        /// <param name="camIndex">Camera index.</param>
        /// <param name="captureApi"><see cref="VideoCapture.API"/></param>
        /// <param name="dpiX">Dpi X</param>
        /// <param name="dpiY">Dpi Y</param>
        public ImageSource(int camIndex = 0, VideoCapture.API captureApi = VideoCapture.API.Any, int dpiX = 96, int dpiY = 96) : base(camIndex, captureApi)
        {
            _image = new Image(Width, Height);
            
            PixelFormat = base.QueryFrame().ToBitmap().PixelFormat;
            DpiX = dpiX; DpiY = dpiY;
            Alias = $"Camera {camIndex} - {BackendName}";
            //ImageGrabbed += ImageSource_ImageGrabbed;
        }


        /// <inheritdoc />
        public double DpiX { get; set; }
        /// <inheritdoc />
        public double DpiY { get; set; }
        /// <inheritdoc />
        public System.Drawing.Imaging.PixelFormat PixelFormat { get; set; }
        /// <inheritdoc />
        public string Alias { get; }

        /// <inheritdoc />
        public bool Retrieve(out IFeatureData image)
        {

            var success = base.Retrieve(_image.Host);
            image = _image;
            return success;
        }
    }

}