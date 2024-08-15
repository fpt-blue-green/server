using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.ModelsDTO
{
	public class TagDTO
	{
		public Guid Id { get; set; }

		public string? TagName { get; set; }

		public bool? IsPremiumTag { get; set; }
	}
}
