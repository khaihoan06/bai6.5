
using System;
using System.Data;
using KaraokeManagement.DAL;
using KaraokeManagement.DTO;

namespace KaraokeManagement.BUS
{
    public class PhongHatBUS
    {
        private readonly PhongHatDAL dal = new PhongHatDAL();

        public DataTable GetAll()
        {
            return dal.GetAll();
        }

        public DataTable GetById(int maPhong)
        {
            if (maPhong <= 0)
                throw new ArgumentException("Mã phòng không hợp lệ.");

            return dal.GetById(maPhong);
        }

        public int Insert(PhongHatDTO phong)
        {
            if (phong == null)
                throw new ArgumentNullException(nameof(phong));

            if (string.IsNullOrWhiteSpace(phong.TenPhong))
                throw new Exception("Tên phòng không được để trống.");

            if (phong.TenPhong.Length > 50)
                throw new Exception("Tên phòng không được vượt quá 50 ký tự.");

            if (phong.GiaTheoGio < 0)
                throw new Exception("Giá theo giờ không được âm.");

            if (phong.LoaiPhong.Length > 30)
                throw new Exception("Loại phòng không được vượt quá 30 ký tự.");

            return dal.Insert(phong);
        }

        public int Update(PhongHatDTO phong)
        {
            if (phong == null)
                throw new ArgumentNullException(nameof(phong));

            if (phong.MaPhong <= 0)
                throw new Exception("Mã phòng không hợp lệ.");

            if (string.IsNullOrWhiteSpace(phong.TenPhong))
                throw new Exception("Tên phòng không được để trống.");

            if (phong.TenPhong.Length > 50)
                throw new Exception("Tên phòng không được vượt quá 50 ký tự.");

            if (phong.GiaTheoGio < 0)
                throw new Exception("Giá theo giờ không được âm.");

            return dal.Update(phong);
        }

        public int Delete(int maPhong)
        {
            if (maPhong <= 0)
                throw new Exception("Mã phòng không hợp lệ.");

            int result = dal.Delete(maPhong);

            if (result == 0)
                throw new Exception(
                    "Không thể xóa phòng đang hát hoặc phòng đã có hóa đơn.");

            return result;
        }

        public int OpenRoom(int maPhong)
        {
            if (maPhong <= 0)
                throw new Exception("Vui lòng chọn phòng hợp lệ.");

            return dal.OpenRoom(maPhong);
        }
    }
}
