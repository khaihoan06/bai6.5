
using System;
using System.Data;
using KaraokeManagement.DAL;
using KaraokeManagement.DTO;

namespace KaraokeManagement.BUS
{
    public class DichVuBUS
    {
        private readonly DichVuDAL dal = new DichVuDAL();

        public DataTable GetAll()
        {
            return dal.GetAll();
        }

        public DataTable GetById(int maDV)
        {
            if (maDV <= 0)
                throw new Exception("Mã dịch vụ không hợp lệ.");

            return dal.GetById(maDV);
        }

        public int Insert(DichVuDTO dv)
        {
            if (dv == null)
                throw new ArgumentNullException(nameof(dv));

            Validate(dv);

            return dal.Insert(dv);
        }

        public int Update(DichVuDTO dv)
        {
            if (dv == null)
                throw new ArgumentNullException(nameof(dv));

            if (dv.MaDV <= 0)
                throw new Exception("Mã dịch vụ không hợp lệ.");

            Validate(dv);

            return dal.Update(dv);
        }

        public int Delete(int maDV)
        {
            if (maDV <= 0)
                throw new Exception("Mã dịch vụ không hợp lệ.");

            int result = dal.Delete(maDV);

            if (result == 0)
            {
                throw new Exception(
                    "Không thể xóa dịch vụ đã được sử dụng trong hóa đơn.");
            }

            return result;
        }

        public int UpdateStock(int maDV, int soLuong)
        {
            if (maDV <= 0)
                throw new Exception("Mã dịch vụ không hợp lệ.");

            if (soLuong == 0)
                throw new Exception("Số lượng cập nhật không được bằng 0.");

            int result = dal.UpdateStock(maDV, soLuong);

            if (result == 0)
                throw new Exception("Không đủ tồn kho hoặc dịch vụ không tồn tại.");

            return result;
        }

        private void Validate(DichVuDTO dv)
        {
            if (string.IsNullOrWhiteSpace(dv.TenDV))
                throw new Exception("Tên dịch vụ không được để trống.");

            if (dv.TenDV.Length > 100)
                throw new Exception("Tên dịch vụ không được vượt quá 100 ký tự.");

            if (dv.DonGia < 0)
                throw new Exception("Đơn giá không được âm.");

            if (dv.TonKho < 0)
                throw new Exception("Tồn kho không được âm.");
        }
    }
}
