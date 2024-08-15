using BusinessObjects.Models;
using Repositories.Implement;
using Repositories.Interface;
using Serilog;
using Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implement
{
	public class TagService : ITagService
	{
		private static readonly ITagRepository _repository = new TagRepository();
		private static ILogger _loggerService = new LoggerService().GetLogger();
		public async Task<IEnumerable<Tag>> GetAllTags()
		{
			var topInflus = await _repository.GetAlls();
			return topInflus.ToList();
		}

	}
}
