using FamilyTreeApp.Core.Services;
using FamilyTreeApp.Core.Models;

namespace FamilyTreeApp.UI.Forms
{
    public partial class EditPersonForm : Form
    {
        private FamilyTreeService familyService;
        private Person person;

        private ComboBox cmbFather;
        private ComboBox cmbMother;
        private ComboBox cmbSpouse;
        private Button btnSave;
        private Button btnCancel;
        private Label lblPersonInfo;

        public EditPersonForm(FamilyTreeService service, Person personToEdit)
        {
            this.familyService = service;
            this.person = personToEdit;
            InitializeComponent();
            LoadPersonInfo();
            LoadOptions();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ClientSize = new Size(450, 280);
            this.Text = "Editar Relaciones";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            int yPos = 20;

            // Info de la persona
            lblPersonInfo = new Label
            {
                Location = new Point(20, yPos),
                Size = new Size(410, 50),
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                TextAlign = ContentAlignment.MiddleCenter,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(lblPersonInfo);
            yPos += 65;

            // Padre
            AddLabel("Padre:", 20, yPos);
            cmbFather = new ComboBox
            {
                Location = new Point(130, yPos),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(cmbFather);
            yPos += 40;

            // Madre
            AddLabel("Madre:", 20, yPos);
            cmbMother = new ComboBox
            {
                Location = new Point(130, yPos),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(cmbMother);
            yPos += 40;

            // Pareja
            AddLabel("Pareja:", 20, yPos);
            cmbSpouse = new ComboBox
            {
                Location = new Point(130, yPos),
                Size = new Size(300, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10)
            };
            this.Controls.Add(cmbSpouse);
            yPos += 50;

            // Botones
            btnSave = new Button
            {
                Text = "Guardar",
                Location = new Point(250, yPos),
                Size = new Size(90, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);

            btnCancel = new Button
            {
                Text = "Cancelar",
                Location = new Point(350, yPos),
                Size = new Size(80, 35),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(btnCancel);

            this.ResumeLayout(false);
        }

        private void AddLabel(string text, int x, int y)
        {
            Label label = new Label
            {
                Text = text,
                Location = new Point(x, y + 3),
                Size = new Size(100, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };
            this.Controls.Add(label);
        }

        private void LoadPersonInfo()
        {
            lblPersonInfo.Text = $"{person.FullName}\n{person.Age} años - {(person.IsAlive ? "Vivo" : "Fallecido")}";
        }

        private void LoadOptions()
        {
            cmbFather.Items.Clear();
            cmbMother.Items.Clear();
            cmbSpouse.Items.Clear();

            cmbFather.Items.Add(new ComboBoxItem { Text = "Ninguno", Value = null });
            cmbMother.Items.Add(new ComboBoxItem { Text = "Ninguno", Value = null });
            cmbSpouse.Items.Add(new ComboBoxItem { Text = "Ninguno", Value = null });

            var members = familyService.GetAllMembers();
            foreach (var member in members)
            {
                if (member.Id == person.Id)
                    continue;

                if (person.ChildrenIds.Contains(member.Id))
                    continue;

                var item = new ComboBoxItem
                {
                    Text = member.FullName,
                    Value = member.Id
                };
                cmbFather.Items.Add(item);
                cmbMother.Items.Add(item);
                cmbSpouse.Items.Add(item);
            }

            SelectCurrent(cmbFather, person.FatherId);
            SelectCurrent(cmbMother, person.MotherId);
            SelectCurrent(cmbSpouse, person.SpouseId);

            cmbFather.DisplayMember = "Text";
            cmbMother.DisplayMember = "Text";
            cmbSpouse.DisplayMember = "Text";
        }

        private void SelectCurrent(ComboBox combo, string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                combo.SelectedIndex = 0;
                return;
            }

            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (combo.Items[i] is ComboBoxItem item && item.Value == id)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }

            combo.SelectedIndex = 0;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            try
            {
                string newFatherId = (cmbFather.SelectedItem as ComboBoxItem)?.Value;
                string newMotherId = (cmbMother.SelectedItem as ComboBoxItem)?.Value;
                string newSpouseId = (cmbSpouse.SelectedItem as ComboBoxItem)?.Value;

                // Actualizar padre
                if (newFatherId != person.FatherId)
                {
                    if (!string.IsNullOrEmpty(person.FatherId))
                    {
                        var oldFather = familyService.GetPerson(person.FatherId);
                        oldFather?.ChildrenIds.Remove(person.Id);
                    }

                    if (!string.IsNullOrEmpty(newFatherId))
                    {
                        var newFather = familyService.GetPerson(newFatherId);
                        if (newFather != null && !newFather.ChildrenIds.Contains(person.Id))
                            newFather.ChildrenIds.Add(person.Id);
                    }

                    person.FatherId = newFatherId;
                }

                // Actualizar madre
                if (newMotherId != person.MotherId)
                {
                    if (!string.IsNullOrEmpty(person.MotherId))
                    {
                        var oldMother = familyService.GetPerson(person.MotherId);
                        oldMother?.ChildrenIds.Remove(person.Id);
                    }

                    if (!string.IsNullOrEmpty(newMotherId))
                    {
                        var newMother = familyService.GetPerson(newMotherId);
                        if (newMother != null && !newMother.ChildrenIds.Contains(person.Id))
                            newMother.ChildrenIds.Add(person.Id);
                    }

                    person.MotherId = newMotherId;
                }

                // Actualizar pareja
                if (newSpouseId != person.SpouseId)
                {
                    if (!string.IsNullOrEmpty(person.SpouseId))
                    {
                        familyService.RemoveMarriage(person.Id);
                    }

                    if (!string.IsNullOrEmpty(newSpouseId))
                    {
                        familyService.SetMarriage(person.Id, newSpouseId);
                    }
                }

                MessageBox.Show("Relaciones actualizadas exitosamente.", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private class ComboBoxItem
        {
            public string Text { get; set; }
            public string Value { get; set; }
        }
    }
}