using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public enum EAdminActionType
    {
        [Description("Tạo mới")]
        Create = 1,

        [Description("Cập nhật")]
        Update = 2,

        [Description("Xóa")]
        Delete = 3,

        [Description("Cập nhập thời gian cấm của 1 người dùng")]
        BanUser = 4,
    }
}
