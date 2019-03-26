using FaceRecognitionApp.Services;
using Microsoft.ProjectOxford.Face.Contract;
using Plugin.Media;
using Plugin.Media.Abstractions;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace FaceRecognitionApp
{
    [DesignTimeVisible(true)]
    public partial class ReadyMainPage
    {
        private byte[] _imageBuffer;
        private FaceRectangle _faceRectangle;

        private readonly FaceService _faceService;

        public ReadyMainPage()
        {
            InitializeComponent();

            _faceService = new FaceService();
            _faceService.Init("");
        }

        private async void OnExecuteButtonClicked(object sender, EventArgs e)
        {
            LoadingView.IsVisible = true;

            var imageStream = await TakePhotoAsync();

            var faces = await _faceService.DetectFaceAsync(imageStream);

            PopulateUI(faces);

            LoadingView.IsVisible = false;
        }

        private async Task<Stream> TakePhotoAsync()
        {
            _imageBuffer = null;
            _faceRectangle = null;

            var cameraOptions = new StoreCameraMediaOptions
            {
                CompressionQuality = 92,
                PhotoSize = PhotoSize.Medium,
                DefaultCamera = CameraDevice.Front
            };

            var mediaFile = await CrossMedia.Current.TakePhotoAsync(cameraOptions);

            var imageStream = mediaFile?.GetStream();
            if (imageStream == null) return null;
            
            _imageBuffer = new byte[imageStream.Length];

            await imageStream.ReadAsync(_imageBuffer, 0, (int) imageStream.Length);

            return new MemoryStream(_imageBuffer);
        }

        private void PopulateUI(Face[] faces)
        {
            if (faces == null || faces.Length <= 0) return;

            var face = faces[0];
            var emotion = face.FaceAttributes.Emotion;
            var headPose = face.FaceAttributes.HeadPose;

            AgeLabel.Text = $"Age: {face.FaceAttributes.Age}";
            SmileLabel.Text = $"Smile: {face.FaceAttributes.Smile}";
            GenderLabel.Text = $"Gender: {face.FaceAttributes.Gender}";
            GlassesLabel.Text = $"Am I Wearing Glasses? {face.FaceAttributes.Glasses}";
            HeadPoseLabel.Text = $"HeadPose: Yaw: {headPose.Yaw}, Pitch: {headPose.Pitch}, Roll: {headPose.Roll}";

            var emotionsText = string.Empty;
            emotionsText += $"Anger: {emotion.Anger * 100}%\n";
            emotionsText += $"Fear: {emotion.Fear * 100}%\n";
            emotionsText += $"Happiness: {emotion.Happiness * 100}%\n";
            emotionsText += $"Sadness: {emotion.Sadness * 100}%\n";
            emotionsText += $"Surprise: {emotion.Surprise * 100}%";

            EmotionsLabel.Text = emotionsText;

            _faceRectangle = face.FaceRectangle;

            ImageCanvas.InvalidateSurface();
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
