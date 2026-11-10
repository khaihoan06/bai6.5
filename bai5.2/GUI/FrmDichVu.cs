
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using KaraokeManagement.BUS;
using KaraokeManagement.DTO;

namespace KaraokeManagement.GUI
{
    public class FrmDichVu : Form
    {
        private readonly DichVuBUS dichVuBUS = new DichVuBUS();

        private DataGridView dgvDichVu = null!;

        private TextBox txtMaDV = null!;
        private TextBox txtTenDV = null!;
        private TextBox txtDonGia = null!;
        private TextBox txtTonKho = null!;
        private TextBox txtTimKiem = null!;

        private Button btnThem = null!;
        private Button btnSua = null!;
        private Button btnXoa = null!;
        private Button btnLamMoi = null!;
        private Button btnTimKiem = null!;

        public FrmDichVu()
        {
            InitializeComponent();
            LoadDichVu();
        }

        private void InitializeComponent()
        {
            Text = "QUẢN LÝ DỊCH VỤ KARAOKE";
            Size = new Size(1100, 720);
            MinimumSize = new Size(950, 620);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            Label lblTitle = new Label
            {
                Text = "QUẢN LÝ DỊCH VỤ",
                Dock = DockStyle.Top,
                Height = 65,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(35, 45, 65)
            };

            // KHU VỰC NHẬP THÔNG TIN
            Panel pnlThongTin = new Panel
            {
                Dock = DockStyle.Top,
                Height = 205,
                Padding = new Padding(20),
                BackColor = Color.White
            };

            Label lblMa = CreateLabel("Mã dịch vụ:", 20, 15);
            txtMaDV = CreateTextBox(20, 45, 220);
            txtMaDV.ReadOnly = true;
            txtMaDV.BackColor = Color.FromArgb(235, 235, 235);

            Label lblTen = CreateLabel("Tên dịch vụ:", 280, 15);
            txtTenDV = CreateTextBox(280, 45, 300);

            Label lblGia = CreateLabel("Đơn giá:", 620, 15);
            txtDonGia = CreateTextBox(620, 45, 200);

            Label lblTon = CreateLabel("Tồn kho:", 20, 100);
            txtTonKho = CreateTextBox(20, 130, 220);

            pnlThongTin.Controls.Add(lblMa);
            pnlThongTin.Controls.Add(txtMaDV);
            pnlThongTin.Controls.Add(lblTen);
            pnlThongTin.Controls.Add(txtTenDV);
            pnlThongTin.Controls.Add(lblGia);
            pnlThongTin.Controls.Add(txtDonGia);
            pnlThongTin.Controls.Add(lblTon);
            pnlThongTin.Controls.Add(txtTonKho);

            // KHU VỰC NÚT CHỨC NĂNG
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

            // KHU VỰC TÌM KIẾM
            Panel pnlTimKiem = new Panel
            {
                Dock = DockStyle.Top,
                Height = 65,
                Padding = new Padding(15),
                BackColor = Color.White
            };

            Label lblTim = new Label
            {
                Text = "Tìm dịch vụ:",
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
                    TimKiemDichVu();
                    e.SuppressKeyPress = true;
                }
            };

            pnlTimKiem.Controls.Add(lblTim);
            pnlTimKiem.Controls.Add(txtTimKiem);
            pnlTimKiem.Controls.Add(btnTimKiem);

            // BẢNG DỮ LIỆU
            Panel pnlBang = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            Label lblDanhSach = new Label
            {
                Text = "DANH SÁCH DỊCH VỤ",
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

            dgvDichVu.CellClick += DgvDichVu_CellClick;

            pnlBang.Controls.Add(dgvDichVu);
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

        private void LoadDichVu()
        {
            try
            {
                DataTable dt = dichVuBUS.GetAll();

                dgvDichVu.DataSource = dt;

                SetHeader("MaDV", "Mã dịch vụ");
                SetHeader("TenDV", "Tên dịch vụ");
                SetHeader("DonGia", "Đơn giá");
                SetHeader("TonKho", "Tồn kho");

                SetFormat("DonGia", "N0");

                dgvDichVu.ClearSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải danh sách dịch vụ:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
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

        private void DgvDichVu_CellClick(
            object? sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvDichVu.Rows[e.RowIndex];

            txtMaDV.Text = Convert.ToString(row.Cells["MaDV"].Value) ?? "";
            txtTenDV.Text = Convert.ToString(row.Cells["TenDV"].Value) ?? "";
            txtDonGia.Text = Convert.ToString(row.Cells["DonGia"].Value) ?? "";
            txtTonKho.Text = Convert.ToString(row.Cells["TonKho"].Value) ?? "";
        }

        private bool ValidateInput(
            out string tenDV,
            out decimal donGia,
            out int tonKho)
        {
            tenDV = txtTenDV.Text.Trim();
            donGia = 0;
            tonKho = 0;

            if (string.IsNullOrWhiteSpace(tenDV))
            {
                MessageBox.Show("Vui lòng nhập tên dịch vụ.");
                txtTenDV.Focus();
                return false;
            }

            if (!decimal.TryParse(txtDonGia.Text.Trim(), out donGia)
                || donGia < 0)
            {
                MessageBox.Show("Đơn giá phải là số không âm.");
                txtDonGia.Focus();
                return false;
            }

            if (!int.TryParse(txtTonKho.Text.Trim(), out tonKho)
                || tonKho < 0)
            {
                MessageBox.Show("Tồn kho phải là số nguyên không âm.");
                txtTonKho.Focus();
                return false;
            }

            return true;
        }

        private void BtnThem_Click(object? sender, EventArgs e)
        {
            if (!ValidateInput(
                out string tenDV,
                out decimal donGia,
                out int tonKho))
                return;

            try
            {
                DichVuDTO dto = new DichVuDTO
                {
                    TenDV = tenDV,
                    DonGia = donGia,
                    TonKho = tonKho
                };

                int result = dichVuBUS.Insert(dto);

                if (result > 0)
                {
                    MessageBox.Show("Thêm dịch vụ thành công!");

                    LoadDichVu();
                    ClearInput();
                }
                else
                {
                    MessageBox.Show("Không thể thêm dịch vụ.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi thêm dịch vụ:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnSua_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaDV.Text, out int maDV))
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần sửa.");
                return;
            }

            if (!ValidateInput(
                out string tenDV,
                out decimal donGia,
                out int tonKho))
                return;

            try
            {
                DichVuDTO dto = new DichVuDTO
                {
                    MaDV = maDV,
                    TenDV = tenDV,
                    DonGia = donGia,
                    TonKho = tonKho
                };

                int result = dichVuBUS.Update(dto);

                if (result > 0)
                {
                    MessageBox.Show("Cập nhật dịch vụ thành công!");

                    LoadDichVu();
                    ClearInput();
                }
                else
                {
                    MessageBox.Show(
                        "Không cập nhật được dịch vụ. " +
                        "Hãy kiểm tra dịch vụ có đang được sử dụng hay không.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi cập nhật dịch vụ:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnXoa_Click(object? sender, EventArgs e)
        {
            if (!int.TryParse(txtMaDV.Text, out int maDV))
            {
                MessageBox.Show("Vui lòng chọn dịch vụ cần xóa.");
                return;
            }

            DialogResult confirm = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa dịch vụ này không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes)
                return;

            try
            {
                int result = dichVuBUS.Delete(maDV);

                if (result > 0)
                {
                    MessageBox.Show("Xóa dịch vụ thành công!");

                    LoadDichVu();
                    ClearInput();
                }
                else
                {
                    MessageBox.Show(
                        "Không thể xóa dịch vụ này. " +
                        "Có thể dịch vụ đã được sử dụng trong hóa đơn.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi xóa dịch vụ:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void BtnLamMoi_Click(object? sender, EventArgs e)
        {
            ClearInput();
            txtTimKiem.Clear();
            LoadDichVu();
        }

        private void BtnTimKiem_Click(object? sender, EventArgs e)
        {
            TimKiemDichVu();
        }

        private void TimKiemDichVu()
        {
            try
            {
                string tuKhoa = txtTimKiem.Text.Trim();

                DataTable dt = dichVuBUS.GetAll();

                if (!string.IsNullOrWhiteSpace(tuKhoa))
                {
                    DataView view = dt.DefaultView;

                    string tuKhoaAnToan =
                        tuKhoa.Replace("'", "''");

                    view.RowFilter =
                        $"Convert(MaDV, 'System.String') LIKE '%{tuKhoaAnToan}%' " +
                        $"OR TenDV LIKE '%{tuKhoaAnToan}%'";

                    dgvDichVu.DataSource = view;
                }
                else
                {
                    dgvDichVu.DataSource = dt;
                }

                SetHeader("MaDV", "Mã dịch vụ");
                SetHeader("TenDV", "Tên dịch vụ");
                SetHeader("DonGia", "Đơn giá");
                SetHeader("TonKho", "Tồn kho");
                SetFormat("DonGia", "N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tìm kiếm:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ClearInput()
        {
            txtMaDV.Clear();
            txtTenDV.Clear();
            txtDonGia.Clear();
            txtTonKho.Clear();

            dgvDichVu.ClearSelection();
            txtTenDV.Focus();
        }
    }
}
