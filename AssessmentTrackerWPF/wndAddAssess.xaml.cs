/*
 *Project: Asssesment Tracker
 * add Assessments Window
 * 
 * Version: 0.1.0
 *
 *Description: A window that displays the list of assessments in a ListView control.
 *
 * Contains: Main and ? other Methods
 * 
 * Written by: John Richardson (M196909)
 *
 *Date: 29 / 08 / 2026
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Interaction logic for wndAddAssess.xaml
    /// </summary>
    public partial class wndAddAssess : Window
    {
        public wndAddAssess()
        {
            InitializeComponent();
            comboContent();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            ///closes Add Assessments window and opens the Main Menu in the same spot.
            MainWindow mainWindow = new MainWindow();

            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;
            mainWindow.Show();
            this.Close();
        }

        private void txtUnitCluster_Copy2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void cmbResult_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        private void comboContent()
            {
            /// Populate the ComboBox with options
            cmbResult.Items.Add("NYM (Not Yet Marked)");
            cmbResult.Items.Add("NYS (Not Yet Satisfactory)");
            cmbResult.Items.Add("S (Satisfactory)");
        }
        /// <summary>
        /// Handles the Click event of the btnAddAssEnter control.
        /// </summary>
        /// <param name="sender"></param> 
        /// <param name="e"></param>
        private void btnAddAssEnter_Click(object sender, RoutedEventArgs e)
        {
            string unitCluster = txtUnitCluster.Text.Trim();
            string assessmentName = txtAssessName.Text.Trim();
            string assessmentType = txtAssessType.Text.Trim();

            // Validate that all fields are filled in and do not contain the | character
            if (unitCluster == "" || assessmentName == "" || assessmentType == "")
            {
                MessageBox.Show("Please fill in all fields.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Validate that the | character is not used in any of the fields
            if ((unitCluster + assessmentName + assessmentType).Contains('|'))
            {
                MessageBox.Show("Please do not use the | character.");
                return;
            }

            // Validate that a due date is selected
            if (dpDueDate.SelectedDate == null)
            {
                MessageBox.Show("Please select a due date.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DateTime dueDate = dpDueDate.SelectedDate.Value;

            // Validate that a result is selected
            if (cmbResult.SelectedItem == null)
            {
                MessageBox.Show("Please select a grade.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string grade = ((string)cmbResult.SelectedItem).Split(' ')[0];

            // Create a new Assessment object
            Assessment newAssessment = new Assessment(0, unitCluster, assessmentName, assessmentType, dueDate, grade);
            App.Manager.AddAssessment(newAssessment);

            try
            {
                App.Manager.SaveAll();
                MessageBox.Show("Assessment added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving assessment: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            

            txtUnitCluster.Clear();
            txtAssessName.Clear();
            txtAssessType.Clear();
            dpDueDate.SelectedDate = null;
            cmbResult.SelectedIndex = 0;
        }
    }
}
