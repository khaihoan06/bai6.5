
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using KaraokeManagement.DTO;

namespace KaraokeManagement.DAL
{
    public class PhongHatDAL
    {
        public DataTable GetAll()
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                SELECT MaPhong, TenPhong, LoaiPhong,
                       GiaTheoGio, TrangThai
                FROM PhongHat
                ORDER BY MaPhong";

            using SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        public DataTable GetById(int maPhong)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                SELECT *
                FROM PhongHat
                WHERE MaPhong = @MaPhong";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@MaPhong", SqlDbType.Int).Value = maPhong;

            using SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        public int Insert(PhongHatDTO phong)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                INSERT INTO PhongHat
                    (TenPhong, LoaiPhong, GiaTheoGio, TrangThai)
                VALUES
                    (@TenPhong, @LoaiPhong, @GiaTheoGio, @TrangThai)";

            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@TenPhong", SqlDbType.NVarChar, 50)
                .Value = phong.TenPhong;

            cmd.Parameters.Add("@LoaiPhong", SqlDbType.NVarChar, 30)
                .Value = string.IsNullOrWhiteSpace(phong.LoaiPhong)
                    ? DBNull.Value : phong.LoaiPhong;

            cmd.Parameters.Add("@GiaTheoGio", SqlDbType.Decimal)
                .Value = phong.GiaTheoGio;

            cmd.Parameters.Add("@TrangThai", SqlDbType.NVarChar, 20)
                .Value = "Trống";

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int Update(PhongHatDTO phong)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                UPDATE PhongHat
                SET TenPhong = @TenPhong,
                    LoaiPhong = @LoaiPhong,
                    GiaTheoGio = @GiaTheoGio
                WHERE MaPhong = @MaPhong
                  AND TrangThai = N'Trống'";

            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@MaPhong", SqlDbType.Int)
                .Value = phong.MaPhong;

            cmd.Parameters.Add("@TenPhong", SqlDbType.NVarChar, 50)
                .Value = phong.TenPhong;

            cmd.Parameters.Add("@LoaiPhong", SqlDbType.NVarChar, 30)
                .Value = string.IsNullOrWhiteSpace(phong.LoaiPhong)
                    ? DBNull.Value : phong.LoaiPhong;

            SqlParameter price = cmd.Parameters.Add(
                "@GiaTheoGio", SqlDbType.Decimal);

            price.Precision = 18;
            price.Scale = 2;
            price.Value = phong.GiaTheoGio;

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int Delete(int maPhong)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                DELETE FROM PhongHat
                WHERE MaPhong = @MaPhong
                  AND TrangThai = N'Trống'
                  AND NOT EXISTS
                  (
                      SELECT 1 FROM HoaDon
                      WHERE HoaDon.MaPhong = PhongHat.MaPhong
                  )";

            using SqlCommand cmd = new SqlCommand(sql, conn);
            cmd.Parameters.Add("@MaPhong", SqlDbType.Int).Value = maPhong;

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        // Mở phòng và tạo hóa đơn trong cùng transaction
        public int OpenRoom(int maPhong)
        {
            using SqlConnection conn = DBConnection.GetConnection();
            conn.Open();

            using SqlTransaction trans =
                conn.BeginTransaction(IsolationLevel.Serializable);

            try
            {
                string checkSql = @"
                    SELECT TrangThai
                    FROM PhongHat WITH (UPDLOCK, HOLDLOCK)
                    WHERE MaPhong = @MaPhong";

                using SqlCommand checkCmd =
                    new SqlCommand(checkSql, conn, trans);

                checkCmd.Parameters.Add("@MaPhong", SqlDbType.Int)
                    .Value = maPhong;

                object? status = checkCmd.ExecuteScalar();

                if (status == null ||
                    status == DBNull.Value ||
                    Convert.ToString(status) != "Trống")
                {
                    throw new Exception("Phòng không tồn tại hoặc đang được sử dụng.");
                }

                string insertSql = @"
                    INSERT INTO HoaDon
                        (MaPhong, GioVao, TongTienPhong,
                         TongTienDichVu, TrangThai)
                    VALUES
                        (@MaPhong, GETDATE(), 0, 0, 0);

                    SELECT CAST(SCOPE_IDENTITY() AS INT);";

                using SqlCommand insertCmd =
                    new SqlCommand(insertSql, conn, trans);

                insertCmd.Parameters.Add("@MaPhong", SqlDbType.Int)
                    .Value = maPhong;

                int maHD = Convert.ToInt32(insertCmd.ExecuteScalar());

                string updateSql = @"
                    UPDATE PhongHat
                    SET TrangThai = N'Đang hát'
                    WHERE MaPhong = @MaPhong
                      AND TrangThai = N'Trống'";

                using SqlCommand updateCmd =
                    new SqlCommand(updateSql, conn, trans);

                updateCmd.Parameters.Add("@MaPhong", SqlDbType.Int)
                    .Value = maPhong;

                if (updateCmd.ExecuteNonQuery() != 1)
                    throw new Exception("Không thể cập nhật trạng thái phòng.");

                trans.Commit();
                return maHD;
            }
            catch
            {
                try { trans.Rollback(); } catch { }
                throw;
            }
        }
    }
}
