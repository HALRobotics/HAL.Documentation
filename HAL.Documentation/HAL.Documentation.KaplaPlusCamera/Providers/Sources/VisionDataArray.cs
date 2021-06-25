using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using HAL.ImageAnalysis.Implementation.Features.Sources;

namespace HAL.Documentation.KaplaPlusCamera.Providers.Sources
{
    public class VisionDataArray : IVisionDataArray
    {
        public VisionDataArray(Mat mat)
        {
            Mat = mat;
        }
        public Mat Mat { get; private set; }
        
        ///<inheritdoc />
        public IImage AsImage()
        {
            var image = Mat.ToImage<Bgr, byte>();
            return new Image(image);
        }

        ///<inheritdoc />
        public Bitmap AsBitmap() => Mat.ToBitmap();

        ///<inheritdoc />
        public void Dispose() => Mat?.Dispose();
    }
}
