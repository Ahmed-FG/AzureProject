﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.Translation;

namespace AzureProject2
{
    class Program
    {
        public static async Task TranslationContinuousRecognitionAsync()
        {
            // Creates an instance of a speech translation config with specified subscription key and service region.
            // Replace with your own subscription key and service region (e.g., "westus").
            var config = SpeechTranslationConfig.FromSubscription("Key", "Region");

            // Sets source and target languages.
            string fromLanguage = "en-US";
            config.SpeechRecognitionLanguage = fromLanguage;
            config.AddTargetLanguage("tr");

            // Sets voice name of synthesis output.
            const string EnglishVoice = "en-US";
            config.VoiceName = EnglishVoice;
            // Creates a translation recognizer using microphone as audio input.
            using (var recognizer = new TranslationRecognizer(config))
            {
                // Subscribes to events.
                recognizer.Recognizing += (s, e) =>
                {
                    Console.WriteLine($"RECOGNIZING in '{fromLanguage}': Text={e.Result.Text}");
                    foreach (var element in e.Result.Translations)
                    {
                        Console.WriteLine($"    TRANSLATING into '{element.Key}': {element.Value}");
                    }
                };

                recognizer.Recognized += (s, e) =>
                {
                    if (e.Result.Reason == ResultReason.TranslatedSpeech)
                    {
                        Console.WriteLine($"\nFinal result: Reason: {e.Result.Reason.ToString()}, recognized text in {fromLanguage}: {e.Result.Text}.");
                        foreach (var element in e.Result.Translations)
                        {
                            Console.WriteLine($"    TRANSLATING into '{element.Key}': {element.Value}");
                        }
                    }
                };

                recognizer.Synthesizing += (s, e) =>
                {
                    var audio = e.Result.GetAudio();
                    Console.WriteLine(audio.Length != 0
                        ? $"AudioSize: {audio.Length}"
                        : $"AudioSize: {audio.Length} (end of synthesis data)");
                };

                recognizer.Canceled += (s, e) =>
                {
                    Console.WriteLine($"\nRecognition canceled. Reason: {e.Reason}; ErrorDetails: {e.ErrorDetails}");
                };

                recognizer.SessionStarted += (s, e) =>
                {
                    Console.WriteLine("\nSession started event.");
                };

                recognizer.SessionStopped += (s, e) =>
                {
                    Console.WriteLine("\nSession stopped event.");
                };

                // Starts continuous recognition. Uses StopContinuousRecognitionAsync() to stop recognition.
                Console.WriteLine("Say something to start translating...");
                await recognizer.StartContinuousRecognitionAsync();

                do
                {
                    Console.WriteLine("Press Enter to stop");
                } while (Console.ReadKey().Key != ConsoleKey.Enter);

                // Stops continuous recognition.
                await recognizer.StopContinuousRecognitionAsync();
            }
        }

        static async Task Main(string[] args)
        {
            await TranslationContinuousRecognitionAsync();
        }
    }
}
