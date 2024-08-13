using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Interface
{
    public interface IFeedBackService
    {
        Task<IEnumerable<Feedback>> GetAllFeedBacks();
        Task<IEnumerable<Feedback>> GetUserFeedBacks(Guid userId);
        Task<double> GetAverageRate(Guid userId);
    }
}
