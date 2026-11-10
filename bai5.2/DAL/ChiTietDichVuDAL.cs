
using System;
using System.Data;
using Microsoft.Data.SqlClient;

namespace KaraokeManagement.DAL
{
    public class ChiTietDichVuDAL
    {
        // Lấy danh sách dịch vụ đã gọi của một hóa đơn
        // Sử dụng SqlDataAdapter và DataTable
        public DataTable GetByHoaDon(int maHD)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                SELECT
                    ct.MaHD,
                    ct.MaDV,
                    dv.TenDV,
                    ct.SoLuong,
                    ct.DonGia,
                    (ct.SoLuong * ct.DonGia) AS ThanhTien
                FROM ChiTietDichVu ct
                INNER JOIN DichVu dv
                    ON ct.MaDV = dv.MaDV
                WHERE ct.MaHD = @MaHD
                ORDER BY dv.TenDV";

            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@MaHD", SqlDbType.Int).Value = maHD;

            using SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        // Gọi thêm dịch vụ hoặc tăng số lượng dịch vụ đã gọi
        public int AddService(
            int maHD,
            int maDV,
            int soLuong)
        {
            if (soLuong <= 0)
                throw new ArgumentException("Số lượng phải lớn hơn 0.");

            using SqlConnection conn = DBConnection.GetConnection();
            conn.Open();

            using SqlTransaction trans =
                conn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                // Kiểm tra hóa đơn còn đang mở
                string checkSql = @"
                    SELECT COUNT(*)
                    FROM HoaDon WITH (UPDLOCK, HOLDLOCK)
                    WHERE MaHD = @MaHD
                      AND TrangThai = 0
                      AND GioRa IS NULL";

                using SqlCommand checkCmd =
                    new SqlCommand(checkSql, conn, trans);

                checkCmd.Parameters.Add("@MaHD", SqlDbType.Int).Value = maHD;

                if (Convert.ToInt32(checkCmd.ExecuteScalar()) != 1)
                    throw new Exception("Hóa đơn không tồn tại hoặc đã thanh toán.");

                // Lấy giá hiện tại của dịch vụ
                string priceSql = @"
                    SELECT DonGia
                    FROM DichVu
                    WHERE MaDV = @MaDV";

                using SqlCommand priceCmd =
                    new SqlCommand(priceSql, conn, trans);

                priceCmd.Parameters.Add("@MaDV", SqlDbType.Int).Value = maDV;

                object? priceValue = priceCmd.ExecuteScalar();

                if (priceValue == null || priceValue == DBNull.Value)
                    throw new Exception("Dịch vụ không tồn tại.");

                decimal donGia = Convert.ToDecimal(priceValue);

                // Nếu dịch vụ đã được gọi thì tăng số lượng
                string upsertSql = @"
                    IF EXISTS
                    (
                        SELECT 1
                        FROM ChiTietDichVu WITH (UPDLOCK, HOLDLOCK)
                        WHERE MaHD = @MaHD AND MaDV = @MaDV
                    )
                    BEGIN
                        UPDATE ChiTietDichVu
                        SET SoLuong = SoLuong + @SoLuong
                        WHERE MaHD = @MaHD AND MaDV = @MaDV;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO ChiTietDichVu
                            (MaHD, MaDV, SoLuong, DonGia)
                        VALUES
                            (@MaHD, @MaDV, @SoLuong, @DonGia);
                    END";

                using SqlCommand upsertCmd =
                    new SqlCommand(upsertSql, conn, trans);

                upsertCmd.Parameters.Add("@MaHD", SqlDbType.Int).Value = maHD;
                upsertCmd.Parameters.Add("@MaDV", SqlDbType.Int).Value = maDV;
                upsertCmd.Parameters.Add("@SoLuong", SqlDbType.Int).Value = soLuong;

                SqlParameter priceParameter =
                    upsertCmd.Parameters.Add("@DonGia", SqlDbType.Decimal);

                priceParameter.Precision = 18;
                priceParameter.Scale = 2;
                priceParameter.Value = donGia;

                upsertCmd.ExecuteNonQuery();

                trans.Commit();
                return 1;
            }
            catch
            {
                try { trans.Rollback(); } catch { }
                throw;
            }
        }
    }
}
