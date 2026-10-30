/*
 * Project: Assessment Tracker
 * File: AssessmentManager.cs
 *
 * Version: 0.1.0
 *
 * Description: Holds the lists of current and completed assessments and
 * loads them from their text files.
 *
 * Contains: AssessmentManager class (LoadAll, LoadFromFile, ParseLine,
 * NumberCurrentAssessments)
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
            NumberCurrentAssessments();
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