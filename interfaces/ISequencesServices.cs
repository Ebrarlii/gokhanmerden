namespace Ishop.Core.Finance.Services
{
    public interface ISequencesServices{
        System.Int64 getNextValue(string sequenceName);
    }
}