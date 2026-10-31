/*
 * Project: Assessment Tracker
 * File: AssessmentManager.cs
 *
 * Version: 0.2.0
 *
 * Description: Holds the lists of current and completed assessments and
 * loads them from their text files.
 *
 * Contains: AssessmentManager class (LoadAll, AddAssessment, LoadFromFile,
 * ParseLine, NumberCurrentAssessments)
 * 
 * Date modified: 30 / 10 / 2026
 *
 * Written by: John Richardson (M196909)
 *
 * Date created: 30 / 10 / 2026
 * Date modified: 30 / 10 / 2026
 */

using System.Globalization;
using System.IO;

namespace AssessmentTrackerWPF
{
    /// <summary>
    /// Manages the current and completed assessment lists and their text files.
    /// </summary>
    public class AssessmentManager
    {
        // File names and the date format used in the text files
        private const string CURRENT_FILE = "assessments.txt";
        private const string COMPLETED_FILE = "completed.txt";
        private const string DATE_FORMAT = "MM-dd-yyyy";

        /// <summary>
        /// Gets the assessments that are not yet completed.
        /// </summary>
        public List<Assessment> CurrentAssessments { get; private set; } = new List<Assessment>();

        /// <summary>
        /// Gets the assessments graded S, kept in order of ID.
        /// </summary>
        public List<Assessment> CompletedAssessments { get; private set; } = new List<Assessment>();

        /// <summary>
        /// Loads both text files into their lists and numbers the current assessments.
        /// </summary>
        public void LoadAll()
        {
            LoadFromFile(CURRENT_FILE, CurrentAssessments);
            LoadFromFile(COMPLETED_FILE, CompletedAssessments);
            CurrentAssessments.Sort((first, second) => first.DueDate.CompareTo(second.DueDate));
            NumberCurrentAssessments();
        }

        /// <summary>
        /// Adds an assessment to the current list, keeping the list in order of due date.
        /// </summary>
        /// <param name="newAssessment">The assessment to add.</param>
        public void AddAssessment(Assessment newAssessment)
        {
            // Find the first assessment that is due after the new one
            int position = 0;
            while (position < CurrentAssessments.Count &&
                   CurrentAssessments[position].DueDate <= newAssessment.DueDate)
            {
                position++;
            }

            // Insert the new assessment at that position, then renumber the list
            CurrentAssessments.Insert(position, newAssessment);
            NumberCurrentAssessments();
        }

        ///<summary>
        /// Savesthe current and completed assessments to their respective text files.
        /// </summary>
        public void SaveAll()
        {
            SaveToFile(CURRENT_FILE, CurrentAssessments, false);
            SaveToFile(COMPLETED_FILE, CompletedAssessments, true);
        }

        ///<summary>
        /// Writes every assessment in the given list to the specified text file, one per line.
        /// </summary>
        /// <param name="filePath">The text file to write to.</param>
        /// <param name="sourceList">The list of assessments to write.</param>
        /// <param name="includeId">Whether to include the ID in the output.</param>
        private void SaveToFile(string filePath, List<Assessment> sourceList, bool includeId)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (Assessment assessment in sourceList)
                {
                    writer.WriteLine(BuildLine(assessment, includeId));
                }
            }
        }

        /// <summary>
        /// Builds a line of text for one assessment, optionally including the ID.
        /// </summary>
        /// <param name="assessment">The assessment to write.</param>
        /// <param name="includeId">Whether to include the ID in the output.</param>
        /// <returns>The line of text, with the parts separated by | characters.</returns>
        private string BuildLine(Assessment assessment, bool includeId)
        {
            string dateText = assessment.DueDate.ToString(DATE_FORMAT, CultureInfo.InvariantCulture);

            string line = string.Join("|", assessment.UnitCluster, assessment.AssessmentName,
                                      assessment.AssessmentType, dateText, assessment.Grade);

            if (includeId)
            {
                line = assessment.Id + "|" + line;
            }

            return line;
        }

        /// <summary>
        /// Reads a text file line by line and adds each valid line to the given list.
        /// </summary>
        /// <param name="filePath">The text file to read.</param>
        /// <param name="targetList">The list to add the assessments to.</param>
        private void LoadFromFile(string filePath, List<Assessment> targetList)
        {
            // Nothing to load if the file does not exist yet
            if (!File.Exists(filePath))
            {
                return;
            }

            using (StreamReader reader = new StreamReader(filePath))
            {
                // Keep reading until the end of the file
                while (!reader.EndOfStream)
                {
                    string? line = reader.ReadLine();

                    // Skip blank lines
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        Assessment? assessment = ParseLine(line);

                        // Only keep lines that were valid
                        if (assessment != null)
                        {
                            targetList.Add(assessment);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Turns one line of a text file into an assessment.
        /// </summary>
        /// <param name="line">The line to read.</param>
        /// <returns>The assessment, or null if the line is not valid.</returns>
        private Assessment? ParseLine(string line)
        {
            List<string> parts = line.Split('|').Select(part => part.Trim()).ToList();

            // Make every line 6 parts: ID, unit, name, type, date, grade
            if (parts.Count == 4)
            {
                parts.Insert(0, "0");
                parts.Add("NYM");
            }
            else if (parts.Count == 5)
            {
                parts.Insert(0, "0");
            }
            else if (parts.Count != 6)
            {
                return null;
            }

            // The ID must be a whole number
            if (!int.TryParse(parts[0], out int id))
            {
                return null;
            }

            // The date must match the format exactly
            // Use InvariantCulture to avoid issues with different locales
            if (!DateTime.TryParseExact(parts[4], DATE_FORMAT, CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out DateTime dueDate))
            {
                return null;
            }

            // The grade must be one of the three allowed values
            string grade = parts[5].ToUpper();
            if (grade != "NYM" && grade != "NYS" && grade != "S")
            {
                return null;
            }

            return new Assessment(id, parts[1], parts[2], parts[3], dueDate, grade);
        }

        /// <summary>
        /// Gives each current assessment a number (1, 2, 3...) in list order.
        /// </summary>
        private void NumberCurrentAssessments()
        {
            for (int i = 0; i < CurrentAssessments.Count; i++)
            {
                CurrentAssessments[i].Id = i + 1;
            }
        }
    }
}