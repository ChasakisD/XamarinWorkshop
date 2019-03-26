using FaceRecognitionApp.Services;
using Microsoft.ProjectOxford.Face.Contract;
using Plugin.Media.Abstractions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FaceRecognitionApp
{
    public partial class MainPage
    {
        private byte[] _imageBuffer;
        private FaceRectangle _faceRectangle;

        private readonly FaceService _faceService;

        public MainPage()
        {
            InitializeComponent();

            _faceService = new FaceService();
            _faceService.Init("");
        }

        private void OnExecuteButtonClicked(object sender, EventArgs e)
        {
            //TODO: Show Loading

            //TODO: Take Photo From User

            //TODO: Get Face From Service
            
            //TODO: Populate UI
        }

        private Task<Stream> TakePhotoAsync()
        {
            _imageBuffer = null;
            _faceRectangle = null;

            var cameraOptions = new StoreCameraMediaOptions
            {
                CompressionQuality = 92,
                PhotoSize = PhotoSize.Medium,
                DefaultCamera = CameraDevice.Front
            };

            //TODO: Get the image from user

            //TODO: Initialize Buffer

            //TODO: Read Stream to buffer

            return null;
        }

        private void PopulateUI(Face[] faces)
        {
            if (faces == null || faces.Length <= 0) return;

            var face = faces[0];
            var headPose = face.FaceAttributes.HeadPose;

            AgeLabel.Text = $"Age: {face.FaceAttributes.Age}";
            SmileLabel.Text = $"Smile: {face.FaceAttributes.Smile}";
            GenderLabel.Text = $"Gender: {face.FaceAttributes.Gender}";
            GlassesLabel.Text = $"Am I Wearing Glasses? {face.FaceAttributes.Glasses}";
            HeadPoseLabel.Text = $"HeadPose: Yaw: {headPose.Yaw}, Pitch: {headPose.Pitch}, Roll: {headPose.Roll}";

            //TODO: Set Emotions Label

            //TODO: Redraw Face Rectangle
        }

        private void OnFaceRectanglePaint(object sender, SKPaintSurfaceEventArgs e)
        {
            if (_imageBuffer == null || _faceRectangle == null) return;

            var canvas = e.Surface.Canvas;

            /* Clear the Canvas */
            canvas.Clear();

            /* Get the SKBitmap and resize it */
            var skBitmap = SKBitmap.Decode(new MemoryStream(_imageBuffer));
            var resizedBitmap = skBitmap.Resize(e.Info, SKFilterQuality.High);

            var unScaledWidth = skBitmap.Width;
            var unScaledHeight = skBitmap.Height;

            /* Find the scale ratio, in order to scale down the rectangle */
            var ratioX = resizedBitmap.Width / (float)unScaledWidth;
            var ratioY = resizedBitmap.Height / (float)unScaledHeight;

            var skPaint = new SKPaint
            {
                StrokeWidth = 10,
                IsAntialias = true,
                Color = SKColors.Green,
                Style = SKPaintStyle.Stroke
            };

            /* Draw the Bitmap */
            canvas.DrawBitmap(resizedBitmap, 0, 0);

            /* Scale down the rectangle */
            var rectangleX = _faceRectangle.Left * ratioX;
            var rectangleY = _faceRectangle.Top * ratioY;
            var rectangleWidth = _faceRectangle.Width * ratioX;
            var rectangleHeight = _faceRectangle.Height * ratioY;

            /* Draw the face rectangle */
            canvas.DrawRect(rectangleX, rectangleY, rectangleWidth, rectangleHeight, skPaint);
        }
    }
}