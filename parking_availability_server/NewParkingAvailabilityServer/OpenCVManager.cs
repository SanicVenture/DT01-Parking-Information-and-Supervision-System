using Microsoft.ML.OnnxRuntime;
using NewParkingAvailabilityServer.Models;
using OpenCvSharp;
using System.Drawing;
using System.Security.Cryptography.Xml;
using static System.Net.Mime.MediaTypeNames;
using System.Data;
using Humanizer;
using SkiaSharp;
using YoloDotNet;
using YoloDotNet.Models;
using YoloDotNet.Extensions;
using YoloDotNet.ExecutionProvider.Cpu;
using Compunet.YoloSharp;
using Compunet.YoloSharp.Plotting;
using SixLabors.ImageSharp;
using Compunet.YoloSharp.Data;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.Fonts;
using Clipper2Lib;

namespace NewParkingAvailabilityServer
{
    public class OpenCVManager
    {
        private SQLManager sqlManager = new SQLManager();
        private string streamURL = "rtsp://admin:Password@10.18.31.38:554";
        private int msTimeout = 120000;
        private int[] spotIds = [1]; //example parking spot IDs
        private bool showDetections = true;

        //This string array of acceptable vehicles is just an example of what the
        //OpenCV implementation will look for.
        private string[] acceptableVehicles = ["car", "motorcycle", "van", "truck", "bus"];

        private Mat DrawLines(Mat image, LineSegmentPoint[] lines, Scalar color, int thickness = 3)
        {
            //image = new Mat(image);
            Mat line_image = new Mat(image.Size(), image.Type());
            foreach (LineSegmentPoint line in lines)
            {
                Cv2.Line(line_image, line.P1.X, line.P1.Y, line.P2.X, line.P2.Y, color, thickness);
            }
            Cv2.AddWeighted(image, 0.8, line_image, 1.0, 0.0, image);

            return image;
        }

        public async void StartImageRecognition()
        {
            //Console.WriteLine("Enter Number of Parking Spots:");

            //string input = Console.ReadLine();

            //move parkingspotamount to here!

            //Maybe do some serious rewrites to properly support multiple parking spots. 
            foreach (int Id in spotIds)
            {
                Task.Run(() => ImageRecognition(Id)); //we are going to support multiple parking spots with tasks.
            }

            //make sure to add the proper mutexing so that we are not trying to change the sql table at the same time.
            //this maybe goes for everywhere tbh
        }

        //private void MouseCallback(MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userData)
        //{
                
        //}

        private bool[] RectangleChecker(Detection detection, OpenCvSharp.Point[][]? polygon)
        {
            int[] ints = new int[polygon.Length];
            int furthestLeftPoint = polygon[0][0].X;
            foreach (OpenCvSharp.Point point in polygon[0])
            {
                if (point.X < furthestLeftPoint)
                {
                    furthestLeftPoint = point.X;
                }
            }

            int topLeftX = detection.Bounds.X;
            int topLeftY = detection.Bounds.Y;
            int bottomRightX = detection.Bounds.X + detection.Bounds.Width;
            int bottomRightY = detection.Bounds.Y + detection.Bounds.Height;

            //first we check if the detection rectangle could intersect or be within with the parking spot polygon. If it not, then we can return false immediately.
            if (!((topLeftX < furthestLeftPoint) && (bottomRightX < furthestLeftPoint)))
            {
                //converting the detection rectangle to Clipper Points and Paths.
                Path64 detectionPath = new Path64();
                detectionPath.Add(new Point64(topLeftX, topLeftY));
                detectionPath.Add(new Point64(bottomRightX, topLeftY));
                detectionPath.Add(new Point64(bottomRightX, bottomRightY));
                detectionPath.Add(new Point64(topLeftX, bottomRightY));

                Path64 polygonPath = new Path64();
                foreach (OpenCvSharp.Point point in polygon[0])
                {
                    polygonPath.Add(new Point64(point.X, point.Y));
                }

                double polygonArea = Clipper.Area(polygonPath);

                Paths64 detectionPaths = new Paths64 { detectionPath };
                Paths64 polygonPaths = new Paths64 { polygonPath };
                Paths64 solution = new Paths64();

                //Finding the Intersection between the detection rectangle and the parking space
                solution = Clipper.Intersect(detectionPaths, polygonPaths, FillRule.NonZero);

                double overlapArea = 0;
                foreach (Path64 path in solution)
                {
                    overlapArea += Clipper.Area(path);
                }

                int objectArea = detection.Bounds.Width * detection.Bounds.Height;

                //the area of overlap has to be at least 50% of the area of the object that is doing the overlapping.

                double decimalOverlap = (overlapArea / objectArea);

                //first boolean weeds out overlaps that are basically negligible, the second boolean tells proper overlaps.
                return [decimalOverlap >= 0.1, decimalOverlap >= 0.5];
            }

            return [false, false]; //placeholder return value
        }

        private async void ImageRecognition(int Id)
        {
            string outputPath = $"output_frame_{Id}.bmp";
            while (true)
            {
                using (var capture = new VideoCapture(streamURL))
                {
                    if (!capture.IsOpened())
                    {
                        Console.WriteLine("ERROR: could not open camera stream.");
                        return;
                    } 

                    using (var frame = new Mat())
                    //using (Mat frame = Cv2.ImRead("input_frame.jpg"))
                    //using (Mat frame = Cv2.ImRead("input_frame_with_cones.png"))
                    {

                        capture.Read(frame);

                        if (frame.Empty())
                        {
                            Thread.Sleep(msTimeout);
                            continue; //skip the rest of the loop and come back to checking the capture.
                        }
                        else
                        {
                            //image saving test
                            // 3. Save the Mat object to a BMP file using Cv2.ImWrite
                            Cv2.ImWrite(outputPath, frame);

                            Console.WriteLine($"Frame successfully saved to {outputPath}");
                        }


                        using (var frameClone = frame.Clone())
                        {
                            Cv2.PyrDown(frameClone, frameClone);


                            //Cv2.CvtColor(frame, dest, ColorConversionCodes.BGR2GRAY);

                            //Cv2.ImShow("Grayscale Image", dest);
                            //Cv2.WaitKey(0);

                            //START OF LOOP

                            OpenCvSharp.Point[][]? polygon = sqlManager.OpenPolygonEntry(Id);


                            //if no points saved...
                            if (polygon == null)
                            {

                                //figure out how to force only 4 points.
                                List<OpenCvSharp.Point> points = new List<OpenCvSharp.Point>();

                                Cv2.NamedWindow("Select ROI");
                                Cv2.ResizeWindow("Select ROI", frameClone.Size());
                                Cv2.SetMouseCallback("Select ROI", (MouseEventTypes @event, int x, int y, MouseEventFlags flags, IntPtr userData) =>
                                {
                                    if (@event == MouseEventTypes.LButtonDown)
                                    {
                                        points.Add(new OpenCvSharp.Point(x, y));
                                        Cv2.Circle(frameClone, new OpenCvSharp.Point(x, y), 3, Scalar.Red, -1);
                                        Cv2.ImShow("Select ROI", frameClone);
                                    }
                                });

                                Cv2.ImShow("Select ROI", frameClone);
                                Cv2.WaitKey(0);

                                Mat mask = new Mat(frameClone.Size(), MatType.CV_8UC1, Scalar.All(0));

                                if (points.Count > 2)
                                {
                                    polygon = [points.ToArray()];

                                    Cv2.FillPoly(mask, polygon, Scalar.All(255)); //fill polygon with white

                                    for (int i = 0; i < polygon[0].Length; i++)
                                    {
                                        polygon[0][i].X *= 2;
                                        polygon[0][i].Y *= 2;
                                    }

                                    await sqlManager.createNewPolygonEntry(polygon, Id);
                                }

                                Mat dst = new Mat();
                                Cv2.BitwiseAnd(frameClone, frameClone, dst, mask);

                                Cv2.ImShow("ROI", dst);
                                Cv2.WaitKey(0);
                            }


                            //Cv2.ImWrite("masked_output.png", dst); //should we be using masked output for detection?
                            using var predictor = new YoloPredictor("yolov8s.onnx");

                            YoloResult<Detection> result = await predictor.DetectAsync(outputPath);

                            Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(outputPath);

                            OpenCVResultsItem openCVresult = new OpenCVResultsItem(Id, false, false);

                            foreach (Detection detection in result)
                            {
                                var rectangle = new SixLabors.ImageSharp.RectangleF(
                                    detection.Bounds.X,
                                    detection.Bounds.Y,
                                    detection.Bounds.Width,
                                    detection.Bounds.Height);
                                image.Mutate(ctx =>
                                {
                                    ctx.Draw(Rgba32.ParseHex("FF0000"), 2, rectangle);
                                    ctx.DrawText(
                                        detection.Name.Name,
                                        SystemFonts.CreateFont("Arial", 32),
                                        Rgba32.ParseHex("FF0000"),
                                        new SixLabors.ImageSharp.PointF(detection.Bounds.X, detection.Bounds.Y + 20));
                                });

                                bool[] objectInParkingSpace = RectangleChecker(detection, polygon);
                                //first boolean is is object in spot, second boolean is if the boolean is properly in the spot. 

                                if (objectInParkingSpace[0] && !objectInParkingSpace[1])
                                {
                                    openCVresult = new OpenCVResultsItem(Id, false, true);
                                }
                                else if (objectInParkingSpace[0] && objectInParkingSpace[1])
                                {
                                    if (acceptableVehicles.Contains(detection.Name.Name))
                                    {
                                        openCVresult = new OpenCVResultsItem(Id, true, true);
                                        break;
                                    }
                                    else
                                    {
                                        openCVresult = new OpenCVResultsItem(Id, false, true);
                                    }
                                }

                                //if those don't pass, here will be where the more classical detection of objects could be thrown in. 

                            }

                            await sqlManager.createnewOpenCVResultsEntry(openCVresult);
                            await sqlManager.CheckForMicrocontrollerData(Id);


                            //EVERYTHING BELOW SHOULD BE TOGGLEABLE SO THAT THE FLOW OF CODE IS NOT INTERRUPTED!

                            if (showDetections == true)
                            {
                                var pen = Pens.Solid(SixLabors.ImageSharp.Color.Blue, 3f);


                                //convert opencv points to sixlabors points
                                SixLabors.ImageSharp.PointF[] polygonSixLabors = new SixLabors.ImageSharp.PointF[]
                                {
                                    new SixLabors.ImageSharp.PointF(polygon[0][0].X, polygon[0][0].Y),
                                    new SixLabors.ImageSharp.PointF(polygon[0][1].X, polygon[0][1].Y),
                                    new SixLabors.ImageSharp.PointF(polygon[0][2].X, polygon[0][2].Y),
                                    new SixLabors.ImageSharp.PointF(polygon[0][3].X, polygon[0][3].Y)
                                };

                                image.Mutate(ctx =>
                                {
                                    ctx.DrawPolygon(pen, polygonSixLabors);
                                });


                                image.SaveAsPng($"output_frame_with_detections_{Id}.png");

                                Mat final = Cv2.ImRead($"output_frame_with_detections_{Id}.png");
                                Cv2.PyrDown(final, final);

                                Cv2.ImShow("YOLO Detections", final);
                                Cv2.WaitKey(0);
                            }


                            //Mat greydst = new Mat();

                            //Cv2.CvtColor(dst.Clone(), greydst, ColorConversionCodes.BGR2GRAY);

                            //Cv2.ImShow("Grayscale Image", greydst);
                            //Cv2.WaitKey(0);

                            //int pixels = Cv2.CountNonZero(greydst);

                            //using (var cannyed_image = new Mat())
                            //{
                            //    Cv2.Canny(dst, cannyed_image, 100, 200);

                            //    Cv2.ImShow("Cannyed Image", cannyed_image);
                            //    Cv2.WaitKey(0);

                            //    int cannypixels = Cv2.CountNonZero(cannyed_image);
                            //}

                                //END OF LOOP



                                #region collapsed

                                //Cv2.GaussianBlur(dest, dest, new OpenCvSharp.Size(5, 5), 0);

                                //Cv2.ImShow("Gaussian Blur", dest);
                                //Cv2.WaitKey(0);

                                //Cv2.Dilate(dest, dest, Mat.Ones(3, 3, MatType.CV_8UC1));

                                //Cv2.ImShow("Dilation", dest);
                                //Cv2.WaitKey(0);

                                //Mat kernel = Cv2.GetStructuringElement(MorphShapes.Ellipse, new OpenCvSharp.Size(2, 2));
                                //Cv2.MorphologyEx(dest, dest, MorphTypes.Close, kernel);

                                //Cv2.ImShow("Morphologically Closed Image", dest);
                                //Cv2.WaitKey(0);


                                //using (var cannyed_image = new Mat())
                                //{
                                //    Cv2.Canny(dest, cannyed_image, 100, 200);

                                //    Cv2.ImShow("Cannyed Image", cannyed_image);
                                //    Cv2.WaitKey(0);

                                //    string roiselect = "Select ROI";



                                //    Rect roi = Cv2.SelectROI(roiselect, cannyed_image);

                                //    Cv2.DestroyWindow(roiselect);

                                //    using (var cropped_image = new Mat(cannyed_image, roi))
                                //    {
                                //        Cv2.ImShow("Cropped Image", cropped_image);
                                //        Cv2.WaitKey(0);

                                //        LineSegmentPoint[] lines = Cv2.HoughLinesP(cropped_image, 6, Math.PI / 60, 80, 40, 25);

                                //        //using (var cropped_original = (new Mat(frame, roi)).Clone())
                                //        //{ 


                                //        //}




                                //        ////image = new Mat(image);
                                //        //Mat line_image = new Mat(cropped_original.Size(), cropped_original.Type());
                                //        //foreach (LineSegmentPoint line in lines)
                                //        //{
                                //        //    Cv2.Line(line_image, line.P1.X, line.P1.Y, line.P2.X, line.P2.Y, Scalar.Red, 3);
                                //        //}
                                //        //Cv2.AddWeighted(image, 0.8, line_image, 1.0, 0.0, image);

                                //        //return image;

                                //        using (var lined_image = DrawLines((new Mat(frame, roi)).Clone(), lines, Scalar.Red))
                                //        {
                                //            Cv2.ImShow("Lined Image", lined_image);
                                //            Cv2.WaitKey(0);
                                //        }

                                //        lines = lines.OrderBy(x => x.P1.X).ToArray();

                                //        //foreach (var line in lines)
                                //        //{
                                //        //    using (var line_image = DrawLines((new Mat(frame, roi)).Clone(), [line], Scalar.Red))
                                //        //    {
                                //        //        Cv2.ImShow("Lined Image", line_image);
                                //        //        Cv2.WaitKey(0);
                                //        //    }
                                //        //}



                                //    }



                                //}

                                #endregion

                            }

                        //Thread.Sleep(msTimeout);
                    }
                }
            }
        }
    }
}
