/*
 * Project: Assessment Tracker
 * File: wndEditSearch.xaml.cs
 *
 * Version: 0.1.0
 *
 * Description: A window for searching the assessments and choosing one to edit.
 *
 * Contains: wndEditSearch class (constructor, ShowAllAssessments, btnSearchEnter_Click,
 * btnChangeSearchType_Click, btnEditAssEnter_Click, btnMainMenu_Click)
 *
 * Written by: John Richardson (M196909)
 *
 * Date created: 05 / 11 / 2026
 * Date modified: 05 / 11 / 2026
 */

using System.Windows;

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Interaction logic for wndEditSearch.xaml
    /// </summary>
    public partial class wndEditSearch : Window
    {
        // True for the general search, false for finding a completed assessment by ID
        private bool isGeneralSearch = true;

        public wndEditSearch()
        {
            InitializeComponent();
            ShowAllAssessments();
        }

        /// <summary>
        /// Clears the search box and shows the assessments for the current search type:
        /// everything for a general search, or only completed assessments for an ID search.
        /// </summary>
        private void ShowAllAssessments()
        {
            txtSearch.Clear();

            if (isGeneralSearch)
            {
                lstNovSearchAssess.ItemsSource = App.Manager.SearchAssessments("");
            }
            else
            {
                lstNovSearchAssess.ItemsSource = new List<Assessment>(App.Manager.CompletedAssessments);
            }
        }

        /// <summary>
        /// Searches the assessments using the chosen type of search.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">The click event data.</param>
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

        /// <summary>
        /// Switches between the general search and the ID search.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">The click event data.</param>
        private void btnChangeSearchType_Click(object sender, RoutedEventArgs e)
        {
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

        /// <summary>
        /// Opens the Edit window for the selected assessment.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">The click event data.</param>
        private void btnEditAssEnter_Click(object sender, RoutedEventArgs e)
        {
            // Make sure a row has been selected
            Assessment? selected = lstNovSearchAssess.SelectedItem as Assessment;

            if (selected == null)
            {
                MessageBox.Show("Please select an assessment to edit.", "Nothing Selected",
                                MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Open the Edit window in the same spot and close this one
            wndEditAssess editWindow = new wndEditAssess(selected);
            editWindow.Left = this.Left;
            editWindow.Top = this.Top;
            editWindow.Show();
            this.Close();
        }

        /// <summary>
        /// Opens the Main Menu in the same spot and closes this window.
        /// </summary>
        /// <param name="sender">The button that was clicked.</param>
        /// <param name="e">The click event data.</param>
        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Left = this.Left;
            mainWindow.Top = this.Top;
            mainWindow.Show();
            this.Close();
        }
    }
}
