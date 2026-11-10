
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using KaraokeManagement.BUS;

namespace KaraokeManagement.GUI
{
    public class FrmBaoCao : Form
    {
        private readonly HoaDonBUS hoaDonBUS = new HoaDonBUS();
        private readonly PhongHatBUS phongHatBUS = new PhongHatBUS();

        private DateTimePicker dtpTuNgay = null!;
        private DateTimePicker dtpDenNgay = null!;

        private DataGridView dgvBaoCao = null!;

        private Label lblTongDoanhThu = null!;
        private Label lblTienPhong = null!;
        private Label lblTienDichVu = null!;
        private Label lblSoHoaDon = null!;
        private Label lblPhongTrong = null!;
        private Label lblPhongDangHat = null!;

        private Panel pnlThongKe = null!;

        private Button btnThongKe = null!;
        private Button btnHomNay = null!;
        private Button btnLamMoi = null!;

        private DataTable dtHoaDon = new DataTable();

        public FrmBaoCao()
        {
            InitializeComponent();
            LoadBaoCao();
        }

        private void InitializeComponent()
        {
            Text = "BÁO CÁO VÀ THỐNG KÊ KARAOKE";
            Size = new Size(1200, 780);
            MinimumSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            // TIÊU ĐỀ
            Label lblTitle = new Label
            {
                Text = "BÁO CÁO VÀ THỐNG KÊ DOANH THU",
                Dock = DockStyle.Top,
                Height = 65,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(35, 45, 65)
            };

            // BỘ LỌC NGÀY
            Panel pnlLoc = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                Padding = new Padding(15),
                BackColor = Color.White
            };

            Label lblTuNgay = new Label
            {
                Text = "Từ ngày:",
                Location = new Point(20, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            dtpTuNgay = new DateTimePicker
            {
                Location = new Point(20, 45),
                Width = 190,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };

            Label lblDenNgay = new Label
            {
                Text = "Đến ngày:",
                Location = new Point(240, 18),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            dtpDenNgay = new DateTimePicker
            {
                Location = new Point(240, 45),
                Width = 190,
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };

            dtpTuNgay.Value = DateTime.Today;
            dtpDenNgay.Value = DateTime.Today;

            btnThongKe = CreateButton(
                "THỐNG KÊ",
                Color.FromArgb(41, 128, 185));

            btnThongKe.Location = new Point(460, 42);
            btnThongKe.Click += BtnThongKe_Click;

            btnHomNay = CreateButton(
                "HÔM NAY",
                Color.FromArgb(39, 174, 96));

            btnHomNay.Location = new Point(600, 42);
            btnHomNay.Click += BtnHomNay_Click;

            btnLamMoi = CreateButton(
                "LÀM MỚI",
                Color.FromArgb(127, 140, 141));

            btnLamMoi.Location = new Point(740, 42);
            btnLamMoi.Click += BtnLamMoi_Click;

            pnlLoc.Controls.Add(lblTuNgay);
            pnlLoc.Controls.Add(dtpTuNgay);
            pnlLoc.Controls.Add(lblDenNgay);
            pnlLoc.Controls.Add(dtpDenNgay);
            pnlLoc.Controls.Add(btnThongKe);
            pnlLoc.Controls.Add(btnHomNay);
            pnlLoc.Controls.Add(btnLamMoi);

            // KHU VỰC THỐNG KÊ
            pnlThongKe = new Panel
            {
                Dock = DockStyle.Top,
                Height = 190,
                Padding = new Padding(15),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            Panel cardDoanhThu = CreateCard(
                "TỔNG DOANH THU",
                "0 đ",
                Color.FromArgb(39, 174, 96),
                15,
                15,
                out lblTongDoanhThu);

            Panel cardTienPhong = CreateCard(
                "DOANH THU PHÒNG",
                "0 đ",
                Color.FromArgb(52, 152, 219),
                300,
                15,
                out lblTienPhong);

            Panel cardTienDichVu = CreateCard(
                "DOANH THU DỊCH VỤ",
                "0 đ",
                Color.FromArgb(155, 89, 182),
                585,
                15,
                out lblTienDichVu);

            Panel cardSoHoaDon = CreateCard(
                "SỐ HÓA ĐƠN",
                "0",
                Color.FromArgb(230, 126, 34),
                870,
                15,
                out lblSoHoaDon);

            Panel cardPhongTrong = CreateCard(
                "PHÒNG TRỐNG",
                "0",
                Color.FromArgb(39, 174, 96),
                15,
                100,
                out lblPhongTrong);

            Panel cardPhongDangHat = CreateCard(
                "PHÒNG ĐANG HÁT",
                "0",
                Color.FromArgb(220, 75, 75),
                300,
                100,
                out lblPhongDangHat);

            // THÊM CÁC THẺ VÀO PANEL
            pnlThongKe.Controls.Add(cardDoanhThu);
            pnlThongKe.Controls.Add(cardTienPhong);
            pnlThongKe.Controls.Add(cardTienDichVu);
            pnlThongKe.Controls.Add(cardSoHoaDon);
            pnlThongKe.Controls.Add(cardPhongTrong);
            pnlThongKe.Controls.Add(cardPhongDangHat);

            // BẢNG BÁO CÁO
            Panel pnlBang = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            Label lblDanhSach = new Label
            {
                Text = "DANH SÁCH HÓA ĐƠN ĐÃ THANH TOÁN",
                Dock = DockStyle.Top,
                Height = 40,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            dgvBaoCao = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoGenerateColumns = true,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                RowTemplate = { Height = 35 }
            };

            pnlBang.Controls.Add(dgvBaoCao);
            pnlBang.Controls.Add(lblDanhSach);

            // THỨ TỰ DOCK
            Controls.Add(pnlBang);
            Controls.Add(pnlThongKe);
            Controls.Add(pnlLoc);
            Controls.Add(lblTitle);
        }

        // TẠO NÚT
        private Button CreateButton(string text, Color color)
        {
            return new Button
            {
                Text = text,
                Size = new Size(120, 40),
                BackColor = color,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
        }

        // TẠO THẺ THỐNG KÊ
        private Panel CreateCard(
            string title,
            string value,
            Color color,
            int x,
            int y,
            out Label lblValue)
        {
            Panel card = new Panel
            {
                Location = new Point(x, y),
                Size = new Size(270, 75),
                BackColor = color
            };

            Label lblTitle = new Label
            {
                Text = title,
                Location = new Point(12, 8),
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            lblValue = new Label
            {
                Text = value,
                Location = new Point(12, 32),
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 16, FontStyle.Bold)
            };

            card.Controls.Add(lblTitle);
            card.Controls.Add(lblValue);

            return card;
        }

        // TẢI DỮ LIỆU BÁO CÁO
        private void LoadBaoCao()
        {
            try
            {
                dtHoaDon = hoaDonBUS.GetAll();

                ThongKeDoanhThu();
                ThongKePhong();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải báo cáo:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // THỐNG KÊ DOANH THU
        private void ThongKeDoanhThu()
        {
            try
            {
                DateTime tuNgay = dtpTuNgay.Value.Date;
                DateTime denNgay = dtpDenNgay.Value.Date;

                if (tuNgay > denNgay)
                {
                    MessageBox.Show(
                        "Ngày bắt đầu không được lớn hơn ngày kết thúc.");

                    return;
                }

                decimal tongTienPhong = 0;
                decimal tongTienDichVu = 0;
                decimal tongDoanhThu = 0;

                int soHoaDon = 0;

                DataTable dtKetQua = dtHoaDon.Clone();

                foreach (DataRow row in dtHoaDon.Rows)
                {
                    if (row["TrangThai"] == DBNull.Value)
                        continue;

                    bool daThanhToan =
                        Convert.ToBoolean(row["TrangThai"]);

                    if (!daThanhToan)
                        continue;

                    if (row["GioRa"] == DBNull.Value)
                        continue;

                    DateTime gioRa =
                        Convert.ToDateTime(row["GioRa"]);

                    // Lọc theo ngày thanh toán
                    if (gioRa.Date < tuNgay ||
                        gioRa.Date > denNgay)
                        continue;

                    decimal tienPhong =
                        row["TongTienPhong"] == DBNull.Value
                        ? 0
                        : Convert.ToDecimal(row["TongTienPhong"]);

                    decimal tienDichVu =
                        row["TongTienDichVu"] == DBNull.Value
                        ? 0
                        : Convert.ToDecimal(row["TongTienDichVu"]);

                    decimal tongTien =
                        row["TongTien"] == DBNull.Value
                        ? tienPhong + tienDichVu
                        : Convert.ToDecimal(row["TongTien"]);

                    tongTienPhong += tienPhong;
                    tongTienDichVu += tienDichVu;
                    tongDoanhThu += tongTien;

                    soHoaDon++;

                    dtKetQua.ImportRow(row);
                }

                // CẬP NHẬT THẺ THỐNG KÊ
                lblTongDoanhThu.Text =
                    tongDoanhThu.ToString("N0") + " đ";

                lblTienPhong.Text =
                    tongTienPhong.ToString("N0") + " đ";

                lblTienDichVu.Text =
                    tongTienDichVu.ToString("N0") + " đ";

                lblSoHoaDon.Text =
                    soHoaDon.ToString();

                // HIỂN THỊ BẢNG
                dgvBaoCao.DataSource = dtKetQua;

                SetHeader("MaHD", "Mã hóa đơn");
                SetHeader("MaPhong", "Mã phòng");
                SetHeader("TenPhong", "Tên phòng");
                SetHeader("GioVao", "Giờ vào");
                SetHeader("GioRa", "Giờ ra");
                SetHeader("TongTienPhong", "Tiền phòng");
                SetHeader("TongTienDichVu", "Tiền dịch vụ");
                SetHeader("TongTien", "Tổng tiền");
                SetHeader("TrangThai", "Trạng thái");

                SetFormat("TongTienPhong", "N0");
                SetFormat("TongTienDichVu", "N0");
                SetFormat("TongTien", "N0");

                SetFormat("GioVao", "dd/MM/yyyy HH:mm");
                SetFormat("GioRa", "dd/MM/yyyy HH:mm");

                dgvBaoCao.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi thống kê doanh thu:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // THỐNG KÊ SỐ PHÒNG
        private void ThongKePhong()
        {
            try
            {
                DataTable dtPhong = phongHatBUS.GetAll();

                int trong = 0;
                int dangHat = 0;

                foreach (DataRow row in dtPhong.Rows)
                {
                    string trangThai =
                        Convert.ToString(row["TrangThai"]) ?? "";

                    if (trangThai == "Trống")
                    {
                        trong++;
                    }
                    else if (trangThai == "Đang hát")
                    {
                        dangHat++;
                    }
                }

                lblPhongTrong.Text = trong.ToString();
                lblPhongDangHat.Text = dangHat.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi thống kê phòng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // ĐỔI TÊN CỘT
        private void SetHeader(string columnName, string header)
        {
            if (dgvBaoCao.Columns[columnName] is DataGridViewColumn column)
            {
                column.HeaderText = header;
            }
        }

        // ĐỊNH DẠNG CỘT
        private void SetFormat(string columnName, string format)
        {
            if (dgvBaoCao.Columns[columnName] is DataGridViewColumn column)
            {
                column.DefaultCellStyle.Format = format;
            }
        }

        // NÚT THỐNG KÊ
        private void BtnThongKe_Click(object? sender, EventArgs e)
        {
            ThongKeDoanhThu();
            ThongKePhong();
        }

        // NÚT HÔM NAY
        private void BtnHomNay_Click(object? sender, EventArgs e)
        {
            dtpTuNgay.Value = DateTime.Today;
            dtpDenNgay.Value = DateTime.Today;

            ThongKeDoanhThu();
            ThongKePhong();
        }

        // NÚT LÀM MỚI
        private void BtnLamMoi_Click(object? sender, EventArgs e)
        {
            LoadBaoCao();
        }
    }
}
