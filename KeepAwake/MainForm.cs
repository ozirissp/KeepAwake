using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace KeepAwake
{
    public partial class MainForm : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private CheckedListBox checkedListBox;
        private readonly WindowManager windowManager;
        private readonly SerializationService serializationService;
        private readonly string saveFilePath;

        public MainForm()
        {
            InitializeComponent();

            // Utiliser ComponentResourceManager pour accéder aux ressources
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));

            // Créer et configurer le NotifyIcon
            trayIcon = new NotifyIcon
            {
                Text = "Prevent Sleep App",
                Icon = (Icon)resources.GetObject("$this.Icon"), // Utiliser l'icône de la ressource
                Visible = true
            };

            // Ajouter un gestionnaire pour le double-clic
            trayIcon.MouseDoubleClick += TrayIcon_MouseDoubleClick;

            // Ajouter un menu contextuel
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Exit", null, OnExit);

            trayIcon.ContextMenuStrip = trayMenu;

            // Elements d'interface
            var infoLabel = new Label
            {
                Text = "Sélectionnez les fenêtres à surveiller :",
                Dock = DockStyle.Top,
                Padding = new Padding(10),
                Font = new Font(Font.FontFamily, 10, FontStyle.Bold)
            };

            checkedListBox = new CheckedListBox
            {
                Dock = DockStyle.Fill
            };

            var buttonPanel = new FlowLayoutPanel
            {
                FlowDirection = FlowDirection.RightToLeft,
                Dock = DockStyle.Bottom,
                Padding = new Padding(10)
            };

            var closeButton = new Button { Text = "Fermer" };
            closeButton.Click += (s, e) => this.Hide();

            var saveButton = new Button { Text = "Enregistrer" };
            saveButton.Click += (s, e) => SaveWindowStates();

            var refreshButton = new Button { Text = "Actualiser" };
            refreshButton.Click += (s, e) => windowManager.FillWindowTitles(checkedListBox);

            buttonPanel.Controls.Add(closeButton);
            buttonPanel.Controls.Add(saveButton);
            buttonPanel.Controls.Add(refreshButton);

            this.Controls.Add(infoLabel);
            this.Controls.Add(checkedListBox);
            this.Controls.Add(buttonPanel);

            // Chemin du fichier de sauvegarde
            saveFilePath = Path.Combine(Application.StartupPath, "windowStates.xml");

            // Initialiser les services
            windowManager = new WindowManager();
            serializationService = new SerializationService(saveFilePath);

            // Charger les états des fenêtres enregistrés
            LoadWindowStates();
        }

        private void TrayIcon_MouseDoubleClick(object? sender, MouseEventArgs e)
        {
            // Afficher et activer la fenêtre principale lors d'un double-clic sur l'icône de la barre des tâches
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
        }

        private void OnExit(object? sender, EventArgs e)
        {
            // Dispose tray icon before exiting the application
            trayIcon.Visible = false;
            trayIcon.Dispose();

            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Code à exécuter lors du chargement de la fenêtre principale
        }

        protected override void OnLoad(EventArgs e)
        {
            Visible = false; // Masquer la fenêtre principale
            ShowInTaskbar = false; // Ne pas montrer dans la barre des tâches

            base.OnLoad(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Save state and dispose of the tray icon before the form closes
            SaveWindowStates();

            trayIcon.Visible = false;
            trayIcon.Dispose();

            base.OnFormClosing(e);
        }

        private void SaveWindowStates()
        {
            List<WindowState> windowStates = windowManager.GetWindowStates(checkedListBox);
            serializationService.Save(windowStates);
        }

        private void LoadWindowStates()
        {
            if (File.Exists(saveFilePath))
            {
                List<WindowState> windowStates = serializationService.Load();
                windowManager.LoadWindowStates(checkedListBox, windowStates);
            }
            else
            {
                windowManager.FillWindowTitles(checkedListBox);
            }
        }
    }
}
