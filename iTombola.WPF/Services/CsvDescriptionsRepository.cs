using iTombola.Core;
using iTombola.Core.Implementations;
using iTombola.Core.Interfaces;
using iTombola.Core.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace iTombola.Services
{
	public class CsvDescriptionsRepository : IDescriptionsRepository
	{
		private static string FilesRelativePath = "Descriptions";
		private readonly ILogger logger;

		public CsvDescriptionsRepository(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			ArgumentNullException.ThrowIfNull(loggerFactory);

			this.logger = loggerFactory.CreateLogger<CsvDescriptionsRepository>();
		}

		private string GetFullPathForFiles()
		{
			var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var fullPath = Path.Combine(exePath, FilesRelativePath);
			return fullPath;
		}

		private string GetDescriptionsFullFileName(string culture, string dialect)
		{
			if (string.IsNullOrWhiteSpace(dialect))
				return Path.Combine(GetFullPathForFiles(), $"{culture}.csv");
			return Path.Combine(GetFullPathForFiles(), $"{culture}-{dialect}.csv");
		}

		public Task<NumberDescriptionInfo> GetDescriptionAsync(string number, string culture,
			string dialect, CancellationToken token = default)
		{
			var result = new NumberDescriptionInfo()
			{
				Culture = culture,
				Dialect = dialect,
			};

			var fileName = GetDescriptionsFullFileName(culture, dialect);

			if (File.Exists(fileName))
			{
				try
				{
					using (TextFieldParser parser = new TextFieldParser(fileName))
					{
						parser.TextFieldType = FieldType.Delimited;
						parser.SetDelimiters(";");
						while (!parser.EndOfData)
						{
							string[] fields = parser.ReadFields();
							if (fields[0] == number)
							{
								result.Content = fields[2];
								result.Type = (DescriptionType)Enum.Parse(typeof(DescriptionType), fields[1],true);
								break;
							}
						}
					}
				}
				catch (Exception ex)
				{
					logger.LogError(ex, "Error during descriptions file parsing");
					result.Content = null;
					result.Type = Core.DescriptionType.Unknown;
				}
			}

			return Task.FromResult(result);
		}
	}
}
