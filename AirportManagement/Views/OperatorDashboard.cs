using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class OperatorDashboard : Form
    {
        private readonly Utilizator _user;
        private readonly ZboruriController _zboruriController = new ZboruriController();
        private readonly AlerteController _alerteController = new AlerteController();

        private Panel header = null!;
        private Panel sidebar = null!;
        private Panel mainPanel = null!;
        private Panel statsPanel = null!;
        private Panel contentPanel = null!;
        private RoundedPanel flightsCard = null!;
        private RoundedPanel alertsCard = null!;
        private DataGridView dgvFlights = null!;
        private FlowLayoutPanel alertsList = null!;
        private Label lblFlightsValue = null!;
        private Label lblPassengersValue = null!;
        private Label lblDelaysValue = null!;
        private Label lblGatesValue = null!;

        public OperatorDashboard(Utilizator user)
        {
            _user = user;
            InitializeComponent();
            LoadDashboardData();
        }

        private void InitializeComponent()
        {
            Text = "Operator Dashboard";
            Width = 1368;
            Height = 720;
            MinimumSize = new Size(1050, 620);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9);
            BackColor = ColorTranslator.FromHtml("#F4F6F8");

            header = new Panel
            {
                Height = 42,
                BackColor = ColorTranslator.FromHtml("#1F416C")
            };

            var logo = new Label
            {
                Text = "Airport Management",
                Left = 18,
                Top = 0,
                Width = 210,
                Height = header.Height,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };
            logo.Paint += (s, e) => DrawPlaneIcon(e.Graphics, new Rectangle(0, 13, 18, 18), ColorTranslator.FromHtml("#9BB7F5"));
            logo.Padding = new Padding(28, 0, 0, 0);
            header.Controls.Add(logo);

            var userName = string.IsNullOrWhiteSpace(_user.Nume) ? _user.Username : _user.Nume;
            var lblUser = new Label
            {
                Text = $"{userName}  |  Operator",
                AutoSize = true,
                Height = header.Height,
                Top = 0,
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleRight,
                Font = new Font("Segoe UI", 8, FontStyle.Bold)
            };

            var btnLogout = new Button
            {
                Text = "Deconectare",
                Width = 112,
                Height = 28,
                Top = 7,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.Transparent,
                ForeColor = Color.White,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 8, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleRight
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.Paint += (s, e) => DrawLogoutIcon(e.Graphics, new Rectangle(4, 7, 14, 14), Color.White);
            btnLogout.Click += (s, e) => Close();

            header.Controls.Add(lblUser);
            header.Controls.Add(btnLogout);

            sidebar = new Panel
            {
                Width = 152,
                BackColor = ColorTranslator.FromHtml("#1F416C")
            };

            var navTop = 12;
            sidebar.Controls.Add(CreateSidebarButton("Dashboard", navTop, true, DrawGridIcon));
            sidebar.Controls.Add(CreateSidebarButton("Zboruri", navTop + 36, false, DrawPlaneIcon));
            sidebar.Controls.Add(CreateSidebarButton("Resurse", navTop + 72, false, DrawMapIcon));
            sidebar.Controls.Add(CreateSidebarButton("Pasageri", navTop + 108, false, DrawUsersIcon));
            sidebar.Controls.Add(CreateSidebarButton("Alerte", navTop + 144, false, DrawBellIcon));
            sidebar.Controls.Add(CreateSidebarButton("Rapoarte", navTop + 180, false, DrawChartIcon));
            sidebar.Controls.Add(CreateSidebarButton("Profil", navTop + 216, false, DrawUserIcon));
            sidebar.Controls.Add(CreateSidebarButton("Administrare", navTop + 252, false, DrawShieldIcon));

            mainPanel = new Panel
            {
                BackColor = ColorTranslator.FromHtml("#F4F6F8"),
                Padding = new Padding(16)
            };

            statsPanel = new Panel
            {
                Height = 78,
                BackColor = Color.Transparent
            };

            var card1 = CreateStatCard("Zboruri Astazi", "142", ColorTranslator.FromHtml("#2F7CF6"), DrawPlaneIcon, out lblFlightsValue);
            var card2 = CreateStatCard("Pasageri Asteptati", "8,432", ColorTranslator.FromHtml("#F4B000"), DrawUsersIcon, out lblPassengersValue);
            var card3 = CreateStatCard("Intarzieri Active", "7", ColorTranslator.FromHtml("#FF3045"), DrawWarningIcon, out lblDelaysValue);
            var card4 = CreateStatCard("Poarta Disponibile", "12/18", ColorTranslator.FromHtml("#08C767"), DrawClockIcon, out lblGatesValue);
            statsPanel.Controls.AddRange(new Control[] { card1, card2, card3, card4 });

            contentPanel = new Panel
            {
                BackColor = Color.Transparent
            };

            flightsCard = new RoundedPanel
            {
                BackColor = Color.White,
                BorderColor = ColorTranslator.FromHtml("#E7EAF0"),
                Radius = 8
            };

            var flightsTitle = new Label
            {
                Text = "Zboruri Live",
                Left = 12,
                Top = 12,
                Width = 180,
                Height = 28,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Black
            };
            flightsCard.Controls.Add(flightsTitle);

            dgvFlights = new DataGridView
            {
                Left = 1,
                Top = 42,
                Width = 600,
                Height = 270,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoGenerateColumns = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgvFlights.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F2F4F7");
            dgvFlights.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgvFlights.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 7, FontStyle.Bold);
            dgvFlights.ColumnHeadersHeight = 32;
            dgvFlights.DefaultCellStyle.Font = new Font("Segoe UI", 7);
            dgvFlights.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#111827");
            dgvFlights.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#EAF1FF");
            dgvFlights.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#111827");
            dgvFlights.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8F9FB");
            dgvFlights.RowTemplate.Height = 31;
            dgvFlights.GridColor = Color.White;
            dgvFlights.CellFormatting += DgvFlights_CellFormatting;
            BuildFlightsGrid();
            flightsCard.Controls.Add(dgvFlights);

            alertsCard = new RoundedPanel
            {
                BackColor = Color.White,
                BorderColor = ColorTranslator.FromHtml("#E7EAF0"),
                Radius = 8
            };

            var alertsTitle = new Label
            {
                Text = "Alerte Recente",
                Left = 12,
                Top = 12,
                Width = 180,
                Height = 28,
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                ForeColor = Color.Black
            };
            alertsCard.Controls.Add(alertsTitle);

            alertsList = new FlowLayoutPanel
            {
                Left = 12,
                Top = 52,
                Width = 300,
                Height = 260,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = Color.White
            };
            alertsCard.Controls.Add(alertsList);

            contentPanel.Controls.Add(flightsCard);
            contentPanel.Controls.Add(alertsCard);
            mainPanel.Controls.Add(statsPanel);
            mainPanel.Controls.Add(contentPanel);

            Controls.Add(mainPanel);
            Controls.Add(sidebar);
            Controls.Add(header);

            Resize += (s, e) => LayoutDashboard();
            header.Resize += (s, e) =>
            {
                btnLogout.Left = header.ClientSize.Width - btnLogout.Width - 16;
                lblUser.Left = btnLogout.Left - lblUser.Width - 8;
            };
            LayoutDashboard();

            WireNavigation();
        }

        private void LayoutDashboard()
        {
            header.SetBounds(0, 0, ClientSize.Width, 42);
            sidebar.SetBounds(0, header.Bottom, 152, Math.Max(0, ClientSize.Height - header.Height));
            mainPanel.SetBounds(sidebar.Right, header.Bottom, Math.Max(0, ClientSize.Width - sidebar.Width), Math.Max(0, ClientSize.Height - header.Height));

            statsPanel.SetBounds(16, 16, Math.Max(0, mainPanel.ClientSize.Width - 32), 78);
            contentPanel.SetBounds(16, 110, Math.Max(0, mainPanel.ClientSize.Width - 32), Math.Max(0, mainPanel.ClientSize.Height - 126));

            var gap = 12;
            var statWidth = Math.Max(180, (statsPanel.ClientSize.Width - gap * 3) / 4);
            for (var i = 0; i < statsPanel.Controls.Count; i++)
            {
                statsPanel.Controls[i].SetBounds(i * (statWidth + gap), 0, statWidth, 78);
            }

            var alertsWidth = Math.Min(340, Math.Max(280, contentPanel.ClientSize.Width / 4));
            alertsCard.SetBounds(contentPanel.ClientSize.Width - alertsWidth, 0, alertsWidth, 322);
            flightsCard.SetBounds(0, 0, Math.Max(450, alertsCard.Left - 16), 322);
            dgvFlights.SetBounds(1, 42, flightsCard.ClientSize.Width - 2, flightsCard.ClientSize.Height - 43);
            alertsList.SetBounds(12, 52, alertsCard.ClientSize.Width - 24, alertsCard.ClientSize.Height - 64);

            header.PerformLayout();
            foreach (Control c in statsPanel.Controls) c.Invalidate();
            flightsCard.Invalidate();
            alertsCard.Invalidate();
        }

        private RoundedPanel CreateStatCard(string title, string value, Color iconBack, Action<Graphics, Rectangle, Color> drawIcon, out Label valueLabel)
        {
            var card = new RoundedPanel
            {
                BackColor = Color.White,
                BorderColor = ColorTranslator.FromHtml("#E7EAF0"),
                Radius = 8
            };

            var iconBox = new RoundedPanel
            {
                Left = 17,
                Top = 18,
                Width = 44,
                Height = 44,
                BackColor = iconBack,
                BorderColor = iconBack,
                Radius = 7
            };
            iconBox.Paint += (s, e) => drawIcon(e.Graphics, new Rectangle(11, 11, 22, 22), Color.White);
            card.Controls.Add(iconBox);

            var titleLabel = new Label
            {
                Text = title,
                Left = 74,
                Top = 20,
                Width = 160,
                Height = 18,
                ForeColor = ColorTranslator.FromHtml("#697386"),
                Font = new Font("Segoe UI", 7),
                BackColor = Color.Transparent
            };

            valueLabel = new Label
            {
                Text = value,
                Left = 74,
                Top = 38,
                Width = 150,
                Height = 26,
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                BackColor = Color.Transparent
            };

            card.Controls.Add(titleLabel);
            card.Controls.Add(valueLabel);
            return card;
        }

        private Button CreateSidebarButton(string text, int top, bool active, Action<Graphics, Rectangle, Color> drawIcon)
        {
            var btn = new Button
            {
                Text = text,
                Left = 10,
                Top = top,
                Width = 132,
                Height = 31,
                Padding = new Padding(30, 0, 0, 0),
                FlatStyle = FlatStyle.Flat,
                BackColor = active ? ColorTranslator.FromHtml("#2D6CEE") : ColorTranslator.FromHtml("#1F416C"),
                ForeColor = Color.White,
                TextAlign = ContentAlignment.MiddleLeft,
                Cursor = Cursors.Hand,
                Font = new Font("Segoe UI", 7, FontStyle.Bold),
                Tag = text
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = ColorTranslator.FromHtml("#2D6CEE");
            btn.Paint += (s, e) => drawIcon(e.Graphics, new Rectangle(12, 8, 15, 15), ColorTranslator.FromHtml("#D6E2FF"));
            btn.Region = new Region(GetRoundedPath(new Rectangle(0, 0, btn.Width, btn.Height), 6));
            return btn;
        }

        private void WireNavigation()
        {
            foreach (Control control in sidebar.Controls)
            {
                if (control is not Button btn) continue;
                var text = btn.Tag?.ToString() ?? string.Empty;
                if (text == "Zboruri") btn.Click += (s, e) => new ZboruriForm().ShowDialog(this);
                if (text == "Pasageri") btn.Click += (s, e) => new PasageriForm().ShowDialog(this);
                if (text == "Resurse") btn.Click += (s, e) => new ResurseForm().ShowDialog(this);
                if (text == "Alerte") btn.Click += (s, e) => new AlerteForm().ShowDialog(this);
                if (text == "Rapoarte") btn.Click += (s, e) => new RapoarteForm().ShowDialog(this);
                if (text == "Profil") btn.Click += (s, e) => new ProfilForm(_user).ShowDialog(this);
                if (text == "Administrare") btn.Click += (s, e) => MessageBox.Show("Sectiunea Administrare este disponibila doar pentru administratori.", "Acces restrictionat");
            }
        }

        private void BuildFlightsGrid()
        {
            dgvFlights.Columns.Clear();
            dgvFlights.Columns.Add(new DataGridViewTextBoxColumn { Name = "nr", HeaderText = "Nr. Zbor", DataPropertyName = "nr", FillWeight = 11 });
            dgvFlights.Columns.Add(new DataGridViewTextBoxColumn { Name = "tip", HeaderText = "Tip", DataPropertyName = "tip", FillWeight = 10 });
            dgvFlights.Columns.Add(new DataGridViewTextBoxColumn { Name = "companie", HeaderText = "Companie", DataPropertyName = "companie", FillWeight = 13 });
            dgvFlights.Columns.Add(new DataGridViewTextBoxColumn { Name = "ruta", HeaderText = "Ruta", DataPropertyName = "ruta", FillWeight = 21 });
            dgvFlights.Columns.Add(new DataGridViewTextBoxColumn { Name = "programat", HeaderText = "Programat", DataPropertyName = "programat", FillWeight = 12 });
            dgvFlights.Columns.Add(new DataGridViewTextBoxColumn { Name = "estimat", HeaderText = "Estimat", DataPropertyName = "estimat", FillWeight = 11 });
            dgvFlights.Columns.Add(new DataGridViewTextBoxColumn { Name = "poarta", HeaderText = "Poarta", DataPropertyName = "poarta", FillWeight = 9 });
            dgvFlights.Columns.Add(new DataGridViewTextBoxColumn { Name = "pista", HeaderText = "Pista", DataPropertyName = "pista", FillWeight = 9 });
            dgvFlights.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "Status", DataPropertyName = "status", FillWeight = 14 });
        }

        private void LoadDashboardData()
        {
            var flights = LoadFlightsTable();
            dgvFlights.DataSource = flights;

            lblFlightsValue.Text = Math.Max(142, flights.Rows.Count).ToString();
            lblPassengersValue.Text = "8,432";
            lblDelaysValue.Text = CountStatus(flights, "intarziat").ToString();
            lblGatesValue.Text = "12/18";

            LoadAlerts();
        }

        private DataTable LoadFlightsTable()
        {
            var table = CreateFlightsTable();
            try
            {
                var source = _zboruriController.GetAll();
                foreach (DataRow row in source.Rows)
                {
                    var cod = row.Table.Columns.Contains("cod") ? row["cod"]?.ToString() ?? string.Empty : string.Empty;
                    var src = row.Table.Columns.Contains("sursa") ? row["sursa"]?.ToString() ?? string.Empty : string.Empty;
                    var dst = row.Table.Columns.Contains("destinatie") ? row["destinatie"]?.ToString() ?? string.Empty : string.Empty;
                    var plecare = GetDate(row, "plecare");
                    var sosire = GetDate(row, "sosire");
                    var status = row.Table.Columns.Contains("status") ? row["status"]?.ToString() ?? "Programat" : "Programat";

                    table.Rows.Add(
                        string.IsNullOrWhiteSpace(cod) ? $"FL{table.Rows.Count + 1000}" : cod,
                        ResolveFlightType(src, dst),
                        ResolveAirline(table.Rows.Count),
                        $"{src} -> {dst}",
                        plecare.ToString("HH:mm"),
                        sosire.ToString("HH:mm"),
                        ResolveGate(table.Rows.Count),
                        ResolveRunway(table.Rows.Count),
                        NormalizeStatus(status)
                    );
                }
            }
            catch
            {
            }

            if (table.Rows.Count == 0)
            {
                table.Rows.Add("RO123", "Sosire", "TAROM", "Bucuresti -> Cluj", "14:30", "14:30", "A3", "08R", "La timp");
                table.Rows.Add("W63456", "Plecare", "Wizz Air", "Cluj -> Londra", "15:00", "15:25", "B7", "26L", "Intarziat");
                table.Rows.Add("FR7821", "Sosire", "Ryanair", "Milano -> Cluj", "15:15", "15:10", "A5", "08R", "Aterizare");
                table.Rows.Add("LH1234", "Plecare", "Lufthansa", "Cluj -> Munchen", "16:00", "16:00", "B2", "26L", "Imbarcare");
                table.Rows.Add("OS891", "Sosire", "Austrian", "Viena -> Cluj", "16:30", "16:30", "A1", "08R", "Programat");
            }

            return table;
        }

        private DataTable CreateFlightsTable()
        {
            var table = new DataTable();
            table.Columns.Add("nr");
            table.Columns.Add("tip");
            table.Columns.Add("companie");
            table.Columns.Add("ruta");
            table.Columns.Add("programat");
            table.Columns.Add("estimat");
            table.Columns.Add("poarta");
            table.Columns.Add("pista");
            table.Columns.Add("status");
            return table;
        }

        private void LoadAlerts()
        {
            alertsList.Controls.Clear();
            try
            {
                var source = _alerteController.GetAll();
                var added = 0;
                foreach (DataRow row in source.Rows)
                {
                    if (added >= 5) break;
                    var message = row.Table.Columns.Contains("mesaj") ? row["mesaj"]?.ToString() ?? string.Empty : string.Empty;
                    var date = GetDate(row, "data");
                    alertsList.Controls.Add(CreateAlertItem(message, date.ToString("HH:mm")));
                    added++;
                }
            }
            catch
            {
            }

            if (alertsList.Controls.Count == 0)
            {
                alertsList.Controls.Add(CreateAlertItem("Zbor W63456 intarziat cu 25 min", "14:05"));
                alertsList.Controls.Add(CreateAlertItem("Poarta A8 in mentenanta", "13:30"));
                alertsList.Controls.Add(CreateAlertItem("Bagaj pierdut zbor RO789", "13:15"));
                alertsList.Controls.Add(CreateAlertItem("Check-in automat B indisponibil", "12:45"));
                alertsList.Controls.Add(CreateAlertItem("Pasager medical zbor FR234", "12:20"));
            }
        }

        private Control CreateAlertItem(string message, string time)
        {
            var panel = new Panel
            {
                Width = Math.Max(230, alertsList.ClientSize.Width - 6),
                Height = 43,
                Margin = new Padding(0, 0, 0, 9),
                BackColor = ColorTranslator.FromHtml("#F6F7F9")
            };

            var accent = new Panel
            {
                Left = 0,
                Top = 0,
                Width = 3,
                Height = panel.Height,
                BackColor = ColorTranslator.FromHtml("#2D7DFF")
            };
            panel.Controls.Add(accent);

            var lblMessage = new Label
            {
                Text = message,
                Left = 10,
                Top = 8,
                Width = panel.Width - 20,
                Height = 17,
                ForeColor = ColorTranslator.FromHtml("#263141"),
                Font = new Font("Segoe UI", 7),
                AutoEllipsis = true
            };
            panel.Controls.Add(lblMessage);

            var lblTime = new Label
            {
                Text = time,
                Left = 10,
                Top = 25,
                Width = panel.Width - 20,
                Height = 14,
                ForeColor = ColorTranslator.FromHtml("#718096"),
                Font = new Font("Segoe UI", 6)
            };
            panel.Controls.Add(lblTime);

            return panel;
        }

        private void DgvFlights_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || dgvFlights.Columns[e.ColumnIndex].Name != "status") return;
            var status = e.Value?.ToString() ?? string.Empty;
            e.CellStyle.ForeColor = Color.White;
            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            e.CellStyle.Font = new Font("Segoe UI", 6, FontStyle.Bold);

            if (status.Equals("La timp", StringComparison.OrdinalIgnoreCase))
                e.CellStyle.BackColor = ColorTranslator.FromHtml("#0BCF6A");
            else if (status.Equals("Intarziat", StringComparison.OrdinalIgnoreCase))
                e.CellStyle.BackColor = ColorTranslator.FromHtml("#FF3045");
            else if (status.Equals("Aterizare", StringComparison.OrdinalIgnoreCase))
                e.CellStyle.BackColor = ColorTranslator.FromHtml("#2D7DFF");
            else if (status.Equals("Imbarcare", StringComparison.OrdinalIgnoreCase))
                e.CellStyle.BackColor = ColorTranslator.FromHtml("#F4B000");
            else
                e.CellStyle.BackColor = ColorTranslator.FromHtml("#6B7280");
        }

        private int CountStatus(DataTable table, string statusPart)
        {
            var count = 0;
            foreach (DataRow row in table.Rows)
            {
                var status = row["status"]?.ToString() ?? string.Empty;
                if (status.ToLower().Contains(statusPart)) count++;
            }
            return Math.Max(7, count);
        }

        private DateTime GetDate(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value) return DateTime.Now;
            return DateTime.TryParse(row[column].ToString(), out var date) ? date : DateTime.Now;
        }

        private string ResolveFlightType(string source, string destination)
        {
            if (string.IsNullOrWhiteSpace(source)) return "Sosire";
            return source.ToLower().Contains("cluj") ? "Plecare" : "Sosire";
        }

        private string ResolveAirline(int index)
        {
            var airlines = new[] { "TAROM", "Wizz Air", "Ryanair", "Lufthansa", "Austrian", "HiSky" };
            return airlines[index % airlines.Length];
        }

        private string ResolveGate(int index)
        {
            var gates = new[] { "A3", "B7", "A5", "B2", "A1", "C4" };
            return gates[index % gates.Length];
        }

        private string ResolveRunway(int index)
        {
            var runways = new[] { "08R", "26L" };
            return runways[index % runways.Length];
        }

        private string NormalizeStatus(string status)
        {
            var normalized = (status ?? string.Empty).Trim().ToLower();
            if (normalized.Contains("intar") || normalized.Contains("delay")) return "Intarziat";
            if (normalized.Contains("ater")) return "Aterizare";
            if (normalized.Contains("imbar") || normalized.Contains("boarding")) return "Imbarcare";
            if (normalized.Contains("timp") || normalized.Contains("on time")) return "La timp";
            return string.IsNullOrWhiteSpace(status) ? "Programat" : status;
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

        private void DrawPlaneIcon(Graphics g, Rectangle r, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(color, 2) { StartCap = LineCap.Round, EndCap = LineCap.Round };
            g.DrawLine(pen, r.Left + r.Width / 2, r.Top + 2, r.Right - 4, r.Bottom - 4);
            g.DrawLine(pen, r.Left + 3, r.Top + 6, r.Left + r.Width / 2, r.Top + r.Height / 2);
            g.DrawLine(pen, r.Left + r.Width / 2, r.Top + r.Height / 2, r.Left + 6, r.Bottom - 2);
            g.DrawLine(pen, r.Left + r.Width / 2 + 1, r.Top + r.Height / 2, r.Right - 2, r.Top + 5);
        }

        private void DrawGridIcon(Graphics g, Rectangle r, Color color)
        {
            using var pen = new Pen(color, 1.6f);
            var size = 5;
            g.DrawRectangle(pen, r.Left, r.Top, size, size);
            g.DrawRectangle(pen, r.Right - size, r.Top, size, size);
            g.DrawRectangle(pen, r.Left, r.Bottom - size, size, size);
            g.DrawRectangle(pen, r.Right - size, r.Bottom - size, size, size);
        }

        private void DrawUsersIcon(Graphics g, Rectangle r, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(color, 1.8f);
            g.DrawEllipse(pen, r.Left + 7, r.Top + 1, 7, 7);
            g.DrawArc(pen, r.Left + 4, r.Top + 10, 14, 9, 190, 160);
            g.DrawEllipse(pen, r.Left, r.Top + 4, 6, 6);
            g.DrawArc(pen, r.Left - 2, r.Top + 12, 10, 7, 190, 150);
        }

        private void DrawWarningIcon(Graphics g, Rectangle r, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(color, 2);
            var points = new[] { new Point(r.Left + r.Width / 2, r.Top + 2), new Point(r.Right - 2, r.Bottom - 2), new Point(r.Left + 2, r.Bottom - 2) };
            g.DrawPolygon(pen, points);
            g.DrawLine(pen, r.Left + r.Width / 2, r.Top + 8, r.Left + r.Width / 2, r.Bottom - 7);
            g.DrawEllipse(pen, r.Left + r.Width / 2 - 1, r.Bottom - 5, 2, 2);
        }

        private void DrawClockIcon(Graphics g, Rectangle r, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(color, 2);
            g.DrawEllipse(pen, r.Left + 2, r.Top + 2, r.Width - 4, r.Height - 4);
            g.DrawLine(pen, r.Left + r.Width / 2, r.Top + 6, r.Left + r.Width / 2, r.Top + r.Height / 2);
            g.DrawLine(pen, r.Left + r.Width / 2, r.Top + r.Height / 2, r.Right - 6, r.Bottom - 7);
        }

        private void DrawMapIcon(Graphics g, Rectangle r, Color color)
        {
            using var pen = new Pen(color, 1.6f);
            g.DrawLine(pen, r.Left + 2, r.Top + 2, r.Left + 2, r.Bottom - 2);
            g.DrawLine(pen, r.Left + 7, r.Top + 4, r.Left + 7, r.Bottom);
            g.DrawLine(pen, r.Right - 3, r.Top + 1, r.Right - 3, r.Bottom - 3);
            g.DrawLine(pen, r.Left + 2, r.Top + 2, r.Left + 7, r.Top + 4);
            g.DrawLine(pen, r.Left + 7, r.Bottom, r.Right - 3, r.Bottom - 3);
        }

        private void DrawBellIcon(Graphics g, Rectangle r, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(color, 1.7f);
            g.DrawArc(pen, r.Left + 3, r.Top + 3, r.Width - 6, r.Height - 4, 200, 140);
            g.DrawLine(pen, r.Left + 4, r.Top + 10, r.Left + 3, r.Bottom - 4);
            g.DrawLine(pen, r.Right - 4, r.Top + 10, r.Right - 3, r.Bottom - 4);
            g.DrawLine(pen, r.Left + 2, r.Bottom - 4, r.Right - 2, r.Bottom - 4);
            g.DrawArc(pen, r.Left + 6, r.Bottom - 6, 4, 4, 0, 180);
        }

        private void DrawChartIcon(Graphics g, Rectangle r, Color color)
        {
            using var pen = new Pen(color, 1.7f);
            g.DrawLine(pen, r.Left + 2, r.Top + 2, r.Left + 2, r.Bottom - 2);
            g.DrawLine(pen, r.Left + 2, r.Bottom - 2, r.Right - 2, r.Bottom - 2);
            g.DrawLine(pen, r.Left + 5, r.Bottom - 5, r.Left + 5, r.Bottom - 8);
            g.DrawLine(pen, r.Left + 10, r.Bottom - 5, r.Left + 10, r.Top + 6);
            g.DrawLine(pen, r.Left + 15, r.Bottom - 5, r.Left + 15, r.Top + 10);
        }

        private void DrawUserIcon(Graphics g, Rectangle r, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(color, 1.7f);
            g.DrawEllipse(pen, r.Left + 5, r.Top + 2, 7, 7);
            g.DrawArc(pen, r.Left + 2, r.Top + 11, 14, 9, 200, 140);
        }

        private void DrawShieldIcon(Graphics g, Rectangle r, Color color)
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            using var pen = new Pen(color, 1.7f);
            var points = new[] { new Point(r.Left + r.Width / 2, r.Top + 1), new Point(r.Right - 3, r.Top + 4), new Point(r.Right - 4, r.Bottom - 5), new Point(r.Left + r.Width / 2, r.Bottom - 1), new Point(r.Left + 3, r.Bottom - 5), new Point(r.Left + 2, r.Top + 4) };
            g.DrawPolygon(pen, points);
        }

        private void DrawLogoutIcon(Graphics g, Rectangle r, Color color)
        {
            using var pen = new Pen(color, 1.5f);
            g.DrawRectangle(pen, r.Left, r.Top + 2, 7, r.Height - 4);
            g.DrawLine(pen, r.Left + 6, r.Top + r.Height / 2, r.Right - 2, r.Top + r.Height / 2);
            g.DrawLine(pen, r.Right - 5, r.Top + 4, r.Right - 2, r.Top + r.Height / 2);
            g.DrawLine(pen, r.Right - 5, r.Bottom - 4, r.Right - 2, r.Top + r.Height / 2);
        }

        private class RoundedPanel : Panel
        {
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int Radius { get; set; } = 8;

            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Color BorderColor { get; set; } = ColorTranslator.FromHtml("#E7EAF0");

            public RoundedPanel()
            {
                DoubleBuffered = true;
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using var path = BuildPath(rect, Radius);
                using var brush = new SolidBrush(BackColor);
                using var pen = new Pen(BorderColor, 1);
                e.Graphics.FillPath(brush, path);
                e.Graphics.DrawPath(pen, path);
                Region = new Region(path);
            }

            private static GraphicsPath BuildPath(Rectangle r, int radius)
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
}
