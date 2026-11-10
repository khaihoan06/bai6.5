
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using KaraokeManagement.BUS;
using KaraokeManagement.DTO;

namespace KaraokeManagement.GUI
{
    public class FrmPhongHat : Form
    {
        private readonly PhongHatBUS phongBUS = new PhongHatBUS();

        private DataGridView dgvPhongHat = null!;

        private TextBox txtMaPhong = null!;
        private TextBox txtTenPhong = null!;
        private TextBox txtLoaiPhong = null!;
        private TextBox txtGiaTheoGio = null!;
        private TextBox txtTimKiem = null!;

        private ComboBox cboTrangThai = null!;

        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;
        private Button btnTimKiem = null!;

        public FrmPhongHat()
        {
            InitializeComponent();
            LoadPhongHat();
        }

        private void InitializeComponent()
        {
            Text = "QUẢN LÝ PHÒNG HÁT";
            Size = new Size(1100, 720);
            MinimumSize = new Size(950, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            // TIÊU ĐỀ
            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ PHÒNG HÁT KARAOKE",
                Dock = DockStyle.Top,
                Height = 65,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(35, 45, 65)
            };

            // THÔNG TIN PHÒNG
            Panel pnlThongTin = new Panel
            {
                Dock = DockStyle.Top,
                Height = 215,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            Label lblMa = CreateLabel("Mã phòng:", 20, 15);
            txtMaPhong = CreateTextBox(20, 45, 220);
            txtMaPhong.ReadOnly = true;
            txtMaPhong.BackColor = Color.FromArgb(235, 235, 235);

            Label lblTen = CreateLabel("Tên phòng:", 280, 15);
            txtTenPhong = CreateTextBox(280, 45, 300);

            Label lblLoai = CreateLabel("Loại phòng:", 620, 15);
            txtLoaiPhong = CreateTextBox(620, 45, 200);

            Label lblGia = CreateLabel("Giá theo giờ:", 20, 105);
            txtGiaTheoGio = CreateTextBox(20, 135, 220);

            Label lblTrangThai = CreateLabel("Trạng thái:", 280, 105);

            cboTrangThai = new ComboBox
            {
                Location = new Point(280, 135),
                Width = 220,
                Height = 30,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            cboTrangThai.Items.Add("Trống");
            cboTrangThai.Items.Add("Đang hát");
            cboTrangThai.SelectedIndex = 0;

            pnlThongTin.Controls.Add(lblMa);
            pnlThongTin.Controls.Add(txtMaPhong);
            pnlThongTin.Controls.Add(lblTen);
            pnlThongTin.Controls.Add(txtTenPhong);
            pnlThongTin.Controls.Add(lblLoai);
            pnlThongTin.Controls.Add(txtLoaiPhong);
            pnlThongTin.Controls.Add(lblGia);
            pnlThongTin.Controls.Add(txtGiaTheoGio);
            pnlThongTin.Controls.Add(lblTrangThai);
            pnlThongTin.Controls.Add(cboTrangThai);

            // NÚT CHỨC NĂNG
            Panel pnlChucNang = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                Padding = new Padding(15),
                BackColor = Color.FromArgb(245, 247, 250)
            };

            btnThem = CreateButton("THÊM", Color.FromArgb(39, 174, 96));
            btnSua = CreateButton("SỬA", Color.FromArgb(52, 152, 219));
            btnXoa = CreateButton("XÓA", Color.FromArgb(220, 75, 75));
            btnLamMoi = CreateButton("LÀM MỚI", Color.FromArgb(127, 140, 141));

            btnThem.Location = new Point(15, 10);
            btnSua.Location = new Point(150, 10);
            btnXoa.Location = new Point(285, 10);
            btnLamMoi.Location = new Point(420, 10);

            btnThem.Click += BtnThem_Click;
            btnSua.Click += BtnSua_Click;
            btnXoa.Click += BtnXoa_Click;
            btnLamMoi.Click += BtnLamMoi_Click;

            pnlChucNang.Controls.Add(btnThem);
            pnlChucNang.Controls.Add(btnSua);
            pnlChucNang.Controls.Add(btnXoa);
            pnlChucNang.Controls.Add(btnLamMoi);

            // TÌM KIẾM
            Panel pnlTimKiem = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                Padding = new Padding(15),
                BackColor = Color.White
            };

            Label lblTimKiem = new Label
            {
                Text = "Tìm phòng:",
                Location = new Point(15, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            txtTimKiem = new TextBox
            {
                Location = new Point(125, 15),
                Width = 300,
                Height = 30
            };

            btnTimKiem = CreateButton(
                "TÌM KIẾM",
                Color.FromArgb(41, 128, 185));

            btnTimKiem.Location = new Point(445, 12);
            btnTimKiem.Click += BtnTimKiem_Click;

            txtTimKiem.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    TimKiemPhong();
                    e.SuppressKeyPress = true;
                }
            };

            pnlTimKiem.Controls.Add(lblTimKiem);
            pnlTimKiem.Controls.Add(txtTimKiem);
            pnlTimKiem.Controls.Add(btnTimKiem);

            // BẢNG PHÒNG
            Panel pnlBang = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            Label lblDanhSach = new Label
            {
                Text = "DANH SÁCH PHÒNG HÁT",
                Dock = DockStyle.Top,
                Height = 35,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            dgvPhongHat = new DataGridView
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

            dgvPhongHat.CellClick += DgvPhongHat_CellClick;
            dgvPhongHat.CellFormatting += DgvPhongHat_CellFormatting;

            pnlBang.Controls.Add(dgvPhongHat);
            pnlBang.Controls.Add(lblDanhSach);

            Controls.Add(pnlBang);
            Controls.Add(pnlTimKiem);
            Controls.Add(pnlChucNang);
            Controls.Add(pnlThongTin);
            Controls.Add(lblTitle);
        }

        private Label CreateLabel(string text, int x, int y)
        {
            return new Label
            {
                Text = text,
                Location = new Point(x, y),
                AutoSize = true,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
        }

        private TextBox CreateTextBox(int x, int y, int width)
        {
            return new TextBox
            {
                Location = new Point(x, y),
                Width = width,
                Height = 30,
                Font = new Font("Segoe UI", 10)
            };
        }

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

        // TẢI DANH SÁCH PHÒNG
        private void LoadPhongHat()
        {
            try
            {
                DataTable dt = phongBUS.GetAll();

                dgvPhongHat.DataSource = dt;

                SetHeader("MaPhong", "Mã phòng");
                SetHeader("TenPhong", "Tên phòng");
                SetHeader("LoaiPhong", "Loại phòng");
                SetHeader("GiaTheoGio", "Giá theo giờ");
                SetHeader("TrangThai", "Trạng thái");

                SetFormat("GiaTheoGio", "N0");

                dgvPhongHat.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải danh sách phòng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void SetHeader(string columnName, string header)
        {
            if (dgvPhongHat.Columns[columnName] is DataGridViewColumn column)
                column.HeaderText = header;
        }

        private void SetFormat(string columnName, string format)
        {
            if (dgvPhongHat.Columns[columnName] is DataGridViewColumn column)
                column.DefaultCellStyle.Format = format;
        }

        // ĐỔI MÀU TRẠNG THÁI
        private void DgvPhongHat_CellFormatting(
            object? sender,
            DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvPhongHat.Columns[e.ColumnIndex].Name != "TrangThai")
                return;

            string trangThai = Convert.ToString(e.Value) ?? "";

            if (trangThai == "Trống")
            {
                e.CellStyle.BackColor = Color.FromArgb(220, 245, 230);
                e.CellStyle.ForeColor = Color.FromArgb(25, 120, 65);
            }
            else if (trangThai == "Đang hát")
            {
                e.CellStyle.BackColor = Color.FromArgb(255, 225, 225);
                e.CellStyle.ForeColor = Color.FromArgb(180, 40, 40);
            }
        }

        // CHỌN PHÒNG TRÊN BẢNG
        private void DgvPhongHat_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvPhongHat.Rows[e.RowIndex];

            txtMaPhong.Text =
                Convert.ToString(row.Cells["MaPhong"].Value) ?? "";

            txtTenPhong.Text =
                Convert.ToString(row.Cells["TenPhong"].Value) ?? "";

            txtLoaiPhong.Text =
                Convert.ToString(row.Cells["LoaiPhong"].Value) ?? "";

            txtGiaTheoGio.Text =
                Convert.ToString(row.Cells["GiaTheoGio"].Value) ?? "";

            string trangThai =
                Convert.ToString(row.Cells["TrangThai"].Value) ?? "Trống";

            cboTrangThai.SelectedItem = trangThai;
        }

        // KIỂM TRA DỮ LIỆU
        private bool ValidateInput(
            out string tenPhong,
            out string loaiPhong,
            out decimal giaTheoGio)
        {
            tenPhong = txtTenPhong.Text.Trim();
            loaiPhong = txtLoaiPhong.Text.Trim();
            giaTheoGio = 0;

            if (string.IsNullOrWhiteSpace(tenPhong))
            {
                MessageBox.Show("Vui lòng nhập tên phòng.");
                txtTenPhong.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(loaiPhong))
            {
                MessageBox.Show("Vui lòng nhập loại phòng.");
                txtLoaiPhong.Focus();
                return false;
            }

            if (!decimal.TryParse(
                txtGiaTheoGio.Text.Trim(),
                out giaTheoGio) || giaTheoGio < 0)
            {
                MessageBox.Show("Giá theo giờ phải là số không âm.");
                txtGiaTheoGio.Focus();
                return false;
            }

            return true;
        }

        // THÊM PHÒNG
        private void BtnThem_Click(object? sender, EventArgs e)
        {
            if (!ValidateInput(
                out string tenPhong,
                out string loaiPhong,
                out decimal giaTheoGio))
                return;

            try
            {
                PhongHatDTO dto = new PhongHatDTO
                {
                    TenPhong = tenPhong,
                    LoaiPhong = loaiPhong,
                    GiaTheoGio = giaTheoGio,
                    TrangThai = "Trống"
                };

                int result = phongBUS.Insert(dto);

                if (result > 0)
                {
                    MessageBox.Show("Thêm phòng thành công!");

                    LoadPhongHat();
                    ClearInput();
                }
                else
                {
                    MessageBox.Show("Không thể thêm phòng.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi thêm phòng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // SỬA PHÒNG
        private void BtnSua_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaPhong.Text, out int maPhong))
            {
                MessageBox.Show("Vui lòng chọn phòng cần sửa.");
                return;
            }

            if (!ValidateInput(
                out string tenPhong,
                out string loaiPhong,
                out decimal giaTheoGio))
                return;

            try
            {
                PhongHatDTO dto = new PhongHatDTO
                {
                    MaPhong = maPhong,
                    TenPhong = tenPhong,
                    LoaiPhong = loaiPhong,
                    GiaTheoGio = giaTheoGio,
                    TrangThai = "Trống"
                };

                int result = phongBUS.Update(dto);

                if (result > 0)
                {
                    MessageBox.Show("Cập nhật phòng thành công!");

                    LoadPhongHat();
                    ClearInput();
                }
                else
                {
                    MessageBox.Show(
                        "Không thể sửa phòng. " +
                        "Phòng có thể đang được sử dụng.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi sửa phòng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // XÓA PHÒNG
        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaPhong.Text, out int maPhong))
            {
                MessageBox.Show("Vui lòng chọn phòng cần xóa.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa phòng này không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                int result = phongBUS.Delete(maPhong);

                if (result > 0)
                {
                    MessageBox.Show("Xóa phòng thành công!");

                    LoadPhongHat();
                    ClearInput();
                }
                else
                {
                    MessageBox.Show(
                        "Không thể xóa phòng này. " +
                        "Phòng có thể đang hoạt động hoặc đã có hóa đơn.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi xóa phòng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // TÌM KIẾM
        private void BtnTimKiem_Click(object? sender, EventArgs e)
        {
            TimKiemPhong();
        }

        private void TimKiemPhong()
        {
            try
            {
                string tuKhoa = txtTimKiem.Text.Trim();

                DataTable dt = phongBUS.GetAll();

                if (!string.IsNullOrWhiteSpace(tuKhoa))
                {
                    DataView view = dt.DefaultView;

                    string tuKhoaAnToan = tuKhoa.Replace("'", "''");

                    view.RowFilter =
                        $"Convert(MaPhong, 'System.String') LIKE '%{tuKhoaAnToan}%' " +
                        $"OR TenPhong LIKE '%{tuKhoaAnToan}%'";

                    dgvPhongHat.DataSource = view;
                }
                else
                {
                    dgvPhongHat.DataSource = dt;
                }

                SetHeader("MaPhong", "Mã phòng");
                SetHeader("TenPhong", "Tên phòng");
                SetHeader("LoaiPhong", "Loại phòng");
                SetHeader("GiaTheoGio", "Giá theo giờ");
                SetHeader("TrangThai", "Trạng thái");

                SetFormat("GiaTheoGio", "N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tìm kiếm phòng:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // LÀM MỚI
        private void BtnLamMoi_Click(object? sender, EventArgs e)
        {
            ClearInput();
            txtTimKiem.Clear();
            LoadPhongHat();
        }

        private void ClearInput()
        {
            txtMaPhong.Clear();
            txtTenPhong.Clear();
            txtLoaiPhong.Clear();
            txtGiaTheoGio.Clear();

            cboTrangThai.SelectedItem = "Trống";

            dgvPhongHat.ClearSelection();

            txtTenPhong.Focus();
        }
    }
}
