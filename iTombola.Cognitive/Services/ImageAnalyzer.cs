using iTombola.Cognitive.Configurations;
using iTombola.Core.Implementations;
using iTombola.Core.Interfaces;
using iTombola.Core.Models;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using static System.Net.WebRequestMethods;

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
		public ComputerVisionClient Authenticate(string endpoint, string key)
		{
			ComputerVisionClient client =
			  new ComputerVisionClient(new ApiKeyServiceClientCredentials(key))
			  { Endpoint = endpoint };
			return client;
		}

		public async Task<ImageAnalyzerResponse> ExtractNumbersImageFromStream(Stream imageData, CancellationToken token)
		{
			var resultService = new ImageAnalyzerResponse()
			{
				Numbers = new List<ExtractedNumberInfo>()
			};

			ComputerVisionClient client = Authenticate(config.Endpoint, config.SubscriptionKey);
            ReadOperationResult results;

            ReadInStreamHeaders? textHeaders = await client.ReadInStreamAsync(imageData);
            var operationId = textHeaders.GetOperationId();

			do
			{
				await Task.Delay(125);
				results = await client.GetReadResultAsync(Guid.Parse(operationId), cancellationToken: token);
			}
			while (results.Status == OperationStatusCodes.Running ||
				results.Status == OperationStatusCodes.NotStarted);

			if (results.Status == OperationStatusCodes.Succeeded)
			{
				foreach (ReadResult page in results.AnalyzeResult.ReadResults)
				{
					foreach (Line line in page.Lines)
					{
						resultService.Numbers.AddRange(ExtractNumbersFromLine(line));
					}
				}
			}
			return resultService;
		}


		private List<ExtractedNumberInfo> ExtractNumbersFromLine(Line line)
		{
			var numbers = new List<ExtractedNumberInfo>();
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
			return numbers;
		}
	}



}
