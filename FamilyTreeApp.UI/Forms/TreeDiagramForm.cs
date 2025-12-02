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
        private const int COUPLE_SPACING = 20;

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

            int startX = 500;
            int startY = 50;

            var processedRoots = new HashSet<string>();

            foreach (var root in roots)
            {
                if (processedRoots.Contains(root.Id))
                    continue;

                var spouse = familyService.GetSpouse(root.Id);

                int width;
                if (spouse != null && roots.Any(r => r.Id == spouse.Id))
                {
                    width = CalculateCoupleSubtreeWidth(root, spouse);
                    DrawCoupleWithChildren(g, root, spouse, startX, startY, width);
                    processedRoots.Add(root.Id);
                    processedRoots.Add(spouse.Id);
                }
                else
                {
                    width = CalculateSubtreeWidth(root);
                    DrawPersonWithChildren(g, root, startX, startY, width);
                    processedRoots.Add(root.Id);
                }

                startX += width + HORIZONTAL_SPACING * 2;
            }
        }

        private int CalculateCoupleSubtreeWidth(Person person1, Person person2)
        {
            int coupleWidth = PERSON_WIDTH * 2 + COUPLE_SPACING;

            var children = GetCoupleChildren(person1.Id, person2.Id);

            if (children.Count == 0)
                return coupleWidth;

            int totalChildrenWidth = 0;
            foreach (var child in children)
            {
                totalChildrenWidth += CalculateSubtreeWidth(child);
            }

            totalChildrenWidth += (children.Count - 1) * HORIZONTAL_SPACING;

            return Math.Max(coupleWidth, totalChildrenWidth);
        }

        private int CalculateSubtreeWidth(Person person)
        {
            var spouse = familyService.GetSpouse(person.Id);

            if (spouse != null)
            {
                return CalculateCoupleSubtreeWidth(person, spouse);
            }

            var children = familyService.GetChildren(person.Id);

            if (children.Count == 0)
                return PERSON_WIDTH;

            int totalChildrenWidth = 0;
            foreach (var child in children)
            {
                totalChildrenWidth += CalculateSubtreeWidth(child);
            }

            totalChildrenWidth += (children.Count - 1) * HORIZONTAL_SPACING;

            return Math.Max(PERSON_WIDTH, totalChildrenWidth);
        }

        private List<Person> GetCoupleChildren(string personId1, string personId2)
        {
            var children1 = familyService.GetChildren(personId1);
            var children2 = familyService.GetChildren(personId2);

            var allChildren = children1.Union(children2).ToList();
            return allChildren;
        }

        private void DrawCoupleWithChildren(Graphics g, Person person1, Person person2, int centerX, int y, int subtreeWidth)
        {
            int coupleWidth = PERSON_WIDTH * 2 + COUPLE_SPACING;
            int coupleStartX = centerX + (subtreeWidth - coupleWidth) / 2;

            Rectangle rect1 = new Rectangle(coupleStartX, y, PERSON_WIDTH, PERSON_HEIGHT);
            DrawPersonBox(g, person1, rect1);
            personRectangles[person1.Id] = rect1;

            Rectangle rect2 = new Rectangle(coupleStartX + PERSON_WIDTH + COUPLE_SPACING, y, PERSON_WIDTH, PERSON_HEIGHT);
            DrawPersonBox(g, person2, rect2);
            personRectangles[person2.Id] = rect2;

            Pen marriagePen = new Pen(Color.Red, 3);
            int lineY = y + PERSON_HEIGHT / 2;
            g.DrawLine(marriagePen, coupleStartX + PERSON_WIDTH, lineY,
                coupleStartX + PERSON_WIDTH + COUPLE_SPACING, lineY);

            // Deco
            Font symbolFont = new Font("Segoe UI", 10, FontStyle.Bold);
            g.DrawString("♥", symbolFont, Brushes.Red,
                coupleStartX + PERSON_WIDTH + COUPLE_SPACING / 2 - 5, lineY - 10);

            /////////////////
            var children = GetCoupleChildren(person1.Id, person2.Id);

            if (children.Count > 0)
            {
                int childY = y + PERSON_HEIGHT + VERTICAL_SPACING;
                int coupleCenterX = coupleStartX + coupleWidth / 2;

                // Linea vertical desde el centro de la pareja
                Pen linePen = new Pen(Color.Black, 2);
                g.DrawLine(linePen, coupleCenterX, y + PERSON_HEIGHT,
                    coupleCenterX, childY - VERTICAL_SPACING / 2);

                // Calcular posiciones de los hijos
                int currentX = centerX;

                if (children.Count > 1)
                {
                    int firstChildWidth = CalculateSubtreeWidth(children[0]);
                    int totalChildrenWidth = 0;
                    foreach (var child in children)
                    {
                        totalChildrenWidth += CalculateSubtreeWidth(child);
                    }
                    totalChildrenWidth += (children.Count - 1) * HORIZONTAL_SPACING;

                    int firstChildCenterX = currentX + firstChildWidth / 2;
                    int lastChildCenterX = currentX + totalChildrenWidth - CalculateSubtreeWidth(children[children.Count - 1]) / 2;

                    // Línea horizontal entre los hijos
                    g.DrawLine(linePen, firstChildCenterX, childY - VERTICAL_SPACING / 2,
                        lastChildCenterX, childY - VERTICAL_SPACING / 2);
                }

                // Dibujar cada hijo
                foreach (var child in children)
                {
                    int childWidth = CalculateSubtreeWidth(child);
                    int childCenterX = currentX + childWidth / 2;

                    g.DrawLine(linePen, childCenterX, childY - VERTICAL_SPACING / 2,childCenterX, childY);

                    DrawPersonWithChildren(g, child, currentX, childY, childWidth); //recursivo

                    currentX += childWidth + HORIZONTAL_SPACING;
                }
            }
        }

        private void DrawPersonWithChildren(Graphics g, Person person, int centerX, int y, int subtreeWidth)
        {
            var spouse = familyService.GetSpouse(person.Id);

            if (spouse != null)
            {
                DrawCoupleWithChildren(g, person, spouse, centerX, y, subtreeWidth);
                return;
            }

            int rectX = centerX + (subtreeWidth - PERSON_WIDTH) / 2;
            Rectangle rect = new Rectangle(rectX, y, PERSON_WIDTH, PERSON_HEIGHT);

            personRectangles[person.Id] = rect;
            DrawPersonBox(g, person, rect);

            var children = familyService.GetChildren(person.Id);

            if (children.Count > 0)
            {
                int childY = y + PERSON_HEIGHT + VERTICAL_SPACING;
                int personCenterX = rectX + PERSON_WIDTH / 2;
                Pen linePen = new Pen(Color.Black, 2);
                g.DrawLine(linePen, personCenterX, y + PERSON_HEIGHT,personCenterX, childY - VERTICAL_SPACING / 2);
                int currentX = centerX;

                if (children.Count > 1)
                {
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

                    g.DrawLine(linePen, firstChildCenterX, childY - VERTICAL_SPACING / 2,lastChildCenterX, childY - VERTICAL_SPACING / 2); // lineas horizontales entre hijos
                }

                foreach (var child in children) // dibujar cada hijo
                {
                    int childWidth = CalculateSubtreeWidth(child);
                    int childCenterX = currentX + childWidth / 2;

                    g.DrawLine(linePen, childCenterX, childY - VERTICAL_SPACING / 2,childCenterX, childY);
                    DrawPersonWithChildren(g, child, currentX, childY, childWidth);

                    currentX += childWidth + HORIZONTAL_SPACING;
                }
            }
        }

        private void DrawPersonBox(Graphics g, Person person, Rectangle rect)
        {
            Brush bgBrush = person.IsAlive ? Brushes.LightGreen : Brushes.LightGray;
            Pen borderPen = new Pen(Color.Black, 2);

            g.FillRectangle(bgBrush, rect);
            g.DrawRectangle(borderPen, rect);

            Font nameFont = new Font("Segoe UI", 11, FontStyle.Bold);
            Font infoFont = new Font("Segoe UI", 9);

            g.DrawString(person.FullName, nameFont, Brushes.Black, rect.X + 10, rect.Y + 10);
            g.DrawString($"{person.Age} años", infoFont, Brushes.DarkGray, rect.X + 10, rect.Y + 35);
            g.DrawString(person.IsAlive ? "Vivo" : "Fallecido", infoFont,person.IsAlive ? Brushes.Green : Brushes.Red, rect.X + 10, rect.Y + 55);
        }
    }
}