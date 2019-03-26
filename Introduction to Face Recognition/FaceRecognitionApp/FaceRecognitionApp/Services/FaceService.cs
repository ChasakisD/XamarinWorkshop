using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FaceRecognitionApp.Services
{
    public class FaceService
    {
        private string _subscriptionKey = "";
        private FaceServiceClient _faceServiceClient;

        public void Init(string subscriptionKey)
        {
            _subscriptionKey = subscriptionKey;

            _faceServiceClient = new FaceServiceClient(_subscriptionKey, "https://westeurope.api.cognitive.microsoft.com/face/v1.0");
        }

        public async Task<Face[]> DetectFaceAsync(Stream imageStream)
        {
            if (imageStream == null) return null;

            var faceAttributes = new[]
            {
                FaceAttributeType.Age,
                FaceAttributeType.Smile,
                FaceAttributeType.Gender,
                FaceAttributeType.Emotion,
                FaceAttributeType.Glasses,
                FaceAttributeType.HeadPose,
                FaceAttributeType.FacialHair
            };

            try
            {
                var facesResponse = await _faceServiceClient.DetectAsync(
                    imageStream, true, true, faceAttributes);

                return facesResponse;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);

                return null;
            }
        }
    }
}
