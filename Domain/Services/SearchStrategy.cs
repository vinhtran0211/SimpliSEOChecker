using Domain.Services.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Services
{
    public class SearchStrategy
    {
        private ISearchBehavior _behavior;
        public SearchStrategy() { }
        public SearchStrategy(ISearchBehavior behavior)
        {
            _behavior = behavior;
        }
        public Task<int> DoCheckRank(string keyword, string urlToFind)
        {
           return _behavior.CheckRank(keyword, urlToFind);
        }

        public void SetBehavior(ISearchBehavior behaviorType)
        {
            _behavior = behaviorType;
        }

    }
}
