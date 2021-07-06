using System.Drawing;
using Emgu.CV;
using Emgu.CV.Structure;
using HAL.ImageAnalysis.Features.Sources;
using HAL.ImageAnalysis.Implementation.Features.Sources;

namespace HAL.Documentation.KaplaPlusCamera.Providers.Sources
{
    /// <summary> Wrapper of Emgu.CV <see cref="Image{TColor, TDepth}"/>. </summary>
    public class Image : IImage
    {
        /// <summary> Create a new image.</summary>
        /// <param name="image">The <see cref="Image{TColor, TDepth}"/> host.</param>
        public Image(Image<Bgr, byte> image)
        {
            Host = image;
        }

        /// <summary> Create a new image from a <see cref="Bitmap"/>.</summary>
        /// <param name="bitmap">Bitmap</param>
        public Image(Bitmap bitmap)
        {
            Host = bitmap.ToImage<Bgr, byte>();
        }

        /// <summary>  Host as <see cref="Image{Bgr, byte}"/>   </summary>
        public Image<Bgr, byte> Host { get; set; }

        /// <summary> </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Image(int width, int height)
        {
            Host = new Image<Bgr, byte>(width, height);
        }

        /// <summary> Input array where <see cref="VisionDataArray"/> is a wrapper of <see cref="Mat"/>. </summary>
        /// <returns></returns>
        public IFeatureRawData RawArray => new VisionDataArray(Host?.Mat);

        /// <summary> Return this image as a <see cref="Bitmap"/>. </summary>
        /// <returns></returns>
        public Bitmap AsBitmap => Host?.ToBitmap();

        ///<inheritdoc/>
        public void Dispose() => Host?.Dispose();
    }
}