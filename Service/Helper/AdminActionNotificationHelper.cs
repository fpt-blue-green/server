using BusinessObjects.Models;
using BusinessObjects;
using Repositories;
using Repositories.Implement;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Supabase.Gotrue;

namespace Service.Helper
{
    public class AdminActionNotificationHelper
    {
        private static IAdminActionRepository _adminActionRepository = new AdminActionRepository();

        public async Task CreateNotification<T>(UserDTO userDTO, EAdminAction? actionType, T? newData, T? oldData, string? objectType = null, bool isReport = false)
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            // Format ngày giờ
            string formattedDateTime = DateTime.Now.ToString("HH:mm 'ngày' dd 'tháng' MM 'năm' yyyy");
            string summary = string.Empty;
            if (!isReport)
            {
                string action = GetDisplayName(actionType);
                // Tạo đoạn văn tóm tắt
                summary = $"Vào lúc {formattedDateTime}, Admin {userDTO.Name} đã thực hiện thao tác [{action}] trên model {objectType ?? typeof(T).Name}. " +
                     (oldData == null ? "" : $"Dữ liệu cũ: {JsonConvert.SerializeObject(oldData, settings)}. ") +
                     (newData == null ? "" : $"Dữ liệu mới sau khi [{action}] là: {JsonConvert.SerializeObject(newData, settings)}.");

            }
            else
            {
                actionType = EAdminAction.BanUser;
                summary = $"Vào lúc {formattedDateTime}, Admin {userDTO.Name} đã cấm người dùng vì vi phạm các quy định. Thông tin chi tiết: {JsonConvert.SerializeObject(newData, settings)}";
            }


            var detail = new AdminAction
            {
                UserId = userDTO.Id,
                ActionType = (int)actionType,
                ActionDetails = summary,
                ObjectType = objectType ?? typeof(T).Name,
            };

            await _adminActionRepository.Create(detail);
        }

        public static string GetDisplayName(Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attribute = (DisplayAttribute)fieldInfo!.GetCustomAttributes(typeof(DisplayAttribute), false)[0];
            return attribute?.Name ?? value.ToString();
        }
    }
}
