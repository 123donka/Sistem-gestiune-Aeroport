using System;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Models;
using AirportManagement.Services;

namespace AirportManagement.Views
{
    public class PasagerEditForm : Form
    {
        private TextBox txtNume, txtTicket;
        private ComboBox cmbZbor;
        private CheckBox chkCheckedIn, chkBoarded;
        private Button btnOk, btnCancel;
        public Pasager? PasagerResult { get; private set; }

        private Pasager? _edit;

        public PasagerEditForm(Pasager? pasager = null)
        {
            _edit = pasager;
            InitializeComponent();
            LoadZboruri();
            if (pasager != null) Populate(pasager);
        }

        private void InitializeComponent()
        {
            Text = _edit == null ? "Adaugă pasager" : "Editează pasager";
            Width = 420; Height = 330; StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 9);

            var lblNume = new Label { Text = "Nume", Left = 20, Top = 20 };
            txtNume = new TextBox { Left = 20, Top = 40, Width = 360 };

            var lblTicket = new Label { Text = "Ticket Nr", Left = 20, Top = 80 };
            txtTicket = new TextBox { Left = 20, Top = 100, Width = 360 };

            var lblZbor = new Label { Text = "Zbor", Left = 20, Top = 140 };
            cmbZbor = new ComboBox { Left = 20, Top = 160, Width = 360, DropDownStyle = ComboBoxStyle.DropDownList };

            chkCheckedIn = new CheckBox { Text = "Checked-in", Left = 20, Top = 200 };
            chkBoarded = new CheckBox { Text = "Boarded", Left = 140, Top = 200 };

            btnOk = new Button { Text = "OK", Left = 60, Top = 240, Width = 120, BackColor = ColorTranslator.FromHtml("#2563EB"), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnOk.FlatAppearance.BorderSize = 0; btnOk.Click += BtnOk_Click;
            btnCancel = new Button { Text = "Cancel", Left = 220, Top = 240, Width = 120 }; btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

            Controls.AddRange(new Control[] { lblNume, txtNume, lblTicket, txtTicket, lblZbor, cmbZbor, chkCheckedIn, chkBoarded, btnOk, btnCancel });
        }

        private void LoadZboruri()
        {
            try
            {
                var service = new ZborService();
                var dt = service.GetAll();
                cmbZbor.DisplayMember = "cod"; cmbZbor.ValueMember = "id";
                cmbZbor.DataSource = dt;
                if (_edit == null && dt.Rows.Count > 0) cmbZbor.SelectedIndex = 0;
            }
            catch { }
        }

        private void Populate(Pasager p)
        {
            txtNume.Text = p.Nume;
            txtTicket.Text = p.TicketNr;
            chkCheckedIn.Checked = p.CheckedIn;
            chkBoarded.Checked = p.Boarded;
            try { cmbZbor.SelectedValue = p.ZborId; } catch { }
        }

        private void BtnOk_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNume.Text)) { MessageBox.Show("Numele este obligatoriu"); return; }
            if (cmbZbor.SelectedValue == null) { MessageBox.Show("Selectează un zbor"); return; }
            PasagerResult = new Pasager
            {
                Id = _edit?.Id ?? 0,
                Nume = txtNume.Text.Trim(),
                TicketNr = txtTicket.Text.Trim(),
                ZborId = Convert.ToInt32(cmbZbor.SelectedValue),
                CheckedIn = chkCheckedIn.Checked,
                Boarded = chkBoarded.Checked
            };
            this.DialogResult = DialogResult.OK;
        }
    }
}
