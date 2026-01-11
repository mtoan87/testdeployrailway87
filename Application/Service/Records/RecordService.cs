using Application.Commons;
using Application.DTO.Orders;
using Application.DTO.Records;
using Application.Interfaces;
using Application.Interfaces.Records;
using Application.Service.Users;
using Mapster;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Records
{
    public class RecordService : IRecordService
    {
        private readonly IUnitOfWork _unitOfWork;
       
        public RecordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
           
        }
        public async Task<Pagination<LogListDTO>> GetPaginationAsync(LogPaginationDTO paginationDTO)
        {
            try
            {
                var pagination = await _unitOfWork.LogRepo.GetPaginationAsync(paginationDTO);

                return new Pagination<LogListDTO>
                {
                    Items = pagination.Items.Adapt<List<LogListDTO>>(),
                    TotalItemsCount = pagination.TotalItemsCount,
                    PageSize = pagination.PageSize,
                    PageIndex = pagination.PageIndex
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"An error occurred while getting pagination: {ex.Message}", ex);
            }
        }
    }
}
