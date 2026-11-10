
using System;
using System.Windows.Forms;

namespace KaraokeManagement.DAL
{
    public class TestConnection
    {
        public static void ShowConnectionResult()
        {
            bool connected = DBConnection.TestConnection();

            if (connected)
            {
                MessageBox.Show(
                    "KẾT NỐI SQL SERVER THÀNH CÔNG!\n" +
                    "Database: KaraokeDB",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    "KẾT NỐI SQL SERVER THẤT BẠI!\n" +
                    "Hãy kiểm tra tên Server và database KaraokeDB.",
                    "Lỗi kết nối",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
