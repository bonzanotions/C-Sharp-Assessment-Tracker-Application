/*
 * Project: Assessment Tracker (WPF)
 * Date created: 22/10/2026
 * Date modified: 22/10/2026
 * Version: 1.0.0
 * Author: John Richardson (M196909)
 * Description: Pop-up window used to add a new assessment or edit an existing one. Checks the input before
 *              closing so that only valid assessments are returned.
 */

using System.Globalization;
using System.Windows;

namespace AssessmentTracker
{
    /// <summary>
    /// Interaction logic for AssessmentWindow.xaml.
    /// </summary>
    public partial class AssessmentWindow : Window
    {
        /// <summary>
        /// The assessment entered by the user, or null if the window was cancelled.
        /// </summary>
        public Assessment? Result { get; private set; }

        /// <summary>
        /// Sets up the window. Pass an assessment to edit it, or null to add a new one.
        /// </summary>
        /// <param name="existing">The assessment to edit, or null for a new assessment.</param>
        public AssessmentWindow(Assessment? existing)
        {
            InitializeComponent();

            TypeComboBox.ItemsSource = new[] { "Portfolio of Tasks", "Knowledge Evidence", "Project", "Practical",
                "Observation" };
            GradeComboBox.ItemsSource = new[] { Assessment.GRADE_NOT_MARKED, Assessment.GRADE_NOT_YET_SATISFACTORY,
                Assessment.GRADE_SATISFACTORY };

            if (existing == null)
            {
                Title = "Add Assessment";
                GradeComboBox.SelectedIndex = 0;
            }
            else
            {
                Title = "Edit Assessment";
                OkButton.Content = "Confirm Edit";
                UnitTextBox.Text = existing.Unit;
                NameTextBox.Text = existing.Name;
                TypeComboBox.Text = existing.Type;
                DueDateTextBox.Text = existing.DueDateText;
                GradeComboBox.SelectedItem = existing.Grade;
            }
        }

        /// <summary>
        /// Checks the input. If it is all valid, stores the assessment in Result and closes the window.
        /// </summary>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            string unit = UnitTextBox.Text.Trim();
            string name = NameTextBox.Text.Trim();
            string type = TypeComboBox.Text.Trim();
            string dateText = DueDateTextBox.Text.Trim();
            string grade = GradeComboBox.SelectedItem as string ?? Assessment.GRADE_NOT_MARKED;

            if (unit.Length == 0 || name.Length == 0 || type.Length == 0)
            {
                ErrorTextBlock.Text = "Please fill in the Unit/Cluster, Name and Type.";
                return;
            }

            // The pipe character separates the fields in the data file, so it cannot be used in the text
            if (unit.Contains(Assessment.FIELD_SEPARATOR) || name.Contains(Assessment.FIELD_SEPARATOR)
                || type.Contains(Assessment.FIELD_SEPARATOR))
            {
                ErrorTextBlock.Text = $"Please do not use the {Assessment.FIELD_SEPARATOR} character.";
                return;
            }

            if (!DateTime.TryParseExact(dateText, Assessment.DATE_FORMAT, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime dueDate))
            {
                ErrorTextBlock.Text = "The due date must look like 06-03-2026 (MM-DD-YYYY).";
                return;
            }

            Result = new Assessment { Unit = unit, Name = name, Type = type, DueDate = dueDate, Grade = grade };
            DialogResult = true;
        }
    }
}
