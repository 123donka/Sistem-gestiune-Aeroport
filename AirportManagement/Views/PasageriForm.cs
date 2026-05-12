using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class PasageriForm : Form
    {
        private DataGridView dgv;
        private PasageriController _controller = new PasageriController();

        public PasageriForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Pasageri";
            Width = 900; Height = 600; StartPosition = FormStartPosition.CenterParent;

            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            var pnlTop = new Panel { Height = 50, Dock = DockStyle.Top };
            var btnAdd = new Button { Text = "Add", Left = 10, Width = 100, Top = 10 };
            var btnEdit = new Button { Text = "Edit", Left = 120, Width = 100, Top = 10 };
            var btnDelete = new Button { Text = "Delete", Left = 230, Width = 100, Top = 10 };
            var btnCheckIn = new Button { Text = "Check-in", Left = 340, Width = 100, Top = 10 };
            var btnBoard = new Button { Text = "Board", Left = 450, Width = 100, Top = 10 };
            var btnRefresh = new Button { Text = "Refresh", Left = 560, Width = 100, Top = 10 };

            btnAdd.Click += BtnAdd_Click; btnEdit.Click += BtnEdit_Click; btnDelete.Click += BtnDelete_Click;
            btnCheckIn.Click += BtnCheckIn_Click; btnBoard.Click += BtnBoard_Click; btnRefresh.Click += (s, e) => LoadData();

            pnlTop.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnCheckIn, btnBoard, btnRefresh });
            Controls.Add(dgv); Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            var dt = _controller.GetAll();
            dgv.DataSource = dt;
            if (dgv.Columns.Contains("checkedin")) dgv.Columns["checkedin"].HeaderText = "Checked-in";
            if (dgv.Columns.Contains("boarded")) dgv.Columns["boarded"].HeaderText = "Boarded";
        }

        private Pasager? GetSelected()
        {
            if (dgv.CurrentRow == null) return null;
            var row = dgv.CurrentRow;
            var p = new Pasager
            {
                Id = Convert.ToInt32(row.Cells["id"].Value),
                Nume = row.Cells["nume"].Value?.ToString() ?? "",
                TicketNr = row.Cells["ticketnr"].Value?.ToString() ?? "",
                ZborId = Convert.ToInt32(row.Cells["zborid"].Value ?? 0),
                CheckedIn = Convert.ToBoolean(row.Cells["checkedin"].Value ?? false),
                Boarded = Convert.ToBoolean(row.Cells["boarded"].Value ?? false)
            };
            return p;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var dlg = new PasagerEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var p = dlg.PasagerResult!;
                _controller.Create(p);
                LoadData();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            var selected = GetSelected(); if (selected == null) return;
            var dlg = new PasagerEditForm(selected);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var p = dlg.PasagerResult!;
                _controller.Update(p);
                LoadData();
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var selected = GetSelected(); if (selected == null) return;
            if (MessageBox.Show("Ștergi pasagerul selectat?", "Confirmare", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _controller.Delete(selected.Id);
                LoadData();
            }
        }

        private void BtnCheckIn_Click(object? sender, EventArgs e)
        {
            var selected = GetSelected(); if (selected == null) return;
            var newVal = !selected.CheckedIn;
            _controller.SetCheckIn(selected.Id, newVal);
            LoadData();
        }

        private void BtnBoard_Click(object? sender, EventArgs e)
        {
            var selected = GetSelected(); if (selected == null) return;
            var newVal = !selected.Boarded;
            _controller.SetBoarded(selected.Id, newVal);
            LoadData();
        }
    }
}
