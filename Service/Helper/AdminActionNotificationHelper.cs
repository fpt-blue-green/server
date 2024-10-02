using BusinessObjects.Models;
using BusinessObjects;
using Repositories;
using Repositories.Implement;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Service.Helper
{
    public class AdminActionNotificationHelper
    {
        private static IAdminActionRepository _adminAdctionRepository = new AdminActionRepository();

        public async Task CreateNotification<T>(UserDTO userDTO, EAdminAction actionType, string modelChange, T? newData, T? oldData)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            string action = GetDisplayName(actionType); ;
            // Format ngày giờ
            string formattedDateTime = DateTime.Now.ToString("HH:mm 'ngày' dd 'tháng' MM 'năm' yyyy");

            // Tạo đoạn văn tóm tắt
            string summary = $"Vào lúc {formattedDateTime}, Admin {userDTO.Name} đã thực hiện thao tác [{action}] trên model {modelChange}. " +
                 (oldData == null ? "" : $"Dữ liệu cũ: {JsonConvert.SerializeObject(oldData, settings)}. ") +
                 (newData == null ? "" : $"Dữ liệu mới sau khi [{action}] là: {JsonConvert.SerializeObject(newData, settings)}.");


            var detail = new AdminAction
            {
                UserId = userDTO.Id,
                ActionType = (int)actionType,
                ActionDetails = summary,
            };

            await _adminAdctionRepository.Create(detail);
        }

        public static string GetDisplayName(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = (DisplayAttribute)fieldInfo!.GetCustomAttributes(typeof(DisplayAttribute), false)[0];
            return attribute?.Name ?? value.ToString();
        }
    }
}
