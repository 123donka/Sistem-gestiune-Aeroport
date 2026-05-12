using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Controllers;

namespace AirportManagement.Views
{
    public class AlerteForm : Form
    {
        private DataGridView dgv;
        private AlerteController _controller = new AlerteController();

        public AlerteForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Alerte"; Width = 800; Height = 600; StartPosition = FormStartPosition.CenterParent;
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            var pnlTop = new Panel { Height = 50, Dock = DockStyle.Top };
            var btnMark = new Button { Text = "Marchează ca citită", Left = 10, Width = 160, Top = 10 };
            btnMark.Click += BtnMark_Click;
            pnlTop.Controls.Add(btnMark);

            Controls.Add(dgv); Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            var dt = _controller.GetAll();
            dgv.DataSource = dt;
            if (dgv.Columns.Contains("citita")) dgv.Columns["citita"].HeaderText = "Citită";
        }

        private void BtnMark_Click(object? sender, EventArgs e)
        {
            if (dgv.CurrentRow == null) return;
            var id = Convert.ToInt32(dgv.CurrentRow.Cells["id"].Value);
            _controller.MarkAsRead(id);
            LoadData();
        }
    }
}
