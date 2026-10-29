/*
 * Project: Assessment Tracker (WPF)
 * Date created: 22/10/2026
 * Date modified: 22/10/2026
 * Version: 1.0.0
 * Author: John Richardson (M196909)
 * Description: Main window. Shows the current and completed assessments, filters them, and lets the user add,
 *              edit, delete, load and save. Data is loaded on start-up and saved after every change and on close.
 */

using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace AssessmentTracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string CURRENT_FILE_NAME = "Unitinfo.txt";
        private const string COMPLETED_FILE_NAME = "Completedinfo.txt";

        private readonly AssessmentManager manager = new AssessmentManager();
        private readonly string currentFilePath = Path.Combine(AppContext.BaseDirectory, CURRENT_FILE_NAME);
        private readonly string completedFilePath = Path.Combine(AppContext.BaseDirectory, COMPLETED_FILE_NAME);

        /// <summary>
        /// Sets up the window, then loads any saved assessments.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            FilterFieldComboBox.ItemsSource = new[] { "Unit", "Name", "Type" };
            FilterFieldComboBox.SelectedIndex = 0;

            LoadData(false);
        }

        /// <summary>
        /// Loads the data files and refreshes the lists on screen.
        /// </summary>
        /// <param name="showMessages">True to tell the user what happened (used by the Load button).</param>
        private void LoadData(bool showMessages)
        {
            try
            {
                int skipped = manager.LoadFromFiles(currentFilePath, completedFilePath);
                RefreshLists();

                if (skipped < 0)
                {
                    StatusTextBlock.Text = "No saved file found yet. Click Add Assessment to start.";
                    if (showMessages)
                    {
                        MessageBox.Show("No saved files were found. Add an assessment and it will be saved " +
                            "automatically.", "Load from File", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else if (skipped > 0)
                {
                    StatusTextBlock.Text = $"Loaded, but {skipped} line(s) in the file could not be read.";
                }
                else
                {
                    StatusTextBlock.Text = $"Loaded {manager.Current.Count} current and " +
                        $"{manager.Completed.Count} completed assessment(s).";
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show($"The file could not be read: {ex.Message}", "Load from File",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"You do not have permission to read the file: {ex.Message}", "Load from File",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// Saves both lists to their files.
        /// </summary>
        /// <returns>True if the save worked.</returns>
        private bool SaveData()
        {
            try
            {
                manager.SaveToFiles(currentFilePath, completedFilePath);
                return true;
            }
            catch (IOException ex)
            {
                MessageBox.Show($"The file could not be saved: {ex.Message}", "Save to File",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"You do not have permission to save the file: {ex.Message}", "Save to File",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return false;
        }

        /// <summary>
        /// Shows both lists on screen, applying the filter (a sequential search) to each.
        /// </summary>
        private void RefreshLists()
        {
            string field = FilterFieldComboBox.SelectedItem as string ?? "Unit";
            string searchText = FilterTextBox.Text.Trim();

            CurrentDataGrid.ItemsSource = AssessmentManager.SequentialSearch(manager.Current, field, searchText);
            CompletedDataGrid.ItemsSource = AssessmentManager.SequentialSearch(manager.Completed, field, searchText);
        }

        /// <summary>
        /// Gets the grid and list for the tab the user is looking at.
        /// </summary>
        /// <returns>The visible grid.</returns>
        private DataGrid ActiveGrid()
        {
            return ListsTabControl.SelectedIndex == 0 ? CurrentDataGrid : CompletedDataGrid;
        }

        /// <summary>
        /// Selects an assessment in the grids after it has been added or edited. Uses binary search to check
        /// that the assessment is in the list.
        /// </summary>
        /// <param name="item">The assessment to select.</param>
        private void SelectAssessment(Assessment item)
        {
            bool isCompleted = item.Grade == Assessment.GRADE_SATISFACTORY;
            List<Assessment> list = isCompleted ? manager.Completed : manager.Current;
            int index = AssessmentManager.BinarySearch(list, item.Unit, item.Name);

            if (index >= 0)
            {
                ListsTabControl.SelectedIndex = isCompleted ? 1 : 0;
                DataGrid grid = ActiveGrid();
                grid.SelectedItem = list[index];
                grid.ScrollIntoView(list[index]);
            }
        }

        /// <summary>Opens the Add window and adds the assessment the user enters.</summary>
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AssessmentWindow dialog = new AssessmentWindow(null) { Owner = this };

            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                if (manager.TryAdd(dialog.Result, out string message))
                {
                    SaveData();
                    RefreshLists();
                    SelectAssessment(dialog.Result);
                    StatusTextBlock.Text = "Assessment added.";
                }
                else
                {
                    MessageBox.Show(message, "Add Assessment", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        /// <summary>Opens the Edit window for the selected assessment.</summary>
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            EditSelected();
        }

        /// <summary>Double-clicking a row edits it.</summary>
        private void DataGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (ActiveGrid().SelectedItem != null)
            {
                EditSelected();
            }
        }

        /// <summary>
        /// Edits the selected assessment. If it is graded S it moves to the completed list.
        /// </summary>
        private void EditSelected()
        {
            if (ActiveGrid().SelectedItem is not Assessment original)
            {
                MessageBox.Show("Please select an assessment to edit.", "Edit Assessment",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            AssessmentWindow dialog = new AssessmentWindow(original) { Owner = this };

            if (dialog.ShowDialog() == true && dialog.Result != null)
            {
                if (manager.TryUpdate(original, dialog.Result, out string message))
                {
                    SaveData();
                    RefreshLists();
                    SelectAssessment(dialog.Result);
                    StatusTextBlock.Text = dialog.Result.Grade == Assessment.GRADE_SATISFACTORY
                        ? "Assessment updated and moved to Completed Assessments."
                        : "Assessment updated.";
                }
                else
                {
                    MessageBox.Show(message, "Edit Assessment", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        /// <summary>Deletes the selected assessment after the user confirms.</summary>
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ActiveGrid().SelectedItem is not Assessment selected)
            {
                MessageBox.Show("Please select an assessment to delete.", "Delete Assessment",
                    MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult answer = MessageBox.Show($"Delete {selected.Unit} {selected.Name}?",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (answer == MessageBoxResult.Yes)
            {
                List<Assessment> list = ListsTabControl.SelectedIndex == 0 ? manager.Current : manager.Completed;
                AssessmentManager.Delete(list, selected);
                SaveData();
                RefreshLists();
                StatusTextBlock.Text = "Assessment deleted.";
            }
        }

        /// <summary>Loads the assessments from the data files.</summary>
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            LoadData(true);
        }

        /// <summary>Saves the assessments to the data files.</summary>
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SaveData())
            {
                StatusTextBlock.Text = "Saved.";
            }
        }

        /// <summary>Closes the window (the data is saved by the Closing event).</summary>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>Saves automatically when the window closes.</summary>
        private void Window_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveData();
        }

        /// <summary>Re-applies the filter when the filter field changes.</summary>
        private void FilterFieldComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                RefreshLists();
            }
        }

        /// <summary>Re-applies the filter as the user types.</summary>
        private void FilterTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                RefreshLists();
            }
        }

        /// <summary>Clears the filter text so every assessment shows.</summary>
        private void ClearFilterButton_Click(object sender, RoutedEventArgs e)
        {
            FilterTextBox.Clear();
        }
    }
}
