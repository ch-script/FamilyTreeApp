using FamilyTreeApp.Core.Services;
using FamilyTreeApp.Core.Models;
using System.Drawing.Drawing2D;

namespace FamilyTreeApp.UI.Forms
{
    public partial class TreeDiagramForm : Form
    {
        private FamilyTreeService familyService;
        private Panel drawPanel;
        private Dictionary<string, Rectangle> personRectangles;
        private const int PERSON_WIDTH = 200;
        private const int PERSON_HEIGHT = 80;
        private const int HORIZONTAL_SPACING = 50;
        private const int VERTICAL_SPACING = 100;

        public TreeDiagramForm(FamilyTreeService service)
        {
            this.familyService = service;
            this.personRectangles = new Dictionary<string, Rectangle>();
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            this.ClientSize = new Size(1200, 800);
            this.Text = "Diagrama del árbol genealógico";
            this.StartPosition = FormStartPosition.CenterParent;
            this.AutoScroll = true;
            this.BackColor = Color.White;

            drawPanel = new Panel
            {
                Location = new Point(0, 0),
                Size = new Size(3000, 3000),
                BackColor = Color.White,
                AutoScroll = false
            };

            drawPanel.Paint += DrawPanel_Paint;
            drawPanel.MouseClick += DrawPanel_MouseClick;

            this.Controls.Add(drawPanel);
            this.ResumeLayout(false);
        }

        private void DrawPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            personRectangles.Clear();

            var roots = familyService.GetRoots();

            if (roots.Count == 0)
            {
                g.DrawString("No hay nadie en el árbol genealógico",
                    new Font("Segoe UI", 14), Brushes.Gray, 50, 50);
                return;
            }

            int startX = 500; // Empezar más centrado
            int startY = 50;

            foreach (var root in roots)
            {
                int width = CalculateSubtreeWidth(root);
                DrawPersonWithChildren(g, root, startX, startY, width);
                startX += width + HORIZONTAL_SPACING * 2;
            }
        }

        private int CalculateSubtreeWidth(Person person)
        {
            var children = familyService.GetChildren(person.Id);

            if (children.Count == 0)
                return PERSON_WIDTH;

            int totalChildrenWidth = 0;
            foreach (var child in children)
            {
                totalChildrenWidth += CalculateSubtreeWidth(child);
            }

            // Agregar espaciado entre hermanos
            totalChildrenWidth += (children.Count - 1) * HORIZONTAL_SPACING;

            return Math.Max(PERSON_WIDTH, totalChildrenWidth);
        }

        private void DrawPersonWithChildren(Graphics g, Person person, int centerX, int y, int subtreeWidth)
        {
            // Calcular posición del rectángulo (centrado en el subtree)
            int rectX = centerX + (subtreeWidth - PERSON_WIDTH) / 2;
            Rectangle rect = new Rectangle(rectX, y, PERSON_WIDTH, PERSON_HEIGHT);

            personRectangles[person.Id] = rect;

            // Dibujar la caja de la persona
            Brush bgBrush = person.IsAlive ? Brushes.LightGreen : Brushes.LightGray;
            Pen borderPen = new Pen(Color.Black, 2);

            g.FillRectangle(bgBrush, rect);
            g.DrawRectangle(borderPen, rect);

            Font nameFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Font infoFont = new Font("Segoe UI", 9);

            g.DrawString(person.FullName, nameFont, Brushes.Black, rectX + 10, y + 10);
            g.DrawString($"{person.Age} años", infoFont, Brushes.DarkGray, rectX + 10, y + 35);
            g.DrawString(person.IsAlive ? "Vivo" : "Fallecido", infoFont,
                person.IsAlive ? Brushes.Green : Brushes.Red, rectX + 10, y + 55);

            // Obtener hijos
            var children = familyService.GetChildren(person.Id);

            if (children.Count > 0)
            {
                int childY = y + PERSON_HEIGHT + VERTICAL_SPACING;
                int personCenterX = rectX + PERSON_WIDTH / 2;

                // Línea vertical desde el padre
                Pen linePen = new Pen(Color.Black, 2);
                g.DrawLine(linePen, personCenterX, y + PERSON_HEIGHT,
                    personCenterX, childY - VERTICAL_SPACING / 2);

                // Calcular posiciones de los hijos
                int currentX = centerX;

                if (children.Count > 1)
                {
                    // Calcular el ancho total de los hijos
                    int firstChildWidth = CalculateSubtreeWidth(children[0]);
                    int lastChildWidth = CalculateSubtreeWidth(children[children.Count - 1]);

                    int firstChildCenterX = currentX + firstChildWidth / 2;
                    int totalChildrenWidth = 0;
                    foreach (var child in children)
                    {
                        totalChildrenWidth += CalculateSubtreeWidth(child);
                    }
                    totalChildrenWidth += (children.Count - 1) * HORIZONTAL_SPACING;

                    int lastChildCenterX = currentX + totalChildrenWidth - lastChildWidth / 2;

                    // Línea horizontal entre los hijos
                    g.DrawLine(linePen, firstChildCenterX, childY - VERTICAL_SPACING / 2,
                        lastChildCenterX, childY - VERTICAL_SPACING / 2);
                }

                // Dibujar cada hijo
                foreach (var child in children)
                {
                    int childWidth = CalculateSubtreeWidth(child);
                    int childCenterX = currentX + childWidth / 2;

                    // Línea vertical hacia abajo al hijo
                    g.DrawLine(linePen, childCenterX, childY - VERTICAL_SPACING / 2,
                        childCenterX, childY);

                    // Dibujar el hijo recursivamente
                    DrawPersonWithChildren(g, child, currentX, childY, childWidth);

                    currentX += childWidth + HORIZONTAL_SPACING;
                }
            }
        }

        private void DrawPanel_MouseClick(object sender, MouseEventArgs e)
        {
            foreach (var kvp in personRectangles)
            {
                if (kvp.Value.Contains(e.Location))
                {
                    string personId = kvp.Key;
                    var person = familyService.GetPerson(personId);

                    if (person != null)
                    {
                        ShowPersonOptions(person);
                    }
                    break;
                }
            }
        }

        private void ShowPersonOptions(Person person)
        {
            string info = $"¿Deseas cambiar la información de {person.FullName}?";

            DialogResult result = MessageBox.Show(
                info, "Opciones", MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            // Aquí puedes agregar la lógica para manejar la respuesta
            if (result == DialogResult.Yes)
            {
                // Abrir formulario de edición
            }
        }
    }
}