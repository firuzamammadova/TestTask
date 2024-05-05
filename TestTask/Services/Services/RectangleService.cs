using Dapper;
using Microsoft.VisualBasic.FileIO;
using Services.Infrastructure;
using Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IRectangleService
    {
        Task<IEnumerable<Rectangle>> GetAllAsync(Segment segment);
    }
    public class RectangleService : IRectangleService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly string _getAllSql = @"SELECT * FROM dbo.Rectangles WHERE x1<=@maxX AND x2>= @minX AND y1<= @maxY AND y2>=@minY ";


        public RectangleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Rectangle>> GetAllAsync(Segment segment)
        {
            try
            {
                var parameters = new
                {
                    maxX = Math.Max(segment.X1, segment.X2),
                    minX = Math.Min(segment.X1, segment.X2),
                    maxY = Math.Max(segment.Y1, segment.Y2),
                    minY = Math.Min(segment.Y1, segment.Y2)
                };
                var a = Math.Max(segment.X1, segment.X2);
                var result = await _unitOfWork.GetConnection()
                    .QueryAsync<Rectangle>(_getAllSql, parameters, _unitOfWork.GetTransaction());
                return result.ToList();
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
}
