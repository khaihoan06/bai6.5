
namespace KaraokeManagement.DTO
{
    public class PhongHatDTO
    {
        public int MaPhong { get; set; }

        public string TenPhong { get; set; } = string.Empty;

        public string LoaiPhong { get; set; } = string.Empty;

        public decimal GiaTheoGio { get; set; }

        public string TrangThai { get; set; } = "Trống";
    }
}
