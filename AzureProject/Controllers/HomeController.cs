using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;

namespace AzureProject.Controllers
{
        public class HomeController : Controller
        {
            private SpeechRecognizer _speechRecognizer;

            public IActionResult Index()
            {
                return View();
            }

            [HttpPost]
            public async Task<IActionResult> StartTranslation()
            {
                _speechRecognizer = new SpeechRecognizer(SpeechConfig.FromSubscription("YourSubscriptionKey", "YourRegion"));

                _speechRecognizer.Recognizing += (s, e) =>
                {
                    // Handle recognizing event
                    // e.Result.Text contains the recognized text
                };

                _speechRecognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.TranslatedSpeech)
                    {
                        var translation = e.Result.;
                        // Handle translated text
                        // translation contains the translated text in English
                    }
                };

                await _speechRecognizer.StartContinuousRecognitionAsync();

                return Ok();
            }

            [HttpPost]
            public async Task<IActionResult> StopTranslation()
            {
                if (_speechRecognizer != null)
                {
                    await _speechRecognizer.StopContinuousRecognitionAsync();
                    _speechRecognizer.Dispose();
                    _speechRecognizer = null;
                }

                return Ok();
            }
        }
    }

