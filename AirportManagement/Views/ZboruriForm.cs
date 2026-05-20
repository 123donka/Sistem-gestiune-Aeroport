using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class ZboruriForm : Form
    {
        private readonly ZboruriController _controller = new ZboruriController();
        private DataTable _source = new DataTable();
        private bool _darkMode;

        private Panel header = null!;
        private Panel filterCard = null!;
        private Panel tableCard = null!;
        private DataGridView dgv = null!;
        private TextBox txtSearch = null!;
        private ComboBox cmbStatus = null!;
        private ComboBox cmbCompany = null!;
        private DateTimePicker dtDate = null!;
        private Button btnAdd = null!;
        private Button btnApply = null!;
        private Button btnReset = null!;

        public ZboruriForm()
        {
            InitializeComponent();
            LoadData();
        }

        public void SetDarkMode(bool darkMode)
        {
            _darkMode = darkMode;

            var pageBack = _darkMode ? ColorTranslator.FromHtml("#111827") : ColorTranslator.FromHtml("#F3F6FB");
            var cardBack = _darkMode ? ColorTranslator.FromHtml("#1F2937") : Color.White;
            var cardBorder = _darkMode ? ColorTranslator.FromHtml("#374151") : ColorTranslator.FromHtml("#E5E7EB");
            var headerBack = _darkMode ? ColorTranslator.FromHtml("#0F172A") : Color.White;
            var titleColor = _darkMode ? Color.White : ColorTranslator.FromHtml("#111827");
            var subtitleColor = _darkMode ? ColorTranslator.FromHtml("#CBD5E1") : ColorTranslator.FromHtml("#64748B");
            var labelColor = _darkMode ? ColorTranslator.FromHtml("#CBD5E1") : ColorTranslator.FromHtml("#475569");
            var controlBack = _darkMode ? ColorTranslator.FromHtml("#0F172A") : Color.White;
            var controlFore = _darkMode ? Color.White : ColorTranslator.FromHtml("#334155");
            var gridHeaderBack = _darkMode ? ColorTranslator.FromHtml("#334155") : ColorTranslator.FromHtml("#F8FAFC");
            var gridHeaderFore = _darkMode ? Color.White : ColorTranslator.FromHtml("#0F172A");
            var gridBack = _darkMode ? ColorTranslator.FromHtml("#1F2937") : Color.White;
            var gridAlt = _darkMode ? ColorTranslator.FromHtml("#273244") : ColorTranslator.FromHtml("#FAFBFD");
            var gridSelection = _darkMode ? ColorTranslator.FromHtml("#334155") : ColorTranslator.FromHtml("#EAF1FF");
            var gridText = _darkMode ? Color.White : ColorTranslator.FromHtml("#111827");

            BackColor = pageBack;
            header.BackColor = headerBack;
            filterCard.BackColor = cardBack;
            tableCard.BackColor = cardBack;

            foreach (Control control in header.Controls)
            {
                if (control is Label lbl)
                {
                    lbl.ForeColor = lbl.Text == "Gestionare Zboruri" ? titleColor : subtitleColor;
                }
            }

            foreach (Control control in filterCard.Controls)
            {
                if (control is Label lbl) lbl.ForeColor = labelColor;
                if (control is TextBox txt)
                {
                    txt.BackColor = controlBack;
                    txt.ForeColor = controlFore;
                }
                if (control is ComboBox cmb)
                {
                    cmb.BackColor = controlBack;
                    cmb.ForeColor = controlFore;
                }
                if (control is DateTimePicker dt)
                {
                    dt.CalendarForeColor = controlFore;
                    dt.CalendarMonthBackground = controlBack;
                    dt.BackColor = controlBack;
                    dt.ForeColor = controlFore;
                }
                if (control is Button btn)
                {
                    if (btn == btnApply)
                    {
                        btn.BackColor = _darkMode ? ColorTranslator.FromHtml("#2563EB") : ColorTranslator.FromHtml("#2563EB");
                    }
                    else if (btn == btnReset)
                    {
                        btn.BackColor = _darkMode ? ColorTranslator.FromHtml("#1E293B") : Color.White;
                        btn.ForeColor = controlFore;
                    }
                }
            }

            dgv.BackgroundColor = cardBack;
            dgv.GridColor = cardBorder;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = gridHeaderBack;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = gridHeaderFore;
            dgv.DefaultCellStyle.BackColor = gridBack;
            dgv.DefaultCellStyle.ForeColor = gridText;
            dgv.DefaultCellStyle.SelectionBackColor = gridSelection;
            dgv.DefaultCellStyle.SelectionForeColor = gridText;
            dgv.AlternatingRowsDefaultCellStyle.BackColor = _darkMode ? ColorTranslator.FromHtml("#243041") : ColorTranslator.FromHtml("#FAFBFD");

            foreach (DataGridViewColumn column in dgv.Columns)
            {
                if (column is DataGridViewButtonColumn) continue;
                column.DefaultCellStyle.BackColor = gridBack;
                column.DefaultCellStyle.ForeColor = gridText;
            }

            header.Invalidate();
            filterCard.Invalidate();
            tableCard.Invalidate();
            dgv.Invalidate();
        }

        private void InitializeComponent()
        {
            Text = "Zboruri";
            Width = 1260;
            Height = 760;
            StartPosition = FormStartPosition.CenterParent;
            Font = new Font("Segoe UI", 9F);
            BackColor = ColorTranslator.FromHtml("#F3F6FB");
            MinimumSize = new Size(1120, 680);

            header = new Panel
            {
                Dock = DockStyle.Top,
                Height = 82,
                BackColor = Color.White,
                Padding = new Padding(22, 14, 22, 12)
            };

            var title = new Label
            {
                Text = "Gestionare Zboruri",
                AutoSize = true,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#111827"),
                Left = 0,
                Top = 0
            };

            var subtitle = new Label
            {
                Text = "Vizualizeaza, editeaza si monitorizeaza zborurile in timp real.",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F),
                ForeColor = ColorTranslator.FromHtml("#64748B"),
                Left = 2,
                Top = 34
            };

            btnAdd = new Button
            {
                Text = "+  Adauga Zbor",
                Width = 132,
                Height = 38,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                BackColor = ColorTranslator.FromHtml("#22C55E"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.Click += BtnAdd_Click;

            header.Controls.Add(title);
            header.Controls.Add(subtitle);
            header.Controls.Add(btnAdd);

            filterCard = CreateCard();
            filterCard.Dock = DockStyle.Top;
            filterCard.Height = 92;
            filterCard.Padding = new Padding(16, 14, 16, 14);

            var lblDate = CreateLabel("Data");
            lblDate.Left = 18;
            lblDate.Top = 14;

            dtDate = new DateTimePicker
            {
                Left = 18,
                Top = 36,
                Width = 160,
                Format = DateTimePickerFormat.Short,
                ShowCheckBox = true,
                Checked = false
            };

            var lblCompany = CreateLabel("Companie");
            lblCompany.Left = 196;
            lblCompany.Top = 14;

            cmbCompany = new ComboBox
            {
                Left = 196,
                Top = 36,
                Width = 210,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbCompany.Items.AddRange(new object[] { "Toate" });
            cmbCompany.SelectedIndex = 0;

            var lblStatus = CreateLabel("Status");
            lblStatus.Left = 424;
            lblStatus.Top = 14;

            cmbStatus = new ComboBox
            {
                Left = 424,
                Top = 36,
                Width = 190,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStatus.Items.AddRange(new object[] { "Toate", "Live", "La timp", "Intarziat", "Imbarcare", "Programat" });
            cmbStatus.SelectedIndex = 0;

            var lblSearch = CreateLabel("Cautare");
            lblSearch.Left = 632;
            lblSearch.Top = 14;

            txtSearch = new TextBox
            {
                Left = 632,
                Top = 36,
                Width = 250,
                PlaceholderText = "Nr. zbor, ruta sau status..."
            };

            btnApply = new Button
            {
                Text = "Aplica",
                Width = 84,
                Height = 34,
                Left = 904,
                Top = 37,
                BackColor = ColorTranslator.FromHtml("#2563EB"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnApply.FlatAppearance.BorderSize = 0;
            btnApply.Click += (s, e) => ApplyFilters();

            btnReset = new Button
            {
                Text = "Reseteaza",
                Width = 92,
                Height = 34,
                Left = 994,
                Top = 37,
                BackColor = Color.White,
                ForeColor = ColorTranslator.FromHtml("#334155"),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnReset.FlatAppearance.BorderColor = ColorTranslator.FromHtml("#CBD5E1");
            btnReset.Click += (s, e) =>
            {
                txtSearch.Clear();
                cmbStatus.SelectedIndex = 0;
                cmbCompany.SelectedIndex = 0;
                dtDate.Checked = false;
                ApplyFilters();
            };

            filterCard.Controls.AddRange(new Control[]
            {
                lblDate, dtDate,
                lblCompany, cmbCompany,
                lblStatus, cmbStatus,
                lblSearch, txtSearch,
                btnApply, btnReset
            });

            tableCard = CreateCard();
            tableCard.Dock = DockStyle.Fill;
            tableCard.Padding = new Padding(16, 14, 16, 16);

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AutoGenerateColumns = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#F8FAFC");
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#0F172A");
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgv.ColumnHeadersHeight = 42;
            dgv.RowTemplate.Height = 38;
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 8.5F);
            dgv.DefaultCellStyle.ForeColor = ColorTranslator.FromHtml("#111827");
            dgv.DefaultCellStyle.SelectionBackColor = ColorTranslator.FromHtml("#EAF1FF");
            dgv.DefaultCellStyle.SelectionForeColor = ColorTranslator.FromHtml("#111827");
            dgv.AlternatingRowsDefaultCellStyle.BackColor = ColorTranslator.FromHtml("#FAFBFD");
            dgv.GridColor = ColorTranslator.FromHtml("#E5E7EB");
            dgv.CellFormatting += Dgv_CellFormatting;
            dgv.CellContentClick += Dgv_CellContentClick;
            dgv.CellPainting += Dgv_CellPainting;

            BuildGrid();
            tableCard.Controls.Add(dgv);

            Controls.Add(tableCard);
            Controls.Add(filterCard);
            Controls.Add(header);

            Resize += (s, e) => LayoutHeader();
            header.Resize += (s, e) => LayoutHeader();
            LayoutHeader();
        }

        private void LayoutHeader()
        {
            btnAdd.Left = Math.Max(0, header.ClientSize.Width - btnAdd.Width - 10);
            btnAdd.Top = 22;
        }

        private static Panel CreateCard()
        {
            var panel = new Panel
            {
                BackColor = Color.White,
                Margin = new Padding(16, 12, 16, 0)
            };
            panel.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                var rect = new Rectangle(0, 0, panel.Width - 1, panel.Height - 1);
                using var path = RoundedRect(rect, 10);
                var borderColor = panel.BackColor == Color.White
                    ? ColorTranslator.FromHtml("#E5E7EB")
                    : ColorTranslator.FromHtml("#374151");
                using var border = new Pen(borderColor, 1);
                e.Graphics.DrawPath(border, path);
            };
            return panel;
        }

        private static Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = ColorTranslator.FromHtml("#475569")
            };
        }

        private void BuildGrid()
        {
            dgv.Columns.Clear();
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", DataPropertyName = "id", Visible = false });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "nr_zbor", HeaderText = "Nr. Zbor", DataPropertyName = "nr_zbor", FillWeight = 12 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "companie_aeriana", HeaderText = "Companie", DataPropertyName = "companie_aeriana", FillWeight = 16 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "oras_origine", HeaderText = "Origine", DataPropertyName = "oras_origine", FillWeight = 15 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "oras_destinatie", HeaderText = "Destinatie", DataPropertyName = "oras_destinatie", FillWeight = 15 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "decolare", HeaderText = "Decolare", DataPropertyName = "decolare", FillWeight = 13 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "aterizare", HeaderText = "Aterizare", DataPropertyName = "aterizare", FillWeight = 13 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "status", HeaderText = "Status", DataPropertyName = "status", FillWeight = 12 });
            dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "edit",
                HeaderText = "Actiuni",
                Text = "Editeaza",
                UseColumnTextForButtonValue = true,
                FillWeight = 10,
                MinimumWidth = 110
            });
            dgv.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "delete",
                HeaderText = "",
                Text = "Sterge",
                UseColumnTextForButtonValue = true,
                FillWeight = 8,
                MinimumWidth = 100
            });
        }

        private void LoadData()
        {
            _source = _controller.GetAll();
            RefreshCompanyFilter();
            ApplyFilters();
        }

        private void RefreshCompanyFilter()
        {
            var current = cmbCompany.SelectedItem?.ToString() ?? "Toate";
            cmbCompany.Items.Clear();
            cmbCompany.Items.Add("Toate");

            foreach (var company in _source.AsEnumerable()
                         .Select(r => GetText(r, "companie_aeriana"))
                         .Where(v => !string.IsNullOrWhiteSpace(v))
                         .Distinct(StringComparer.OrdinalIgnoreCase)
                         .OrderBy(v => v))
            {
                cmbCompany.Items.Add(company);
            }

            cmbCompany.SelectedItem = cmbCompany.Items.Contains(current) ? current : "Toate";
            if (cmbCompany.SelectedIndex < 0) cmbCompany.SelectedIndex = 0;
        }

        private void ApplyFilters()
        {
            var display = CreateDisplayTable();
            var search = txtSearch.Text.Trim().ToLowerInvariant();
            var statusFilter = cmbStatus.SelectedItem?.ToString() ?? "Toate";
            var companyFilter = cmbCompany.SelectedItem?.ToString() ?? "Toate";
            var dateEnabled = dtDate.Checked;
            var dateValue = dtDate.Value.Date;

            foreach (DataRow row in _source.Rows)
            {
                var id = GetInt(row, "id");
                var cod = GetText(row, "numar_zbor");
                var companie = GetText(row, "companie_aeriana");
                var sursa = GetText(row, "oras_origine");
                var destinatie = GetText(row, "oras_destinatie");
                var plecare = GetDate(row, "data_ora_programata");
                var sosire = GetDate(row, "data_ora_estimata");
                var status = NormalizeStatus(GetText(row, "status"));

                if (dateEnabled && plecare.Date != dateValue && sosire.Date != dateValue) continue;
                if (!string.Equals(statusFilter, "Toate", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(status, statusFilter, StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.Equals(companyFilter, "Toate", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(companie, companyFilter, StringComparison.OrdinalIgnoreCase)) continue;
                if (!string.IsNullOrWhiteSpace(search) &&
                    !Contains(search, cod, companie, sursa, destinatie, status)) continue;

                display.Rows.Add(
                    id,
                    string.IsNullOrWhiteSpace(cod) ? "-" : cod,
                    string.IsNullOrWhiteSpace(companie) ? "-" : companie,
                    string.IsNullOrWhiteSpace(sursa) ? "-" : sursa,
                    string.IsNullOrWhiteSpace(destinatie) ? "-" : destinatie,
                    FormatTime(plecare),
                    FormatTime(sosire),
                    status
                );
            }

            dgv.DataSource = display;
            dgv.Columns["id"].Visible = false;
        }

        private static DataTable CreateDisplayTable()
        {
            var table = new DataTable();
            table.Columns.Add("id", typeof(int));
            table.Columns.Add("nr_zbor");
            table.Columns.Add("companie_aeriana");
            table.Columns.Add("oras_origine");
            table.Columns.Add("oras_destinatie");
            table.Columns.Add("decolare");
            table.Columns.Add("aterizare");
            table.Columns.Add("status");
            return table;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var dlg = new ZborEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                _controller.Create(dlg.ZborResult!);
                LoadData();
            }
        }

        private void Dgv_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var colName = dgv.Columns[e.ColumnIndex].Name;
            if (colName != "edit" && colName != "delete") return;

            var row = dgv.Rows[e.RowIndex];
            var z = GetSourceFlightById(GetRowInt(row, "id"));
            if (z == null) return;

            if (colName == "edit")
            {
                var dlg = new ZborEditForm(z);
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    _controller.Update(dlg.ZborResult!);
                    LoadData();
                }
            }
            else
            {
                var confirm = MessageBox.Show("Stergi acest zbor?", "Confirmare", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (confirm == DialogResult.Yes)
                {
                    _controller.Delete(z.Id);
                    LoadData();
                }
            }
        }

        private void Dgv_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var name = dgv.Columns[e.ColumnIndex].Name;
            if (name != "edit" && name != "delete") return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);
            var buttonRect = new Rectangle(e.CellBounds.Left + 12, e.CellBounds.Top + 7, e.CellBounds.Width - 24, e.CellBounds.Height - 14);
            var isEdit = name == "edit";
            var fill = isEdit ? ColorTranslator.FromHtml("#2563EB") : ColorTranslator.FromHtml("#EF4444");
            var text = isEdit ? "Editeaza" : "Sterge";

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var path = RoundedRect(buttonRect, 8);
            using var brush = new SolidBrush(fill);
            e.Graphics.FillPath(brush, path);
            TextRenderer.DrawText(
                e.Graphics,
                text,
                new Font("Segoe UI", 8.5F, FontStyle.Bold),
                buttonRect,
                Color.White,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis
            );
            e.Handled = true;
        }

        private void Dgv_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var name = dgv.Columns[e.ColumnIndex].Name;
            if (name == "status")
            {
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                var status = e.Value?.ToString() ?? string.Empty;
                if (status.Equals("Live", StringComparison.OrdinalIgnoreCase))
                    e.CellStyle.BackColor = ColorTranslator.FromHtml("#10B981");
                else if (status.Equals("Intarziat", StringComparison.OrdinalIgnoreCase))
                    e.CellStyle.BackColor = ColorTranslator.FromHtml("#EF4444");
                else if (status.Equals("Imbarcare", StringComparison.OrdinalIgnoreCase))
                    e.CellStyle.BackColor = ColorTranslator.FromHtml("#F59E0B");
                else if (status.Equals("La timp", StringComparison.OrdinalIgnoreCase))
                    e.CellStyle.BackColor = ColorTranslator.FromHtml("#3B82F6");
                else
                    e.CellStyle.BackColor = ColorTranslator.FromHtml("#6B7280");
            }

            if (name == "edit" || name == "delete")
            {
                var bg = _darkMode ? ColorTranslator.FromHtml("#1F2937") : Color.White;
                e.CellStyle.BackColor = bg;
                e.CellStyle.SelectionBackColor = bg;
            }
        }

        private static string GetText(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) && row[column] != DBNull.Value ? row[column].ToString() ?? string.Empty : string.Empty;
        }

        private static int GetInt(DataRow row, string column)
        {
            return row.Table.Columns.Contains(column) && int.TryParse(row[column]?.ToString(), out var value) ? value : 0;
        }

        private static DateTime GetDate(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column) || row[column] == DBNull.Value) return DateTime.MinValue;
            return DateTime.TryParse(row[column].ToString(), out var date) ? date : DateTime.MinValue;
        }

        private static bool Contains(string search, params string[] values)
        {
            return values.Any(value => !string.IsNullOrWhiteSpace(value) && value.ToLowerInvariant().Contains(search));
        }

        private static string ResolveFlightType(string source, string destination)
        {
            if (string.IsNullOrWhiteSpace(source) && string.IsNullOrWhiteSpace(destination)) return "-";
            if (!string.IsNullOrWhiteSpace(source) && source.ToLowerInvariant().Contains("cluj")) return "Plecare";
            return "Sosire";
        }

        private static string FormatTime(DateTime date)
        {
            return date <= DateTime.MinValue ? "-" : date.ToString("HH:mm");
        }

        private static string NormalizeStatus(string status)
        {
            var normalized = (status ?? string.Empty).Trim().ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(normalized)) return "Programat";
            if (normalized.Contains("live")) return "Live";
            if (normalized.Contains("intar")) return "Intarziat";
            if (normalized.Contains("imbar")) return "Imbarcare";
            if (normalized.Contains("timp")) return "La timp";
            return char.ToUpper(normalized[0]) + normalized[1..];
        }

        private static int GetRowInt(DataGridViewRow row, string column)
        {
            return int.TryParse(row.Cells[column].Value?.ToString(), out var value) ? value : 0;
        }

        private static string GetRowText(DataGridViewRow row, string column)
        {
            return row.Cells[column].Value?.ToString() ?? string.Empty;
        }

        private Zbor? GetSourceFlightById(int id)
        {
            foreach (DataRow row in _source.Rows)
            {
                if (GetInt(row, "id") != id) continue;

                return new Zbor
                {
                    Id = id,
                    Cod = GetText(row, "numar_zbor"),
                    CompanieAeriana = GetText(row, "companie_aeriana"),
                    TipZbor = GetText(row, "tip_zbor"),
                    Sursa = GetText(row, "oras_origine"),
                    Destinatie = GetText(row, "oras_destinatie"),
                    Plecare = GetDate(row, "data_ora_programata"),
                    Sosire = GetDate(row, "data_ora_estimata"),
                    Status = GetText(row, "status")
                };
            }

            return null;
        }

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            var path = new GraphicsPath();
            var diameter = radius * 2;
            path.StartFigure();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
