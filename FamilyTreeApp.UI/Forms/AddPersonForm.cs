using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FamilyTreeApp.Core.Services;
using FamilyTreeApp.Core.Models;

namespace FamilyTreeApp.UI.Forms
{
    // Formulario para agregar una nueva persona al arbol genealogico
    public partial class AddPersonForm : Form
    {
        private FamilyTreeService familyService;

        private TextBox txtFullName;
        private TextBox txtIdNumber;
        private DateTimePicker dtpBirthDate;
        private CheckBox chkIsAlive;
        private TextBox txtAge;
        private TextBox txtLatitude;
        private TextBox txtLongitude;
        private TextBox txtAddress;
        private TextBox txtPhotoPath;
        private ComboBox cmbFather;
        private ComboBox cmbMother;
        private Button btnSave;
        private Button btnCancel;
        private Button btnBrowsePhoto;

        public AddPersonForm(FamilyTreeService service)
        {
            this.familyService = service;
            InitializeComponent();
            LoadParentOptions();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ClientSize = new Size(500, 650);
            this.Text = "Agregar Persona";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            int yPos = 20;
            int labelWidth = 120;
            int controlWidth = 300;
            int spacing = 45;


            /////////////////////////////////////////


            // Nombre completo
            AddLabel("Nombre completo:", 20, yPos, labelWidth);
            txtFullName = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(controlWidth, 25)
            };
            this.Controls.Add(txtFullName);
            yPos += spacing;


            ////////////////////////////////////////////


            // Cedula
            AddLabel("Número de cédula:", 20, yPos, labelWidth);
            txtIdNumber = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(controlWidth, 25)
            };
            this.Controls.Add(txtIdNumber);
            yPos += spacing;


            ////////////////////////////////////////////

            // Fecha de nacimiento
            AddLabel("Fecha de nacimiento:", 20, yPos, labelWidth);
            dtpBirthDate = new DateTimePicker
            {
                Location = new Point(150, yPos),
                Size = new Size(controlWidth, 25),
                Format = DateTimePickerFormat.Short,
                MaxDate = DateTime.Today
            };
            this.Controls.Add(dtpBirthDate);
            yPos += spacing;

            ///////////////////////////////////////////

            // xd
            chkIsAlive = new CheckBox
            {
                Text = "Sigues con vida?",
                Location = new Point(150, yPos),
                Size = new Size(200, 25),
                Checked = true
            };
            chkIsAlive.CheckedChanged += ChkIsAlive_CheckedChanged;
            this.Controls.Add(chkIsAlive);
            yPos += spacing;

            ///////////////////////////////////////////

            // Edad
            AddLabel("Edad:", 20, yPos, labelWidth);
            txtAge = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(100, 25),
                ReadOnly = true
            };
            this.Controls.Add(txtAge);
            yPos += spacing;


            ////////////////////////////////////////////

            // Latitud
            AddLabel("Latitud:", 20, yPos, labelWidth);
            txtLatitude = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(controlWidth, 25),
                PlaceholderText = "Ej: 9,9281 (ojo a las comas)"
            };
            this.Controls.Add(txtLatitude);
            yPos += spacing;


            ////////////////////////////////////////////

            // Longitud
            AddLabel("Longitud:", 20, yPos, labelWidth);
            txtLongitude = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(controlWidth, 25),
                PlaceholderText = "Ej: -84,0907"
            };
            this.Controls.Add(txtLongitude);
            yPos += spacing;




            ///////////////////////////////////////(///////

            // Direccion
            AddLabel("Dirección:", 20, yPos, labelWidth);
            txtAddress = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(controlWidth, 25)
            };
            this.Controls.Add(txtAddress);
            yPos += spacing;



            ////////////////////////////////////////////


            // Fotito
            AddLabel("Fotografia:", 20, yPos, labelWidth);
            txtPhotoPath = new TextBox
            {
                Location = new Point(150, yPos),
                Size = new Size(220, 25),
                ReadOnly = true
            };
            this.Controls.Add(txtPhotoPath);

            btnBrowsePhoto = new Button
            {
                Text = "Adjuntar",
                Location = new Point(380, yPos),
                Size = new Size(70, 25)
            };
            btnBrowsePhoto.Click += BtnBrowsePhoto_Click;
            this.Controls.Add(btnBrowsePhoto);
            yPos += spacing;


            ///////////////////////////////////////
            // Padre
            AddLabel("Padre:", 20, yPos, labelWidth);
            cmbFather = new ComboBox
            {
                Location = new Point(150, yPos),
                Size = new Size(controlWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbFather);
            yPos += spacing;


            /////////////////////////////////////////////
            // Madre
            AddLabel("Madre:", 20, yPos, labelWidth);
            cmbMother = new ComboBox
            {
                Location = new Point(150, yPos),
                Size = new Size(controlWidth, 25),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            this.Controls.Add(cmbMother);
            yPos += spacing;



            ///////////////////////////////////////////////////
            // Botones
            btnSave = new Button
            {
                Text = "Guardar",
                Location = new Point(280, yPos),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancelar",
                Location = new Point(370, yPos),
                Size = new Size(80, 30),
                FlatStyle = FlatStyle.Flat
            };
            btnCancel.Click += BtnCancel_Click;
            this.Controls.Add(btnCancel);

            this.ResumeLayout(false);

            CalculateAge();
            dtpBirthDate.ValueChanged += (s, e) => CalculateAge();
        }

        // Agrega una etiqueta al formulario
        private void AddLabel(string text, int x, int y, int width)
        {
            Label label = new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                Size = new Size(width, 20),
                Font = new Font("Segoe UI", 9)
            };
            this.Controls.Add(label);
        }

        // Carga las opciones de padres disponibles
        private void LoadParentOptions()
        {
            cmbFather.Items.Clear();
            cmbMother.Items.Clear();

            cmbFather.Items.Add(new ComboBoxItem { Text = "Ninguno", Value = null });
            cmbMother.Items.Add(new ComboBoxItem { Text = "Ninguno", Value = null });

            var members = familyService.GetAllMembers();
            foreach (var member in members)
            {
                var item = new ComboBoxItem
                {
                    Text = member.FullName,
                    Value = member.Id
                };
                cmbFather.Items.Add(item);
                cmbMother.Items.Add(item);
            }

            cmbFather.SelectedIndex = 0;
            cmbMother.SelectedIndex = 0;
            cmbFather.DisplayMember = "Text";
            cmbMother.DisplayMember = "Text";
        }

        // Calcula la edad
        private void CalculateAge()
        {
            var today = DateTime.Today;
            int age = today.Year - dtpBirthDate.Value.Year;
            if (dtpBirthDate.Value.Date > today.AddYears(-age))
                age--;

            txtAge.Text = age.ToString();
        }

        private void ChkIsAlive_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkIsAlive.Checked)
            {
                txtAge.ReadOnly = false;
                txtAge.BackColor = SystemColors.Window;
            }
            else
            {
                txtAge.ReadOnly = true;
                txtAge.BackColor = SystemColors.Control;
                CalculateAge();
            }
        }

        private void BtnBrowsePhoto_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Archivos de imagen|*.jpg;*.jpeg;*.png;*.bmp|Todos los archivos|*.*"; // AAAA
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    txtPhotoPath.Text = ofd.FileName;
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
                return;

            try
            {
                var person = new Person
                {
                    FullName = txtFullName.Text.Trim(),
                    IdNumber = txtIdNumber.Text.Trim(),
                    BirthDate = dtpBirthDate.Value,
                    Age = int.Parse(txtAge.Text),
                    IsAlive = chkIsAlive.Checked,
                    PhotoPath = txtPhotoPath.Text,
                    Residence = new GeoCoordinates
                    {
                        Latitude = double.Parse(txtLatitude.Text),
                        Longitude = double.Parse(txtLongitude.Text),
                        Address = txtAddress.Text.Trim()
                    }
                };

                string fatherId = (cmbFather.SelectedItem as ComboBoxItem)?.Value;
                string motherId = (cmbMother.SelectedItem as ComboBoxItem)?.Value;

                if (familyService.AddPerson(person, fatherId, motherId))
                {
                    MessageBox.Show("Persona agregada exitosamente","Exito",MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Error al agregar la persona", "Error",MessageBoxButtons.OK,MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Valida LOS Vvalores ingresados
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtFullName.Text))
            {
                MessageBox.Show("Oye ocupas poner el nombre completo", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtIdNumber.Text))
            {
                MessageBox.Show("El numero de cedula que?", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!double.TryParse(txtLatitude.Text, out double lat) || lat < -90 || lat > 90)
            {
                MessageBox.Show("La latitud es entre -90 y 90 grados w", "Validacion",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (!double.TryParse(txtLongitude.Text, out double lon) || lon < -180 || lon > 180)
            {
                MessageBox.Show("La longitud debe ser entre -180 y 180", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private class ComboBoxItem // nodo ahh
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}