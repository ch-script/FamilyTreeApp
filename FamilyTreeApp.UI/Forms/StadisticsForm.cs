using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FamilyTreeApp.Core.Services;

namespace FamilyTreeApp.UI.Forms
{
    // formulario que muestra las estadisticas de distancias entre familiares
    public partial class StatisticsForm : Form
    {
        private FamilyTreeService familyService;

        public StatisticsForm(FamilyTreeService service)
        {
            this.familyService = service;
            InitializeComponent();
            LoadStatistics();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ClientSize = new Size(600, 400);
            this.Text = "Estadísticas de Distancias";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;

            this.ResumeLayout(false);
        }

        // Carga y muestra las estadísticas calculadas.
        private void LoadStatistics()
        {
            var stats = familyService.GetStatistics();

            var mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(20),
                AutoScroll = true
            };

            int yPos = 20;

            // Titulo
            Label lblTitle = new Label
            {
                Text = "Estadísticas de Distancias entre Familiares",
                Location = new Point(20, yPos),
                Size = new Size(560, 30),
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };
            mainPanel.Controls.Add(lblTitle);
            yPos += 50;

            // Oar más lejano
            GroupBox grpFarthest = CreateStatisticGroup(
                "Par de Familiares Más Lejanos",
                stats.FarthestPair.Person1?.FullName ?? "N/A",
                stats.FarthestPair.Person2?.FullName ?? "N/A",
                stats.MaxDistance,
                yPos
            );
            mainPanel.Controls.Add(grpFarthest);
            yPos += 110;

            // Par más cercano
            GroupBox grpClosest = CreateStatisticGroup(
                "Par de familiares más Cercanos",
                stats.ClosestPair.Person1?.FullName ?? "N/A",
                stats.ClosestPair.Person2?.FullName ?? "N/A",
                stats.MinDistance,
                yPos
            );
            mainPanel.Controls.Add(grpClosest);
            yPos += 110;

            // Distancia promedio
            GroupBox grpAverage = new GroupBox
            {
                Text = "Distancia promedio",
                Location = new Point(20, yPos),
                Size = new Size(540, 80),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Label lblAverage = new Label
            {
                Text = $"{stats.AverageDistance:F2} km",
                Location = new Point(20, 30),
                Size = new Size(500, 30),
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215),
                TextAlign = ContentAlignment.MiddleCenter
            };
            grpAverage.Controls.Add(lblAverage);
            mainPanel.Controls.Add(grpAverage);
            yPos += 100;

            // Botón cerrar
            Button btnClose = new Button
            {
                Text = "Cerrar",
                Location = new Point(250, yPos),
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10)
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();
            mainPanel.Controls.Add(btnClose);

            this.Controls.Add(mainPanel);
        }

        // Crea un grupo visual para mostrar una estadistica de par de personas
        private GroupBox CreateStatisticGroup(string title, string person1, string person2,
            double distance, int yPosition)
        {
            GroupBox group = new GroupBox
            {
                Text = title,
                Location = new Point(20, yPosition),
                Size = new Size(540, 100),
                Font = new Font("Segoe UI", 10, FontStyle.Bold)
            };

            Label lbl1 = new Label
            {
                Text = $"Persona 1: {person1}",
                Location = new Point(20, 25),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 9)
            };

            Label lbl2 = new Label
            {
                Text = $"Persona 2: {person2}",
                Location = new Point(20, 45),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 9)
            };

            Label lblDistance = new Label
            {
                Text = $"Distancia: {distance:F2} km",
                Location = new Point(20, 65),
                Size = new Size(500, 20),
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.FromArgb(0, 120, 215)
            };

            group.Controls.AddRange(new Control[] { lbl1, lbl2, lblDistance });

            return group;
        }
    }
}