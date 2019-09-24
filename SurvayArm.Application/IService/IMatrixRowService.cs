using SurvayArm.Application.Dto;
using System.Collections.Generic;

namespace SurvayArm.Application.IService
{
    public interface IMatrixRowService
    {
        void UpdateOption(IList<MatrixRowDto> dto, int fieldOptionId);

        IList<MatrixRowDto> GetOptionByFieldOptionId(int fieldOptionId);

        bool DeleteRowsByFieldOptionId(int fieldOptionId);

        void Save();
    }
}
