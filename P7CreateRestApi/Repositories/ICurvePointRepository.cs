using P7CreateRestApi.Domain;

namespace P7CreateRestApi.Repositories
{
    public interface ICurvePointRepository
    {
        Task<CurvePoint> CreateCurvePointAsync(CurvePoint curvePoint);
        Task<CurvePoint> GetCurvePointByIdAsync(int id);
        Task<IEnumerable<CurvePoint>> GetAllCurvePointsAsync();
        Task<CurvePoint> UpdateCurvePointAsync(CurvePoint curvePoint);
        Task<bool> DeleteCurvePointAsync(int id);
    }
}