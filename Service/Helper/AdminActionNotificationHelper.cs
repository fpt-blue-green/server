using BusinessObjects.Models;
using BusinessObjects;
using Repositories;
using Repositories.Implement;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Supabase.Gotrue;
using System;

namespace Service.Helper
{
    public class AdminActionNotificationHelper
    {
        private static IAdminActionRepository _adminActionRepository = new AdminActionRepository();

        public async Task CreateNotification<T>(UserDTO userDTO, EAdminActionType actionType, T? newData, T? oldData, string? objectType = null, bool isReport = false)
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

            switch ((int)actionType){
                case (int)EAdminActionType.BanUser:
                    summary = $"Vào lúc {formattedDateTime}, Admin {userDTO.Name} đã cấm/hủy cấm người dùng. Thông tin chi tiết: {JsonConvert.SerializeObject(newData, settings)}";
                    break;
                case (int)EAdminActionType.ApproveWithDraw:
                    summary = $"Vào lúc {formattedDateTime}, Admin {userDTO.Name} đã Phê duyệt yêu cầu rút tiền. Thông tin chi tiết: {JsonConvert.SerializeObject(newData, settings)}";
                    break;
                case (int)EAdminActionType.RejectWithDraw:
                    summary = $"Vào lúc {formattedDateTime}, Admin {userDTO.Name} đã Từ chối yêu cầu rút tiền. Thông tin chi tiết: {JsonConvert.SerializeObject(newData, settings)}";
                    break;
                default:
                    string action = ((EAdminActionType)actionType!).GetEnumDescription();
                    summary = $"Vào lúc {formattedDateTime}, Admin {userDTO.Name} đã thực hiện thao tác [{action}] trên model {objectType ?? typeof(T).Name}. " +
                         (oldData == null ? "" : $"Dữ liệu cũ: {JsonConvert.SerializeObject(oldData, settings)}. ") +
                         (newData == null ? "" : $"Dữ liệu mới sau khi [{action}] là: {JsonConvert.SerializeObject(newData, settings)}.");
                    break;
            };
           
            var detail = new AdminAction
            {
                UserId = userDTO.Id,
                ActionType = (int)actionType!,
                ActionDetails = summary,
                ObjectType = objectType ?? typeof(T).Name,
            };

            await _adminActionRepository.Create(detail);
        }
    }
}
