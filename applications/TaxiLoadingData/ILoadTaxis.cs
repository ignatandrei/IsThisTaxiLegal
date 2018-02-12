using System.Threading.Tasks;
using TaxiObjects;

namespace TaxiLoadingData
{
    public interface ILoadTaxis
    {
        Task<TaxiAutorization> TaxiFromPlateSqlite(string plateNumber);
        Task<TaxiAutorizations> TaxiFromPlateSqliteAll();
    }
}