using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AirportManagement.Controllers;
using AirportManagement.Models;

namespace AirportManagement.Views
{
    public class ResurseForm : Form
    {
        private DataGridView dgv;
        private ResurseController _controller = new ResurseController();

        public ResurseForm()
        {
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Resurse"; Width = 800; Height = 600; StartPosition = FormStartPosition.CenterParent;
            dgv = new DataGridView { Dock = DockStyle.Fill, ReadOnly = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, SelectionMode = DataGridViewSelectionMode.FullRowSelect };

            var pnlTop = new Panel { Height = 50, Dock = DockStyle.Top };
            var btnAdd = new Button { Text = "Add", Left = 10, Width = 100, Top = 10 };
            var btnEdit = new Button { Text = "Edit", Left = 120, Width = 100, Top = 10 };
            var btnDelete = new Button { Text = "Delete", Left = 230, Width = 100, Top = 10 };
            var btnAssign = new Button { Text = "Alocă", Left = 340, Width = 100, Top = 10 };
            btnAdd.Click += BtnAdd_Click; btnEdit.Click += BtnEdit_Click; btnDelete.Click += BtnDelete_Click; btnAssign.Click += BtnAssign_Click;
            pnlTop.Controls.AddRange(new Control[] { btnAdd, btnEdit, btnDelete, btnAssign });

            Controls.Add(dgv); Controls.Add(pnlTop);
        }

        private void LoadData()
        {
            var dt = _controller.GetAll();
            dgv.DataSource = dt;
        }

        private Resursa? GetSelected()
        {
            if (dgv.CurrentRow == null) return null;
            var row = dgv.CurrentRow;
            return new Resursa
            {
                Id = Convert.ToInt32(row.Cells["id"].Value),
                Nume = row.Cells["nume"].Value?.ToString() ?? string.Empty,
                Tip = row.Cells["tip"].Value?.ToString() ?? string.Empty,
                Disponibila = Convert.ToBoolean(row.Cells["disponibila"].Value ?? true)
            };
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            var dlg = new ResursaEditForm();
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var r = dlg.ResursaResult!;
                _controller.Create(r);
                LoadData();
            }
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            var sel = GetSelected(); if (sel == null) return;
            var dlg = new ResursaEditForm(sel);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var r = dlg.ResursaResult!;
                _controller.Update(r);
                LoadData();
            }
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            var sel = GetSelected(); if (sel == null) return;
            if (MessageBox.Show("Ștergi resursa selectată?", "Confirmare", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                _controller.Delete(sel.Id);
                LoadData();
            }
        }

        private void BtnAssign_Click(object? sender, EventArgs e)
        {
            var sel = GetSelected(); if (sel == null) return;
            // small dialog to select zbor
            using var dlg = new Form { Width = 400, Height = 160, StartPosition = FormStartPosition.CenterParent, Text = "Selectează zbor" };
            var cmb = new ComboBox { Left = 20, Top = 20, Width = 340, DropDownStyle = ComboBoxStyle.DropDownList };
            try
            {
                var zs = new AirportManagement.Services.ZborService();
                var dt = zs.GetAll();
                cmb.DisplayMember = "cod"; cmb.ValueMember = "id"; cmb.DataSource = dt;
            }
            catch { }
            var btnOk = new Button { Text = "Alocă", Left = 80, Top = 60, Width = 100 };
            var btnCancel = new Button { Text = "Anulează", Left = 200, Top = 60, Width = 100 };
            btnOk.Click += (s, ev) => { if (cmb.SelectedValue != null) { var z = Convert.ToInt32(cmb.SelectedValue); _controller.AssignToZbor(sel.Id, z); dlg.DialogResult = DialogResult.OK; dlg.Close(); } };
            btnCancel.Click += (s, ev) => { dlg.DialogResult = DialogResult.Cancel; dlg.Close(); };
            dlg.Controls.Add(cmb); dlg.Controls.Add(btnOk); dlg.Controls.Add(btnCancel);
            if (dlg.ShowDialog() == DialogResult.OK) LoadData();
        }
    }
}

