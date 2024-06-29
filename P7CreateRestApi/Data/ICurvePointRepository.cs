using Dot.Net.WebApi.Domain;

namespace P7CreateRestApi.Data
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