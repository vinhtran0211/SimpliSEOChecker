using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services.Interface
{

    public interface ISearchBehavior // add more search behavior with this interface if needed
    {
        Task<int> CheckRank(string keyword, string urlToFind);
    }

}
