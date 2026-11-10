
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using KaraokeManagement.BUS;

namespace KaraokeManagement.GUI
{
    public class FrmThanhToan : Form
    {
        private readonly HoaDonBUS hoaDonBUS = new HoaDonBUS();
        private readonly PhongHatBUS phongHatBUS = new PhongHatBUS();

        private readonly int maPhong;
        private readonly int maHD;

        private DateTime gioVao;
        private decimal giaTheoGio;
        private decimal tongTienDichVu;

        private Label lblPhong = null!;
        private Label lblHoaDon = null!;
        private Label lblGioVao = null!;
        private Label lblGioHienTai = null!;
        private Label lblThoiGian = null!;
        private Label lblGiaGio = null!;
        private Label lblTienPhong = null!;
        private Label lblTienDichVu = null!;
        private Label lblTongTien = null!;

        private DataGridView dgvDichVu = null!;
        private Button btnThanhToan = null!;
        private Button btnDong = null!;

        private System.Windows.Forms.Timer timer = null!;

        public FrmThanhToan(int maPhong, int maHD)
        {
            this.maPhong = maPhong;
            this.maHD = maHD;

            InitializeComponent();
            LoadThongTinThanhToan();
        }

        private void InitializeComponent()
        {
            Text = "THANH TOÁN HÓA ĐƠN";
            Size = new Size(900, 700);
            MinimumSize = new Size(800, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            Label lblTitle = new Label
            {
                Text = "THANH TOÁN KARAOKE",
                Dock = DockStyle.Top,
                Height = 65,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(35, 45, 65)
            };

            Panel pnlThongTin = new Panel
            {
                Dock = DockStyle.Top,
                Height = 190,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            lblPhong = CreateInfoLabel("Phòng:", 20, 15);
            lblHoaDon = CreateInfoLabel("Mã hóa đơn:", 450, 15);
            lblGioVao = CreateInfoLabel("Giờ vào:", 20, 55);
            lblGioHienTai = CreateInfoLabel("Giờ hiện tại:", 450, 55);
            lblThoiGian = CreateInfoLabel("Thời gian hát:", 20, 95);
            lblGiaGio = CreateInfoLabel("Giá theo giờ:", 450, 95);

            pnlThongTin.Controls.Add(lblPhong);
            pnlThongTin.Controls.Add(lblHoaDon);
            pnlThongTin.Controls.Add(lblGioVao);
            pnlThongTin.Controls.Add(lblGioHienTai);
            pnlThongTin.Controls.Add(lblThoiGian);
            pnlThongTin.Controls.Add(lblGiaGio);

            Panel pnlDichVu = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20)
            };

            Label lblTitleDichVu = new Label
            {
                Text = "CHI TIẾT DỊCH VỤ",
                Dock = DockStyle.Top,
                Height = 35,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            dgvDichVu = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White
            };

            pnlDichVu.Controls.Add(dgvDichVu);
            pnlDichVu.Controls.Add(lblTitleDichVu);

            Panel pnlTongKet = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 165,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            lblTienPhong = CreateInfoLabel("Tiền phòng: 0 đ", 20, 10);
            lblTienDichVu = CreateInfoLabel("Tiền dịch vụ: 0 đ", 20, 45);

            lblTongTien = new Label
            {
                Text = "TỔNG THANH TOÁN: 0 đ",
                Location = new Point(20, 85),
                AutoSize = true,
                Font = new Font("Segoe UI", 17, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 75, 75)
            };

            btnThanhToan = new Button
            {
                Text = "XÁC NHẬN THANH TOÁN",
                Size = new Size(220, 42),
                Location = new Point(490, 30),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnDong = new Button
            {
                Text = "ĐÓNG",
                Size = new Size(120, 42),
                Location = new Point(720, 30),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnThanhToan.Click += BtnThanhToan_Click;
            btnDong.Click += (s, e) => Close();

            pnlTongKet.Controls.Add(lblTienPhong);
            pnlTongKet.Controls.Add(lblTienDichVu);
            pnlTongKet.Controls.Add(lblTongTien);
            pnlTongKet.Controls.Add(btnThanhToan);
            pnlTongKet.Controls.Add(btnDong);

            Controls.Add(pnlDichVu);
            Controls.Add(pnlTongKet);
            Controls.Add(pnlThongTin);
            Controls.Add(lblTitle);

            timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };

            timer.Tick += (s, e) => CapNhatTien();

            FormClosed += (s, e) => timer.Stop();
        }

        private Label CreateInfoLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 55, 75)
            };
        }

        private void LoadThongTinThanhToan()
        {
            try
            {
                DataTable dtPhong = phongHatBUS.GetById(maPhong);
                DataTable dtHoaDon = hoaDonBUS.GetById(maHD);

                if (dtPhong.Rows.Count == 0)
                    throw new Exception("Không tìm thấy phòng.");

                if (dtHoaDon.Rows.Count == 0)
                    throw new Exception("Không tìm thấy hóa đơn.");

                DataRow rowPhong = dtPhong.Rows[0];
                DataRow rowHoaDon = dtHoaDon.Rows[0];

                if (rowPhong["GiaTheoGio"] == DBNull.Value)
                    throw new Exception("Không tìm thấy giá phòng.");

                if (rowHoaDon["GioVao"] == DBNull.Value)
                    throw new Exception("Không tìm thấy giờ vào.");

                if (Convert.ToBoolean(rowHoaDon["TrangThai"]))
                    throw new Exception("Hóa đơn này đã được thanh toán.");

                giaTheoGio = Convert.ToDecimal(rowPhong["GiaTheoGio"]);
                gioVao = Convert.ToDateTime(rowHoaDon["GioVao"]);

                string tenPhong =
                    Convert.ToString(rowPhong["TenPhong"]) ?? "";

                lblPhong.Text = "Phòng: " + tenPhong;
                lblHoaDon.Text = "Mã hóa đơn: " + maHD;

                lblGioVao.Text =
                    "Giờ vào: " + gioVao.ToString("dd/MM/yyyy HH:mm:ss");

                lblGiaGio.Text =
                    "Giá theo giờ: " + giaTheoGio.ToString("N0") + " đ";

                LoadDanhSachDichVu();
                CapNhatTien();

                timer.Start();
            }
            catch (Exception ex)
            {
                timer.Stop();
                btnThanhToan.Enabled = false;

                MessageBox.Show(
                    "Không thể tải thông tin thanh toán:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadDanhSachDichVu()
        {
            DataTable dt = hoaDonBUS.GetServices(maHD);

            dgvDichVu.DataSource = dt;

            SetHeader("MaHD", "Mã hóa đơn");
            SetHeader("MaDV", "Mã dịch vụ");
            SetHeader("TenDV", "Tên dịch vụ");
            SetHeader("SoLuong", "Số lượng");
            SetHeader("DonGia", "Đơn giá");
            SetHeader("ThanhTien", "Thành tiền");

            SetFormat("DonGia", "N0");
            SetFormat("ThanhTien", "N0");

            tongTienDichVu = 0;

            foreach (DataRow row in dt.Rows)
            {
                if (row["ThanhTien"] != DBNull.Value)
                {
                    tongTienDichVu += Convert.ToDecimal(
                        row["ThanhTien"]);
                }
            }
        }

        private void SetHeader(string columnName, string header)
        {
            if (dgvDichVu.Columns[columnName] is DataGridViewColumn column)
                column.HeaderText = header;
        }

        private void SetFormat(string columnName, string format)
        {
            if (dgvDichVu.Columns[columnName] is DataGridViewColumn column)
                column.DefaultCellStyle.Format = format;
        }

        private void CapNhatTien()
        {
            DateTime gioHienTai = DateTime.Now;

            TimeSpan thoiGian = gioHienTai - gioVao;

            if (thoiGian < TimeSpan.Zero)
                thoiGian = TimeSpan.Zero;

            decimal soGio = (decimal)thoiGian.TotalSeconds / 3600m;

            decimal tienPhong = Math.Round(
                soGio * giaTheoGio,
                0,
                MidpointRounding.AwayFromZero);

            decimal tongTien = tienPhong + tongTienDichVu;

            lblGioHienTai.Text =
                "Giờ hiện tại: " +
                gioHienTai.ToString("dd/MM/yyyy HH:mm:ss");

            lblThoiGian.Text =
                "Thời gian hát: " +
                $"{(int)thoiGian.TotalHours} giờ " +
                $"{thoiGian.Minutes} phút " +
                $"{thoiGian.Seconds} giây";

            lblTienPhong.Text =
                "Tiền phòng: " + tienPhong.ToString("N0") + " đ";

            lblTienDichVu.Text =
                "Tiền dịch vụ: " +
                tongTienDichVu.ToString("N0") + " đ";

            lblTongTien.Text =
                "TỔNG THANH TOÁN: " +
                tongTien.ToString("N0") + " đ";
        }

        private void BtnThanhToan_Click(object? sender, EventArgs e)
        {
            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn thanh toán hóa đơn này không?",
                "Xác nhận thanh toán",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                btnThanhToan.Enabled = false;
                timer.Stop();

                decimal tongTien = hoaDonBUS.Checkout(maHD);

                MessageBox.Show(
                    "THANH TOÁN THÀNH CÔNG!\n\n" +
                    "Mã hóa đơn: " + maHD + "\n" +
                    "Tổng thanh toán: " + tongTien.ToString("N0") + " đ\n\n" +
                    "Phòng đã được trả về trạng thái trống.",
                    "Thông báo",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                btnThanhToan.Enabled = true;
                timer.Start();

                MessageBox.Show(
                    "Thanh toán thất bại:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
