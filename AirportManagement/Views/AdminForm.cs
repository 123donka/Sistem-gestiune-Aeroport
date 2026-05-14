using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class AdminForm : Form
    {
        private DataGridView dgv;
        private UtilizatoriController _controller = new UtilizatoriController();
        private System.Data.DataTable _dt;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnHeaderAction;
        private Label lblCurrentUser;
        private TabControl tabControl;
        private Panel header;
        private Utilizator? _currentUser;

        public AdminForm(Utilizator? currentUser = null)
        {
            _currentUser = currentUser;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Administrare"; Width = 1000; Height = 680; StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 9);
            BackColor = ColorTranslator.FromHtml("#F8F9FA");

            header = new Panel { Height = 70, Dock = DockStyle.Top, BackColor = ColorTranslator.FromHtml("#2563EB") };
            var lblTitle = new Label { Text = "Administrare", Left = 20, Top = 18, AutoSize = true, Font = new Font("Segoe UI", 16, FontStyle.Bold), ForeColor = Color.White, BackColor = Color.Transparent };
            var userName = string.IsNullOrWhiteSpace(_currentUser?.Nume) ? _currentUser?.Username ?? "Admin" : _currentUser.Nume;
            var userRole = string.IsNullOrWhiteSpace(_currentUser?.Rol) ? "Admin" : char.ToUpper(_currentUser.Rol[0]) + _currentUser.Rol.Substring(1);
            lblCurrentUser = new Label
            {
                Text = $"{userName} | {userRole}",
                AutoSize = true,
                Top = 24,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnHeaderAction = new Button
            {
                Text = "Deconectare",
                Width = 130,
                Height = 34,
                Left = 0,
                Top = 18,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = ColorTranslator.FromHtml("#DC2626"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnHeaderAction.FlatAppearance.BorderSize = 0;
            btnHeaderAction.Click += (s, e) => Close();
            header.Controls.Add(lblTitle);
            header.Controls.Add(lblCurrentUser);
            header.Controls.Add(btnHeaderAction);
            void LayoutHeaderRight()
            {
                btnHeaderAction.Left = header.ClientSize.Width - btnHeaderAction.Width - 20;
                lblCurrentUser.Left = btnHeaderAction.Left - lblCurrentUser.Width - 18;
            }
            header.Resize += (s, e) => LayoutHeaderRight();

            var container = new Panel { Dock = DockStyle.Fill, Padding = new Padding(20) };

            tabControl = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 10) };
            var tabUsers = new TabPage("Utilizatori") { Padding = new Padding(10) };
            var tabSettings = new TabPage("Setări Sistem") { Padding = new Padding(10) };
            var tabLogs = new TabPage("Log-uri") { Padding = new Padding(10) };
            var tabBackup = new TabPage("Backup") { Padding = new Padding(10) };

            var topPanel = new Panel { Height = 56, Dock = DockStyle.Top, BackColor = Color.Transparent };

            var searchContainer = new Panel { Left = 0, Top = 10, Width = 420, Height = 36, BackColor = Color.White };
            searchContainer.Paint += (s, e) => {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pb = new Pen(ColorTranslator.FromHtml("#E5E7EB"), 1.5f) { LineJoin = LineJoin.Round };
                var r = new Rectangle(0, 0, searchContainer.Width - 1, searchContainer.Height - 1);
                e.Graphics.DrawPath(pb, GetRoundedPath(r, 8));
                searchContainer.Region = new Region(GetRoundedPath(new Rectangle(0, 0, searchContainer.Width, searchContainer.Height), 8));
            };

            txtSearch = new TextBox { PlaceholderText = "Caută utilizator...", Left = 10, Top = 6, Width = searchContainer.Width - 20, BorderStyle = BorderStyle.None };
            txtSearch.Font = new Font("Segoe UI", 9);
            txtSearch.TextChanged += (s, e) => ApplyFilter();
            searchContainer.Controls.Add(txtSearch);

            btnAdd = new Button { Text = "+ Adaugă Utilizator", Width = 160, Height = 36, Anchor = AnchorStyles.Top | AnchorStyles.Right, FlatStyle = FlatStyle.Flat, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Region = new Region(GetRoundedPath(new Rectangle(0, 0, btnAdd.Width, btnAdd.Height), 10));
            btnAdd.Click += BtnAdd_Click;

            topPanel.Controls.Add(searchContainer);
            topPanel.Controls.Add(btnAdd);

            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect, AllowUserToAddRows = false, BackgroundColor = Color.White, BorderStyle = BorderStyle.None };
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#111827");
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 48;
            dgv.RowTemplate.Height = 44;
            dgv.RowTemplate.DefaultCellStyle.Padding = new Padding(8, 6, 8, 6);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8FAFC");
            dgv.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#2563EB");
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.GridColor = ColorTranslator.FromHtml("#E6E9EE");
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeRows = false;
            dgv.CellFormatting += Dgv_CellFormatting;
            dgv.CellContentClick += Dgv_CellContentClick;

            tabUsers.Controls.Add(dgv);
            tabUsers.Controls.Add(topPanel);

            tabSettings.Controls.Add(new Label { Text = "Setări sistem (în lucru)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            tabLogs.Controls.Add(new Label { Text = "Log-uri (în lucru)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            tabBackup.Controls.Add(new Label { Text = "Backup (în lucru)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });

            tabControl.TabPages.AddRange(new TabPage[] { tabUsers, tabSettings, tabLogs, tabBackup });

            container.Controls.Add(tabControl);

            Controls.Add(container);
            Controls.Add(header);
            LayoutHeaderRight();

            tabControl.Resize += (s, e) => { btnAdd.Left = tabUsers.ClientSize.Width - btnAdd.Width - 10; };
        }

        public void SetEmbedded(bool embedded)
        {
            if (header != null)
            {
                header.Visible = !embedded;
            }
            BackColor = embedded ? Color.White : ColorTranslator.FromHtml("#F8F9FA");
        }

        private void LoadData()
        {
            _dt = _controller.GetAll();
            BuildGrid();
            dgv.DataSource = _dt;
        }

        private void BuildGrid()
        {
            dgv.Columns.Clear();
            dgv.AutoGenerateColumns = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = "id", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "nume", HeaderText = "Nume", DataPropertyName = "nume" });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "username", HeaderText = "Username", DataPropertyName = "username" });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "rol", HeaderText = "Rol", DataPropertyName = "rol", Width = 120 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "activ", HeaderText = "Activ", DataPropertyName = "activ", Width = 80 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "ultima_logare", HeaderText = "Ultima Logare", DataPropertyName = "ultima_logare", Width = 160 });

            var editCol = new DataGridViewButtonColumn { Name = "edit", HeaderText = "Acțiuni", Text = "Editează", UseColumnTextForButtonValue = true, Width = 90 };
            var delCol = new DataGridViewButtonColumn { Name = "delete", HeaderText = "", Text = "Șterge", UseColumnTextForButtonValue = true, Width = 90 };

            editCol.FlatStyle = FlatStyle.Flat;
            editCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            editCol.DefaultCellStyle.BackColor = Color.White;
            editCol.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#2563EB");

            delCol.FlatStyle = FlatStyle.Flat;
            delCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            delCol.DefaultCellStyle.BackColor = Color.White;
            delCol.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#DC2626");

            dgv.Columns.Add(editCol);
            dgv.Columns.Add(delCol);

            // Ensure every column header uses the configured header style (keep headers white)
            foreach (DataGridViewColumn c in dgv.Columns)
            {
                c.HeaderCell.Style.BackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;
                c.HeaderCell.Style.ForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                c.HeaderCell.Style.SelectionBackColor = dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor;
                c.HeaderCell.Style.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor;
            }

            // Ensure the ID column header uses the same header background (white-like)
            if (dgv.Columns.Contains("id"))
            {
                dgv.Columns["id"].HeaderCell.Style.BackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;
                dgv.Columns["id"].HeaderCell.Style.ForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
                dgv.Columns["id"].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private Utilizator? GetSelected()
        {
            if (dgv.CurrentRow == null) return null;
            var row = dgv.CurrentRow;
            return new Utilizator
            {
                Id = Convert.ToInt32(row.Cells["id"].Value),
                Nume = row.Cells["nume"].Value?.ToString() ?? string.Empty,
                Username = row.Cells["username"].Value?.ToString() ?? string.Empty,
                Rol = row.Cells["rol"].Value?.ToString() ?? string.Empty
            };
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var dlg = new UtilizatorEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var u = dlg.UtilizatorResult!;
                _controller.Create(u);
                LoadData();
            }
        }

        private void Dgv_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var col = dgv.Columns[e.ColumnIndex];
            if (col.Name == "edit")
            {
                var id = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["id"].Value);
                var full = _controller.GetById(id);
                if (full == null) return;
                var dlg = new UtilizatorEditForm(full);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    var u = dlg.UtilizatorResult!;
                    _controller.Update(u);
                    LoadData();
                }
            }
            else if (col.Name == "delete")
            {
                var id = Convert.ToInt32(dgv.Rows[e.RowIndex].Cells["id"].Value);
                var nume = dgv.Rows[e.RowIndex].Cells["nume"].Value?.ToString() ?? "";
                if (MessageBox.Show($"Ștergi utilizatorul '{nume}'?", "Confirmare", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    _controller.Delete(id);
                    LoadData();
                }
            }
        }

        private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dgv.Columns[e.ColumnIndex].Name == "activ")
            {
                if (e.Value == null || e.Value == DBNull.Value) { e.Value = "Inactiv"; return; }
                var val = Convert.ToInt32(e.Value);
                e.Value = val == 1 ? "Activ" : "Inactiv";
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.BackColor = val == 1 ? ColorTranslator.FromHtml("#10B981") : ColorTranslator.FromHtml("#6B7280");
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgv.Columns[e.ColumnIndex].Name == "rol")
            {
                if (e.Value == null || e.Value == DBNull.Value) return;
                var r = e.Value.ToString() ?? string.Empty;
                var rl = r.ToLower();
                if (rl.Contains("admin"))
                {
                    e.CellStyle.BackColor = ColorTranslator.FromHtml("#EF4444");
                    e.CellStyle.ForeColor = Color.White;
                }
                else if (rl.Contains("operator"))
                {
                    e.CellStyle.BackColor = ColorTranslator.FromHtml("#2563EB");
                    e.CellStyle.ForeColor = Color.White;
                }
                else
                {
                    e.CellStyle.BackColor = ColorTranslator.FromHtml("#6B7280");
                    e.CellStyle.ForeColor = Color.White;
                }
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgv.Columns[e.ColumnIndex].Name == "edit")
            {
                e.CellStyle.BackColor = Color.White;
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#2563EB");
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgv.Columns[e.ColumnIndex].Name == "delete")
            {
                e.CellStyle.BackColor = Color.White;
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#DC2626");
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgv.Columns[e.ColumnIndex].Name == "ultima_logare")
            {
                if (e.Value == null || e.Value == DBNull.Value) { e.Value = "-"; return; }
                if (DateTime.TryParse(e.Value.ToString(), out var dt))
                {
                    e.Value = dt.ToLocalTime().ToString("dd.MM.yyyy HH:mm");
                }
            }
        }

        private void ApplyFilter()
        {
            if (_dt == null) return;
            try
            {
                var txt = txtSearch.Text.Trim().Replace("'","''");
                if (string.IsNullOrEmpty(txt)) _dt.DefaultView.RowFilter = string.Empty;
                else _dt.DefaultView.RowFilter = $"nume LIKE '%{txt}%' OR username LIKE '%{txt}%'";
            }
            catch
            {
            }
        }

        private GraphicsPath GetRoundedPath(Rectangle r, int radius)
        {
            var d = radius * 2;
            var path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(r.X, r.Y, d, d, 180, 90);
            path.AddArc(r.Right - d, r.Y, d, d, 270, 90);
            path.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
            path.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
