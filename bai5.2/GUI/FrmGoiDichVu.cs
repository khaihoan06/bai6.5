
using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using KaraokeManagement.BUS;

namespace KaraokeManagement.GUI
{
    public class FrmGoiDichVu : Form
    {
        private readonly DichVuBUS dichVuBUS = new DichVuBUS();
        private readonly HoaDonBUS hoaDonBUS = new HoaDonBUS();

        private readonly int maPhong;
        private readonly int maHD;

        private ComboBox cboDichVu = null!;
        private NumericUpDown nudSoLuong = null!;
        private DataGridView dgvDichVu = null!;
        private DataGridView dgvDaGoi = null!;
        private Label lblTongTien = null!;
        private Label lblThongTinPhong = null!;

        private Button btnThem = null!;
        private Button btnLamMoi = null!;
        private Button btnDong = null!;

        public FrmGoiDichVu(int maPhong, int maHD)
        {
            this.maPhong = maPhong;
            this.maHD = maHD;

            InitializeComponent();

            LoadDichVu();
            LoadDichVuDaGoi();
        }

        private void InitializeComponent()
        {
            Text = "GỌI DỊCH VỤ KARAOKE";
            Size = new Size(1050, 700);
            MinimumSize = new Size(900, 600);
            StartPosition = FormStartPosition.CenterParent;
            BackColor = Color.FromArgb(245, 247, 250);
            Font = new Font("Segoe UI", 10);

            // TIÊU ĐỀ
            lblThongTinPhong = new Label
            {
                Text = $"GỌI DỊCH VỤ - PHÒNG {maPhong} | HÓA ĐƠN {maHD}",
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(35, 45, 65)
            };

            // KHU VỰC CHỌN DỊCH VỤ
            Panel pnlChon = new Panel
            {
                Dock = DockStyle.Top,
                Height = 115,
                Padding = new Padding(15),
                BackColor = Color.White
            };

            Label lblDV = new Label
            {
                Text = "Chọn dịch vụ:",
                Location = new Point(20, 18),
                AutoSize = true
            };

            cboDichVu = new ComboBox
            {
                Location = new Point(20, 48),
                Width = 330,
                DropDownStyle = ComboBoxStyle.DropDownList
            };

            Label lblSL = new Label
            {
                Text = "Số lượng:",
                Location = new Point(380, 18),
                AutoSize = true
            };

            nudSoLuong = new NumericUpDown
            {
                Location = new Point(380, 48),
                Width = 100,
                Minimum = 1,
                Maximum = 1000,
                Value = 1
            };

            btnThem = new Button
            {
                Text = "THÊM DỊCH VỤ",
                Location = new Point(520, 45),
                Size = new Size(170, 38),
                BackColor = Color.FromArgb(39, 174, 96),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            btnThem.Click += BtnThem_Click;

            pnlChon.Controls.Add(lblDV);
            pnlChon.Controls.Add(cboDichVu);
            pnlChon.Controls.Add(lblSL);
            pnlChon.Controls.Add(nudSoLuong);
            pnlChon.Controls.Add(btnThem);

            // DANH SÁCH DỊCH VỤ
            Label lblDanhSach = new Label
            {
                Text = "DANH SÁCH DỊCH VỤ",
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            dgvDichVu = CreateGrid();

            Panel pnlDanhSach = new Panel
            {
                Dock = DockStyle.Top,
                Height = 190,
                Padding = new Padding(15)
            };

            pnlDanhSach.Controls.Add(dgvDichVu);
            pnlDanhSach.Controls.Add(lblDanhSach);

            // DỊCH VỤ ĐÃ GỌI
            Label lblDaGoi = new Label
            {
                Text = "DỊCH VỤ ĐÃ GỌI",
                Dock = DockStyle.Top,
                Height = 35,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            dgvDaGoi = CreateGrid();

            Panel pnlDaGoi = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(15)
            };

            pnlDaGoi.Controls.Add(dgvDaGoi);
            pnlDaGoi.Controls.Add(lblDaGoi);

            // THANH CUỐI FORM
            Panel pnlFooter = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 75,
                BackColor = Color.White,
                Padding = new Padding(15)
            };

            lblTongTien = new Label
            {
                Text = "Tổng tiền dịch vụ: 0 đ",
                Location = new Point(20, 22),
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 75, 75)
            };

            btnLamMoi = new Button
            {
                Text = "LÀM MỚI",
                Size = new Size(120, 38),
                Location = new Point(690, 15),
                Cursor = Cursors.Hand
            };

            btnDong = new Button
            {
                Text = "ĐÓNG",
                Size = new Size(120, 38),
                Location = new Point(825, 15),
                BackColor = Color.FromArgb(52, 73, 94),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };

            btnLamMoi.Click += (s, e) =>
            {
                LoadDichVu();
                LoadDichVuDaGoi();
            };

            btnDong.Click += (s, e) => Close();

            pnlFooter.Controls.Add(lblTongTien);
            pnlFooter.Controls.Add(btnLamMoi);
            pnlFooter.Controls.Add(btnDong);

            // THÊM CÁC KHU VỰC VÀO FORM
            Controls.Add(pnlDaGoi);
            Controls.Add(pnlFooter);
            Controls.Add(pnlDanhSach);
            Controls.Add(pnlChon);
            Controls.Add(lblThongTinPhong);
        }

        // TẠO DATAGRIDVIEW
        private DataGridView CreateGrid()
        {
            return new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode =
                    DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AutoGenerateColumns = true
            };
        }

        // ĐỔI TÊN CỘT AN TOÀN
        private void SetColumnHeader(
            DataGridView grid,
            string columnName,
            string headerText)
        {
            if (grid.Columns[columnName] is DataGridViewColumn column)
            {
                column.HeaderText = headerText;
            }
        }

        // ĐỊNH DẠNG CỘT AN TOÀN
        private void SetColumnFormat(
            DataGridView grid,
            string columnName,
            string format)
        {
            if (grid.Columns[columnName] is DataGridViewColumn column)
            {
                column.DefaultCellStyle.Format = format;
            }
        }

        // TẢI DANH SÁCH DỊCH VỤ
        private void LoadDichVu()
        {
            try
            {
                DataTable dt = dichVuBUS.GetAll();

                cboDichVu.DataSource = null;

                cboDichVu.DisplayMember = "TenDV";
                cboDichVu.ValueMember = "MaDV";
                cboDichVu.DataSource = dt;

                dgvDichVu.DataSource = dt;

                SetColumnHeader(
                    dgvDichVu, "MaDV", "Mã dịch vụ");

                SetColumnHeader(
                    dgvDichVu, "TenDV", "Tên dịch vụ");

                SetColumnHeader(
                    dgvDichVu, "DonGia", "Đơn giá");

                SetColumnHeader(
                    dgvDichVu, "TonKho", "Tồn kho");

                SetColumnFormat(
                    dgvDichVu, "DonGia", "N0");
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

        // TẢI DỊCH VỤ ĐÃ GỌI
        private void LoadDichVuDaGoi()
        {
            try
            {
                DataTable dt = hoaDonBUS.GetServices(maHD);

                dgvDaGoi.DataSource = dt;

                SetColumnHeader(
                    dgvDaGoi, "MaHD", "Mã hóa đơn");

                SetColumnHeader(
                    dgvDaGoi, "MaDV", "Mã dịch vụ");

                SetColumnHeader(
                    dgvDaGoi, "TenDV", "Tên dịch vụ");

                SetColumnHeader(
                    dgvDaGoi, "SoLuong", "Số lượng");

                SetColumnHeader(
                    dgvDaGoi, "DonGia", "Đơn giá");

                SetColumnHeader(
                    dgvDaGoi, "ThanhTien", "Thành tiền");

                SetColumnFormat(
                    dgvDaGoi, "DonGia", "N0");

                SetColumnFormat(
                    dgvDaGoi, "ThanhTien", "N0");

                decimal tongTien = 0;

                foreach (DataRow row in dt.Rows)
                {
                    if (row["ThanhTien"] != DBNull.Value)
                    {
                        tongTien += Convert.ToDecimal(
                            row["ThanhTien"]);
                    }
                }

                lblTongTien.Text =
                    "Tổng tiền dịch vụ: " +
                    tongTien.ToString("N0") + " đ";
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Lỗi tải dịch vụ đã gọi:\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // XỬ LÝ THÊM DỊCH VỤ
        private void BtnThem_Click(object? sender, EventArgs e)
        {
            try
            {
                if (cboDichVu.SelectedValue == null)
                {
                    MessageBox.Show(
                        "Vui lòng chọn dịch vụ.",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (!int.TryParse(
                    Convert.ToString(cboDichVu.SelectedValue),
                    out int maDV))
                {
                    MessageBox.Show(
                        "Không xác định được mã dịch vụ.");

                    return;
                }

                int soLuong = Convert.ToInt32(nudSoLuong.Value);

                int result = hoaDonBUS.AddService(
                    maHD,
                    maDV,
                    soLuong);

                if (result > 0)
                {
                    MessageBox.Show(
                        "Thêm dịch vụ thành công!",
                        "Thông báo",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    nudSoLuong.Value = 1;

                    LoadDichVu();
                    LoadDichVuDaGoi();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Không thể thêm dịch vụ!\n" + ex.Message,
                    "Lỗi",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
    }
}
