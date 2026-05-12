using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using AirportManagement.Controllers;

namespace AirportManagement.Views
{
    public class RapoarteForm : Form
    {
        private ComboBox cmbReport;
        private Button btnGen;
        private DataGridView dgv;
        private ZboruriController _zController = new ZboruriController();
        private PasageriController _pController = new PasageriController();

        public RapoarteForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Rapoarte"; Width = 900; Height = 600; StartPosition = FormStartPosition.CenterParent; Font = new Font("Segoe UI", 9);
            cmbReport = new ComboBox { Left = 20, Top = 20, Width = 300, DropDownStyle = ComboBoxStyle.DropDownList };
            cmbReport.Items.AddRange(new object[] { "Zboruri (lista)", "Pasageri per zbor" }); cmbReport.SelectedIndex = 0;
            btnGen = new Button { Text = "Generează", Left = 340, Top = 18, Width = 120 };
            btnGen.Click += BtnGen_Click;

            dgv = new DataGridView { Left = 20, Top = 60, Width = 840, Height = 480, Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };

            Controls.AddRange(new Control[] { cmbReport, btnGen, dgv });
        }

        private void BtnGen_Click(object? sender, EventArgs e)
        {
            if (cmbReport.SelectedIndex == 0)
            {
                dgv.DataSource = _zController.GetAll();
            }
            else
            {
                var pdt = _pController.GetAll();
                var zdt = _zController.GetAll();
                var result = new DataTable();
                result.Columns.Add("zbor_cod");
                result.Columns.Add("pasageri_count", typeof(int));

                var groups = pdt.AsEnumerable().GroupBy(r => r.Field<long?>("zborid")).Select(g => new { ZborId = g.Key, Count = g.Count() });
                foreach (var g in groups)
                {
                    var cod = zdt.AsEnumerable().FirstOrDefault(r => r.Field<long?>("id") == g.ZborId)?.Field<string>("cod") ?? (g.ZborId?.ToString() ?? "-");
                    result.Rows.Add(cod, g.Count);
                }

                dgv.DataSource = result;
            }
        }
    }
}
