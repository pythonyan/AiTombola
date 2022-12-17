using iTombola.Core;
using iTombola.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace iTombola.Services
{
    public class DialectsService
    {
        public DialectsService(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			ArgumentNullException.ThrowIfNull(loggerFactory);

			this.logger = loggerFactory.CreateLogger<CsvDescriptionsRepository>();
		}

		private static string FilesRelativePath = "Descriptions";
		private readonly ILogger logger;

		private string GetFullPathForFiles()
		{
			var exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			var fullPath = Path.Combine(exePath, FilesRelativePath);
			return fullPath;
		}

		private Task<Dialect> ExtractDialectFromFileAsync(string descriptionFile)
		{
			var dialect = new Dialect();
			using (TextFieldParser parser = new TextFieldParser(descriptionFile))
			{
				parser.TextFieldType = FieldType.Delimited;
				parser.SetDelimiters(";");
				while (!parser.EndOfData)
				{
					string[] fields = parser.ReadFields();

					switch (fields[0].ToLower())
					{
						case "displayname":
							dialect.DisplayName = fields[1];
							break;
						case "dialect":
							dialect.DialectName = fields[1];
							break;
						case "culture":
							dialect.Culture = fields[1];
							break;
						default:
							break;
					}
				}
			}

			return Task.FromResult(dialect);
		}

		public async Task<List<Dialect>> LoadAsync() 
        {
			var result = new List<Dialect>();

			var filePath = GetFullPathForFiles();

			var descriptionFiles=Directory.EnumerateFiles(filePath, "*.csv");

			foreach (var descriptionFile in descriptionFiles)
			{
				result.Add(await ExtractDialectFromFileAsync(descriptionFile));
			}

			return result;
		}

		
	}
}
