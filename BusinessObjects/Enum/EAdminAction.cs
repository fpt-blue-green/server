using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public enum EAdminAction
    {
        [Display(Name = "Tạo mới")]
        Create = 1,

        [Display(Name = "Cập nhật")]
        Update = 2,

        [Display(Name = "Xóa")]
        Delete = 3,

        [Display(Name = "Cập nhập thời gian cấm của 1 người dùng")]
        BanUser = 4,
    }
}
