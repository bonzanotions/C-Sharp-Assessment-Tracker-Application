/*
 * Project: Assessment Tracker
 * File: wndDeleteAssess.xaml.cs
 *
 * Version: 0.1.0
 *
 * Description: The Delete Assessments window. Lets the user search the current
 * and completed assessments, pick one from the results and delete it.
 *
 * Contains: wndDeleteAssess class (button click methods)
 *
 * Written by: John Richardson (M196909)
 *
 * Date created: 31 / 10 / 2026
 * Date modified: 31 / 10 / 2026
 */

using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Interaction logic for wndDeleteAssess.xaml
    /// </summary>
    public partial class wndDeleteAssess : Window
    {
        // True for the general search, false for finding a completed assessment by ID
        private bool isGeneralSearch = true;

        public wndDeleteAssess()
        {
            InitializeComponent();
            ShowAllAssessments();
        }

        /// <summary>
        /// Clears the search box and shows every current and completed assessment.
        /// </summary>
        private void ShowAllAssessments()
        {
            txtSearch.Clear();
            lstNovSearchAssess.ItemsSource = App.Manager.SearchAssessments("");
        }

        private void lstViewAssess_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            // Opens the Main Menu in the same spot and closes this window
            MainWindow mainWindow = new MainWindow();
            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;
            mainWindow.Show();
            this.Close();
        }

        private void btnSearchEnter_Click(object sender, RoutedEventArgs e)
        {
            // General search: show the best matches from both lists
            if (isGeneralSearch)
            {
                lstNovSearchAssess.ItemsSource = App.Manager.SearchAssessments(txtSearch.Text);
                return;
            }

            // ID search: the text must be a whole number
            if (!int.TryParse(txtSearch.Text.Trim(), out int id))
            {
                MessageBox.Show("Please enter the ID as a whole number.", "Input Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Binary search on the completed list
            Assessment? found = App.Manager.FindCompletedById(id);

            if (found == null)
            {
                MessageBox.Show("No completed assessment has that ID.", "Not Found",
                                MessageBoxButton.OK, MessageBoxImage.Information);
                lstNovSearchAssess.ItemsSource = null;
                return;
            }

            // Show the one result in the list
            lstNovSearchAssess.ItemsSource = new List<Assessment> { found };
        }

        private void btnChangeSearchType_Click(object sender, RoutedEventArgs e)
        {
            // Switch between the two kinds of search
            isGeneralSearch = !isGeneralSearch;

            if (isGeneralSearch)
            {
                lblSearchType.Content = "General Search - Type Anything";
            }
            else
            {
                lblSearchType.Content = "Find Completed Assessment by ID - Type a Number";
            }

            ShowAllAssessments();
        }

        private void btnDelAssEnter_Click(object sender, RoutedEventArgs e)
        {
            // Make sure a row has been selected
            Assessment? selected = lstNovSearchAssess.SelectedItem as Assessment;

            if (selected == null)
            {
                MessageBox.Show("Please select an assessment to delete.", "Nothing Selected",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Ask the user to confirm
            MessageBoxResult answer = MessageBox.Show(
                "Delete " + selected.UnitCluster + " " + selected.AssessmentName + "?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (answer != MessageBoxResult.Yes)
            {
                return;
            }

            // Remove it from whichever list it is in, then save
            App.Manager.DeleteAnyAssessment(selected);

            try
            {
                App.Manager.SaveAll();
            }
            catch (IOException)
            {
                MessageBox.Show("The assessment was deleted, but the file could not be saved.");
            }

            ShowAllAssessments();
        }
    }
}
