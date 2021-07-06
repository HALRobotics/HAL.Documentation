using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.Aruco;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using HAL.Documentation.KaplaPlusCamera.Providers.Sources;
using HAL.ImageAnalysis.Features;
using HAL.ImageAnalysis.Features.Providers;
using HAL.ImageAnalysis.Features.Sources;
using HAL.ImageAnalysis.Implementation.Features;
using HAL.ImageAnalysis.Implementation.Features.Providers;
using HAL.ImageAnalysis.Implementation.Features.Sources;
using HAL.Spatial;
using HAL.Units.Angle;

namespace HAL.Documentation.KaplaPlusCamera.Providers
{
    /// <summary>   Multi marker tracking provider implementation.  </summary
    public class MultiMarkerProvider : FeatureProvider, IImageFeatureProvider
    {
        public MultiMarkerProvider(IImageSourceBuilder imageSourceBuilder, List<Marker> markers = null, Dictionary arucoDictionnary = null, double scale = 0.01, MatrixFrame referenceFrame = null) : base(imageSourceBuilder, true, true, "MultiMarkerProvider")
        {
            Scale = scale;
            ReferenceFrame = referenceFrame ?? MatrixFrame.Identity;
            RegisteredArucoMarkers = arucoDictionnary ?? new Dictionary(Dictionary.PredefinedDictionaryName.Dict4X4_100);
            Parameters = DetectorParameters.GetDefault(); //default detector parameters
            RegistereMarkers = new Dictionary<int, Marker>();
            if (markers is object) { AddMarkers(markers); }
        }


        #region Properties
        private bool IsFiltered { get; set; }
        private double Scale { get; set; }
        public MatrixFrame ReferenceFrame { get; private set; }
        private Matrix<double> DsC { get; set; }
        private Matrix<double> Calib { get; set; }
        private Dictionary<int, Marker> RegistereMarkers { get; set; }
        private DetectorParameters Parameters { get; }
        private Dictionary RegisteredArucoMarkers { get; }
        private Mat RotationVectors { get; set; }
        private Mat TranslationVectors { get; set; }
        private bool IsCameraCalibrated { get; set; } 
        #endregion


        private void AddMarkers(List<Marker> markers)
        {
            RegistereMarkers = new Dictionary<int, Marker>();
            foreach (var marker in markers)
            {
                if (!RegistereMarkers.TryGetValue(marker.MarkerProperty.Identity, out _)) RegistereMarkers.Add(marker.MarkerProperty.Identity, marker);
            }
        }

        /// <summary> Initialize. </summary>
        public override Task Initialize()
        {
            IsCameraCalibrated = CalibrateCamera();
            // add filters if any properties are provided.
            return base.Initialize();
        }


        // todo [IMPROVEMENT] : Add auto camera calibration emgu functions.
        private bool CalibrateCamera()
        {
            Calib = new Matrix<double>(3, 3);
            DsC = new Matrix<double>(1, 4);
            RotationVectors = new Mat();
            TranslationVectors = new Mat();

            //make the distortion matrix
            DsC[0, 0] = -0.175286;
            DsC[0, 1] = 0.0287032;
            DsC[0, 2] = 0;
            DsC[0, 3] = 0;

            // Default camera matrix
            //Calib[0, 0] = 1398.38;
            //Calib[0, 1] = 0;
            //Calib[0, 2] = 1082.4;
            //Calib[1, 0] = 0;
            //Calib[1, 1] = 1398.38;
            //Calib[1, 2] = 627.885;
            //Calib[2, 0] = 0;
            //Calib[2, 1] = 0;
            //Calib[2, 2] = 1;

            // Camera calibration data obtained with a Logitech C505e camera
            //Calib[0, 0] = 784.54613882409024;
            //Calib[0, 1] = 0;
            //Calib[0, 2] = 319.60375964132993;
            //Calib[1, 0] = 0;
            //Calib[1, 1] = 783.71393991949878;
            //Calib[1, 2] = 239.69825105041411;
            //Calib[2, 0] = 0;
            //Calib[2, 1] = 0;
            //Calib[2, 2] = 1;

            // Camera calibration data obtained with a DELL XPS 15 laptop webcam.
           
            Calib[0, 0] = 568.13032402531428;
            Calib[0, 1] = 0;
            Calib[0, 2] = 320.2162834514254;
            Calib[1, 0] = 0;
            Calib[1, 1] = 561.75888138687321;
            Calib[1, 2] = 238.62106862613078;
            Calib[2, 0] = 0;
            Calib[2, 1] = 0;
            Calib[2, 2] = 1;

            return true;
        }

        ///<inheritdoc/>
        public override List<IFeature> GrabFeatures(out IFeatureRawData processed) => DetectMarkers(out processed);


        private List<IFeature> DetectMarkers(out IFeatureRawData filtered)
        {

            if (!(SourceRawData is VisionDataArray data))
            {
                filtered = SourceRawData;
                return new List<IFeature>();
            }
            Mat imgcv = data.Mat;
            //Make all the variables for detecting Markers
            VectorOfVectorOfPointF corners = new VectorOfVectorOfPointF();
            VectorOfVectorOfPointF rejectedimgpoints = new VectorOfVectorOfPointF();
            VectorOfInt ids = new VectorOfInt();


            // todo [IMPROVEMENT] put this as calibration settings.
            float markersize = Convert.ToSingle(100f);     //size of the aruco markers
            MCvScalar color = new MCvScalar(255, 0, 0);     //color to draw the markers in

            //detect markers
            ArucoInvoke.DetectMarkers(imgcv, RegisteredArucoMarkers, corners, ids, Parameters, rejectedimgpoints);
            int size = ids.Size;
            //check if anything was detected - if they were, do stuff
            if (size != 0)
            {
                //draw the markers in the image
                ArucoInvoke.DrawDetectedMarkers(imgcv, corners, ids, color);
                ArucoInvoke.EstimatePoseSingleMarkers(corners, markersize, Calib, DsC, RotationVectors, TranslationVectors);
                for (int i = 0; i < size; i++)
                {
                    using Mat rvecMat = RotationVectors.Row(i);
                    using Mat tvecMat = TranslationVectors.Row(i);
                    using VectorOfDouble rvec = new VectorOfDouble();
                    using VectorOfDouble tvec = new VectorOfDouble();
                    DrawMarkerAxis(imgcv, rvecMat, tvecMat, rvec, tvec, markersize);
                    TryAddNewAddMarker(ids, i, out Marker marker);

                    // transferring data from openCV matrix type (mat) into arrays to rebuild position and rotation data.
                        var rvecMatArray = new double[3];
                        rvecMat.CopyTo(rvecMatArray);
                        var tvecMatArray = new double[3];
                        tvecMat.CopyTo(tvecMatArray);

                    var position = new Vector3D(tvecMatArray[0], tvecMatArray[1], tvecMatArray[2]);
                        var rotation =
                            new Vector3D(rvecMatArray[0], rvecMatArray[1],
                                rvecMatArray[2]); // axis-angle rotation vector.

                    //GetLocalizationFromCorners(corners, i, out Vector3D position, out RotationMatrix rotation);
                    
                    // Finally, set properties of the current marker object.

                        marker.Position = position/1000; // Conversion in (m)
                        
                        marker.Rotation = new RotationMatrix(rotation, new Angle(rotation.Magnitude));

                        // Transform the absolute marker position into the ReferenceFrame and erase marker position and rotation data.
                        var markerRelativeToReferenceFrame = new MatrixFrame(marker.Position, marker.Rotation);
                        // Orient marker frame with respect to reference frame (camera frame).
                        markerRelativeToReferenceFrame = markerRelativeToReferenceFrame.Transform(ReferenceFrame);
                        marker.Position = markerRelativeToReferenceFrame.Position;
                        marker.Rotation = markerRelativeToReferenceFrame.Rotation;

                }
            }
            filtered = new VisionDataArray(imgcv);
            return RegistereMarkers.Values.ToList<IFeature>();
        }

        private bool TryAddNewAddMarker(VectorOfInt markerIndexes, int index, out Marker marker)
        {
            var isNewMarker = false;
            int markerIndex = markerIndexes[index];
            var featureExist = RegistereMarkers.TryGetValue(markerIndex, out marker);
            if (featureExist && marker is null)
            {
                isNewMarker = true;
                marker = new Marker(markerIndex);
            }
            if (!IsFiltered && !featureExist)
            {
                isNewMarker = true;
                marker = new Marker(markerIndex);
                RegistereMarkers.Add(markerIndex, marker);
            }
            return isNewMarker;
        }

        private void DrawMarkerAxis(Mat imgcv, Mat rvecMat, Mat tvecMat, VectorOfDouble rvec, VectorOfDouble tvec, float markersize)
        {
            double[] values = new double[3];
            rvecMat.CopyTo(values);
            rvec.Push(values);
            tvecMat.CopyTo(values);
            tvec.Push(values);
            ArucoInvoke.DrawAxis(imgcv, Calib, DsC, rvecMat, tvecMat, markersize);
        }

        private void GetLocalizationFromCorners(VectorOfVectorOfPointF corners, int index, out Vector3D position, out RotationMatrix rotation)
        {
            var cornerA = corners[index][0];
            var cornerB = corners[index][1];
            var cornerC = corners[index][2];
            var ptOrigin = new Vector3D(cornerA.Y * Scale, cornerA.X * Scale, 0); ;
            var xAxis = new Vector3D(cornerB.X, cornerB.Y, 0) - ptOrigin;
            var yAxis = new Vector3D(cornerC.X, cornerC.Y, 0) - ptOrigin;

            position = ptOrigin + new Vector3D(cornerA.Y * Scale, cornerA.X * Scale, 0);
            rotation = new RotationMatrix(-Vector3D.XAxis, Vector3D.YAxis, -Vector3D.ZAxis);
        }
    }
}