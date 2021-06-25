using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using HAL.Documentation.KaplaPlusCamera.Providers.Sources;
using HAL.ImageAnalysis.Features;
using HAL.ImageAnalysis.Features.Providers;
using HAL.ImageAnalysis.Features.Sources;
using HAL.ImageAnalysis.Implementation.Features.Sources;
using HAL.Spatial;

namespace HAL.Documentation.KaplaPlusCamera.Providers
{
    public class CirclePositionsProvider : FeatureProvider
    {
        public CirclePositionsProvider(IImageSourceBuilder imageSourceBuilder) : base(imageSourceBuilder, false) { }


        private List<Circle> GetFeatures(List<CircleF> circles)
        {
            var featureCircles = new List<Circle>();
            foreach (var circle in circles)
            {
                featureCircles.Add(new Circle("", new Vector3D(circle.Center.X, circle.Center.Y, 0), RotationMatrix.Identity, circle.Radius));
            }
            return featureCircles;
        }

        private static UMat SetImage(VisionDataArray img)
        {
            //Convert the image to grayscale and filter out the noise
            UMat uimage = new UMat();
            CvInvoke.CvtColor(img.Mat, uimage, ColorConversion.Bgr2Gray);

            //use image pyr to remove noise
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);
            return uimage;
        }

        private List<IFeature> DetectCircles(out IFeatureRawData filtered)
        {
            if (!(SourceRawData is VisionDataArray data))
            {
                filtered = SourceRawData;
                return new List<IFeature>();
            }
            var uimage = SetImage(data);
            //use image pyr to remove noise
            UMat pyrDown = new UMat();
            CvInvoke.PyrDown(uimage, pyrDown);
            CvInvoke.PyrUp(pyrDown, uimage);
            double cannyThreshold = 180.0;
            double circleAccumulatorThreshold = 120;
            List<CircleF> circles = CvInvoke.HoughCircles(uimage, HoughType.Gradient, 2.0, 20.0, cannyThreshold, circleAccumulatorThreshold, 5).ToList();

            Image<Bgr, Byte> circleImage = data.Mat.ToImage<Bgr, Byte>();

            foreach (var circle in circles)
            {
                circleImage.Draw(circle, new Bgr(Color.Brown), 2);
            }

            filtered = new VisionDataArray(circleImage.Mat);

            return GetFeatures(circles).ToList<IFeature>();
        }
        public override List<IFeature> GrabFeatures(out IFeatureRawData processed) => DetectCircles(out processed);
    }
}