/*
 * Project: Assessment Tracker
 * File: wndEditAssess.xaml.cs
 *
 * Version: 0.1.0
 *
 * Description: A window for changing the details of an existing assessment.
 *
 * Contains: wndEditAssess class (constructor, FillBoxes, btnSaveEdit_Click,
 * btnCancelEdit_Click, ReturnToMenu)
 *
 * Written by: John Richardson (M196909)
 *
 * Date created: 05 / 11 / 2026
 * Date modified: 05 / 11 / 2026
 */

using System.IO;
using System.Windows;

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Interaction logic for wndEditAssess.xaml
    /// </summary>
    public partial class wndEditAssess : Window
    {
        // The assessment being edited
        private readonly Assessment assessmentToEdit;

        /// <summary>
        /// Opens the window with the details of the chosen assessment filled in.
        /// </summary>
        /// <param name="selectedAssessment">The assessment to edit.</param>
        public wndEditAssess(Assessment selectedAssessment)
        {
            InitializeComponent();
            assessmentToEdit = selectedAssessment;

            cmbResult.Items.Add("NYM (Not Yet Marked)");
            cmbResult.Items.Add("NYS (Not Yet Satisfactory)");
            cmbResult.Items.Add("S (Satisfactory)");

            FillBoxes();
        }

        /// <summary>
        /// Puts the details of the assessment into the boxes.
        /// </summary>
        private void FillBoxes()
        {
            lblId.Content = "ID: " + assessmentToEdit.Id;
            txtUnitCluster.Text = assessmentToEdit.UnitCluster;
            txtAssessName.Text = assessmentToEdit.AssessmentName;
            txtAssessType.Text = assessmentToEdit.AssessmentType;
            dpDueDate.SelectedDate = assessmentToEdit.DueDate;

            // Select the grade in the drop-down, e.g. "S" matches "S (Satisfactory)"
            foreach (string item in cmbResult.Items)
            {
                if (item.StartsWith(assessmentToEdit.Grade + " "))
                {
                    cmbResult.SelectedItem = item;
                }
            }
        }

        /// <summary>
        /// Checks the details, then saves the changes to the assessment.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">The click event data.</param>
        private void btnSaveEdit_Click(object sender, RoutedEventArgs e)
        {
            string unitCluster = txtUnitCluster.Text.Trim();
            string assessmentName = txtAssessName.Text.Trim();
            string assessmentType = txtAssessType.Text.Trim();

            // Validate that all fields are filled in
            if (unitCluster == "" || assessmentName == "" || assessmentType == "")
            {
                MessageBox.Show("Please fill in all fields.", "Input Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate that the | character is not used
            if ((unitCluster + assessmentName + assessmentType).Contains('|'))
            {
                MessageBox.Show("Please do not use the | character.", "Input Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate that a due date is selected
            if (dpDueDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a due date.", "Input Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime dueDate = dpDueDate.SelectedDate.Value;

            // Validate that a grade is selected
            if (cmbResult.SelectedItem == null)
            {
                MessageBox.Show("Please select a grade.", "Input Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string grade = ((string)cmbResult.SelectedItem).Split(' ')[0];

            // Refuse a duplicate, ignoring the assessment being edited
            if (App.Manager.IsDuplicate(unitCluster, assessmentName, assessmentType, assessmentToEdit))
            {
                MessageBox.Show("This assessment already exists. Please check the unit, name and type.",
                                "Duplicate Assessment", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Change the assessment and save to the files
            App.Manager.UpdateAssessment(assessmentToEdit, unitCluster, assessmentName,
                                         assessmentType, dueDate, grade);

            try
            {
                App.Manager.SaveAll();
                MessageBox.Show("Assessment updated successfully!", "Success",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (IOException)
            {
                MessageBox.Show("The assessment was changed, but the file could not be saved.",
                                "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            ReturnToMenu();
        }

        /// <summary>
        /// Closes the window without saving any changes.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">The click event data.</param>
        private void btnCancelEdit_Click(object sender, RoutedEventArgs e)
        {
            ReturnToMenu();
        }

        /// <summary>
        /// Opens the Main Menu in the same spot and closes this window.
        /// </summary>
        private void ReturnToMenu()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;
            mainWindow.Show();
            this.Close();
        }
    }
}