using SurvayArm.Application.Dto;
using System.Collections.Generic;

namespace SurvayArm.Application.IService
{
    public interface IMatrixHeaderService 
    {
        void UpdateOption(IList<MatrixHeaderDto> dto, int fieldOptionId);

        IList<MatrixHeaderDto> GetOptionByFieldOptionId(int fieldOptionId);

        bool DeleteHeadersByFieldOptionId(int fieldOptionId);

        void Save();
    }
}
