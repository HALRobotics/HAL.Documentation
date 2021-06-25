using HAL.ImageAnalysis.Features.Sources;
using HAL.ImageAnalysis.Implementation.Features.Sources;

namespace HAL.Documentation.KaplaPlusCamera.Providers.Sources
{
    /// <summary>  </summary>
    public class ImageSourceBuilder : IImageSourceBuilder
    {
        /// <summary>Create a new image source builder with a specified source index.</summary>
        /// <param name="sourceIndex">Source index.</param>
        public ImageSourceBuilder(int sourceIndex = 0) { SourceIndex = sourceIndex; }

        ///<inheritdoc />
        public int SourceIndex { get; }

        ///<inheritdoc />
        public IFeatureDataSource Build() => new ImageSource(SourceIndex);

    }

}
