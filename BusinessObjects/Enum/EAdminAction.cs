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
        Delete = 3
    }
}
