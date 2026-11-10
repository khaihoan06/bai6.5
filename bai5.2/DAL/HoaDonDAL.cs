
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace KaraokeManagement.DAL
{
    public class HoaDonDAL
    {
        public DataTable GetAll()
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                SELECT
                    hd.MaHD,
                    hd.MaPhong,
                    p.TenPhong,
                    hd.GioVao,
                    hd.GioRa,
                    hd.TongTienPhong,
                    hd.TongTienDichVu,
                    hd.TongTienPhong + hd.TongTienDichVu AS TongTien,
                    hd.TrangThai
                FROM HoaDon hd
                INNER JOIN PhongHat p ON hd.MaPhong = p.MaPhong
                ORDER BY hd.MaHD DESC";

            using SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        public DataTable GetById(int maHD)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                SELECT *
                FROM HoaDon
                WHERE MaHD = @MaHD";

            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@MaHD", SqlDbType.Int).Value = maHD;

            using SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        // Lấy hóa đơn đang mở của phòng
        public DataTable GetOpenInvoiceByRoom(int maPhong)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                SELECT TOP 1 *
                FROM HoaDon
                WHERE MaPhong = @MaPhong
                  AND TrangThai = 0
                  AND GioRa IS NULL
                ORDER BY MaHD DESC";

            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@MaPhong", SqlDbType.Int).Value = maPhong;

            using SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        // Thanh toán hóa đơn bằng SqlTransaction
        public decimal Checkout(int maHD)
        {
            using SqlConnection conn = DBConnection.GetConnection();
            conn.Open();

            using SqlTransaction trans =
                conn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                // 1. Lấy hóa đơn và khóa bản ghi để tránh thanh toán trùng
                string invoiceSql = @"
                    SELECT
                        hd.MaPhong,
                        hd.GioVao,
                        hd.TrangThai,
                        p.GiaTheoGio
                    FROM HoaDon hd WITH (UPDLOCK, HOLDLOCK)
                    INNER JOIN PhongHat p
                        ON hd.MaPhong = p.MaPhong
                    WHERE hd.MaHD = @MaHD";

                int maPhong;
                DateTime gioVao;
                decimal giaTheoGio;

                using (SqlCommand cmd =
                    new SqlCommand(invoiceSql, conn, trans))
                {
                    cmd.Parameters.Add("@MaHD", SqlDbType.Int).Value = maHD;

                    using SqlDataReader reader = cmd.ExecuteReader();

                    if (!reader.Read())
                        throw new Exception("Không tìm thấy hóa đơn.");

                    maPhong = Convert.ToInt32(reader["MaPhong"]);
                    gioVao = Convert.ToDateTime(reader["GioVao"]);

                    if (Convert.ToBoolean(reader["TrangThai"]))
                        throw new Exception("Hóa đơn đã được thanh toán.");

                    giaTheoGio = Convert.ToDecimal(reader["GiaTheoGio"]);
                }

                // 2. Kiểm tra thời gian sử dụng phòng
                DateTime gioRa = DateTime.Now;

                double soGiay = (gioRa - gioVao).TotalSeconds;

                if (soGiay <= 0)
                    throw new Exception("Thời gian sử dụng phòng không hợp lệ.");

                // Tính tiền theo thời gian thực tế, không tự làm tròn giờ
                decimal soGio = (decimal)soGiay / 3600m;

                decimal tienPhong = Math.Round(
                    soGio * giaTheoGio,
                    0,
                    MidpointRounding.AwayFromZero);

                // 3. Tính tiền dịch vụ
                string serviceTotalSql = @"
                    SELECT ISNULL(
                        SUM(SoLuong * DonGia), 0)
                    FROM ChiTietDichVu
                    WHERE MaHD = @MaHD";

                decimal tienDichVu;

                using (SqlCommand cmd =
                    new SqlCommand(serviceTotalSql, conn, trans))
                {
                    cmd.Parameters.Add("@MaHD", SqlDbType.Int).Value = maHD;

                    tienDichVu = Convert.ToDecimal(cmd.ExecuteScalar());
                }

                // 4. Trừ tồn kho cho từng dịch vụ
                string detailSql = @"
                    SELECT MaDV, SoLuong
                    FROM ChiTietDichVu
                    WHERE MaHD = @MaHD";

                DataTable details = new DataTable();

                using (SqlCommand cmd =
                    new SqlCommand(detailSql, conn, trans))
                using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                {
                    cmd.Parameters.Add("@MaHD", SqlDbType.Int).Value = maHD;
                    adapter.Fill(details);
                }

                foreach (DataRow row in details.Rows)
                {
                    int maDV = Convert.ToInt32(row["MaDV"]);
                    int soLuong = Convert.ToInt32(row["SoLuong"]);

                    string stockSql = @"
                        UPDATE DichVu
                        SET TonKho = TonKho - @SoLuong
                        WHERE MaDV = @MaDV
                          AND TonKho >= @SoLuong";

                    using SqlCommand stockCmd =
                        new SqlCommand(stockSql, conn, trans);

                    stockCmd.Parameters.Add("@MaDV", SqlDbType.Int)
                        .Value = maDV;

                    stockCmd.Parameters.Add("@SoLuong", SqlDbType.Int)
                        .Value = soLuong;

                    int affected = stockCmd.ExecuteNonQuery();

                    if (affected != 1)
                    {
                        throw new Exception(
                            "Dịch vụ mã " + maDV +
                            " không đủ tồn kho. Giao dịch đã bị hủy.");
                    }
                }

                // 5. Cập nhật hóa đơn
                string updateInvoiceSql = @"
                    UPDATE HoaDon
                    SET GioRa = @GioRa,
                        TongTienPhong = @TongTienPhong,
                        TongTienDichVu = @TongTienDichVu,
                        TrangThai = 1
                    WHERE MaHD = @MaHD
                      AND TrangThai = 0";

                using (SqlCommand cmd =
                    new SqlCommand(updateInvoiceSql, conn, trans))
                {
                    cmd.Parameters.Add("@MaHD", SqlDbType.Int).Value = maHD;
                    cmd.Parameters.Add("@GioRa", SqlDbType.DateTime).Value = gioRa;

                    SqlParameter roomMoney =
                        cmd.Parameters.Add("@TongTienPhong", SqlDbType.Decimal);

                    roomMoney.Precision = 18;
                    roomMoney.Scale = 2;
                    roomMoney.Value = tienPhong;

                    SqlParameter serviceMoney =
                        cmd.Parameters.Add("@TongTienDichVu", SqlDbType.Decimal);

                    serviceMoney.Precision = 18;
                    serviceMoney.Scale = 2;
                    serviceMoney.Value = tienDichVu;

                    if (cmd.ExecuteNonQuery() != 1)
                        throw new Exception("Không thể cập nhật hóa đơn.");
                }

                // 6. Trả phòng
                string updateRoomSql = @"
                    UPDATE PhongHat
                    SET TrangThai = N'Trống'
                    WHERE MaPhong = @MaPhong
                      AND TrangThai = N'Đang hát'";

                using (SqlCommand cmd =
                    new SqlCommand(updateRoomSql, conn, trans))
                {
                    cmd.Parameters.Add("@MaPhong", SqlDbType.Int)
                        .Value = maPhong;

                    if (cmd.ExecuteNonQuery() != 1)
                        throw new Exception("Không thể cập nhật trạng thái phòng.");
                }

                // 7. Hoàn tất toàn bộ giao dịch
                trans.Commit();

                return tienPhong + tienDichVu;
            }
            catch
            {
                try
                {
                    trans.Rollback();
                }
                catch
                {
                    // Giao dịch có thể đã kết thúc
                }

                throw;
            }
        }
    }
}
