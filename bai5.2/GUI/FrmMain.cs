
using System;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using KaraokeManagement.BUS;

namespace KaraokeManagement.GUI
{
    public class FrmMain : Form
    {
        private readonly PhongHatBUS phongBUS = new PhongHatBUS();

        private Panel pnlMenu = null!;
        private Panel pnlHeader = null!;
        private Panel pnlContent = null!;
        private FlowLayoutPanel flpPhongHat = null!;
        private Label lblTitle = null!;
        private Label lblThongKe = null!;

        public FrmMain()
        {
            InitializeComponent();
            LoadPhongHat();
        }

        private void InitializeComponent()
        {
            Text = "KARAOKE MANAGEMENT SYSTEM";
            Size = new Size(1200, 750);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(1000, 650);
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            // MENU
            pnlMenu = new Panel
            {
                Dock = DockStyle.Left,
                Width = 220,
                BackColor = Color.FromArgb(30, 39, 56)
            };

            Label lblLogo = new Label
            {
                Text = "KARAOKE\nMANAGEMENT",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                Dock = DockStyle.Top,
                Height = 110,
                TextAlign = ContentAlignment.MiddleCenter
            };

            Button btnPhong = CreateMenuButton("SƠ ĐỒ PHÒNG");
            Button btnDichVu = CreateMenuButton("QUẢN LÝ DỊCH VỤ");
            Button btnHoaDon = CreateMenuButton("QUẢN LÝ HÓA ĐƠN");
            Button btnRefresh = CreateMenuButton("LÀM MỚI DỮ LIỆU");

            btnPhong.Click += (s, e) => LoadPhongHat();

            btnRefresh.Click += (s, e) => LoadPhongHat();

            btnDichVu.Click += (s, e) =>
            {
                MessageBox.Show(
                    "Chức năng quản lý dịch vụ sẽ được xây dựng ở phần tiếp theo.",
                    "Thông báo");
            };

            btnHoaDon.Click += (s, e) =>
            {
                MessageBox.Show(
                    "Chức năng quản lý hóa đơn sẽ được xây dựng ở phần tiếp theo.",
                    "Thông báo");
            };

            pnlMenu.Controls.Add(btnRefresh);
            pnlMenu.Controls.Add(btnHoaDon);
            pnlMenu.Controls.Add(btnDichVu);
            pnlMenu.Controls.Add(btnPhong);
            pnlMenu.Controls.Add(lblLogo);

            // HEADER
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 75,
                BackColor = Color.White
            };

            lblTitle = new Label
            {
                Text = "SƠ ĐỒ PHÒNG HÁT",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 45, 65),
                AutoSize = true,
                Location = new Point(25, 20)
            };

            lblThongKe = new Label
            {
                Text = "Đang tải dữ liệu...",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.DimGray,
                AutoSize = true,
                Location = new Point(700, 28),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblThongKe);

            // CONTENT
            pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            Label lblHuongDan = new Label
            {
                Text = "Danh sách phòng hát - Nhấn vào phòng trống để mở phòng",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                ForeColor = Color.FromArgb(60, 70, 90)
            };

            flpPhongHat = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                WrapContents = true,
                BackColor = Color.FromArgb(245, 247, 250),
                Padding = new Padding(5)
            };

            pnlContent.Controls.Add(flpPhongHat);
            pnlContent.Controls.Add(lblHuongDan);

            Controls.Add(pnlContent);
            Controls.Add(pnlHeader);
            Controls.Add(pnlMenu);
        }

        private Button CreateMenuButton(string text)
        {
            return new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 55,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(42, 54, 75),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(20, 0, 0, 0),
                Cursor = Cursors.Hand
            };
        }

        private void LoadPhongHat()
        {
            try
            {
                DataTable dt = phongBUS.GetAll();

                flpPhongHat.Controls.Clear();

                int phongTrong = 0;
                int phongDangHat = 0;

                foreach (DataRow row in dt.Rows)
                {
                    int maPhong = Convert.ToInt32(row["MaPhong"]);
                    string tenPhong = Convert.ToString(row["TenPhong"]) ?? "";
                    string loaiPhong = Convert.ToString(row["LoaiPhong"]) ?? "";
                    decimal gia = Convert.ToDecimal(row["GiaTheoGio"]);
                    string trangThai = Convert.ToString(row["TrangThai"]) ?? "Trống";

                    bool dangHat = trangThai == "Đang hát";

                    if (dangHat)
                        phongDangHat++;
                    else
                        phongTrong++;

                    Button btn = new Button
                    {
                        Width = 205,
                        Height = 145,
                        Margin = new Padding(12),
                        FlatStyle = FlatStyle.Flat,
                        BackColor = dangHat
                            ? Color.FromArgb(220, 75, 75)
                            : Color.FromArgb(39, 174, 96),
                        ForeColor = Color.White,
                        Font = new Font("Segoe UI", 11, FontStyle.Bold),
                        Text =
                            tenPhong + Environment.NewLine +
                            loaiPhong + Environment.NewLine +
                            gia.ToString("N0") + " đ/giờ" +
                            Environment.NewLine +
                            (dangHat ? "ĐANG HÁT" : "TRỐNG"),
                        Tag = maPhong,
                        Cursor = Cursors.Hand
                    };

                    btn.FlatAppearance.BorderSize = 0;

                    btn.Click += RoomButton_Click;

                    flpPhongHat.Controls.Add(btn);
                }

                lblThongKe.Text =
                    $"Tổng: {dt.Rows.Count} phòng     " +
                    $"Trống: {phongTrong}     " +
                    $"Đang hát: {phongDangHat}";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể tải danh sách phòng!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RoomButton_Click(object? sender, EventArgs e)
        {
            if (sender is not Button btn || btn.Tag == null)
                return;

            int maPhong = Convert.ToInt32(btn.Tag);

            try
            {
                DataTable dt = phongBUS.GetById(maPhong);

                if (dt.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy phòng.");
                    LoadPhongHat();
                    return;
                }

                string trangThai =
                    Convert.ToString(dt.Rows[0]["TrangThai"]) ?? "Trống";

                if (trangThai == "Trống")
                {
                    DialogResult result = MessageBox.Show(
                        "Bạn có muốn mở phòng này không?",
                        "Xác nhận mở phòng",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (result == DialogResult.Yes)
                    {
                        int maHD = phongBUS.OpenRoom(maPhong);

                        MessageBox.Show(
                            "Mở phòng thành công!\nMã hóa đơn: " + maHD,
                            "Thông báo",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

                        LoadPhongHat();
                    }
                }
                else
                {
                    MessageBox.Show(
                        "Phòng đang có khách.\n" +
                        "Chức năng gọi dịch vụ và thanh toán sẽ được bổ sung ở phần tiếp theo.",
                        "Thông tin phòng",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể xử lý phòng!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                LoadPhongHat();
            }
        }
    }
}
