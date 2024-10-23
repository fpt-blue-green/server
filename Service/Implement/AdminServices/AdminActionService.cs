using AutoMapper;
using BusinessObjects;
using BusinessObjects.Models;
using OfficeOpenXml;
using Repositories;
using Repositories.Implement;

namespace Service
{
    public class AdminActionService : IAdminActionService
    {
        private static IAdminActionRepository _adminActionRepository = new AdminActionRepository();
        private static readonly IMapper _mapper = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<AutoMapperProfile>();
        }).CreateMapper();

        public async Task<IEnumerable<AdminActionDTO>> GetAdminAction()
        {
            var adminActions = (await _adminActionRepository.GetAdminActions()).Take(100).ToList();
            return _mapper.Map<IEnumerable<AdminActionDTO>>(adminActions);
        }

        public async Task<(byte[] fileContent, string fileName)> GetDataFile()
        {
            var adminActions = await _adminActionRepository.GetAdminActions();
            var data = CreateExcel(adminActions);
            return (data, $"AdminAction_{DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}.csv");
        }

        public static byte[] CreateExcel(IEnumerable<AdminAction> adminActions)
        {

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet = package.Workbook.Worksheets.Add("Admin Actions");

                worksheet.Cells[1, 1].Value = "ID";
                worksheet.Cells[1, 2].Value = "Action Type";
                worksheet.Cells[1, 3].Value = "Action Date (UTC+7)";
                worksheet.Cells[1, 4].Value = "Object Type";
                worksheet.Cells[1, 5].Value = "User Name";
                worksheet.Cells[1, 6].Value = "User Email";
                worksheet.Cells[1, 7].Value = "Action Details";

                int row = 2;

                foreach (var action in adminActions)
                {
                    worksheet.Cells[row, 1].Value = action.Id;
                    worksheet.Cells[row, 2].Value = action.ActionType;
                    worksheet.Cells[row, 3].Value = action.ActionDate.ToString("yyyy-MM-dd HH:mm:ss");
                    worksheet.Cells[row, 4].Value = action.ObjectType;
                    worksheet.Cells[row, 5].Value = action.User?.DisplayName;
                    worksheet.Cells[row, 6].Value = action.User?.Email;
                    worksheet.Cells[row, 7].Value = action.ActionDetails;
                    row++;
                }

                worksheet.Cells[2, 7, row - 1, 7].Style.WrapText = true;
                worksheet.Cells.AutoFitColumns();
                worksheet.Column(7).Width = 180;
                return package.GetAsByteArray();
            }
        }

    }
}
