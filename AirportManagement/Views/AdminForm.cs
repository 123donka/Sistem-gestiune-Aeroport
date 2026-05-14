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
        private readonly UtilizatoriController _controller = new UtilizatoriController();
        private System.Data.DataTable _dt;
        private TextBox txtSearch;
        private Button btnAdd;
        private Button btnThemeToggle;
        private Button btnHeaderAction;
        private Label lblCurrentUser;
        private TabControl tabControl;
        private Panel header;
        private Panel sidebar;
        private Panel mainPanel;
        private Panel cardPanel;
        private Label pageTitle;
        private readonly Utilizator? _currentUser;
        private bool _darkMode;

        public AdminForm(Utilizator? currentUser = null)
        {
            _currentUser = currentUser;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Administrare";
            Width = 1200;
            Height = 720;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 10);
            BackColor = ColorTranslator.FromHtml("#F3F4F6");

            header = new Panel { Height = 64, Dock = DockStyle.Top, BackColor = ColorTranslator.FromHtml("#1E3A8A") };
            var appTitle = new Label
            {
                Text = "Aeroport Management",
                Left = 24,
                Top = 19,
                AutoSize = true,
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.Transparent
            };

            var userName = string.IsNullOrWhiteSpace(_currentUser?.Nume) ? _currentUser?.Username ?? "Administrator" : _currentUser.Nume;
            var userRole = string.IsNullOrWhiteSpace(_currentUser?.Rol) ? "Admin" : FormatRole(_currentUser.Rol);
            lblCurrentUser = new Label
            {
                Text = $"{userName}  |  {userRole}",
                AutoSize = true,
                Top = 21,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };

            btnThemeToggle = new Button
            {
                Text = "Dark",
                Width = 74,
                Height = 30,
                Top = 17,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            btnThemeToggle.FlatAppearance.BorderSize = 0;
            btnThemeToggle.Click += (s, e) =>
            {
                _darkMode = !_darkMode;
                ApplyTheme();
            };

            btnHeaderAction = new Button
            {
                Text = "Deconectare",
                Width = 122,
                Height = 30,
                Top = 17,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            btnHeaderAction.FlatAppearance.BorderSize = 0;
            btnHeaderAction.Click += (s, e) => Close();

            header.Controls.Add(appTitle);
            header.Controls.Add(btnThemeToggle);
            header.Controls.Add(lblCurrentUser);
            header.Controls.Add(btnHeaderAction);

            void LayoutHeaderRight()
            {
                btnHeaderAction.Left = header.ClientSize.Width - btnHeaderAction.Width - 24;
                lblCurrentUser.Left = btnHeaderAction.Left - lblCurrentUser.Width - 22;
                btnThemeToggle.Left = lblCurrentUser.Left - btnThemeToggle.Width - 18;
            }
            header.Resize += (s, e) => LayoutHeaderRight();

            sidebar = new Panel
            {
                Width = 170,
                Dock = DockStyle.Left,
                BackColor = ColorTranslator.FromHtml("#1E3A8A"),
                Padding = new Padding(0, 35, 0, 0)
            };
            var btnAdmin = CreateSidebarButton("Administrare", true);
            sidebar.Controls.Add(btnAdmin);

            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(28, 32, 28, 28),
                BackColor = ColorTranslator.FromHtml("#F3F4F6")
            };
            pageTitle = new Label
            {
                Text = "Administrare",
                Dock = DockStyle.Top,
                Height = 58,
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#30323A"),
                BackColor = Color.Transparent
            };

            cardPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(8) };
            cardPanel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, cardPanel.Width - 1, cardPanel.Height - 1);
                using var border = new Pen(ColorTranslator.FromHtml("#E5E7EB"), 1);
                e.Graphics.DrawPath(border, GetRoundedPath(rect, 10));
                cardPanel.Region = new Region(GetRoundedPath(rect, 10));
            };

            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                DrawMode = TabDrawMode.OwnerDrawFixed,
                ItemSize = new Size(140, 48),
                SizeMode = TabSizeMode.Fixed
            };
            tabControl.DrawItem += TabControl_DrawItem;

            var tabUsers = new TabPage("Utilizatori") { Padding = new Padding(2, 12, 2, 2), BackColor = Color.White };
            var tabSettings = new TabPage("Setari Sistem") { Padding = new Padding(10), BackColor = Color.White };
            var tabLogs = new TabPage("Log-uri") { Padding = new Padding(10), BackColor = Color.White };
            var tabBackup = new TabPage("Backup") { Padding = new Padding(10), BackColor = Color.White };

            var topPanel = new Panel { Height = 58, Dock = DockStyle.Top, BackColor = Color.White };

            var searchContainer = new Panel { Left = 0, Top = 8, Width = 390, Height = 40, BackColor = Color.White };
            searchContainer.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using var pb = new Pen(ColorTranslator.FromHtml("#CBD5E1"), 1.5f) { LineJoin = LineJoin.Round };
                var r = new Rectangle(0, 0, searchContainer.Width - 1, searchContainer.Height - 1);
                e.Graphics.DrawPath(pb, GetRoundedPath(r, 8));
                searchContainer.Region = new Region(GetRoundedPath(new Rectangle(0, 0, searchContainer.Width, searchContainer.Height), 8));
            };

            txtSearch = new TextBox
            {
                PlaceholderText = "Cauta utilizator...",
                Left = 14,
                Top = 9,
                Width = searchContainer.Width - 28,
                BorderStyle = BorderStyle.None,
                Font = new Font("Segoe UI", 10)
            };
            txtSearch.TextChanged += (s, e) => ApplyFilter();
            searchContainer.Controls.Add(txtSearch);

            btnAdd = new Button
            {
                Text = "+  Adauga Utilizator",
                Width = 190,
                Height = 40,
                Top = 8,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Region = new Region(GetRoundedPath(new Rectangle(0, 0, btnAdd.Width, btnAdd.Height), 10));
            btnAdd.Click += BtnAdd_Click;

            topPanel.Controls.Add(searchContainer);
            topPanel.Controls.Add(btnAdd);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8FAFC");
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#F8FAFC");
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 54;
            dgv.RowTemplate.Height = 58;
            dgv.RowTemplate.DefaultCellStyle.Padding = new Padding(12, 6, 12, 6);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 11, FontStyle.Bold);
            dgv.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8FAFC");
            dgv.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#2563EB");
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;
            dgv.GridColor = Color.White;
            dgv.RowHeadersVisible = false;
            dgv.AllowUserToResizeRows = false;
            dgv.CellFormatting += Dgv_CellFormatting;
            dgv.CellContentClick += Dgv_CellContentClick;

            tabUsers.Controls.Add(dgv);
            tabUsers.Controls.Add(topPanel);

            tabSettings.Controls.Add(new Label { Text = "Setari sistem (in lucru)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            tabLogs.Controls.Add(new Label { Text = "Log-uri (in lucru)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            tabBackup.Controls.Add(new Label { Text = "Backup (in lucru)", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter });
            tabControl.TabPages.AddRange(new TabPage[] { tabUsers, tabSettings, tabLogs, tabBackup });

            cardPanel.Controls.Add(tabControl);
            mainPanel.Controls.Add(cardPanel);
            mainPanel.Controls.Add(pageTitle);

            Controls.Add(mainPanel);
            Controls.Add(sidebar);
            Controls.Add(header);

            header.Dock = DockStyle.None;
            sidebar.Dock = DockStyle.None;
            mainPanel.Dock = DockStyle.None;

            void LayoutShell()
            {
                header.SetBounds(0, 0, ClientSize.Width, 64);
                sidebar.SetBounds(0, header.Bottom, 170, Math.Max(0, ClientSize.Height - header.Height));
                mainPanel.SetBounds(sidebar.Right, header.Bottom, Math.Max(0, ClientSize.Width - sidebar.Width), Math.Max(0, ClientSize.Height - header.Height));
                LayoutHeaderRight();
                mainPanel.PerformLayout();
                cardPanel.Invalidate();
            }
            Resize += (s, e) => LayoutShell();
            LayoutShell();

            tabControl.Resize += (s, e) => PositionAddButton(tabUsers);
            tabUsers.Resize += (s, e) => PositionAddButton(tabUsers);
            PositionAddButton(tabUsers);
            ApplyTheme();
        }

        private Button CreateSidebarButton(string text, bool active)
        {
            var btn = new Button
            {
                Text = text,
                Dock = DockStyle.Top,
                Height = 54,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(24, 0, 0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = active ? ColorTranslator.FromHtml("#2563EB") : ColorTranslator.FromHtml("#1E3A8A"),
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 11, FontStyle.Bold)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#2563EB");
            return btn;
        }

        private void PositionAddButton(TabPage tabUsers)
        {
            if (btnAdd == null) return;
            btnAdd.Left = Math.Max(0, tabUsers.ClientSize.Width - btnAdd.Width - 6);
        }

        private void TabControl_DrawItem(object? sender, DrawItemEventArgs e)
        {
            var tab = tabControl.TabPages[e.Index];
            var selected = e.Index == tabControl.SelectedIndex;
            var bounds = e.Bounds;

            using var bg = new SolidBrush(_darkMode ? ColorTranslator.FromHtml("#111827") : Color.White);
            e.Graphics.FillRectangle(bg, bounds);

            var color = selected ? ColorTranslator.FromHtml("#60A5FA") : (_darkMode ? ColorTranslator.FromHtml("#CBD5E1") : ColorTranslator.FromHtml("#6B7280"));
            TextRenderer.DrawText(e.Graphics, tab.Text, tabControl.Font, bounds, color, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

            if (selected)
            {
                using var line = new Pen(ColorTranslator.FromHtml("#60A5FA"), 3);
                e.Graphics.DrawLine(line, bounds.Left + 8, bounds.Bottom - 2, bounds.Right - 8, bounds.Bottom - 2);
            }
        }

        private void ApplyTheme()
        {
            var pageBack = _darkMode ? ColorTranslator.FromHtml("#0F172A") : ColorTranslator.FromHtml("#F3F4F6");
            var cardBack = _darkMode ? ColorTranslator.FromHtml("#111827") : Color.White;
            var headerBack = _darkMode ? ColorTranslator.FromHtml("#0B1220") : ColorTranslator.FromHtml("#1E3A8A");
            var sidebarBack = _darkMode ? ColorTranslator.FromHtml("#111827") : ColorTranslator.FromHtml("#1E3A8A");
            var text = _darkMode ? ColorTranslator.FromHtml("#F8FAFC") : ColorTranslator.FromHtml("#111827");
            var muted = _darkMode ? ColorTranslator.FromHtml("#CBD5E1") : ColorTranslator.FromHtml("#30323A");
            var tableAlt = _darkMode ? ColorTranslator.FromHtml("#1F2937") : ColorTranslator.FromHtml("#F8FAFC");
            var tableHeader = _darkMode ? ColorTranslator.FromHtml("#1F2937") : ColorTranslator.FromHtml("#F8FAFC");

            BackColor = pageBack;
            header.BackColor = headerBack;
            sidebar.BackColor = sidebarBack;
            mainPanel.BackColor = pageBack;
            cardPanel.BackColor = cardBack;
            pageTitle.ForeColor = muted;
            btnThemeToggle.Text = _darkMode ? "Light" : "Dark";
            btnThemeToggle.BackColor = _darkMode ? ColorTranslator.FromHtml("#F59E0B") : ColorTranslator.FromHtml("#2563EB");

            ApplyThemeToControls(cardPanel, cardBack, text);
            ApplyThemeToControls(mainPanel, pageBack, text);

            foreach (Control c in sidebar.Controls)
            {
                if (c is Button b)
                {
                    var active = b.Text == "Administrare";
                    b.BackColor = active ? ColorTranslator.FromHtml("#2563EB") : sidebarBack;
                    b.ForeColor = Color.White;
                    b.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#2563EB");
                }
            }

            if (dgv != null)
            {
                dgv.BackgroundColor = cardBack;
                dgv.DefaultCellStyle.BackColor = cardBack;
                dgv.DefaultCellStyle.ForeColor = text;
                dgv.AlternatingRowsDefaultCellStyle.BackColor = tableAlt;
                dgv.AlternatingRowsDefaultCellStyle.ForeColor = text;
                dgv.ColumnHeadersDefaultCellStyle.BackColor = tableHeader;
                dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = tableHeader;
                dgv.ColumnHeadersDefaultCellStyle.ForeColor = text;
                dgv.GridColor = _darkMode ? ColorTranslator.FromHtml("#111827") : Color.White;

                foreach (DataGridViewColumn c in dgv.Columns)
                {
                    c.HeaderCell.Style.BackColor = tableHeader;
                    c.HeaderCell.Style.SelectionBackColor = tableHeader;
                    c.HeaderCell.Style.ForeColor = text;
                    c.HeaderCell.Style.SelectionForeColor = text;
                }
            }

            tabControl.Invalidate();
            cardPanel.Invalidate();
        }

        private void ApplyThemeToControls(Control parent, Color back, Color text)
        {
            foreach (Control c in parent.Controls)
            {
                if (c is TabPage || c is Panel)
                {
                    c.BackColor = back;
                }
                else if (c is Label label)
                {
                    label.ForeColor = text;
                    label.BackColor = Color.Transparent;
                }
                else if (c is TextBox tb)
                {
                    tb.BackColor = back;
                    tb.ForeColor = text;
                }

                if (c.HasChildren)
                {
                    ApplyThemeToControls(c, back, text);
                }
            }
        }

        public void SetEmbedded(bool embedded)
        {
            if (header != null) header.Visible = !embedded;
            if (sidebar != null) sidebar.Visible = !embedded;
            if (pageTitle != null) pageTitle.Visible = !embedded;
            if (mainPanel != null)
            {
                mainPanel.Padding = embedded ? new Padding(0) : new Padding(28, 32, 28, 28);
                mainPanel.BackColor = embedded ? Color.White : ColorTranslator.FromHtml("#F3F4F6");
            }
            if (cardPanel != null)
            {
                cardPanel.Padding = embedded ? new Padding(4) : new Padding(8);
            }
            BackColor = embedded ? Color.White : ColorTranslator.FromHtml("#F3F4F6");
        }

        private void LoadData()
        {
            _dt = _controller.GetAll();
            BuildGrid();
            dgv.DataSource = _dt;
            ApplyTheme();
        }

        private void BuildGrid()
        {
            dgv.Columns.Clear();
            dgv.AutoGenerateColumns = false;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", HeaderText = "ID", DataPropertyName = "id", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "nume", HeaderText = "Nume", DataPropertyName = "nume", FillWeight = 18 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "username", HeaderText = "Username", DataPropertyName = "username", FillWeight = 18 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "rol", HeaderText = "Rol", DataPropertyName = "rol", FillWeight = 14 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "activ", HeaderText = "Activ", DataPropertyName = "activ", FillWeight = 12 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "ultima_logare", HeaderText = "Ultima Logare", DataPropertyName = "ultima_logare", FillWeight = 20 });

            var editCol = new DataGridViewButtonColumn { Name = "edit", HeaderText = "Actiuni", Text = "Editeaza", UseColumnTextForButtonValue = true, FillWeight = 9 };
            var delCol = new DataGridViewButtonColumn { Name = "delete", HeaderText = "", Text = "Sterge", UseColumnTextForButtonValue = true, FillWeight = 9 };

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

            foreach (DataGridViewColumn c in dgv.Columns)
            {
                c.HeaderCell.Style.BackColor = dgv.ColumnHeadersDefaultCellStyle.BackColor;
                c.HeaderCell.Style.ForeColor = dgv.ColumnHeadersDefaultCellStyle.ForeColor;
                c.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                c.HeaderCell.Style.SelectionBackColor = dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor;
                c.HeaderCell.Style.SelectionForeColor = dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor;
            }
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
                var nume = dgv.Rows[e.RowIndex].Cells["nume"].Value?.ToString() ?? string.Empty;
                if (MessageBox.Show($"Stergi utilizatorul '{nume}'?", "Confirmare", MessageBoxButtons.YesNo) == DialogResult.Yes)
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
                var role = e.Value.ToString() ?? string.Empty;
                var normalized = role.ToLower();
                if (normalized.Contains("admin"))
                {
                    e.Value = "Admin";
                    e.CellStyle.BackColor = ColorTranslator.FromHtml("#EF4444");
                    e.CellStyle.ForeColor = Color.White;
                }
                else if (normalized.Contains("operator"))
                {
                    e.Value = "Operator";
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
                e.CellStyle.BackColor = _darkMode ? ColorTranslator.FromHtml("#111827") : Color.White;
                e.CellStyle.ForeColor = ColorTranslator.FromHtml("#2563EB");
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }

            if (dgv.Columns[e.ColumnIndex].Name == "delete")
            {
                e.CellStyle.BackColor = _darkMode ? ColorTranslator.FromHtml("#111827") : Color.White;
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
                var txt = txtSearch.Text.Trim().Replace("'", "''");
                _dt.DefaultView.RowFilter = string.IsNullOrEmpty(txt)
                    ? string.Empty
                    : $"nume LIKE '%{txt}%' OR username LIKE '%{txt}%'";
            }
            catch
            {
            }
        }

        private string FormatRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role)) return "Admin";
            return char.ToUpper(role[0]) + role.Substring(1);
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
