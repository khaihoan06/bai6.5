
using System;
using System.Data;
using KaraokeManagement.DAL;

namespace KaraokeManagement.BUS
{
    public class HoaDonBUS
    {
        private readonly HoaDonDAL dal = new HoaDonDAL();

        private readonly ChiTietDichVuDAL chiTietDAL =
            new ChiTietDichVuDAL();

        // Lấy toàn bộ danh sách hóa đơn
        public DataTable GetAll()
        {
            return dal.GetAll();
        }

        // Lấy hóa đơn theo mã
        public DataTable GetById(int maHD)
        {
            if (maHD <= 0)
                throw new Exception("Mã hóa đơn không hợp lệ.");

            return dal.GetById(maHD);
        }

        // Lấy hóa đơn đang mở theo mã phòng
        public DataTable GetOpenInvoiceByRoom(int maPhong)
        {
            if (maPhong <= 0)
                throw new Exception("Mã phòng không hợp lệ.");

            return dal.GetOpenInvoiceByRoom(maPhong);
        }

        // Lấy danh sách dịch vụ đã gọi trong hóa đơn
        public DataTable GetServices(int maHD)
        {
            if (maHD <= 0)
                throw new Exception("Mã hóa đơn không hợp lệ.");

            return chiTietDAL.GetByHoaDon(maHD);
        }

        // Thêm dịch vụ vào hóa đơn
        public int AddService(int maHD, int maDV, int soLuong)
        {
            if (maHD <= 0)
                throw new Exception("Mã hóa đơn không hợp lệ.");

            if (maDV <= 0)
                throw new Exception("Mã dịch vụ không hợp lệ.");

            if (soLuong <= 0)
                throw new Exception("Số lượng phải lớn hơn 0.");

            return chiTietDAL.AddService(maHD, maDV, soLuong);
        }

        // Thanh toán hóa đơn
        public decimal Checkout(int maHD)
        {
            if (maHD <= 0)
                throw new Exception("Mã hóa đơn không hợp lệ.");

            return dal.Checkout(maHD);
        }
    }
}
