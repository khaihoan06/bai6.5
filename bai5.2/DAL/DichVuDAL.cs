
using System;
using System.Data;
using Microsoft.Data.SqlClient;
using KaraokeManagement.DTO;

namespace KaraokeManagement.DAL
{
    public class DichVuDAL
    {
        public DataTable GetAll()
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                SELECT MaDV, TenDV, DonGia, TonKho
                FROM DichVu
                ORDER BY MaDV";

            using SqlDataAdapter adapter = new SqlDataAdapter(sql, conn);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        public DataTable GetById(int maDV)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                SELECT *
                FROM DichVu
                WHERE MaDV = @MaDV";

            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@MaDV", SqlDbType.Int).Value = maDV;

            using SqlDataAdapter adapter = new SqlDataAdapter(cmd);

            DataTable dt = new DataTable();
            adapter.Fill(dt);

            return dt;
        }

        public int Insert(DichVuDTO dv)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                INSERT INTO DichVu (TenDV, DonGia, TonKho)
                VALUES (@TenDV, @DonGia, @TonKho)";

            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@TenDV", SqlDbType.NVarChar, 100)
                .Value = dv.TenDV;

            SqlParameter price =
                cmd.Parameters.Add("@DonGia", SqlDbType.Decimal);

            price.Precision = 18;
            price.Scale = 2;
            price.Value = dv.DonGia;

            cmd.Parameters.Add("@TonKho", SqlDbType.Int)
                .Value = dv.TonKho;

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int Update(DichVuDTO dv)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                UPDATE DichVu
                SET TenDV = @TenDV,
                    DonGia = @DonGia,
                    TonKho = @TonKho
                WHERE MaDV = @MaDV";

            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@MaDV", SqlDbType.Int).Value = dv.MaDV;

            cmd.Parameters.Add("@TenDV", SqlDbType.NVarChar, 100)
                .Value = dv.TenDV;

            SqlParameter price =
                cmd.Parameters.Add("@DonGia", SqlDbType.Decimal);

            price.Precision = 18;
            price.Scale = 2;
            price.Value = dv.DonGia;

            cmd.Parameters.Add("@TonKho", SqlDbType.Int)
                .Value = dv.TonKho;

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int Delete(int maDV)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                DELETE FROM DichVu
                WHERE MaDV = @MaDV
                  AND NOT EXISTS
                  (
                      SELECT 1 FROM ChiTietDichVu
                      WHERE ChiTietDichVu.MaDV = DichVu.MaDV
                  )";

            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@MaDV", SqlDbType.Int).Value = maDV;

            conn.Open();
            return cmd.ExecuteNonQuery();
        }

        public int UpdateStock(int maDV, int soLuong)
        {
            using SqlConnection conn = DBConnection.GetConnection();

            string sql = @"
                UPDATE DichVu
                SET TonKho = TonKho + @SoLuong
                WHERE MaDV = @MaDV
                  AND TonKho + @SoLuong >= 0";

            using SqlCommand cmd = new SqlCommand(sql, conn);

            cmd.Parameters.Add("@MaDV", SqlDbType.Int).Value = maDV;
            cmd.Parameters.Add("@SoLuong", SqlDbType.Int).Value = soLuong;

            conn.Open();
            return cmd.ExecuteNonQuery();
        }
    }
}
