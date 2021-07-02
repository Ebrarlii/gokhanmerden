using Ishop.Core.Finance.Data;
using Microsoft.Extensions.Configuration;

namespace Ishop.Core.Finance.Services
{
    public class SequenceServices : ISequencesServices
    {
        FinanceUnitOfWork _financeUnitOfWork;
        public SequenceServices(IConfiguration config){
            _financeUnitOfWork = new FinanceUnitOfWork(config);
        }
        
        public System.Int64 getNextValue(string sequenceName)
        {
            Sequences sequences = _financeUnitOfWork.SequencesRepository.Get(p=> p.sequenceName == sequenceName);
            System.Int64 value = sequences.sequenceValue + 1;
            _financeUnitOfWork.SequencesRepository.Update(sequences);
            _financeUnitOfWork.Save();

            return value;
        }
    }
}