using Azure.AI.Vision.ImageAnalysis;
using Azure;
using iTombola.Cognitive.Configurations;
using iTombola.Core.Implementations;
using iTombola.Core.Interfaces;
using iTombola.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace iTombola.Cognitive.Services
{
    public class ImageAnalyzer : IImageAnalyzer
	{
		private readonly ILogger logger;
		private readonly ImagenAnalyzerConfiguration config;

		public ImageAnalyzer(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			ArgumentNullException.ThrowIfNull(configuration);
			ArgumentNullException.ThrowIfNull(loggerFactory);

			config = ImagenAnalyzerConfiguration.Load(configuration);
			logger = loggerFactory.CreateLogger<TombolaService>();
		}

        public ImageAnalysisClient Authenticate(string endpoint, string key)
        {
            var credentials = new AzureKeyCredential(key);
            return new ImageAnalysisClient(new Uri(endpoint), credentials);
        }

        public async Task<ImageAnalyzerResponse> ExtractNumbersImageFromStream(Stream imageData, CancellationToken token)
        {
            var resultService = new ImageAnalyzerResponse()
            {
                Numbers = new List<ExtractedNumberInfo>()
            };

            var client = Authenticate(config.Endpoint, config.SubscriptionKey);

            var results = await client.AnalyzeAsync(BinaryData.FromStream(imageData),
                    VisualFeatures.Read, cancellationToken: token);

            if (!results.GetRawResponse().IsError && results.Value != null && results.Value.Read != null)
            {
                foreach (var block in results.Value.Read.Blocks)
                {
                    foreach (var line in block.Lines)
                    {
                        resultService.Numbers.AddRange(ExtractNumbersFromLine(line));
                    }
                }
            }

            return resultService;
        }

        private List<ExtractedNumberInfo> ExtractNumbersFromLine(DetectedTextLine line)
        {
            var numbers = new List<ExtractedNumberInfo>();
            if (line != null)
            {
                foreach (var word in line.Words)
                {
                    if (word != null && !string.IsNullOrWhiteSpace(word.Text))
                    {
                        if (int.TryParse(word.Text, out var intNumber) && (intNumber > 0 && intNumber <= 90))
                        {
                            numbers.Add(new ExtractedNumberInfo()
                            {
                                Confidence = word.Confidence,
                                Number = word.Text
                            });
                        }
                    }
                }
            }
            return numbers;
        }
    }



}
