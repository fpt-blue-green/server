using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Interface
{
    public interface IFeedBackRepository
    {
        Task<IEnumerable<Feedback>> GetAlls();
        Task<Feedback> GetById(Guid id);
        Task Create(Feedback feedback);
        Task Update(Feedback feedback);
        Task Delete(Guid id);
    }
}
