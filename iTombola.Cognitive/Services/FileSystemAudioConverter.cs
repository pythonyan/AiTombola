using iTombola.Cognitive.Configurations;
using iTombola.Core;
using iTombola.Core.Implementations;
using iTombola.Core.Interfaces;
using iTombola.Core.Models;
using iTombola.Core.Utilities;
using Microsoft.CognitiveServices.Speech;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Media;

namespace iTombola.Cognitive.Services
{
	public class FileSystemAudioConverter : IAudioConverter
	{
		private readonly ILogger logger;
		private readonly FileSystemAudioConverterConfiguration config;

		public FileSystemAudioConverter(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			ArgumentNullException.ThrowIfNull(configuration);
			ArgumentNullException.ThrowIfNull(loggerFactory);

			config = FileSystemAudioConverterConfiguration.Load(configuration);
			logger = loggerFactory.CreateLogger<TombolaService>();
		}

		public async Task<AudioConverterResponse> ConvertDescriptionToAudio(NumberDescriptionInfo description, CancellationToken token = default)
		{
			var result = new AudioConverterResponse
			{
				Audio = null,
				Culture = description.Culture,
				InputText = description.Content,
				AudioFilePath = null
			};

			if (!string.IsNullOrWhiteSpace(description.Content))
			{
				result.AudioFilePath = await SynthetizeTextAsync(description.Content, description.Culture ,description.Type, token);
			}
			return result;
		}

		public async Task<AudioConverterResponse> ConvertTextToAudio(string text, string culture, CancellationToken token = default)
		{
			var result = new AudioConverterResponse
			{
				Audio = null,
				Culture = culture,
				InputText = text
			};

			if (!string.IsNullOrWhiteSpace(text))
			{
				result.AudioFilePath = await SynthetizeTextAsync(text, culture, DescriptionType.Text, token);
			}
			return result;
		}

		private async Task<string> SynthetizeTextAsync(string text, string culture, DescriptionType type, CancellationToken token)
		{
			var speechConfig = SpeechConfig.FromSubscription(config.SpeechKey, config.SpeechRegion);
			speechConfig.SpeechSynthesisLanguage = culture;
			if (!string.IsNullOrWhiteSpace(config.VoiceName))
				speechConfig.SpeechSynthesisVoiceName=config.VoiceName;

			var synthesizer = new SpeechSynthesizer(speechConfig, null);

			SpeechSynthesisResult speechResult = null;
			switch (type)
			{
				case DescriptionType.Text:
					speechResult = await synthesizer.SpeakTextAsync(text);
					break;
				case DescriptionType.SSML:
					speechResult = await synthesizer.SpeakSsmlAsync(text);
					break;
				case DescriptionType.Unknown:
				default:
					break;
			}

			if (speechResult != null)
			{
				if (speechResult.Reason == ResultReason.SynthesizingAudioCompleted)
				{
					logger.LogTrace($"Synthesis completed for text \"{text}\"");
					using var audioDataStream = AudioDataStream.FromResult(speechResult);

					var audioPath = config.GetAudioFullPath();
					Directory.CreateDirectory(audioPath);
					var filename = Path.Combine(audioPath, $"{Guid.NewGuid()}.wav");
					await audioDataStream.SaveToWaveFileAsync(filename);

					return filename;
				}
				else if (speechResult.Reason == ResultReason.Canceled)
				{
					var cancellation = SpeechSynthesisCancellationDetails.FromResult(speechResult);
					Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

					if (cancellation.Reason == CancellationReason.Error)
					{
						logger.LogTrace($"CANCELED: ErrorCode={cancellation.ErrorCode}");
						logger.LogTrace($"CANCELED: ErrorDetails=\"{cancellation.ErrorDetails}\"");
					}
				}
			}
			return null;
		}

	}
}
