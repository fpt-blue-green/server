using BusinessObjects.Models;
using BusinessObjects.ModelsDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
	public interface ITagService
	{
		Task<IEnumerable<TagDTO>> GetAllTags();
	}
}
