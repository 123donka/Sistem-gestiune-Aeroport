using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Security;
using System.Text.Json;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Data;
using AirportManagement.Models;
using MySql.Data.MySqlClient;

namespace AirportManagement.Views
{
    public class AdminForm : Form
    {
        private DataGridView dgv;
        private readonly UtilizatoriController _controller = new UtilizatoriController();
        private System.Data.DataTable _dt;
        private TextBox txtSearch;
        private Button btnExport;
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
        private TextBox txtAirportName;
        private TextBox txtOpenTime;
        private TextBox txtCloseTime;
        private TextBox txtGates;
        private TextBox txtRunways;
        private Button btnSaveSettings;
        private Label lblLastBackupDate;
        private FlowLayoutPanel backupListPanel;
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

            btnExport = new Button
            {
                Text = "Export",
                Width = 124,
                Height = 40,
                Top = 8,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#10B981"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Region = new Region(GetRoundedPath(new Rectangle(0, 0, btnExport.Width, btnExport.Height), 10));
            btnExport.Click += BtnExport_Click;

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

            topPanel.Controls.Add(btnExport);
            topPanel.Controls.Add(searchContainer);
            topPanel.Controls.Add(btnAdd);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ScrollBars = ScrollBars.Both,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None
            };
            tabUsers.AutoScroll = true;
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
            dgv.CellPainting += Dgv_CellPainting;
            dgv.DataBindingComplete += Dgv_DataBindingComplete;

            tabUsers.Controls.Add(dgv);
            tabUsers.Controls.Add(topPanel);

            BuildSettingsTab(tabSettings);
            BuildBackupTab(tabBackup);
            tabControl.TabPages.AddRange(new TabPage[] { tabUsers, tabSettings, tabBackup });

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

            topPanel.Resize += (s, e) => PositionTopButtons(topPanel, searchContainer);
            PositionTopButtons(topPanel, searchContainer);
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

        private void PositionTopButtons(Panel topPanel, Panel searchContainer)
        {
            if (btnExport == null || btnAdd == null) return;

            var rightPadding = 6;
            btnAdd.Left = Math.Max(0, topPanel.ClientSize.Width - btnAdd.Width - rightPadding);
            btnExport.Left = Math.Max(0, btnAdd.Left - btnExport.Width - 10);

            searchContainer.Width = Math.Max(240, btnExport.Left - 20);
            txtSearch.Width = Math.Max(180, searchContainer.Width - 28);
        }

        private void BuildSettingsTab(TabPage tabSettings)
        {
            tabSettings.AutoScroll = true;

            var canvas = new Panel
            {
                Left = 18,
                Top = 18,
                Width = 760,
                Height = 360,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            var title = new Label
            {
                Text = "Setari aeroport",
                Left = 4,
                Top = 0,
                AutoSize = true,
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                BackColor = Color.Transparent
            };

            var subtitle = new Label
            {
                Text = "Configureaza datele generale si capacitatea operationala.",
                Left = 5,
                Top = 34,
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#64748B"),
                BackColor = Color.Transparent
            };

            var airportCard = CreateSettingsCard(0, 72, 460, 116, ColorTranslator.FromHtml("#EFF6FF"));
            airportCard.Controls.Add(CreateSettingsLabel("Nume Aeroport", 22, 18));
            txtAirportName = CreateSettingsTextBox(22, 44, 414);
            airportCard.Controls.Add(txtAirportName);

            var scheduleCard = CreateSettingsCard(0, 206, 460, 134, Color.White);
            scheduleCard.Controls.Add(CreateSettingsLabel("Program functionare", 22, 18));
            scheduleCard.Controls.Add(CreateSettingsLabel("Ora Deschidere", 22, 54));
            txtOpenTime = CreateSettingsTextBox(22, 78, 194);
            scheduleCard.Controls.Add(txtOpenTime);
            scheduleCard.Controls.Add(CreateSettingsLabel("Ora Inchidere", 242, 54));
            txtCloseTime = CreateSettingsTextBox(242, 78, 194);
            scheduleCard.Controls.Add(txtCloseTime);

            var capacityCard = CreateSettingsCard(484, 72, 256, 190, Color.White);
            capacityCard.Controls.Add(CreateSettingsLabel("Capacitate", 22, 18));
            capacityCard.Controls.Add(CreateInfoTile("Porti", "Total porti disponibile", 22, 54, out txtGates));
            capacityCard.Controls.Add(CreateInfoTile("Piste", "Piste operationale", 22, 116, out txtRunways));

            btnSaveSettings = new Button
            {
                Text = "Salveaza Setari",
                Width = 154,
                Height = 40,
                Left = 586,
                Top = 300,
                FlatStyle = FlatStyle.Flat,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSaveSettings.FlatAppearance.BorderSize = 0;
            btnSaveSettings.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#1D4ED8");
            btnSaveSettings.Region = new Region(GetRoundedPath(new Rectangle(0, 0, btnSaveSettings.Width, btnSaveSettings.Height), 10));
            btnSaveSettings.Click += BtnSaveSettings_Click;

            var note = new Label
            {
                Text = "Modificarile se salveaza local si se reincarca la urmatoarea pornire.",
                Left = 486,
                Top = 270,
                Width = 254,
                Height = 38,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#64748B"),
                BackColor = Color.Transparent
            };

            canvas.Controls.Add(title);
            canvas.Controls.Add(subtitle);
            canvas.Controls.Add(airportCard);
            canvas.Controls.Add(scheduleCard);
            canvas.Controls.Add(capacityCard);
            canvas.Controls.Add(note);
            canvas.Controls.Add(btnSaveSettings);
            tabSettings.Controls.Add(canvas);

            LoadSystemSettings();
        }

        private Panel CreateSettingsCard(int left, int top, int width, int height, Color accent)
        {
            var panel = new Panel
            {
                Left = left,
                Top = top,
                Width = width,
                Height = height,
                BackColor = Color.Transparent
            };

            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
                using var bg = new SolidBrush(_darkMode ? ColorTranslator.FromHtml("#1F2937") : Color.White);
                using var border = new Pen(_darkMode ? ColorTranslator.FromHtml("#334155") : ColorTranslator.FromHtml("#E2E8F0"), 1);
                e.Graphics.FillPath(bg, GetRoundedPath(rect, 12));
                e.Graphics.DrawPath(border, GetRoundedPath(rect, 12));

                using var accentBrush = new SolidBrush(_darkMode ? ColorTranslator.FromHtml("#1E3A8A") : accent);
                e.Graphics.FillPath(accentBrush, GetRoundedPath(new Rectangle(10, 10, panel.Width - 21, 8), 4));
            };

            return panel;
        }

        private Panel CreateInfoTile(string title, string description, int left, int top, out TextBox input)
        {
            var tile = new Panel
            {
                Left = left,
                Top = top,
                Width = 212,
                Height = 50,
                BackColor = Color.Transparent
            };

            tile.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, tile.Width - 1, tile.Height - 1);
                using var bg = new SolidBrush(_darkMode ? ColorTranslator.FromHtml("#111827") : ColorTranslator.FromHtml("#F8FAFC"));
                using var border = new Pen(_darkMode ? ColorTranslator.FromHtml("#334155") : ColorTranslator.FromHtml("#E2E8F0"), 1);
                e.Graphics.FillPath(bg, GetRoundedPath(rect, 10));
                e.Graphics.DrawPath(border, GetRoundedPath(rect, 10));
            };

            tile.Controls.Add(new Label
            {
                Text = title,
                Left = 14,
                Top = 7,
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                BackColor = Color.Transparent
            });

            tile.Controls.Add(new Label
            {
                Text = description,
                Left = 14,
                Top = 27,
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#64748B"),
                BackColor = Color.Transparent
            });

            input = CreateSettingsTextBox(152, 10, 44);
            input.TextAlign = HorizontalAlignment.Center;
            tile.Controls.Add(input);

            return tile;
        }

        private Label CreateSettingsLabel(string text, int left, int top)
        {
            return new Label
            {
                Text = text,
                Left = left,
                Top = top,
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                BackColor = Color.Transparent
            };
        }

        private TextBox CreateSettingsTextBox(int left, int top, int width)
        {
            return new TextBox
            {
                Left = left,
                Top = top,
                Width = width,
                Height = 30,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Segoe UI", 10),
                BackColor = Color.White,
                ForeColor = ColorTranslator.FromHtml("#111827")
            };
        }

        private string SettingsFilePath => Path.Combine(AppContext.BaseDirectory, "systemsettings.json");

        private void LoadSystemSettings()
        {
            var settings = new SystemSettings
            {
                AirportName = "Aeroport International Cluj-Napoca",
                OpenTime = "05:00 AM",
                CloseTime = "11:00 PM",
                Gates = 18,
                Runways = 4
            };

            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var json = File.ReadAllText(SettingsFilePath);
                    settings = JsonSerializer.Deserialize<SystemSettings>(json) ?? settings;
                }
            }
            catch
            {
            }

            txtAirportName.Text = settings.AirportName;
            txtOpenTime.Text = settings.OpenTime;
            txtCloseTime.Text = settings.CloseTime;
            txtGates.Text = settings.Gates.ToString();
            txtRunways.Text = settings.Runways.ToString();
        }

        private void BtnSaveSettings_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtAirportName.Text))
            {
                MessageBox.Show("Introdu numele aeroportului.", "Setari Sistem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtGates.Text.Trim(), out var gates) || gates < 0)
            {
                MessageBox.Show("Numarul de porti trebuie sa fie un numar valid.", "Setari Sistem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtRunways.Text.Trim(), out var runways) || runways < 0)
            {
                MessageBox.Show("Numarul de piste trebuie sa fie un numar valid.", "Setari Sistem", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var settings = new SystemSettings
            {
                AirportName = txtAirportName.Text.Trim(),
                OpenTime = txtOpenTime.Text.Trim(),
                CloseTime = txtCloseTime.Text.Trim(),
                Gates = gates,
                Runways = runways
            };

            try
            {
                var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(SettingsFilePath, json);
                MessageBox.Show("Setarile au fost salvate.", "Setari Sistem", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Setarile nu au putut fi salvate: {ex.Message}", "Setari Sistem", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class SystemSettings
        {
            public string AirportName { get; set; } = string.Empty;
            public string OpenTime { get; set; } = string.Empty;
            public string CloseTime { get; set; } = string.Empty;
            public int Gates { get; set; }
            public int Runways { get; set; }
        }

        private void BuildBackupTab(TabPage tabBackup)
        {
            tabBackup.AutoScroll = true;

            var canvas = new Panel
            {
                Left = 18,
                Top = 18,
                Width = 860,
                Height = 420,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };

            var lastCard = CreateSettingsCard(0, 0, 840, 64, ColorTranslator.FromHtml("#EFF6FF"));
            lastCard.Controls.Add(new Label
            {
                Text = "Ultimul Backup",
                Left = 22,
                Top = 18,
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                BackColor = Color.Transparent
            });

            lblLastBackupDate = new Label
            {
                Text = "-",
                Left = 22,
                Top = 38,
                AutoSize = true,
                Font = new Font("Segoe UI", 9),
                ForeColor = ColorTranslator.FromHtml("#475569"),
                BackColor = Color.Transparent
            };
            lastCard.Controls.Add(lblLastBackupDate);

            var btnCreate = CreateBackupActionButton("Creare Backup Manual", 0, 82, ColorTranslator.FromHtml("#2563EB"));
            btnCreate.Click += (s, e) => CreateManualBackup();

            var btnRestore = CreateBackupActionButton("Restaurare din Backup", 0, 126, ColorTranslator.FromHtml("#00C853"));
            btnRestore.Click += (s, e) => RestoreBackupFromDialog();

            var btnConfigure = CreateBackupActionButton("Configurare Backup Automat", 0, 170, ColorTranslator.FromHtml("#6B7280"));
            btnConfigure.Click += (s, e) => ConfigureAutomaticBackup();

            var separator = new Panel
            {
                Left = 0,
                Top = 224,
                Width = 840,
                Height = 1,
                BackColor = ColorTranslator.FromHtml("#E5E7EB")
            };

            var availableLabel = new Label
            {
                Text = "Backup-uri Disponibile",
                Left = 0,
                Top = 246,
                AutoSize = true,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                BackColor = Color.Transparent
            };

            backupListPanel = new FlowLayoutPanel
            {
                Left = 0,
                Top = 276,
                Width = 840,
                Height = 132,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.Transparent
            };

            canvas.Controls.Add(lastCard);
            canvas.Controls.Add(btnCreate);
            canvas.Controls.Add(btnRestore);
            canvas.Controls.Add(btnConfigure);
            canvas.Controls.Add(separator);
            canvas.Controls.Add(availableLabel);
            canvas.Controls.Add(backupListPanel);
            tabBackup.Controls.Add(canvas);

            RunAutomaticBackupIfDue();
            RefreshBackupList();
        }

        private Button CreateBackupActionButton(string text, int left, int top, Color color)
        {
            var button = new Button
            {
                Text = text,
                Left = left,
                Top = top,
                Width = 840,
                Height = 34,
                FlatStyle = FlatStyle.Flat,
                BackColor = color,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0),
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            button.FlatAppearance.BorderSize = 0;
            button.Region = new Region(GetRoundedPath(new Rectangle(0, 0, button.Width, button.Height), 8));
            return button;
        }

        private string BackupFolderPath => Path.Combine(AppContext.BaseDirectory, "backups");

        private void RefreshBackupList()
        {
            Directory.CreateDirectory(BackupFolderPath);

            var files = Directory.GetFiles(BackupFolderPath, "*.sql")
                .Select(path => new FileInfo(path))
                .OrderByDescending(file => file.LastWriteTime)
                .ToList();

            if (lblLastBackupDate != null)
            {
                lblLastBackupDate.Text = files.Count == 0
                    ? "Nu exista backup-uri create."
                    : files[0].LastWriteTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture);
            }

            if (backupListPanel == null) return;
            backupListPanel.Controls.Clear();

            if (files.Count == 0)
            {
                backupListPanel.Controls.Add(new Label
                {
                    Text = "Nu exista backup-uri disponibile.",
                    Width = 820,
                    Height = 34,
                    TextAlign = ContentAlignment.MiddleLeft,
                    ForeColor = ColorTranslator.FromHtml("#64748B"),
                    BackColor = Color.Transparent
                });
                return;
            }

            foreach (var file in files)
            {
                backupListPanel.Controls.Add(CreateBackupListItem(file));
            }
        }

        private Panel CreateBackupListItem(FileInfo file)
        {
            var item = new Panel
            {
                Width = 820,
                Height = 42,
                Margin = new Padding(0, 0, 0, 8),
                BackColor = ColorTranslator.FromHtml("#F8FAFC"),
                Tag = file.FullName
            };

            item.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, item.Width - 1, item.Height - 1);
                using var bg = new SolidBrush(_darkMode ? ColorTranslator.FromHtml("#1F2937") : ColorTranslator.FromHtml("#F8FAFC"));
                e.Graphics.FillPath(bg, GetRoundedPath(rect, 8));
            };

            var date = new Label
            {
                Text = file.LastWriteTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),
                Left = 12,
                Top = 6,
                AutoSize = true,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                BackColor = Color.Transparent
            };

            var size = new Label
            {
                Text = FormatFileSize(file.Length),
                Left = 12,
                Top = 22,
                AutoSize = true,
                Font = new Font("Segoe UI", 8),
                ForeColor = ColorTranslator.FromHtml("#64748B"),
                BackColor = Color.Transparent
            };

            var restore = new LinkLabel
            {
                Text = "Restaureaza",
                AutoSize = true,
                Left = 724,
                Top = 13,
                LinkColor = ColorTranslator.FromHtml("#2563EB"),
                ActiveLinkColor = ColorTranslator.FromHtml("#1D4ED8"),
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 8)
            };
            restore.Click += (s, e) => RestoreBackup(file.FullName);

            item.Controls.Add(date);
            item.Controls.Add(size);
            item.Controls.Add(restore);
            return item;
        }

        private void CreateManualBackup()
        {
            try
            {
                var path = CreateBackupFile();
                RefreshBackupList();
                MessageBox.Show($"Backup creat cu succes:\n{path}", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup-ul nu a putut fi creat: {ex.Message}", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string CreateBackupFile()
        {
            Directory.CreateDirectory(BackupFolderPath);
            var filePath = Path.Combine(BackupFolderPath, $"backup_{DateTime.Now:yyyyMMdd_HHmmss}.sql");
            var sb = new StringBuilder();

            sb.AppendLine("-- AirportManagement backup");
            sb.AppendLine($"-- Created: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            sb.AppendLine("SET FOREIGN_KEY_CHECKS=0;");

            using var conn = DbContext.GetConnection();
            conn.Open();

            var tables = GetExistingBackupTables(conn);
            foreach (var table in tables.AsEnumerable().Reverse())
                sb.AppendLine($"DELETE FROM `{table}`;");

            foreach (var table in tables)
            {
                using var cmd = new MySqlCommand($"SELECT * FROM `{table}`;", conn);
                using var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var columns = new List<string>();
                    var values = new List<string>();

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        columns.Add($"`{reader.GetName(i)}`");
                        values.Add(ToSqlValue(reader.GetValue(i)));
                    }

                    sb.AppendLine($"INSERT INTO `{table}` ({string.Join(",", columns)}) VALUES ({string.Join(",", values)});");
                }
            }

            sb.AppendLine("SET FOREIGN_KEY_CHECKS=1;");
            File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
            return filePath;
        }

        private List<string> GetExistingBackupTables(MySqlConnection conn)
        {
            var wantedTables = new[] { "utilizatori", "zboruri", "pasageri", "resurse", "resurse_alocari", "alerte", "logactivitati" };
            var builder = new MySqlConnectionStringBuilder(DbContext.ConnectionString);
            var dbName = builder.Database;
            var existing = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            using var cmd = new MySqlCommand(
                "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=@db",
                conn);
            cmd.Parameters.AddWithValue("@db", dbName);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
                existing.Add(reader.GetString(0));

            return wantedTables.Where(existing.Contains).ToList();
        }

        private void RestoreBackupFromDialog()
        {
            Directory.CreateDirectory(BackupFolderPath);
            using var dialog = new OpenFileDialog
            {
                Title = "Alege backup-ul pentru restaurare",
                InitialDirectory = BackupFolderPath,
                Filter = "SQL backup (*.sql)|*.sql|Toate fisierele (*.*)|*.*"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
                RestoreBackup(dialog.FileName);
        }

        private void RestoreBackup(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Fisierul de backup nu exista.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "Restaurarea va inlocui datele curente cu cele din backup. Continui?",
                "Restaurare Backup",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);
            if (confirm != DialogResult.Yes) return;

            try
            {
                using var conn = DbContext.GetConnection();
                conn.Open();
                var script = new MySqlScript(conn, File.ReadAllText(filePath, Encoding.UTF8));
                script.Execute();
                LoadData();
                RefreshBackupList();
                MessageBox.Show("Backup-ul a fost restaurat cu succes.", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Backup-ul nu a putut fi restaurat: {ex.Message}", "Backup", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConfigureAutomaticBackup()
        {
            var settings = LoadBackupSettings();
            using var dialog = new Form
            {
                Text = "Configurare Backup Automat",
                Width = 360,
                Height = 210,
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };

            var chkEnabled = new CheckBox { Text = "Activeaza backup automat", Left = 22, Top = 22, Width = 260, Checked = settings.Enabled };
            var lblTime = new Label { Text = "Ora zilnica", Left = 22, Top = 64, AutoSize = true };
            var txtTime = new TextBox { Left = 22, Top = 88, Width = 120, Text = settings.Time };
            var btnOk = new Button { Text = "Salveaza", Left = 152, Top = 130, Width = 86, Height = 32, DialogResult = DialogResult.OK, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            var btnCancel = new Button { Text = "Anuleaza", Left = 244, Top = 130, Width = 86, Height = 32, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0;

            dialog.Controls.AddRange(new Control[] { chkEnabled, lblTime, txtTime, btnOk, btnCancel });
            dialog.AcceptButton = btnOk;
            dialog.CancelButton = btnCancel;

            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            if (!TimeSpan.TryParse(txtTime.Text.Trim(), CultureInfo.InvariantCulture, out _))
            {
                MessageBox.Show("Ora trebuie sa fie in format HH:mm.", "Backup Automat", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            settings.Enabled = chkEnabled.Checked;
            settings.Time = txtTime.Text.Trim();
            File.WriteAllText(BackupSettingsPath, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
            MessageBox.Show("Configurarea backup-ului automat a fost salvata.", "Backup Automat", MessageBoxButtons.OK, MessageBoxIcon.Information);
            RunAutomaticBackupIfDue();
            RefreshBackupList();
        }

        private string BackupSettingsPath => Path.Combine(AppContext.BaseDirectory, "backupsettings.json");

        private BackupSettings LoadBackupSettings()
        {
            try
            {
                if (File.Exists(BackupSettingsPath))
                {
                    var json = File.ReadAllText(BackupSettingsPath);
                    return JsonSerializer.Deserialize<BackupSettings>(json) ?? new BackupSettings();
                }
            }
            catch
            {
            }

            return new BackupSettings();
        }

        private void RunAutomaticBackupIfDue()
        {
            var settings = LoadBackupSettings();
            if (!settings.Enabled) return;
            if (!TimeSpan.TryParse(settings.Time, CultureInfo.InvariantCulture, out var scheduledTime)) return;

            var today = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            if (settings.LastRunDate == today) return;
            if (DateTime.Now.TimeOfDay < scheduledTime) return;

            try
            {
                CreateBackupFile();
                settings.LastRunDate = today;
                File.WriteAllText(BackupSettingsPath, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
            }
            catch
            {
            }
        }

        private string ToSqlValue(object value)
        {
            if (value == null || value == DBNull.Value) return "NULL";

            if (value is DateTime dt)
                return $"'{dt:yyyy-MM-dd HH:mm:ss.ffffff}'";

            if (value is bool b)
                return b ? "1" : "0";

            if (value is byte[] bytes)
                return "0x" + BitConverter.ToString(bytes).Replace("-", string.Empty);

            if (value is sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal)
                return Convert.ToString(value, CultureInfo.InvariantCulture) ?? "NULL";

            var escaped = value.ToString()?.Replace("\\", "\\\\").Replace("'", "''") ?? string.Empty;
            return $"'{escaped}'";
        }

        private string FormatFileSize(long bytes)
        {
            string[] units = { "B", "KB", "MB", "GB" };
            var size = (double)bytes;
            var unit = 0;
            while (size >= 1024 && unit < units.Length - 1)
            {
                size /= 1024;
                unit++;
            }

            return $"{size:0.#} {units[unit]}";
        }

        private class BackupSettings
        {
            public bool Enabled { get; set; }
            public string Time { get; set; } = "03:00";
            public string LastRunDate { get; set; } = string.Empty;
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
            if (dgv.Columns.Contains("id"))
            {
                dgv.Columns["id"].Visible = false;
            }
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

            var editCol = new DataGridViewButtonColumn { Name = "edit", HeaderText = "Actiuni", Text = "Editeaza", UseColumnTextForButtonValue = true, FillWeight = 11, MinimumWidth = 120 };
            var delCol = new DataGridViewButtonColumn { Name = "delete", HeaderText = "", Text = "Sterge", UseColumnTextForButtonValue = true, FillWeight = 10, MinimumWidth = 110 };

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

        private void Dgv_DataBindingComplete(object? sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgv.Columns.Contains("id"))
            {
                dgv.Columns["id"].Visible = false;
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

        private void BtnExport_Click(object? sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
            {
                Title = "Export utilizatori",
                Filter = "Excel Workbook (*.xlsx)|*.xlsx",
                DefaultExt = "xlsx",
                AddExtension = true,
                FileName = $"utilizatori_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            try
            {
                ExportUsersToExcel(dialog.FileName);
                MessageBox.Show("Tabelul a fost exportat cu succes.", "Export", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Exportul a esuat: {ex.Message}", "Export", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportUsersToExcel(string filePath)
        {
            var exportColumns = dgv.Columns
                .Cast<DataGridViewColumn>()
                .Where(c => c.Visible && c.Name != "id" && c.Name != "edit" && c.Name != "delete")
                .ToList();

            var rows = dgv.Rows
                .Cast<DataGridViewRow>()
                .Where(r => !r.IsNewRow)
                .ToList();

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            WriteZipEntry(archive, "[Content_Types].xml", GetContentTypesXml());
            WriteZipEntry(archive, "_rels/.rels", GetRootRelsXml());
            WriteZipEntry(archive, "xl/workbook.xml", GetWorkbookXml());
            WriteZipEntry(archive, "xl/_rels/workbook.xml.rels", GetWorkbookRelsXml());
            WriteZipEntry(archive, "xl/styles.xml", GetStylesXml());
            WriteZipEntry(archive, "xl/worksheets/sheet1.xml", GetWorksheetXml(exportColumns, rows));
        }

        private static void WriteZipEntry(ZipArchive archive, string entryName, string content)
        {
            var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);
            using var writer = new StreamWriter(entry.Open(), Encoding.UTF8);
            writer.Write(content);
        }

        private static string GetContentTypesXml() => @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Types xmlns=""http://schemas.openxmlformats.org/package/2006/content-types"">
  <Default Extension=""rels"" ContentType=""application/vnd.openxmlformats-package.relationships+xml""/>
  <Default Extension=""xml"" ContentType=""application/xml""/>
  <Override PartName=""/xl/workbook.xml"" ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.sheet.main+xml""/>
  <Override PartName=""/xl/worksheets/sheet1.xml"" ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.worksheet+xml""/>
  <Override PartName=""/xl/styles.xml"" ContentType=""application/vnd.openxmlformats-officedocument.spreadsheetml.styles+xml""/>
</Types>";

        private static string GetRootRelsXml() => @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/officeDocument"" Target=""xl/workbook.xml""/>
</Relationships>";

        private static string GetWorkbookXml() => @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<workbook xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"" xmlns:r=""http://schemas.openxmlformats.org/officeDocument/2006/relationships"">
  <sheets>
    <sheet name=""Utilizatori"" sheetId=""1"" r:id=""rId1""/>
  </sheets>
</workbook>";

        private static string GetWorkbookRelsXml() => @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<Relationships xmlns=""http://schemas.openxmlformats.org/package/2006/relationships"">
  <Relationship Id=""rId1"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/worksheet"" Target=""worksheets/sheet1.xml""/>
  <Relationship Id=""rId2"" Type=""http://schemas.openxmlformats.org/officeDocument/2006/relationships/styles"" Target=""styles.xml""/>
</Relationships>";

        private static string GetStylesXml() => @"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>
<styleSheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"">
  <fonts count=""1"">
    <font>
      <sz val=""11""/>
      <color theme=""1""/>
      <name val=""Calibri""/>
      <family val=""2""/>
    </font>
  </fonts>
  <fills count=""1"">
    <fill>
      <patternFill patternType=""none""/>
    </fill>
  </fills>
  <borders count=""1"">
    <border>
      <left/>
      <right/>
      <top/>
      <bottom/>
      <diagonal/>
    </border>
  </borders>
  <cellStyleXfs count=""1"">
    <xf numFmtId=""0"" fontId=""0"" fillId=""0"" borderId=""0""/>
  </cellStyleXfs>
  <cellXfs count=""1"">
    <xf numFmtId=""0"" fontId=""0"" fillId=""0"" borderId=""0"" xfId=""0""/>
  </cellXfs>
</styleSheet>";

        private string GetWorksheetXml(IReadOnlyList<DataGridViewColumn> columns, IReadOnlyList<DataGridViewRow> rows)
        {
            var sb = new StringBuilder();
            sb.Append(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?>");
            sb.AppendLine();
            sb.AppendLine(@"<worksheet xmlns=""http://schemas.openxmlformats.org/spreadsheetml/2006/main"">");
            sb.AppendLine("  <sheetData>");

            sb.Append("    <row r=\"1\">");
            for (var i = 0; i < columns.Count; i++)
            {
                var cellRef = $"{GetExcelColumnName(i + 1)}1";
                sb.Append($"<c r=\"{cellRef}\" t=\"inlineStr\"><is><t>{EscapeXml(columns[i].HeaderText)}</t></is></c>");
            }
            sb.AppendLine("</row>");

            for (var rowIndex = 0; rowIndex < rows.Count; rowIndex++)
            {
                var row = rows[rowIndex];
                sb.Append($"    <row r=\"{rowIndex + 2}\">");
                for (var colIndex = 0; colIndex < columns.Count; colIndex++)
                {
                    var column = columns[colIndex];
                    var cellRef = $"{GetExcelColumnName(colIndex + 1)}{rowIndex + 2}";
                    var value = row.Cells[column.Name].FormattedValue?.ToString() ?? string.Empty;
                    sb.Append($"<c r=\"{cellRef}\" t=\"inlineStr\"><is><t>{EscapeXml(value)}</t></is></c>");
                }
                sb.AppendLine("</row>");
            }

            sb.AppendLine("  </sheetData>");
            sb.AppendLine("</worksheet>");
            return sb.ToString();
        }

        private static string EscapeXml(string value) => SecurityElement.Escape(value) ?? string.Empty;

        private static string GetExcelColumnName(int columnNumber)
        {
            var dividend = columnNumber;
            var columnName = string.Empty;
            while (dividend > 0)
            {
                var modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo) + columnName;
                dividend = (dividend - modulo) / 26;
            }

            return columnName;
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

        private void Dgv_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var columnName = dgv.Columns[e.ColumnIndex].Name;
            if (columnName != "edit" && columnName != "delete") return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

            var buttonRect = new Rectangle(
                e.CellBounds.Left + 12,
                e.CellBounds.Top + 12,
                Math.Max(72, e.CellBounds.Width - 24),
                Math.Max(30, e.CellBounds.Height - 24)
            );

            var isEdit = columnName == "edit";
            var backColor = isEdit ? ColorTranslator.FromHtml("#2563EB") : ColorTranslator.FromHtml("#DC2626");
            var borderColor = isEdit ? ColorTranslator.FromHtml("#1D4ED8") : ColorTranslator.FromHtml("#B91C1C");
            var text = isEdit ? "Editeaza" : "Sterge";

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = GetRoundedPath(buttonRect, 8);
            using var brush = new SolidBrush(backColor);
            using var border = new Pen(borderColor, 1);
            e.Graphics.FillPath(brush, path);
            e.Graphics.DrawPath(border, path);

            TextRenderer.DrawText(
                e.Graphics,
                text,
                dgv.Font,
                buttonRect,
                Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
            );

            e.Handled = true;
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
