using Dot.Net.WebApi.Data;
using Dot.Net.WebApi.Domain;
using Microsoft.EntityFrameworkCore;
using System;

namespace P7CreateRestApi.Data
{
    public class CurvePointRepository : ICurvePointRepository
    {
        private readonly LocalDbContext _context;

        public CurvePointRepository(LocalDbContext context)
        {
            _context = context;
        }

        public async Task<CurvePoint> CreateCurvePointAsync(CurvePoint curvePoint)
        {
            _context.CurvePoints.Add(curvePoint);
            await _context.SaveChangesAsync();
            return curvePoint;
        }

        public async Task<CurvePoint> GetCurvePointByIdAsync(int id)
        {
            return await _context.CurvePoints.FindAsync(id);
        }

        public async Task<IEnumerable<CurvePoint>> GetAllCurvePointsAsync()
        {
            return await _context.CurvePoints.ToListAsync();
        }

        public async Task<CurvePoint> UpdateCurvePointAsync(CurvePoint curvePoint)
        {
            _context.CurvePoints.Update(curvePoint);
            await _context.SaveChangesAsync();
            return curvePoint;
        }

        public async Task<bool> DeleteCurvePointAsync(int id)
        {
            var curvePoint = await _context.CurvePoints.FindAsync(id);
            if (curvePoint == null)
            {
                return false;
            }

            _context.CurvePoints.Remove(curvePoint);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
