using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WMPLib;
using FamilyTreeApp.Core.Services;
using FamilyTreeApp.Core.Models;
using System.Diagnostics;

namespace FamilyTreeApp.UI.Forms
{
    // el formulario principal de la app, asigna todo
    public partial class MainForm : Form
    {
        private FamilyTreeService familyService;
        private Button btnAddPerson;
        private Button btnViewMap;
        private Button btnStatistics;
        private Button btnViewTree;
        private ListBox lstMembers;
        private Panel pnlDetails;
        private Label lblMemberCount;
        public MainForm()
        {
            InitializeComponent();
            familyService = new FamilyTreeService();
            SetupCustomComponents();
            UpdateMemberList();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // MainForm
            this.ClientSize = new System.Drawing.Size(900, 600);
            this.Name = "MainForm";
            this.Text = "Árbol Genealógico";
            this.StartPosition = FormStartPosition.CenterScreen;


            // Musicaa

            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Notes Aeriennes - HOYO-MiX.mp3");

            // Verifica si el archivo existe
            if (File.Exists(filePath))
            {
                var player = new WindowsMediaPlayer();
                player.URL = filePath; 
                player.settings.volume = 10;
                player.settings.autoStart = true;
                player.settings.setMode("loop", true);
                Debug.WriteLine("Reproduciendo: " + filePath);
            }
            else
            {
                Debug.WriteLine("El archivo de musica no se encontró, es posible que falte colocarle las propiedades de recurso incrustado...");
            }

            this.ResumeLayout(false);
        }
        private void SetupCustomComponents()
        {
            // Panel superior con botones
            Panel topPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.FromArgb(240, 240, 240)
            };

            // El addpersonform
            btnAddPerson = new Button
            {
                Text = "Agregar persona",
                Location = new Point(10, 15),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnAddPerson.FlatAppearance.BorderSize = 0;
            btnAddPerson.Click += BtnAddPerson_Click;

            // El mapform 
            btnViewMap = new Button
            {
                Text = "Ver mapa",
                Location = new Point(150, 15),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnViewMap.FlatAppearance.BorderSize = 0;
            btnViewMap.Click += BtnViewMap_Click;

            // el stadisticsform
            btnStatistics = new Button
            {
                Text = "Estadísticas",
                Location = new Point(290, 15),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnStatistics.FlatAppearance.BorderSize = 0;
            btnStatistics.Click += BtnStatistics_Click;

            btnViewTree = new Button
            {
                Text = "Ver arbol",
                Location = new Point(430, 15),
                Size = new Size(130, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnViewTree.FlatAppearance.BorderSize = 0;
            btnViewTree.Click += BtnViewTree_Click;

            topPanel.Controls.Add(btnViewTree);

            // muestra camtodad de miembros es uin label
            lblMemberCount = new Label
            {
                Location = new Point(450, 20),
                Size = new Size(200, 25),
                Text = "Miembros: 0",
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            topPanel.Controls.AddRange(new Control[]
            {
                btnAddPerson,
                btnViewMap,
                btnStatistics,
                lblMemberCount
            });

            // lista de miembros
            lstMembers = new ListBox
            {
                Dock = DockStyle.Left,
                Width = 300,
                Font = new Font("Segoe UI", 9)
            };
            lstMembers.SelectedIndexChanged += LstMembers_SelectedIndexChanged;

            // panel detalles
            pnlDetails = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(10),
                BackColor = Color.White
            };


            Label lblNoSelection = new Label
            {
                Text = "Seleccione un miembro para ver detalles",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11),
                ForeColor = Color.Gray
            };
            pnlDetails.Controls.Add(lblNoSelection);

            this.Controls.Add(pnlDetails);
            this.Controls.Add(lstMembers);
            this.Controls.Add(topPanel);
        }

        // esto se maneja con eventos
        private void BtnAddPerson_Click(object sender, EventArgs e)
        {
            var addForm = new AddPersonForm(familyService);
            if (addForm.ShowDialog() == DialogResult.OK)
            {
                UpdateMemberList();
            }
        }
        // muestra mapa
        private void BtnViewMap_Click(object sender, EventArgs e)
        {
            if (familyService.GetMemberCount() == 0)
            {
                MessageBox.Show("Agregue al menos un miembro antes de ver el mapa.",
                    "Árbol vacío",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var mapForm = new MapForm(familyService);
            mapForm.ShowDialog();
        }

        // muestra estadisticas
        private void BtnStatistics_Click(object sender, EventArgs e)
        {
            if (familyService.GetMemberCount() < 2)
            {
                MessageBox.Show("Se necesitan al menos 2 miembros para calcular estadisticas",
                    "Insuficientes datos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var statsForm = new StatisticsForm(familyService);
            statsForm.ShowDialog();
        }

        // muestra el treeview
        private void BtnViewTree_Click(object sender, EventArgs e)
        {
            if (familyService.GetMemberCount() == 0)
            {
                MessageBox.Show("Agregue aunque sea un miembro antes de ver el arbol",
                    "Arbol vacio", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var treeForm = new TreeDiagramForm(familyService);
            treeForm.ShowDialog();
        }


        private void UpdateMemberList() // Actualiza la lista de miembros
        {
            lstMembers.Items.Clear();
            var members = familyService.GetAllMembers();

            foreach (var member in members)
            {
                lstMembers.Items.Add($"{member.FullName} - {member.Age} años");
            }

            lblMemberCount.Text = $"Miembros: {members.Count}";
        }

        // cambio de seleccion
        private void LstMembers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstMembers.SelectedIndex < 0)
                return;

            var members = familyService.GetAllMembers();
            if (lstMembers.SelectedIndex >= members.Count)
                return;

            var selectedPerson = members[lstMembers.SelectedIndex];
            ShowPersonDetails(selectedPerson);
        }

        // detalles d pj especifica
        private void ShowPersonDetails(Person person)
        {
            pnlDetails.Controls.Clear();

            var detailsFlow = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Padding = new Padding(15)
            };

            detailsFlow.Controls.Add(CreateDetailLabel("Nombre:", person.FullName));
            detailsFlow.Controls.Add(CreateDetailLabel("Cédula:", person.IdNumber));
            detailsFlow.Controls.Add(CreateDetailLabel("Fecha de nacimiento:",person.BirthDate.ToShortDateString()));
            detailsFlow.Controls.Add(CreateDetailLabel("Edad:",$"{person.Age} años"));
            detailsFlow.Controls.Add(CreateDetailLabel("Estado:",person.IsAlive ? "Vivo" : "Fallecido"));
            detailsFlow.Controls.Add(CreateDetailLabel("Ubicación:",$"Lat: {person.Residence.Latitude:F6}, Lon: {person.Residence.Longitude:F6}"));

            pnlDetails.Controls.Add(detailsFlow);
        }

        // Crea una etiqueta de detalle
        private Panel CreateDetailLabel(string label, string value)
        {
            Panel panel = new Panel { Height = 40, Width = 400 };

            Label lblName = new Label
            {
                Text = label,
                Location = new Point(0, 5),
                Size = new Size(150, 20),
                Font = new Font("Segoe UI", 9, FontStyle.Bold)
            };

            Label lblValue = new Label
            {
                Text = value,
                Location = new Point(155, 5),
                Size = new Size(240, 20),
                Font = new Font("Segoe UI", 9)
            };

            panel.Controls.Add(lblName);
            panel.Controls.Add(lblValue);

            return panel;
        }
    }
}