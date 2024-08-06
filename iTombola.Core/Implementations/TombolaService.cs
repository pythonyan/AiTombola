using iTombola.Core.Interfaces;
using iTombola.Core.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Core.Implementations
{
    public class TombolaService : ITombolaService
    {
        private readonly ILogger<TombolaService> logger;
        private readonly IImageAnalyzer imageAnalyzer;
        private readonly IAudioConverter audioConverter;
        private readonly IDescriptionsRepository descriptionRepository;

        public TombolaService(IImageAnalyzer imageAnalyzer, IAudioConverter audioConverter,
            IDescriptionsRepository descriptionRepository, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(imageAnalyzer);
            ArgumentNullException.ThrowIfNull(audioConverter);
            ArgumentNullException.ThrowIfNull(descriptionRepository);
            ArgumentNullException.ThrowIfNull(loggerFactory);

            this.imageAnalyzer = imageAnalyzer;
            this.audioConverter = audioConverter;
            this.descriptionRepository = descriptionRepository;
            this.logger = loggerFactory.CreateLogger<TombolaService>();
        }

        public async Task<ExtractedNumbersResponse> ExtractNumberFromStream(Stream imageData, string culture, string dialect,string voiceName,
            double expectedConfidence, int maximumResultNumber = 1, CancellationToken token = default)
        {
            var numberResult = new ExtractedNumbersResponse()
            {
                IsValid = false,
                Numbers = new List<ExtractedNumberInfo>()
            };

            var imgAnalysisResult = await this.imageAnalyzer.ExtractNumbersImageFromStream(imageData, token);


            if (imgAnalysisResult.Numbers.Any())

            {
                numberResult.IsValid = true;
                var validNumbers = imgAnalysisResult.Numbers
                    .Where(n => n.Confidence >= expectedConfidence)
                    .OrderByDescending(n => n.Confidence)
                    .Take(maximumResultNumber);

                foreach (var number in validNumbers)
                {
                    if (token.IsCancellationRequested) break;

                    var audioConverterResult = await this.audioConverter.ConvertTextToAudio(number.Number, culture,voiceName, token);
                    if (audioConverterResult.Audio != null)
                        number.NumberAudio = audioConverterResult.Audio;
                    if (audioConverterResult.AudioFilePath != null)
                        number.NumberAudioFilePath = audioConverterResult.AudioFilePath;

                    number.Description = await this.descriptionRepository.GetDescriptionAsync(number.Number, culture, dialect, token);

                    if (number.HasDescription())
                    {
                        audioConverterResult = await this.audioConverter.ConvertDescriptionToAudio(number.Description, voiceName, token);
                        if (audioConverterResult.Audio != null)
                            number.DescriptionAudio = audioConverterResult.Audio;
                        if (audioConverterResult.AudioFilePath != null)
                            number.DescriptionAudioFilePath = audioConverterResult.AudioFilePath;
                    }
                    numberResult.Numbers.Add(number);
                }
            }

            return numberResult;
        }
    }
}
