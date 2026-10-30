/*
 * Project: Assessment Tracker
 * File: MainWindow.xaml.cs
 *
 * Version: 0.2.0
 *
 * Description: The main menu window. Opens the other windows and exits the
 * application.
 *
 * Contains: MainWindow class (button click methods)
 *
 * Written by: John Richardson (M196909)
 *
 * Date created: 29 / 08 / 2026
 * Date modified: 30 / 10 / 2026
 */

using System.Windows;

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnViewAssess_Click(object sender, RoutedEventArgs e)
        {
            // Opens the View Assessments window in the same spot and hides the main window
            wndViewAssess viewAssessWindow = new wndViewAssess();
            viewAssessWindow.Left = this.Left;
            viewAssessWindow.Top = this.Top;
            viewAssessWindow.Show();

            this.Hide();
        }

        private void btnAddAssess_Click(object sender, RoutedEventArgs e)
        {
            // Opens the Add Assessments window in the same spot and hides the main window
            wndAddAssess addAssessWindow = new wndAddAssess();
            addAssessWindow.Left = this.Left;
            addAssessWindow.Top = this.Top;
            addAssessWindow.Show();

            this.Hide();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// This button will delete a chosen assessment from the list of assessments.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">The click event data.</param>
        private void btnDelAssess_Click(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// This button is currently under construction and will be implemented in the future.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">The click event data.</param>
        private void btnEditAssess_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button Under Construction");
        }
    }
}